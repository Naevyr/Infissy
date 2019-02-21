using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Infissy.Framework;
using static Infissy.Properties.GameProperties.GameInitializationValues;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using static Infissy.Properties.CardProperties;
using UnityEngine.Diagnostics;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; set; }
    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connectMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public GameObject displayManagerPrefab;
    
    public Field field { set; get; }
    public DBCaller DBCaller;

    Client c;


    public InputField nameInput;



    void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
        
    }

    public void ConnectButton()
    {
        mainMenu.SetActive(false);
        connectMenu.SetActive(true);
    }
    public void HostButton()
    {
       
        try
        {
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();

            c = Instantiate(clientPrefab).GetComponent<Client>();
            
            
            //Temp
            
            



            c.clientName = nameInput.text;
            c.isHost = true;
            if (c.clientName == "")
                c.clientName = "Server/Host";
            c.ConnectToServer("127.0.0.1", 6312);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);

        }

        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }
 

    public void LoginButton(string userin/* temp*/,string passwin)
    {
        DBCaller.Utente utente=null;
        try
        {
           utente= DBCaller.UtentiData.First(x => x.NomeUtente == userin);
        }
        catch{}
        if (utente==null)
        {
            Debug.Log("Utente non trovato");
        }
        else
        {
            if (utente.Password==passwin)
            {
                Debug.Log("Login OK");
            }
            else
            {
                Debug.Log("Password errata");
            }
        }
        

        
    }
    public void ConnectToServerButton()
    {
        string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (hostAddress == "")
            hostAddress = "127.0.0.1";

        try
        {
            c = Instantiate(clientPrefab).GetComponent<Client>();

            
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Client";
            c.ConnectToServer(hostAddress, 6312);
            connectMenu.SetActive(true);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    public void BackButton()
    {
        mainMenu.SetActive(true);
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);

        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }
    public void StartGame()
    {
        StartCoroutine(FieldInitializationCoroutine());
        SceneManager.LoadScene("Game");
        
    }






    IEnumerator FieldInitializationCoroutine()
    {

        var request = UnityWebRequest.Get("http://www.bargiua.it/Infissy/Calls/gcfm.aspx?idmazzo=1");
        yield return request.SendWebRequest();



        List<Card> deck = new List<Card>();
        string cardsData = request.downloadHandler.text;

        Debug.Log(request.downloadHandler.isDone + request.downloadHandler.text);
       
        var mazzo = new List<Card>();
        var mazzoStringL = cardsData.Split('#')[1].Split(new string[] { "<br>" }, StringSplitOptions.None);

        foreach (var carta in mazzoStringL)
        {
            var cardElements = carta.Split(';');

            if (cardElements.Length != 1)
            {

                deck.Add(Card.Initialize(
                 int.Parse(cardElements[0]),
                    //Title
                 cardElements[2],
                    //Image
                 Resources.Load<Sprite>("Image1"),
                    //TempAbsolute
                 500,
                    //Desc
                 cardElements[3],
                    //ReferenceCity
                 (CardReferenceCity)int.Parse(cardElements[4]),
                    //TempRarity
                 (CardRarity)2,
                    //TempEffects
                 null,
                    //TempProgresso
                 0,
                    //TempType
                 (CardType)int.Parse(cardElements[6]),
                    //Population
                 cardElements[10],
                    //Gold
                 cardElements[12],
                    //Resources
                 cardElements[11]));


            }




        }
        Card[] deck2 = new Card[deck.Count];
        for (int i = 0; i < deck.Count; i++)
        {
            var cardCopy = deck[i];
            deck2[i] = Card.Initialize(cardCopy.IDCard + 50, cardCopy.Title, cardCopy.CardImage, cardCopy.Absolute, cardCopy.Description, cardCopy.ReferenceCity, cardCopy.Rarity, cardCopy.Effects, cardCopy.Progress, cardCopy.Type, cardCopy.PopulationCost, cardCopy.GoldCost, cardCopy.ResourcesCost);
        }
       
       
        
        if(c.isHost == true)
        {
            c.FieldReference = Field.Initalize(c, Player.Initialize(new Stack<Card>(deck), 5, 500, 500, 500, true), Player.Initialize(new Stack<Card>(deck2), 5, 500, 500, 500, true));
        }
        else
        {
            c.FieldReference = Field.Initalize(c, Player.Initialize(new Stack<Card>(deck2), 5, 500, 500, 500, true), Player.Initialize(new Stack<Card>(deck), 5, 500, 500, 500, true));
        }
        field = c.FieldReference;
        Instantiate(displayManagerPrefab);



    }


    
}
