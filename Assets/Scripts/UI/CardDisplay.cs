using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour {
    
    public Card card;
    public Text title;
    public Image img;
    public Text absolute;
    public Text description;
    public string country;
    public string rarity;
    public int friendly;
    public string effect;
    public bool progress;
    public int type;
    
    void Start () {

        title.text = card.title;
        img.sprite = card.img;
        absolute.text = card.absolute.ToString();
        description.text = card.description;
        country = card.country;
        rarity = card.rarity;
        friendly = card.friendly;
        effect = card.effect;
        progress = card.progress;
        type = card.type;
		
	}
	
}
