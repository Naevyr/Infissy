using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static InfissyProperties.CardProperties;

public class Card : MonoBehaviour {

    //Absolute, healable, desctructible e targetable rimangono private e descrivono lo stato attuale della carta, sono modificabili solo tramite metodi specifici.
    //
    //Le altre variabili non dovrebbero necessitare alcuna proprietà in quanto utilizzate principalmente per la visualizzazione, pensando di separarle in
    //una ulteriore classe.
    private int absolute;
    private bool healable;
    private bool destructible;
    private bool targetable;


    public int Absolute { get{ return absolute;} }
    public bool Healable { get{ return healable; } }
    public bool Destructible {get{return destructible;}}
    partial bool Targetable { get{ return targetable;}}

    public readonly string Title;
    public readonly Sprite CardImage;
    public readonly string Description;
    public readonly CardReferenceCity ReferenceCity;
    //Rarità della carta nel ritrovamento nei pacchetti unita all'estetica, ho usato metalli dell'epoca per poterne usare i colori della carta visualizzata.
    public readonly CardRarity Rarity;
    private readonly bool Friendly;
    public readonly CardEffect Effect;
    public readonly bool Progress;
    public readonly CardType Type; 


    


    
    public Start(){


    }
    
    /// <summary>
    /// Inizializza la carta con dei campi basilari.
    /// </summary>
    public void CardInitialize (){
        Title = "Sample Title";
        CardImage = null;
        Description = "Sample Description";
        ReferenceCity = CardReferenceCity.Other;
        Rarity = CardRarity.Steel;
        Friendly = true;
        Effect = CardTargetEffect.Ally.AllyGold;
        Progress = false;
        Type = CardType.Attack;

    }


    public void CardInitialize (string title,
                            Sprite cardImage,
                            int absolute,
                            string description,
                            CardReferenceCity referenceCity,
                            CardRarity rarity,
                            bool friendly,
                            CardEffect effect,
                            bool progress,
                            CardType type)
    {
        Title = title;
        CardImage = cardImage;
        this.absolute = absolute;
        Description = description;
        ReferenceCity = referenceCity;
        Rarity = rarity;
        Friendly = friendly;
        Effect = effect;
        Progress = progress;
        CardType = type;


        switch(type){
            
            //TODO:Struttura forse non curabile
            case CardType.Structure:
            case CardType.Attack:
            healable = true;
            destructible = true;
            targetable = true;
            break;
            default:
            healable = false;
            destructible = false;
            targetable = false;


        }
    }

    
    public bool DamageCard(int absoluteDamage)
    {
        bool cardDestroyed = false;
        if((absolute - absoluteDamage)<= 0){
            if(destructible == true){

                cardDestroyed = true;
                 //DestroyCard()
            }
           
        }else{
            absolute -= absoluteDamage;
        }
        return cardDestroyed;
    }

    public bool HealCard(int absoluteHeal){
    bool healed = false;
    if(healable = true){
        absolute += absoluteHeal;
        healed = true;
    }
    return healed;
    }

}
