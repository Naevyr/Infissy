using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Infissy.Framework;

using static Infissy.Properties.DisplayProperties;

public class CardDisplay : MonoBehaviour {
    
    public Card card;
    public Text title;
    public Image artwork;
    public Text absolute;
    public Text description;
    public string country;
    public string rarity;
    public string effect;
    public bool progress;
    public Text type;
    public Text popoulation;
    public Text firstMaterial;
    public Text money;


    

    public void SetCard (Card card) {

        this.card = card;

        
    }

    public void RefreshValues(CardDisplayType displayType)
    {
        //Partial-full refresh needed
        switch (displayType)
        {
            case CardDisplayType.HandCard:
                title.text = card.Title;
                artwork.sprite = card.CardImage;
                absolute.text = card.Absolute.ToString();
                popoulation.text = card.PopulationCost.ToString();
                firstMaterial.text = card.ResourcesCost.ToString();
                money.text = card.GoldCost.ToString();
                break;
            case CardDisplayType.StructureCard:
            case CardDisplayType.UnitCard:
                
                artwork.sprite = card.CardImage;
                absolute.text = card.Absolute.ToString();
                break;

        }
        

    }

}
