using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using UnityEngine;
using System.Threading;

using Infissy.Framework;
using System.Text;
using System.Threading.Tasks;

public class Client : MonoBehaviour
{
    public string clientName;
    public bool isHost;
    public bool IsConnected = false;

    private bool socketReady;
    public ClientWebSocket webSocketClient;

    Byte[] bufferData = new byte[20];
    public Field FieldReference { get; set; }
    public List<Card> cardMovedBuffer = new List<Card>();
    public List<Card[]> targetCardBuffer = new List<Card[]>();

    private List<GameClient> players = new List<GameClient>();



    //Sistemare Inizializzazione
    public async void ConnectToServer()
    {

        try
        {
            CancellationToken cancellationToken;
            cancellationToken.Register(new Action(OnConnectionFailed));
            string local = "ws://127.0.0.1:24951/3b1ddef8-71fe-4b1d-af38-095ca1e04f80";
            string heroku = "wss://infissy-backend.herokuapp.com";
            webSocketClient = new ClientWebSocket();
            await webSocketClient.ConnectAsync(new Uri(heroku),cancellationToken);

            Debug.Log(webSocketClient.State);
            

            CheckAvailableData();
            socketReady = true;
           
        }
        catch (Exception e)
        {
            Debug.Log("Socket Error:" + e.Message);
        }
       
    }

    void OnConnectionFailed(){



        }

 
   
   
    


    //Send message to server
    public void Send(string data)
    {
        if (!socketReady)
            return;

        

        ArraySegment<byte> dataSegment = new ArraySegment<byte>(Encoding.ASCII.GetBytes(data));
        Task sendTask = webSocketClient.SendAsync(dataSegment,WebSocketMessageType.Text,true,new CancellationToken());
        sendTask.Wait();
        
       
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
                if (targetCards[i] != null)
                {
                    foreach (var card in targetCards[i])
                    {
                        if (card != null)
                            cardTargetMoveMessage += card.IDCard + ":";
                        else
                            cardTargetMoveMessage += ":";
                    }
                    if (cardTargetMoveMessage != "")
                    {
                        cardTargetMoveMessage = cardTargetMoveMessage.Remove(cardTargetMoveMessage.Length - 1);
                    }
                }



               
                if (i == cardPlayed.Length - 1)
                {
                    Send($"SMOV|{cardPlayed[i].IDCard.ToString()}|{ cardTargetMoveMessage}|True");
                    break;
                }
                    
                else
                    Send($"SMOV|{cardPlayed[i].IDCard.ToString()}|{ cardTargetMoveMessage}|False");
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

            case "SSTA":
                Debug.Log(aData[1]);
                if(aData[1] == "True")
                {
                    IsConnected = true;
                }
                break;

            case "SPLY":

                var cardsPlayedIDs = aData[1].Split(':');
                List<Card> cardsMoved = new List<Card>();
                if (cardsPlayedIDs[0] != "")
                {
                    foreach (var cardPlayedID in cardsPlayedIDs)
                    {


                        cardsMoved.Add(FieldReference.FindCard(int.Parse(cardPlayedID)));

                    }
                }

                FieldReference.ChangePhase(cardsMoved.ToArray(), null, false);

                break;
            case "SMOV":

                string[] cardTargetsIDs = {  "" };
                if (aData[2] != "")
                    cardTargetsIDs = aData[2].Split(':');

                var cardMovedIDstring = aData[1];
                int cardMovedID;
                if (int.TryParse(cardMovedIDstring,out cardMovedID))
                {
                    Debug.Log(cardMovedIDstring);
                    //If it hasn't finished it calls the if, if not it calls the else
                    if (!bool.Parse(aData[3]))
                    {
                        Card cardMoved = FieldReference.FindCard(cardMovedID);

                        List<Card> cardsTarget = new List<Card>();
                        cardMovedBuffer.Add(cardMoved);
                        for (int i = 0; i < cardTargetsIDs.Length; i++)
                        {
                            if (cardTargetsIDs[i] != "")
                                cardsTarget.Add(FieldReference.FindCard(int.Parse(cardTargetsIDs[i])));
                           
                            else
                            {
                                cardsTarget.Add(null);
                                cardMoved.Effects[i].EffectTarget = Infissy.Properties.CardProperties.CardEffectTarget.EnemyPopulation;
                                
                            }
                                

                        }
                        targetCardBuffer.Add(cardsTarget.ToArray());
                    }
                    else
                    {
                        List<Card> cardsTarget = new List<Card>();
                        Card cardMoved = FieldReference.FindCard(cardMovedID);
                        cardMovedBuffer.Add(cardMoved);
                        for (int i = 0; i < cardTargetsIDs.Length; i++)
                        {
                            if (cardTargetsIDs[i] != "")
                                cardsTarget.Add(FieldReference.FindCard(int.Parse(cardTargetsIDs[i])));
                            else
                            {
                                cardsTarget.Add(null);
                                cardMoved.Effects[i].EffectTarget = Infissy.Properties.CardProperties.CardEffectTarget.EnemyPopulation;

                            }
                        }



                        targetCardBuffer.Add(cardsTarget.ToArray());
                        FieldReference.ChangePhase(cardMovedBuffer.ToArray(), targetCardBuffer.ToArray(), false);

                        cardMovedBuffer = new List<Card>();
                        cardsTarget = new List<Card>();
                    }
                }
                else
                {
                    FieldReference.ChangePhase(null, null, false);
                }

                break;

        }

        CheckAvailableData();

    }

    private async void CheckAvailableData()
    {
        
        WebSocketReceiveResult asyncStatus = await webSocketClient.ReceiveAsync(new ArraySegment<byte>(bufferData), new CancellationToken(false));

       
        if ( asyncStatus.MessageType == WebSocketMessageType.Text)
        {
            string result = Encoding.ASCII.GetString(bufferData);
            result = result.Replace("\0", string.Empty);

            OnIncomingData(result);
        }

        Array.Clear(bufferData, 0, bufferData.Length);
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }
    public void CloseSocket()
    {
        if (!socketReady)
        {
            return;
        }
        webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "ClientClosedConnection", new CancellationToken());
        socketReady = false;
    }

    public class GameClient
    {
        public string name;
        public bool isHost;
    }
}
