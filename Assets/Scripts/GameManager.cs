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

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public GameObject displayManagerPrefab;
    
    public Field field { set; get; }
    public DBCaller DBCaller;

    Client client;


    int port = 80;
    int clientID;

    public InputField nameInput;



    void Start()
    {
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectButton()
    {
        StartCoroutine(ConnectToServer());
        

    }

    IEnumerator ConnectToServer()
    {

        
        var clientPrefabIstance = Instantiate(clientPrefab);
        client = clientPrefabIstance.GetComponent<Client>();

        var selected = GameObject.FindGameObjectWithTag("TempLabel").GetComponent<Dropdown>().value;
        if (selected == 0)
            clientID = 2;
        else
            clientID = 4;

        
   

        var serverRequest = UnityWebRequest.Get("http://icanhazip.com");

        yield return serverRequest.SendWebRequest();
        string externalip = serverRequest.downloadHandler.text;

        externalip = externalip.Remove(externalip.Length-2);

        serverRequest = UnityWebRequest.Get("http://www.bargiua.it/Infissy/Matmak/addqueue.aspx?utente=" + clientID.ToString() + "&ip=" + externalip + "&port=" + port);

        yield return serverRequest.SendWebRequest();


        string serverInfo = "";

        
        serverRequest = UnityWebRequest.Get("http://www.bargiua.it/Infissy/Matmak/AskQueue.aspx?utente=" + clientID.ToString());
        yield return serverRequest.SendWebRequest();

        serverInfo = serverRequest.downloadHandler.text;

        if (!serverInfo.Contains('#'))
        {
           client.ListenForRemote(port);
        }
        else
        {
            var remoteInfo = serverInfo.Split('#')[1].Split(';');

            try
            {
                client.ConnectToRemote(remoteInfo[1], int.Parse(remoteInfo[2]));
            }
            catch (Exception e)
            {

                Debug.Log(e.Message);
            }
            
            foreach (var item in remoteInfo)
            {
                Debug.Log(item);
            }
            serverRequest = UnityWebRequest.Get("http://www.bargiua.it/Infissy/Matmak/EndQueue.aspx?utente=" + clientID.ToString());
            yield return serverRequest.SendWebRequest();
            serverRequest = UnityWebRequest.Get("http://www.bargiua.it/Infissy/Matmak/EndQueue.aspx?utente=" + clientID.ToString());
            yield return serverRequest.SendWebRequest();

        }

        while (!client.IsConnected)
        {
            yield return new WaitForSeconds(0.1f);
        }





       

       

        SceneManager.LoadScene(1);

        StartCoroutine(FieldInitializationCoroutine());

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
       
       
        
        if(client.isHost == true)
        {
            client.FieldReference = Field.Initalize(client, Player.Initialize(new Stack<Card>(deck), 5, 500, 500, 500, true), Player.Initialize(new Stack<Card>(deck2), 5, 500, 500, 500, true));
        }
        else
        {
            client.FieldReference = Field.Initalize(client, Player.Initialize(new Stack<Card>(deck2), 5, 500, 500, 500, true), Player.Initialize(new Stack<Card>(deck), 5, 500, 500, 500, true));
        }
        field = client.FieldReference;
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
