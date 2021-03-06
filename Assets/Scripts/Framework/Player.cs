﻿using System;
using System.Collections.Generic;

using static Infissy.Properties.CardProperties;
namespace Infissy.Framework
{
    public class Player
    {

        Client client;
        bool localPlayer;
        public delegate void PlayerAffectHandler(Player player, PlayerEventArgs args);
        public event PlayerAffectHandler OnDestroy;
        //Ottimizzazione sucessiva da fare
        private int gold;
        private int population;
        private int resources;
        private Stack<Card> deckCards;
        private List<Card> handCards;
        private List<Card> graveyardCards;
        private List<Card>[] inFieldCards;
        bool healable=true;
        bool targetable=true;
        private int progress=0;

        public int Gold { get { return gold; } }
        public int Population { get { return population; } }
        public int Resources { get { return resources; } }
        public List<Card> HandCards { get { return handCards; } }
        public Stack<Card> DeckCards { get { return deckCards; } }
        public List<Card> GraveyardCards { get { return graveyardCards; } }
        public List<Card>[] InFieldCards { get { return inFieldCards; } }

        public int Progress { get { return progress; } }
        public bool Healable { get { return healable; } }
        public bool Targetable { get { return targetable; } }
        
        public Client Client
        {
            get
            {
                return client;
            }

            set
            {
                client = value;
            }
        }

        public void Draw()
        {
            if (deckCards.Count != 0)
            {
                handCards.Add(deckCards.Pop());
            }
            else
            {
                for (int i = 0; i < graveyardCards.Count; i++)
                {
                    //Eventual shuffle in case of 
                    deckCards.Push(graveyardCards[i]);
                    graveyardCards.RemoveAt(i);
                }
            }
        }

        public void Draw(int nCard)
        {
            for (int i = 0; i < nCard; i++)
            {
                Draw();
            }
        }

        public void PlayCard(Card card){
                for(int i = 0; i < handCards.Count; i++)
                {
                if (handCards[i] == card)
                {
                    bool playerAffected = true;
                    if (card.SpawnEffects != null)
                    {
                        foreach (var effect in card.SpawnEffects)
                        {
                            switch (effect.EffectType)
                            {
                                case CardEffectType.ValueIncrement:

                                    playerAffected = AffectPlayer(effect.EffectValue, effect.EffectTarget);


                                    break;

                                case CardEffectType.PercentualIncrement:

                                    int playerAffectedResource = 0;

                                    switch (effect.EffectTarget)
                                    {
                                        case CardEffectTarget.AllyGold:
                                            playerAffectedResource = gold;
                                            break;

                                        case CardEffectTarget.AllyPopulation:
                                            playerAffectedResource = Population;
                                            break;

                                        case CardEffectTarget.AllyResources:
                                            playerAffectedResource = Resources;
                                            break;
                                    }

                                    int operationValue;
                                    operationValue = playerAffectedResource * effect.EffectValue / 100;

                                    playerAffected = AffectPlayer(operationValue, effect.EffectTarget);
                                    break;
                            }
                        }
                        if (playerAffected)
                        {
                            inFieldCards[(int)card.Type].Add(card);
                            handCards.Remove(handCards[i]);
                            card.OnDestroy += Player_OnCardDestroy;
                        }
                       

                    }
                }
            }
        }

        

        public bool AffectPlayer(int effectValue, CardEffectTarget affectTarget)
        {
            bool playerAffected = true;
            switch (affectTarget)
            {

                //Affect gold
                case CardEffectTarget.AllyGold:
                    if (gold + effectValue >= 0)
                        gold += effectValue;
                    else
                        playerAffected = false;
                    
                    break;

                //Affect population
                case CardEffectTarget.AllyPopulation:

                    if (effectValue >= 0)
                    {
                        if (healable)
                        {
                            population += effectValue;
                        }
                    }
                    else
                    {
                        //Damaging 
                        if ((population + effectValue) < 0)
                        {
                            population = 0;
                            OnDestroy.Invoke(this, new PlayerEventArgs());
                        }
                        else
                        {
                            population += effectValue;
                        }
                    }

                    break;

                //Affect Resources
                case CardEffectTarget.AllyResources:
                    if (resources + effectValue >= 0)
                            resources += effectValue;
                    else
                        playerAffected = false;
                    break;
            }
            return playerAffected;
        }

        internal void Player_OnCardDestroy(Card card, CardEventArgs args)
        {
            inFieldCards[(int)card.Type].Remove(card);
            graveyardCards.Add(card);
        }

        public void SetPlayerStatus(CardEffectTarget status, bool Healable_Targetable)
        {
            switch (status)
            {
                case CardEffectTarget.Healable:
                    healable = Healable_Targetable;
                    break;
                case CardEffectTarget.Targetable:
                    targetable = Healable_Targetable;
                    break;
            }
        }

        /// <summary>
        /// Player initialize, returns a player instance with set values;
        /// </summary>
        /// <param name="deckCards">Starting deckCards</param>
        /// <param name="startHandCards">Number of starting handCards cards</param>
        /// <param name="startResources">Starting resources</param>
        /// <param name="startGold">Starting gold</param>
        /// <param name="startPopulation">Starting population</param>
        /// <returns></returns>
        public static Player Initialize(Stack<Card> deckCards,
                               int startingDrawCards,
                               int startResources,
                               int startGold,
                               int startPopulation,
                               bool localPlayer)
        {

            

            Player player = new Player();

            player.handCards = new List<Card>();
            player.inFieldCards = new List<Card>[]{ new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>()};
            player.graveyardCards = new List<Card>();
            player.gold = startGold;
            player.population = startPopulation;
            player.resources = startResources;
            player.deckCards = deckCards;
            player.localPlayer = localPlayer;
            player.Draw(startingDrawCards);

            return player;
        }


        public class PlayerEventArgs : EventArgs {

           
        }
    }
}
