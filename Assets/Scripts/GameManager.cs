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
            Debug.Log(carta);
            if (cardElements.Length != 1)
            {
                var InitializedCard = Card.Initialize(
                 int.Parse(cardElements[0]),
                 //Title
                 cardElements[2],
                 //Image
                 Resources.Load<Sprite>(cardElements[1]),
                 //TempAbsolute
                 int.Parse(cardElements[7]),
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
                 cardElements[11]);

                CardEffect[] cardSpawnEffects = new CardEffect[3];
                for (int i = 0; i < cardSpawnEffects.Length; i++)
                {
                    cardSpawnEffects[i].EffectTarget = CardEffectTarget.AllyGold + i;
                    cardSpawnEffects[i].EffectType = CardEffectType.ValueIncrement;
                }
                

                cardSpawnEffects[0].EffectValue = -int.Parse(InitializedCard.GoldCost);
                cardSpawnEffects[1].EffectValue = -int.Parse(InitializedCard.PopulationCost);
                cardSpawnEffects[2].EffectValue = -int.Parse(InitializedCard.ResourcesCost);
                InitializedCard.SpawnEffects = cardSpawnEffects;
                CardEffect[] cardEffects = new CardEffect[1];
                if(InitializedCard.Type == CardType.Attack)
                {
                    cardEffects[0].EffectTarget = CardEffectTarget.EnemyUnit;
                    cardEffects[0].EffectType = CardEffectType.ValueIncrement;
                    cardEffects[0].EffectValue = -InitializedCard.Absolute;
                }
                else
                {
                    cardEffects = new CardEffect[3];
                    cardEffects[0].EffectTarget = CardEffectTarget.AllyGold;
                    cardEffects[0].EffectType = CardEffectType.ValueIncrement;
                    cardEffects[0].EffectValue = -cardSpawnEffects[0].EffectValue;
                    cardEffects[1].EffectTarget = CardEffectTarget.AllyPopulation;
                    cardEffects[1].EffectType = CardEffectType.ValueIncrement;
                    cardEffects[1].EffectValue = -cardSpawnEffects[1].EffectValue;
                    cardEffects[2].EffectTarget = CardEffectTarget.AllyResources;
                    cardEffects[2].EffectType = CardEffectType.ValueIncrement;
                    cardEffects[2].EffectValue = -cardSpawnEffects[2].EffectValue;
                }
                
                InitializedCard.Effects = cardEffects; 


                deck.Add(InitializedCard);

            }



        }
        Card[] deck2 = new Card[deck.Count];
        for (int i = 0; i < deck.Count; i++)
        {
            var cardCopy = deck[i];
            deck2[i] = Card.Initialize(cardCopy.IDCard + 50, cardCopy.Title, cardCopy.CardImage, cardCopy.Absolute, cardCopy.Description, cardCopy.ReferenceCity, cardCopy.Rarity, cardCopy.Effects, cardCopy.Progress, cardCopy.Type, cardCopy.PopulationCost, cardCopy.GoldCost, cardCopy.ResourcesCost);
            deck2[i].Effects = cardCopy.Effects;
            deck2[i].SpawnEffects = cardCopy.SpawnEffects;
            CardEffect[] cardEffects = new CardEffect[3];
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
        field.Player.OnDestroy += RestartGame;
        field.RemotePlayer.OnDestroy += RestartGame;
        Instantiate(displayManagerPrefab);


    }

    private void RestartGame(Player player, Player.PlayerEventArgs args)
    {
        
        SceneManager.LoadScene(0);
        foreach(var gameObjectToDestroy in GameObject.FindGameObjectsWithTag("DontDestroyOnLoad"))
        {
            Destroy(gameObjectToDestroy);

        }
        Destroy(this.gameObject);
    }
}
