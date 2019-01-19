using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infissy.Framework;
public class Deck : MonoBehaviour {

    public GameObject Card;
    public GameObject Hand;
    List<Card> DeckList;
    List<Card> DeadList;    
    int rnd;
    GameObject CardInst;

    void Start()
    {
        DeckList = new List<Card>();
        DeadList = new List<Card>();

    }

    public void Create(List<Card> List) {
     
        DeckList.AddRange(List);                

    }

    public void Draw(){


        /*if (DeckList.Count==0)
        {
            DeckList.AddRange(DeadList);
        }
        rnd = new System.Random().Next(1, x);        
        Card = DeckList[rnd];        
        DeadList.Add(DeckList[rnd]);
        DeckList.RemoveAt(rnd);*/
        CardInst = Instantiate(Card) as GameObject;
        /*title.text = Card.title;
        img.sprite = Card.img;
        absolute.text = Card.absolute.ToString();
        description.text = Card.description;
        country = Card.country;
        rarity = Card.rarity;
        friendly = Card.friendly;
        effect = Card.effect;
        progress = Card.progress;
        type = Card.type;*/
        CardInst.transform.SetParent(Hand.transform);

    }   

}
