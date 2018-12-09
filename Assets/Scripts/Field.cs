using System;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{

    public static Field Instance { set; get; }
    public bool isHost;
    private bool isHostTurn;
    public bool isTurn;

    public GameObject PlayerButton;
    public GameObject EnemyButton;

    public GameObject cardPrefab;

    private Player player;
    private void Start()
    {
        Instance = this;
        player = new Player();
        player.client = FindObjectOfType<Client>();
        isHost = player.client.isHost;
        isHostTurn = true;
        isTurn = isHost;
    }


    public void Move(string player, int card)
    {
        if (this.player.client.clientName == player)
        {
            int currentVal;
            try
            {
                currentVal = int.Parse(EnemyButton.GetComponent<Button>().GetComponentInChildren<Text>().text);
            }
            catch (Exception)
            {
                currentVal = 0;
            }
            currentVal += card;
            EnemyButton.GetComponent<Button>().GetComponentInChildren<Text>().text = currentVal.ToString();
            isTurn = false;
            var cardins = Instantiate(cardPrefab);
            cardins.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.red);


        }
        else
        {
            int currentVal;
            try
            {
                currentVal = int.Parse(PlayerButton.GetComponent<Button>().GetComponentInChildren<Text>().text);
            }
            catch (Exception)
            {
                currentVal = 0;
            }
            currentVal += card;
            PlayerButton.GetComponent<Button>().GetComponentInChildren<Text>().text = currentVal.ToString();
            isTurn = true;
            var cardins = Instantiate(cardPrefab);
            cardins.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.blue);
        }
    }
    public void SendMove()
    {
        if (isTurn)
        {
            player.client.Send("MOVE|" + player.client.clientName + "|1");
        }
    }
    public void EndTurn() =>
        isTurn = false;

    public void Draw(string player)
    {
        if (this.player.client.clientName == player)
        {
            //player draw
        }
        else
        {
            //enemy draw
        }
    }
    public void SendDraw()
    {
        if (isTurn)
        {
            player.client.Send("DRAW|" + player.client.clientName);
        }
    }

    public bool CheckVictory() =>
        player.popolazione < 100 ? true : false;

}
