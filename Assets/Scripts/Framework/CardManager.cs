//Manages cards relations, handles buildings and calls, as well as interacting with the players
using static Infissy.Properties.CardProperties;

namespace Infissy.Framework
{
    public class CardManager
    {

        Player player;
        Player remotePlayer;

        public static CardManager Initialize(Player player, Player remotePlayer)
        {

            CardManager cardManager = new CardManager();
            cardManager.player = player;
            cardManager.remotePlayer = remotePlayer;
            return cardManager;

        }




        public Card FindCard(int idCard)
        {


            foreach (var handCard in player.HandCards)
            {
                if (handCard.IDCard == idCard)
                {
                    return handCard;
                }

            }

            foreach (var fieldCards in player.InFieldCards)
            {

                foreach (var fielSectorCard in fieldCards)
                {

                    if (fielSectorCard.IDCard == idCard)
                    {
                        return fielSectorCard;
                    }

                }

            }

            foreach (var graveyardCard in player.GraveyardCards)
            {

                if (graveyardCard.IDCard == idCard)
                {
                    return graveyardCard;
                }
            }

            foreach (var deckCard in player.DeckCards)
            {

                if (deckCard.IDCard == idCard)
                {
                    return deckCard;
                }
            }

            foreach (var handCard in remotePlayer.HandCards)
            {
                if (handCard.IDCard == idCard)
                {
                    return handCard;
                }

            }

            foreach (var fieldCards in remotePlayer.InFieldCards)
            {

                foreach (var fielSectorCard in fieldCards)
                {

                    if (fielSectorCard.IDCard == idCard)
                    {
                        return fielSectorCard;
                    }

                }

            }

            foreach (var graveyardCard in remotePlayer.GraveyardCards)
            {

                if (graveyardCard.IDCard == idCard)
                {
                    return graveyardCard;
                }
            }

            foreach (var deckCard in remotePlayer.DeckCards)
            {

                if (deckCard.IDCard == idCard)
                {
                    return deckCard;
                }
            }
            return null;

        }


        public void PlayCard(Card card, bool localPlayerMove)
        {
            foreach (var effect in card.Effects)
            {

                if (effect.EffectTarget != CardEffectTarget.None)
                {
                    switch (effect.EffectType)
                    {

                        //Flat Value Effect     
                        case CardEffectType.ValueIncrement:
                            if ((int)effect.EffectTarget > 5)
                            {
                                remotePlayer.AffectPlayer(effect.EffectValue, effect.EffectTarget - 5);
                            }
                            else
                            {
                                player.AffectPlayer(effect.EffectValue, effect.EffectTarget);
                            }

                            break;
                        case CardEffectType.PercentualIncrement:


                            //Percentual Effect
                            int playerAffectedResource = default(int);
                            int operationValue;
                            switch (effect.EffectTarget)
                            {

                                case CardEffectTarget.AllyGold:
                                    playerAffectedResource = player.Gold;
                                    break;

                                case CardEffectTarget.AllyPopulation:
                                    playerAffectedResource = player.Population;
                                    break;

                                case CardEffectTarget.AllyResources:
                                    playerAffectedResource = player.Resources;
                                    break;

                                case CardEffectTarget.EnemyGold:
                                    playerAffectedResource = remotePlayer.Gold;
                                    break;

                                case CardEffectTarget.EnemyPopulation:
                                    playerAffectedResource = remotePlayer.Population;
                                    break;

                                case CardEffectTarget.EnemyResources:
                                    playerAffectedResource = player.Resources;
                                    break;


                            }

                            operationValue = playerAffectedResource * effect.EffectValue / 100;

                            if ((int)effect.EffectTarget > 5)
                            {
                                remotePlayer.AffectPlayer(operationValue, effect.EffectTarget - 5);
                            }
                            else
                            {
                                player.AffectPlayer(operationValue, effect.EffectTarget);
                            }
                            break;
                        case CardEffectType.Healable:
                        case CardEffectType.Targetable:
                        //Check eventually resource regeneration !Important


                        case CardEffectType.CardDraw:
                            if ((int)effect.EffectTarget > 5)
                            {
                                remotePlayer.Draw(effect.EffectValue);
                            }
                            else
                            {
                                player.Draw(effect.EffectValue);
                            }
                            break;

                    }

                }
            }
            //Set eventual playerUse
            card.usage += 1;
        }
        public void PlayCard(Card card, bool localPlayerMove, Card[] targetCard)
        {


        }

        void PlayCardOnRemote(int idCard, int idTarget)
        {
            //todo
            player.Client.Send($"PLCT|{idCard}|{idTarget}");

        }
        void PlayCardOnRemote(int idCard)
        {
            //todo
            player.Client.Send($"PLCA|{idCard}");

        }

    }



}