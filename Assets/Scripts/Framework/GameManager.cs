using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using Infissy.Framework;
public class GameManager : MonoBehaviour
{

   

    //Da rivedere. Contiene parti di networking da eventualmente separare in un diverso file
    public static GameManager Instance { get; set; }
    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connectMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public DBCaller DBCaller;

    public InputField nameInput;



    ///




    Field field;
    Player player;
    RemotePlayer remotePlayer;

    [Header("Initialized values")]
    public int PlayerPopulation = 1000;
    public int PlayerGold = 500;
    public int PlayerResources = 500;
    public int PlayerStartingHandCards = 5;


    ///




    void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);

       
        
        


        
    }


    public void ConnectToServerButton()
    {
       
        try
        {
            GetComponent<Client>().ConnectToServer("127.0.0.1",6312);
            
            c.ConnectToServer(hostAddress, 6312);
            serverMenu.SetActive = false;

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
        SceneManager.LoadScene("Game");
    }











    void InitializeAssets(){

        player = Player.Initialize(InitializeDeck(),
                                PlayerStartingHandCards,
                                PlayerResources,
                                PlayerGold,
                                PlayerPopulation);
        //TODO: DeckInizialization
         Stack<Card> Cards = new Stack<Card>();
        field = Field.Initialize(player,Cards);

    }
}
