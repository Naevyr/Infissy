using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using static Infissy.Properties.CardProperties;
namespace Infissy.Framework
{
    public class Player 
    {
      
       

        bool localPlayer;

        //Ottimizzazione sucessiva da fare
        private int gold;
        private int population;
        private int resources;

        
        private Stack<Card> deckCards;
        private Stack<Card> handCards; 
        
        private Stack<Card> graveyardCards;
        private Stack<Card>[] inFieldCards;


        public int Gold {get { return gold; } }
        public int Population { get { return population; } }
        public int Resources { get { return resources; } }
        public Stack<Card> HandCards {get {return handCards; } }
        public Stack<Card> DeckCards {get {return handCards; } }
        public Stack<Card> GraveyardCards {get {return handCards; } }
        public Stack<Card>[] InFieldCards {get {return handCards; } }
       
        public bool Healable { get{ return healable; } }
        public bool Targetable { get{ return targetable;}}

        
        public void Draw()
        {
            handCards.Push(deckCards.Pop());
        }


        public void Draw(int nCard)
        {
            for (int i = 0; i <= nCard; i++)
            {
                Draw();
            }
        }



    public virtual bool AffectPlayer(int effectValue, CardEffectTarget affectTarget, bool localMove){
        bool affected = true;

        switch(affectTarget){
            

            //Affect gold
            case CardEffectTarget.AllyGold:
                gold += effectValue;

            break;

            //Affect population
            case CardEffectTarget.AllyPopulation:


                    if(effectValue >= 0){ 
                        if(healable = true){
                            population += effectValue;
                            }else{
                            affected = false;
                            }
                    }else{
                    //Damaging 
                        if((population + effectValue) < 0 ){
                           
                            
                                population = 0;
                            
                        }else{
                            population += effectValue;
                        }
        
                    }


                break;

            //Affect Resources
            case CardEffectTarget.AllyResources:
                if((resources + effectValue) < 0){
                    resources = 0;
                }
            


            
        }
        
        
    
    return affected;
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
                                int startHandCards,
                                int startResources,
                                int startGold,
                                int startPopulation,
                                bool localPlayer){
            
            Player player = new Player();
            
            player.gold = startGold;
            player.population = startPopulation;
            player.resources = startResources;
            
            player.deckCards = deckCards;

            player.localPlayer = localPlayer;

            player.Draw(startHandCards);
            return player;
        }

    }
}
