using Infissy.Framework;
using UnityEngine;
using static Infissy.Properties.CardProperties;
using static Infissy.Properties.GameProperties;
namespace Infissy.Framework
{

public class Field 
{
    //?

    public bool isHost;
    private bool isHostTurn;
    public bool isTurn;



    
    private Player remotePlayer;

    private GamePhase gamePhase;
    private Player player;




    public Player Player { get{ return player; } }
    public Player RemotePLayer  { get { return remotePlayer; } }
    




    public static Field Initalize(Client Client, Player Player, Player RemotePlayer){

            //?

            Field field = new Field();

            field.player = Player;
            field.player.Client = Client;
            field.remotePlayer = RemotePlayer;
        
        
            


            //Need revision, using attack phase to permit initial draw
            field.gamePhase = GamePhase.AttackPhase;
            return field;
    }



    

    


    public void SendMove(Card movedCard, Card[] targetCards)
    {
           

            string targetCardIDs = "";
           

                



            
            foreach(var targetCard in targetCards)
            {
                if(targetCard == null)
                {
                    targetCardIDs += -1 + ":";
                }
                targetCardIDs += targetCard.IDCard + ":";
            }

         
            targetCardIDs.Remove(targetCardIDs.Length - 3, 2); //Removes last 2 separators


            player.Client.Send("MOVE|" + movedCard.IDCard + "|" + targetCardIDs);
       
    }
    


    
    public void ChangePhase(Card[] InteractedCards, Card[] targetCards){

        //Needs some fixing
        switch (gamePhase){
            case GamePhase.DrawPhase:
                player.Draw(GameInitializationValues.CardDrawNumber);
                remotePlayer.Draw(GameInitializationValues.CardDrawNumber);
                
                foreach(var structureCard in player.InFieldCards[(int)CardType.Structure]){
                    MoveCard(structureCard,false,null);
                }

                    gamePhase++;


                    break;
                    
            case GamePhase.PlayPhase:
            
                    foreach(var playedCard in InteractedCards)
                    {

                        player.PlayCard(playedCard);

                    }


                    //PlayerInteraction
                
                break;
            case GamePhase.MovePhase:
                //Player sets
                    break;
            case GamePhase.AttackPhase:
                    //Interaction
                    //Eventual moveCard call from here
                    //
                    foreach (var playedCard in InteractedCards)
                    {
                        MoveCard(playedCard, true, targetCards);
                    }
                    ChangePhase(null,null);

                    break;
            }
            



    }

    




    //?
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


                switch (localPlayerMove)
                {
                    case true:
                        switch (effect.EffectType)
                        {
                            case CardEffectType.ValueIncrement:
                                player.AffectPlayer(effect.EffectValue, effect.EffectTarget);
                                break;
                            case CardEffectType.PercentualIncrement:

                                int playerAffectedResource = 0;


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

                                }

                                int operationValue;
                                operationValue = playerAffectedResource * effect.EffectValue / 100;


                                player.AffectPlayer(operationValue, effect.EffectTarget);
                                break;
                                //Finisci la definizione dello spawn delle carte
                        }


                        break;
                    case false:
                        switch (effect.EffectType)
                        {
                            case CardEffectType.ValueIncrement:
                                remotePlayer.AffectPlayer(effect.EffectValue, effect.EffectTarget);
                                break;
                            case CardEffectType.PercentualIncrement:

                                int playerAffectedResource = 0;


                                switch (effect.EffectTarget)
                                {

                                    case CardEffectTarget.AllyGold:
                                        playerAffectedResource = remotePlayer.Gold;
                                        break;

                                    case CardEffectTarget.AllyPopulation:
                                        playerAffectedResource = remotePlayer.Population;
                                        break;

                                    case CardEffectTarget.AllyResources:
                                        playerAffectedResource = remotePlayer.Resources;
                                        break;

                                }

                                int operationValue;
                                operationValue = playerAffectedResource * effect.EffectValue / 100;


                                remotePlayer.AffectPlayer(operationValue, effect.EffectTarget);
                                break;
                                //Finisci la definizione dello spawn delle carte
                        }
                        break;

                }
                
            
           

        }

        
    }
    public void MoveCard(Card card, bool localPlayerMove, Card[] targetCards)
    {   


        if(localPlayerMove == true)
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
                                switch (effect.EffectTarget)
                                {

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
                                if (targetCards[targetIndex] == null)
                                {

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
            else
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
                                switch (effect.EffectTarget)
                                {

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
                                int remotePlayerAffectedResource = default(int);

                                switch (effect.EffectTarget)
                                {

                                    case CardEffectTarget.AllyGold:
                                        remotePlayerAffectedResource = remotePlayer.Gold;
                                        break;

                                    case CardEffectTarget.AllyPopulation:
                                        remotePlayerAffectedResource = remotePlayer.Population;
                                        break;

                                    case CardEffectTarget.AllyResources:
                                        remotePlayerAffectedResource = remotePlayer.Resources;
                                        break;

                                    case CardEffectTarget.EnemyGold:
                                        remotePlayerAffectedResource =player.Gold;
                                        break;

                                    case CardEffectTarget.EnemyPopulation:
                                        remotePlayerAffectedResource = player.Population;
                                        break;

                                    case CardEffectTarget.EnemyResources:
                                        remotePlayerAffectedResource = player.Resources;
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
                                if (targetCards[targetIndex] == null)
                                {

                                    int operationValue;
                                    operationValue = remotePlayerAffectedResource * effect.EffectValue / 100;

                                    if ((int)effect.EffectTarget > 5)
                                    {
                                        player.AffectPlayer(operationValue, effect.EffectTarget - 5);
                                    }
                                    else
                                    {
                                        remotePlayer.AffectPlayer(operationValue, effect.EffectTarget);
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
                                    player.Draw(effect.EffectValue);
                                }
                                else
                                {
                                    remotePlayer.Draw(effect.EffectValue);
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
    


}


}
