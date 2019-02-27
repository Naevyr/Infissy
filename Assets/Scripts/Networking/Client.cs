using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using Infissy.Framework;

public class Client : MonoBehaviour
{
    public string clientName;
    public bool isHost;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public Field FieldReference { get; set; }
    public List<Card> cardMovedBuffer;
    public List<Card[]> targetCardBuffer;




    private List<GameClient> players = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    //Sistemare Inizializzazione
    public bool ConnectToServer(string host, int port)
    {

        if (socketReady)
            return false;
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket Error:" + e.Message);
        }
        return socketReady;

    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }
    }
    //Send message to server
    public void Send(string data)
    {
        if (!socketReady)
            return;
        writer.WriteLine(data);
        writer.Flush();

    }


    //Placeholder for play phase method
    public void SendCards(Card[] cardsPlayed)
    {
        string cardPlayed = "";
        foreach (var card in cardsPlayed)
        {
            cardPlayed += card.IDCard + ":";
        }
        if (cardPlayed != "")
        {

            cardPlayed = cardPlayed.Remove(cardPlayed.Length - 1);
        }
        Send("SPLY|" + cardPlayed);
    }


    public void SendPlayedCards(Card[] cardPlayed, Card[][] targetCards)
    {   if(cardPlayed != null)
        {
            for (int i = 0; i < cardPlayed.Length; i++)
            {
                string cardTargetMoveMessage = "";

                foreach (var card in targetCards[i])
                {
                    cardTargetMoveMessage += card.IDCard + ":";
                }
                if (cardTargetMoveMessage != "")
                {

                    cardTargetMoveMessage = cardTargetMoveMessage.Remove(cardPlayed.Length - 1);
                }
                if (i == cardPlayed.Length - 1)
                    Send($"SMOV|{cardPlayed[i].ToString()}|{ cardTargetMoveMessage}|True");
                else
                    Send($"SMOV|{cardPlayed[i].ToString()}|{ cardTargetMoveMessage}|False");
            }
            
        }
        else {
            Send("SMOV|||True");
        }
        

    }


    //Read Message from Server
    private void OnIncomingData(string data)
    {
        
        var aData = data.Split('|');
        switch (aData[0])
        {
            case "SWHO":
                for (int i = 1; i < aData.Length - 1; i++)
                {
                    UserConnected(aData[i], false);
                }
                Send("CWHO|" + clientName + "|" + (isHost ? 1 : 0).ToString());
                break;
            case "SCNN":
                UserConnected(aData[1], false);
                break;

            case "SPLY":




                var cardsMovedIDs = aData[1].Split(':');
                List<Card> cardsMoved = new List<Card>();
                if (cardsMovedIDs[0] != "")
                {

                    foreach (var cardMovedID in cardsMovedIDs)
                    {



                        cardsMoved.Add(FieldReference.FindCard(int.Parse(cardMovedID)));


                    }
                }


                FieldReference.ChangePhase(cardsMoved.ToArray(), null, false);



                break;
            case "SMOV":
                
                var cardTargetsIDs = aData[2].Split(':');
                var cardPlayedID = aData[1];
                int cardMoved;
                if (int.TryParse(cardPlayedID,out cardMoved))
                {
                    Debug.Log(cardPlayedID);
                    if (!bool.Parse(aData[3]))
                    {
                        List<Card> cardsTarget = new List<Card>();
                        cardMovedBuffer.Add(FieldReference.FindCard(cardMoved));
                        for (int i = 0; i < cardTargetsIDs.Length; i++)
                        {
                            cardsTarget.Add(FieldReference.FindCard(int.Parse(aData[3])));
                        }
                    }
                    else
                    {
                        List<Card> cardsTarget = new List<Card>();
                        cardMovedBuffer.Add(FieldReference.FindCard(int.Parse(cardPlayedID)));
                        for (int i = 0; i < cardTargetsIDs.Length; i++)
                        {
                            cardsTarget.Add(FieldReference.FindCard(int.Parse(aData[3])));
                        }
                        FieldReference.ChangePhase(cardMovedBuffer.ToArray(), targetCardBuffer.ToArray(), false);

                    }

                }
                else
                {
                    FieldReference.ChangePhase(null, null, false);
                }



                break;
          



        }
    }
    private void UserConnected(string name, bool host)
    {
        GameClient c = new GameClient();
        c.name = name;

        players.Add(c);

        if (players.Count == 2)
        {
            GameManager.Instance.StartGame();
        }


    }
    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
        {
            return;
        }
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }



    public class GameClient
    {
        public string name;
        public bool isHost;
    }
}
