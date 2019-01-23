//Manages cards relations, handles buildings and calls, as well as interacting with the players
using static Infissy.Properties.CardProperties;

namespace Infissy.Framework {
    public class CardManager{
        
        Player player;
        Player remotePlayer;
        
        public static CardManager Initialize (Player player, Player remotePlayer){

            CardManager cardManager = new CardManager ();
            cardManager.player = player;
            cardManager.remotePLayer = remotePlayer;
            

        }




        public Card FindCard(int idCard){
            

            foreach(var handCard in player.HandCards){
                        if(handCard.IDCard == idCard){
                        return handCard;
                    } 

            }

            foreach(var fieldCards in player.InFieldCards){

                    foreach(var fielSectorCard in fieldCards){

                    if(fieldSectorCard.IDCard == idCard){
                        return fieldSectorCard;
                    }       
                    
                }

            }

            foreach(var graveyardCard in player.GraveyardCards){

                if(graveyardCard.IDCard == idCard){
                        return graveyardCard;
                    }   
            }
            
            foreach(var deckCard in player.DeckCards){

                if(deckCard.IDCard == idCard){
                        return deckCard;
                    }   
            }

            foreach(var handCard in remotePlayer.HandCards){
                        if(handCard.IDCard == idCard){
                        return handCard;
                    } 

            }

            foreach(var fieldCards in remotePlayer.InFieldCards){

                    foreach(var fielSectorCard in fieldCards){

                    if(fieldSectorCard.IDCard == idCard){
                        return fieldSectorCard;
                    }       
                    
                }

            }

            foreach(var graveyardCard in remotePlayer.GraveyardCards){

                if(graveyardCard.IDCard == idCard){
                        return graveyardCard;
                    }   
            }
            
            foreach(var deckCard in remotePlayer.DeckCards){

                if(deckCard.IDCard == idCard){
                        return deckCard;
                    }   
            }
            
        }


        public void PlayCard(Card card, bool localPlayerMove) {  
            foreach(var effect in card.Effects){
                



                if(effect.EffectTarget != CardEffectTarget.None){



                    switch(effect.EffectType){
                        
                        //Flat Value Effect     
                        case CardEffectType.ValueIncrement:
                            if(effect.EffectTarget > 5){
                                remotePlayer.AffectPlayer(effect.EffectValue,effect.EffectTarget-5);
                            }else{
                                player.AffectPlayer(effect.EffectValue,CardEffectTarget);
                            }
                            
                        break;
                        case CardEffectType.PercentualIncrement:
                            

                            //Percentual Effect
                            int playerAffectedResource;
                            int operationValue;
                            switch(effect.EffectTarget){

                                case CardEffectTarget.AllyGold:
                                playerAffectedResource = player.Gold;
                                break;

                                case CardEffectTarget.AllyPopulation:
                                playerAffectedResource = player.AllyPopulation;
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

                            if(effect.EffectTarget > 5){
                                remotePlayer.AffectPlayer(operationValue,effect.EffectTarget-5);
                            }else{
                                player.AffectPlayer(operationValue,CardEffectTarget);
                            }

                        case CardEffectType.Healable:
                        case CardEffectType.Targetable:
                        //Check eventually resource regeneration !Important
                           
                            
                        case CardEffectType.CardDraw:
                        if(effect.EffectTarget > 5){
                                    remotePlayer.CardDraw(effect.EffectValue);
                                }else{
                                    player.CardDraw(effect.EffectValue);
                                }
                        break;

                    }

                }
            }
            //Set eventual playerUse
            card.usage += 1;
        }
        public void PlayCard(Card card, bool localPlayerMove, Card[] targetCard){
        

        }

        void PlayCardOnRemote(int idCard, int idTarget){
            encoderClient.EncodeMoveMessageToRemote(idCard);
            
        }
        void PlayCardOnRemote(int idCard){

            
          
               
        encoderClient.EncodeMoveMessageToRemote(idCard);
              
               
        
        }
        
    }



}