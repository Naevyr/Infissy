using Infissy.Framework;
using UnityEngine;
using static Infissy.Properties.CardProperties;
using static Infissy.Properties.GameProperties;
public class Field : MonoBehaviour
{
    //?
    public static Field Instance { set; get; }
    public bool isHost;
    private bool isHostTurn;
    public bool isTurn;



    
    private Player remotePlayer;

    private GamePhase gamePhase;
    private Player player;




    public Player Player { get{ return player; } }
    public Player RemotePLayer  { get { return remotePlayer; } }
    




    public Field Initalize(Client Client, Player Player, Player RemotePlayer){
        
        //?
        Instance = this;


        player = Player;
        player.Client = Client;
        remotePlayer = RemotePlayer;
        
        
        //?
        isHost = player.Client.isHost;
        isHostTurn = true;
        isTurn = isHost;
    

        //Need revision, using attack phase to permit initial draw
        gamePhase = GamePhase.AttackPhase;

    }



    

    

    //testing pourpose
    public void Move(string player, int card)
    {

    }
    public void SendMove()
    {
        if (isTurn)
        {
            player.Client.Send("MOVE|" + player.Client.clientName + "|1");
        }
    }
    


    
    public void ChangePhase(Card[] InteractedCards){
        switch(gamePhase){
            case GamePhase.DrawPhase:
                player.Draw(GameInitializationValues.CardDrawNumber);
                remotePlayer.Draw(GameInitializationValues.CardDrawNumber);
                
                foreach(var structureCard in player.InFieldCards[CardType.Structure]){
                    MoveCard(structureCard,false,null);
                }
                break;
            case GamePhase.PlayPhase:
            //PlayerInteraction
                break;
            case GamePhase.MovePhase:
            //PlayerInteraction
                break;
            case GamePhase.AttackPhase:
            //Interaction
            //Eventual moveCard call from here
            //
            ChangePhase(null);
            }
            



    }

    





    public void Draw(string player)
    {
        if (this.player.Client.clientName == player)
        {
            //player draw
        }
        else
        {
            //enemy draw
        }
    }
    public void SendDraw()
    {
        if (isTurn)
        {
            player.Client.Send("DRAW|" + player.Client.clientName);
        }
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

    public void PlayCard(Card card, bool localPlayerMove){
        
        
        //!Important, fix card target to avoid double affecting the host player
        
        player.PlayCard(card);
        foreach(var effect in card.SpawnEffects){
            
            switch(effect.EffectType){
                case CardEffectType.ValueIncrement:
                player.AffectPlayer(effect.EffectValue, effect.EffectTarget);
                break;
                case CardEffectType.PercentualIncrement:

                int playerAffectedResource;


                switch(effect.EffectTarget){

                    case CardEffectTarget.AllyGold:
                                playerAffectedResource = player.Gold;
                                break;

                            case CardEffectTarget.AllyPopulation:
                                playerAffectedResource = player.Population;
                                break;

                            case CardEffectTarget.AllyResources:
                                playerAffectedResource = player.Resources;
                                break;

                }
            
                int operationValue; 
                operationValue = playerAffectedResource * effect.EffectValue / 100;

                
                player.AffectPlayer(operationValue);
                //Finisci la definizione dello spawn delle carte
            }
           

        }

        
    }
    public void MoveCard(Card card, bool localPlayerMove, Card[] targetCards)
    {


        
        int targetIndex = 0;
        foreach (var effect in card.Effects)
        {




            if (effect.EffectTarget != CardEffectTarget.None)
            {



                switch (effect.EffectType)
                {

                    //Flat Value Effect     
                    case CardEffectType.ValueIncrement:
                        switch(effect.EffectTarget){

                            case CardEffectTarget.AllyPopulation:
                            case CardEffectTarget.AllyGold:
                            case CardEffectTarget.AllyResources:
                            player.AffectPlayer(effect.EffectValue, effect.EffectTarget);
                            break;
                            case CardEffectTarget.EnemyGold:
                            case CardEffectTarget.EnemyPopulation:
                            case CardEffectTarget.EnemyResources:
                            remotePlayer.AffectPlayer(effect.EffectValue, effect.EffectTarget - 5);
                            break;
                            case CardEffectTarget.AllyStructure:
                            case CardEffectTarget.AllyUnit:
                            case CardEffectTarget.EnemyStructure:
                            case CardEffectTarget.EnemyUnit:
                            targetCards[targetIndex].AffectCard(effect.EffectValue);
                         
                            break;



                        }
                    break;
                    case CardEffectType.PercentualIncrement:

                        //Percentual Effect
                        int playerAffectedResource = default(int);
                        
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

                            
                            case CardEffectTarget.AllyStructure:
                            case CardEffectTarget.EnemyStructure:
                            case CardEffectTarget.AllyUnit:
                            case CardEffectTarget.EnemyUnit:
                            int affectValue = targetCards[targetIndex].Absolute * effect.EffectValue / 100;
                            
                            card.AffectCard(affectValue);
                            
                            break;

                        }
                        //Need refactoring, checks for target since effect without target is directed to the player 
                        if(targetCards[targetIndex] == null){
                            
                            int operationValue; 
                            operationValue = playerAffectedResource * effect.EffectValue / 100;

                            if ((int)effect.EffectTarget > 5)
                            {
                                remotePlayer.AffectPlayer(operationValue, effect.EffectTarget - 5);
                            }
                            else
                            {
                                player.AffectPlayer(operationValue, effect.EffectTarget);
                            }
                        

                        }
                        break;

                    case CardEffectType.Healable:
                    case CardEffectType.Targetable:
                        //Check eventually resource regeneration !Important

                        break;
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
            
            targetIndex++;
        }
        //Set eventual playerUse
        card.usage += 1;
    }
    


}
