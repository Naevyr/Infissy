using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Infissy.Properties.CardProperties;
using static Infissy.Properties.GameProperties.CardEventData;

namespace Infissy.Framework
{



public class Card {

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
    public bool Targetable { get{ return targetable;}}




        public delegate void CardDestroyedHandler(Card card,CardEventArgs args);
        public event CardDestroyedHandler OnDestroy;

    public string Title;
    public Sprite CardImage;
    public string Description;
    public CardReferenceCity ReferenceCity;
    //Rarità della carta nel ritrovamento nei pacchetti unita all'estetica, ho usato metalli dell'epoca per poterne usare i colori della carta visualizzata.
    public CardRarity Rarity;
    

    public string PopulationCost;
    public string GoldCost;
    public string ResourcesCost;

    public int usage;
    public CardEffect[] Effects;
    public CardEffect[] SpawnEffects;
    public int Progress;
    public CardType Type; 
    public int IDCard;
    



    public static Card Initialize (int IdCard,
                            string title,
                            Sprite cardImage,
                            int absolute,
                            string description,
                            CardReferenceCity referenceCity,
                            CardRarity rarity,
                            CardEffect[] effect,
                            int progress,
                            CardType type,
                            string populationCost,
                            string goldCost,
                            string resourceCost)
    {

            Card card = new Card();
            card.IDCard = IdCard;
        card.Title = title;

            //Set image path
            card.CardImage = cardImage;
        card.absolute = absolute;
        card.Description = description;
        card.ReferenceCity = referenceCity;
        card.Rarity = rarity;
            card.PopulationCost = populationCost;
            card.GoldCost = goldCost;
            card.ResourcesCost = resourceCost;
            card.Effects = effect;
        card.Progress = progress;
        card.Type = type;


        switch(type){
            
            //TODO:Struttura forse non curabile
            case CardType.Structure:
            case CardType.Attack:
                card.healable = true;
                card.destructible = true;
                card.targetable = true;
                break;
            default:
                card.healable = false;
                card.destructible = false;
                card.targetable = false;
                break;

        }

            return card;
    }

    
    

    public bool AffectCard(int absoluteValue){
        bool affected = true;
        //Healing
        if(absoluteValue >= 0){
                if(healable == true){
                absolute += absoluteValue;
                
            }else{
                affected = false;
            }
        }else{
            //Damaging 
            if((absolute + absoluteValue) < 0 ){
                if(destructible == true){
                    //DestroyCard()
                    
                }else{
                    absolute = 0;
                        OnDestroy.Invoke(this, new CardEventArgs(this, CardEventType.cardDestroyed));
                }
            }else{
                absolute += absoluteValue;
            }
            
        }
    
        return affected;
    }

}

    public struct CardEffect{
            
          
            public CardEffectTarget EffectTarget;
            public int EffectValue;
            public CardEffectType EffectType;

        

    }


    public class CardEventArgs : EventArgs
    {
        private Card sourceCard;
        CardEventType eventType;

        public CardEventArgs(Card cardSource, CardEventType eventType)
        {
            sourceCard = cardSource;
            this.eventType = eventType;
        }

        public Card GetCard()
        {
            return sourceCard;
        }


    }



}
