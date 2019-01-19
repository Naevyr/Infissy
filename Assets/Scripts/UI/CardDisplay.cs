using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Infissy.Framework;
public class CardDisplay : MonoBehaviour {
    
    public Card card;
    public Text title;
    public Sprite img;
    public Text absolute;
    public Text description;
    public string country;
    public string rarity;
    public int friendly;
    public string effect;
    public bool progress;
    public int type;
    
    void Start () {

        title.text = card.Title;
        img.sprite = card.CardImage;
        absolute.text = card.Absolute.ToString();
        description.text = card.description;
        country = card.country;
        rarity = card.rarity;
        friendly = card.friendly;
        effect = card.effect;
        progress = card.progress;
        type = card.type;
		
	}
	
}
