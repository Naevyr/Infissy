using System.Collections.Generic;

using static Infissy.Properties.CardProperties;
namespace Infissy.Framework
{
    public class Player
    {


        Client client;
        bool localPlayer;

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



        public int Gold { get { return gold; } }
        public int Population { get { return population; } }
        public int Resources { get { return resources; } }
        public List<Card> HandCards { get { return handCards; } }
        public Stack<Card> DeckCards { get { return deckCards; } }
        public List<Card> GraveyardCards { get { return graveyardCards; } }
        public List<Card>[] InFieldCards { get { return inFieldCards; } }

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
            handCards.Push(deckCards.Pop());
        }


        public void Draw(int nCard)
        {
            for (int i = 0; i <= nCard; i++)
            {
                Draw();
            }
        }

        public void PlayCard(Card card){
            foreach(var handCard in handCards){
                if(handCard == card){
                
                    inFieldCards[card.Type].Add(card);
                    break;

                }
            }

        }

        public void AffectPlayer(int effectValue, CardEffectTarget affectTarget)
        {
           

            switch (affectTarget)
            {


                //Affect gold
                case CardEffectTarget.AllyGold:
                    gold += effectValue;
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

                        }
                        else
                        {
                            population += effectValue;
                        }

                    }

                    break;

                //Affect Resources
                case CardEffectTarget.AllyResources:
                    if ((resources + effectValue) < 0)
                    {
                        resources = 0;
                    }
                    break;
            }

           
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
                               int startHandCards,
                               int startResources,
                               int startGold,
                               int startPopulation,
                               bool localPlayer)
        {

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
