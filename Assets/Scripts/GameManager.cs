using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Infissy.Framework;
using static Infissy.Properties.GameProperties.GameInitializationValues;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; set; }
    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connectMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public DBCaller DBCaller;

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

            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            
            
            //Temp
            Field gameField = InitializeGame(c);
            
            c.clientName = nameInput.text;
            c.isHost = true;
            if (c.clientName == "")
                c.clientName = "Server/Host";
            c.ConnectToServer("127.0.0.1", 6312,gameField);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);

        }

        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }
    //Temp Field Initialization method
    private Field InitializeGame(Client c)
    {

        List<Card> deck = DBCaller.GCFM(1);
        Player player = Player.Initialize(new Stack<Card>(deck), 5, InitialPlayerResources, InitialPlayerGold, InitialPlayerPopulation, true);
        Player remotePlayer = Player.Initialize(new Stack<Card>(deck), 5, InitialPlayerResources, InitialPlayerGold, InitialPlayerPopulation, true);
        Field field = Field.Initalize(c, player, remotePlayer);
        return field;
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
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            Field gameField = InitializeGame(c);

            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Client";
            c.ConnectToServer(hostAddress, 6312,gameField);
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
        SceneManager.LoadScene("Game");
    }
}
