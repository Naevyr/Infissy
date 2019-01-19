//Manages cards relations, handles buildings and calls, as well as interacting with the players


namespace Infissy.Framework {
    public class CardManager{
        
        Field field;
        
        public static CardManager Initialize (Field field){

            CardManager cardManager = new CardManager ();
            cardManager.field = field;
            

        }




        public Card FindCard(int idCard){
            Card card;

            foreach(var handCard in field.Player.HandCards){
                        if(handCard.IDCard == idCard){
                        return handCard;
                    } 

            }

            foreach(var fieldCards in field.Player.InFieldCards){

                    foreach(var fielSectorCard in fieldCards){

                    if(fieldSectorCard.IDCard == idCard){
                        return fieldSectorCard;
                    }       
                    
                }

            }

            foreach(var graveyardCard in field.Player.GraveyardCards){

                if(graveyardCard.IDCard == idCard){
                        return graveyardCard;
                    }   
            }
            
            foreach(var deckCard in field.Player.DeckCards){

                if(deckCard.IDCard == idCard){
                        return deckCard;
                    }   
            }
            
        }


        public void PlayCard(Card card, bool localPlayerMove) {  
            foreach(var effect in card.Effects){
                

            }
        
        
        }
        public void PlayCard(Card card, bool localPlayerMove, Card targetCard){
        

        }

        void PlayCardOnRemote(int idCard, int idTarget){
            encoderClient.EncodeMoveMessageToRemote(idCard);
            
        }
        void PlayCardOnRemote(int idCard){

            
          
               
        encoderClient.EncodeMoveMessageToRemote(idCard);
              
               
        
        }
        
    }



}