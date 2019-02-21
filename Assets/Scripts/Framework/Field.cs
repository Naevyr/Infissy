using Infissy.Framework;
using System.Collections.Generic;
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

    ClickPriority clickPriority = ClickPriority.notYetDefined;
    private List<Card>[] cardPlayBuffer = new List<Card>[2];
     
    public GamePhase GamePhase { get { return gamePhase; } }
    public Player Player { get{ return player; } }
    public Player RemotePlayer  { get { return remotePlayer; } }
    




    public static Field Initalize(Client Client, Player Player, Player RemotePlayer){

            //?

            Field field = new Field();

            field.player = Player;
            field.player.Client = Client;
            field.remotePlayer = RemotePlayer;
        
        
            


            //Need revision, using attack phase to permit initial draw
            field.gamePhase = GamePhase.PlayPhase;
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
    


    
    public void ChangePhase(Card[] InteractedCards, Card[] targetCards, bool localPlayerMove){

        //Needs some fixing
        switch (gamePhase){
            case GamePhase.DrawPhase:
                player.Draw(GameInitializationValues.CardDrawNumber);
                remotePlayer.Draw(GameInitializationValues.CardDrawNumber);
                    if (player.InFieldCards[(int)CardType.Structure].Count != 0)
                    {
                        foreach (var structureCard in player.InFieldCards[(int)CardType.Structure])
                        {
                            MoveCard(structureCard, false, null);
                        }
                    }
                

                    gamePhase = GamePhase.PlayPhase;
                    

                    break;
                    
            case GamePhase.PlayPhase:

                   
                     
                    if(cardPlayBuffer[0] == null)
                    {
                        (cardPlayBuffer[0] = new List<Card>()).AddRange(InteractedCards);
                        if (localPlayerMove)
                        {
                            clickPriority = ClickPriority.localFirstClick;
                            player.Client.SendCards(InteractedCards);
                            GameObject.FindObjectOfType<DisplayManager>().ChangePhaseButton.enabled = false;
                        }
                        else
                        {
                            clickPriority = ClickPriority.enemyFirstClick;
                        }
                        GameObject.FindObjectOfType<DisplayManager>().ChangePhaseButton.enabled = false;


                    }
                    else
                    {
                        int stepNumber = Mathf.Max(cardPlayBuffer[0].Count, InteractedCards.Length);

                        for (int i = 0; i < stepNumber; i++)
                        {
                             if(clickPriority == ClickPriority.localFirstClick)
                            {
                                if(i < cardPlayBuffer[0].Count)
                                {
                                    PlayCard(cardPlayBuffer[0][i], true);
                                }
                                if (i < InteractedCards.Length)
                                {
                                    PlayCard(InteractedCards[i], false);
                                }

                            }
                            else
                            {

                                if (i < cardPlayBuffer[0].Count)
                                {
                                    PlayCard(cardPlayBuffer[0][i], false);
                                }
                                if (i < InteractedCards.Length)
                                {
                                    PlayCard(InteractedCards[i], true);
                                }
                            }
                        }
                        GameObject.FindObjectOfType<DisplayManager>().RefreshCards();
                        
                        GameObject.FindObjectOfType<DisplayManager>().ChangePhaseButton.enabled = true;
                        gamePhase = GamePhase.DrawPhase;
                        clickPriority = ClickPriority.notYetDefined;
                        cardPlayBuffer = new List<Card>[2];
                    }



                   


                    
                    //PlayerInteraction

                    break;
            case GamePhase.MovePhase:
                    
                    //


                    gamePhase++;
                    break;
            case GamePhase.AttackPhase:
                    //Interaction
                    //Eventual moveCard call from here
                    //
                    if (cardPlayBuffer == null)
                    {
                        cardPlayBuffer[0].AddRange(InteractedCards);
                        cardPlayBuffer[1].AddRange(targetCards);
                        if (localPlayerMove)
                        {
                            clickPriority = ClickPriority.localFirstClick;

                        }
                        else
                        {
                            clickPriority = ClickPriority.enemyFirstClick;
                        }

                    }
                    else
                    {
                        int stepNumber = Mathf.Max(cardPlayBuffer[0].Count, InteractedCards.Length);

                        for (int i = 0; i < stepNumber; i++)
                        {
                            if (clickPriority == ClickPriority.localFirstClick)
                            {
                                if (i < cardPlayBuffer[0].Count)
                                {
                                    MoveCard(cardPlayBuffer[0][i], true,cardPlayBuffer[1].ToArray());
                                }
                                if (i < InteractedCards.Length)
                                {
                                    MoveCard(InteractedCards[i], false,targetCards);
                                }

                            }
                        }
                        gamePhase++;
                        ChangePhase(null, null, true);
                    }


                   

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
            //Refactor
            if (localPlayerMove)
            {
                player.PlayCard(card);
            }
            else
            {
                remotePlayer.PlayCard(card);
            }
        
        if(card.SpawnEffects != null)
            {
                foreach (var effect in card.SpawnEffects)
                {


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
