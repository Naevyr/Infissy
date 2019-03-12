using Infissy.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Infissy.Properties.CardProperties;
using static Infissy.Properties.GameProperties;
namespace Infissy.Framework
{
   public class Field 
        {
        Button changePhaseButton;
        ClickPriority clickPriority = ClickPriority.notYetDefined;

        private Card[] cardPlayBuffer;
        private Card[][] cardTargetBuffer;
        public GamePhase GamePhase { get; private set; }
        public Player Player { get; private set; }
        public Player RemotePlayer { get; private set; }


        //Temp
        public DisplayManager displayManager { get; set; }

        public static Field Initalize(Client Client, Player Player, Player RemotePlayer){
            Field field = new Field();

            field.Player = Player;
            field.Player.Client = Client;
            field.RemotePlayer = RemotePlayer;
            field.GamePhase = GamePhase.PlayPhase;
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

            Player.Client.Send("MOVE|" + movedCard.IDCard + "|" + targetCardIDs);
    }
    

    
    public void ChangePhase(Card[] InteractedCards, Card[][] targetCards, bool localPlayerMove){
        //Needs some fixing
        switch (GamePhase){
            case GamePhase.DrawPhase:
                Player.Draw(GameInitializationValues.CardDrawNumber);
                RemotePlayer.Draw(GameInitializationValues.CardDrawNumber);
                    if (Player.InFieldCards[(int)CardType.Structure].Count != 0)
                    {
                        foreach (var structureCard in Player.InFieldCards[(int)CardType.Structure])
                        {
                            MoveCard(structureCard, false, null);
                        }
                    }
                

                    GamePhase = GamePhase.PlayPhase;
                    GameObject.FindObjectOfType<DisplayManager>().RefreshCards();
                    GameObject.FindObjectOfType<DisplayManager>().RefreshValues();
                    displayManager.HandInteraction = true;
                    break;
                    
            case GamePhase.PlayPhase:
                    

                    changePhaseButton = GameObject.FindObjectOfType<DisplayManager>().ChangePhaseButton;
                

                    if (clickPriority == ClickPriority.notYetDefined)
                    {
                        cardPlayBuffer = InteractedCards;
                        
                        if (localPlayerMove)
                        {
                            clickPriority = ClickPriority.localFirstClick;
                            Player.Client.SendCards(InteractedCards);
                            //Temp
                            displayManager.HandInteraction = false;
                            displayManager.FieldUnitInteraction = false;
                            changePhaseButton.interactable = false;
                        }
                        else
                        {
                            clickPriority = ClickPriority.enemyFirstClick;
                        }
                        GameObject.FindObjectOfType<DisplayManager>().PhaseMessage = "PlayPhaseSecondPart";

                    }
                    else
                    {



                        if (localPlayerMove == true)
                        {
                            Player.Client.SendCards(InteractedCards);
                        }


                        int bufferLenght;
                        int movedCards;

                        if (cardPlayBuffer == null)
                            bufferLenght = 0;
                        else
                            bufferLenght = cardPlayBuffer.Length;
                        if (InteractedCards == null)
                            movedCards = 0;
                        else
                            movedCards = InteractedCards.Length;

                        Debug.Log($"Buffer :{bufferLenght} Interacted :{movedCards}");

                        int stepNumber = Mathf.Max(bufferLenght, movedCards);

                            for (int i = 0; i < stepNumber; i++)
                            {
                                if (clickPriority == ClickPriority.localFirstClick)
                                {
                                    if (i < bufferLenght)
                                    {
                                        PlayCard(cardPlayBuffer[i], true);
                                    }
                                    if (i < movedCards)
                                    {
                                        PlayCard(InteractedCards[i], false);
                                    }
                                }
                                else
                                {
                                    if (i <  bufferLenght)
                                    {
                                        PlayCard(cardPlayBuffer[i], false);
                                    }
                                    if (i <movedCards)
                                    {
                                        PlayCard(InteractedCards[i], true);
                                    }
                                    
                                }
                            
                        }

                        

                        //Temp
                        changePhaseButton.interactable= true;
                        displayManager.FieldUnitInteraction = true;
                        GameObject.FindObjectOfType<DisplayManager>().RefreshCards();

                        displayManager.HandInteraction = false;
                        GameObject.FindObjectOfType<DisplayManager>().RefreshValues();
                        GamePhase = GamePhase.AttackPhase;
                        clickPriority = ClickPriority.notYetDefined;
                        cardPlayBuffer = null;
                        GameObject.FindObjectOfType<DisplayManager>().PhaseMessage = "AttackPhase";
                    }

                   

                    
                    //PlayerInteraction

                    break;
            case GamePhase.MovePhase:
                    
                    //

                    
            case GamePhase.AttackPhase:
                    //Interaction
                    //Eventual moveCard calfrom her
                    

                    
                    //

                    if (clickPriority == ClickPriority.notYetDefined)
                    {
                        cardPlayBuffer = InteractedCards;
                        cardTargetBuffer = targetCards;
                       

                       
                        if (localPlayerMove)
                        {
                            clickPriority = ClickPriority.localFirstClick;
                            Player.Client.SendPlayedCards(InteractedCards,targetCards);
                            changePhaseButton.interactable = false;
                            displayManager.HandInteraction = false;
                            displayManager.FieldUnitInteraction = false;
                        }
                        else
                        {
                            clickPriority = ClickPriority.enemyFirstClick;
                        }
                    }
                    else
                    {

                        if (localPlayerMove)
                        {
                            Player.Client.SendPlayedCards(InteractedCards, targetCards);
                        }


                        int bufferLenght;
                        int movedCards;

                        if (cardPlayBuffer == null)
                        bufferLenght = 0;
                        else
                            bufferLenght = cardPlayBuffer.Length;
                        if (InteractedCards == null)
                            movedCards = 0;
                        else
                            movedCards = InteractedCards.Length;

                        Debug.Log($"Buffer :{bufferLenght} Interacted :{movedCards}");

                        int stepNumber = Mathf.Max(bufferLenght, movedCards);
                            for (int i = 0; i < stepNumber; i++)
                            {
                                if (clickPriority == ClickPriority.localFirstClick)
                                {

                                    if (i < bufferLenght && cardPlayBuffer[i] != null)
                                    {
                                            MoveCard(cardPlayBuffer[i], true, cardTargetBuffer[i]);
                                       
                                        
                                    }

                                    if (i < movedCards && InteractedCards[i] != null)
                                    {
                                     
                                       
                                            MoveCard(InteractedCards[i], false, targetCards[i]);
                                       


                                    }
                            }
                                else
                                {
                                    if (i < bufferLenght)
                                    {
                                         MoveCard(cardPlayBuffer[i], false,cardTargetBuffer[i]);
                                    }
                                    if (i < movedCards)
                                    {
                                       
                                            MoveCard(InteractedCards[i], localPlayerMove: true, targetCards[i]);
                                      
                                      
                                    }
                                }
                            }


                        //Temp

                        changePhaseButton.interactable = true;

                        displayManager.HandInteraction = true;

                        GameObject.FindObjectOfType<DisplayManager>().RefreshCards();
                       

                        GameObject.FindObjectOfType<DisplayManager>().RefreshValues();
                        GameObject.FindObjectOfType<DisplayManager>().PhaseMessage = "DrawPhase";
                        GamePhase = GamePhase.DrawPhase;
                        
                        clickPriority = ClickPriority.notYetDefined;
                        cardPlayBuffer = null;
                        cardTargetBuffer = null;
                        displayManager.FieldUnitInteraction = false;

                        ChangePhase(null, null, true);
                    }

                   

                    break;
            }
    }

        private void CardDestroyed(Card card, CardEventArgs args)
        {

        }

        public Card FindCard(int idCard)
        {
            //Local handCard
            for (int i = 0; i < Player.HandCards.Count; i++)
            {
                if (Player.HandCards[i].IDCard == idCard)
                {
                    return Player.HandCards[i];
                }
            }

            for (int i = 0; i < Player.InFieldCards.Length; i++)
            {
                for (int y = 0; y < Player.InFieldCards[i].Count; y++)
                {
                    if (Player.InFieldCards[i][y].IDCard == idCard)
                    {
                        return Player.InFieldCards[i][y];
                    }
                }
            }

            for (int i = 0; i < Player.GraveyardCards.Count; i++)
            {
                if (Player.GraveyardCards[i].IDCard == idCard)
                {
                    return Player.GraveyardCards[i];
                }
            }

            for (int i = 0; i < RemotePlayer.HandCards.Count; i++)
            {
                if (RemotePlayer.HandCards[i].IDCard == idCard)
                {
                    return RemotePlayer.HandCards[i];
                }
            }

            for (int i = 0; i < RemotePlayer.InFieldCards.Length; i++)
            {
                for (int y = 0; y < RemotePlayer.InFieldCards[i].Count; y++)
                {
                    if (RemotePlayer.InFieldCards[i][y].IDCard == idCard)
                    {
                        return RemotePlayer.InFieldCards[i][y];
                    }
                }
            }

            for (int i = 0; i < RemotePlayer.GraveyardCards.Count; i++)
            {
                if (RemotePlayer.GraveyardCards[i].IDCard == idCard)
                {
                    return RemotePlayer.GraveyardCards[i];
                }
            }

            return null;
    }

    public void PlayCard(Card card, bool localPlayerMove){
            
            if (localPlayerMove)
            {
                Player.PlayCard(card);
            }
            else
            {
                RemotePlayer.PlayCard(card);
            }

    }
            
        

        
    
    public void MoveCard(Card card, bool localPlayerMove, Card[] targetCards)
    {   
        if(localPlayerMove)
            {
                int targetIndex = 0;
                if(card != null)
                {
                    foreach (var effect in card.Effects)
                    {
                        var effectTarget = effect.EffectTarget;
                        if (effect.EffectTarget != CardEffectTarget.None)
                        {
                            EnemyPlayerAttack:
                            switch (effect.EffectType)
                            {
                               
                                //Flat Value Effect     
                                case CardEffectType.ValueIncrement:
                                    switch (effectTarget)
                                    {
                                        case CardEffectTarget.AllyPopulation:
                                        case CardEffectTarget.AllyGold:
                                        case CardEffectTarget.AllyResources:
                                            Player.AffectPlayer(effect.EffectValue, effect.EffectTarget);
                                            break;
                                        case CardEffectTarget.EnemyGold:
                                        case CardEffectTarget.EnemyPopulation:
                                        case CardEffectTarget.EnemyResources:
                                            
                                            RemotePlayer.AffectPlayer(effect.EffectValue, effect.EffectTarget - 5);
                                            break;
                                        case CardEffectTarget.AllyStructure:
                                        case CardEffectTarget.AllyUnit:
                                        case CardEffectTarget.EnemyStructure:
                                        case CardEffectTarget.EnemyUnit:
                                            var effectValue = -card.Absolute;
                                            //Temp
                                            if(targetCards[targetIndex] != null && targetCards[targetIndex].Absolute > 0)
                                            {
                                                targetCards[targetIndex].AffectCard(effectValue);
                                            }
                                                
                                            else
                                            {
                                                effectTarget = CardEffectTarget.EnemyPopulation;
                                             goto EnemyPlayerAttack;
                                            }
                                            break;
                                    }
                                    break;
                                case CardEffectType.PercentualIncrement:

                                    //Percentual Effect
                                    int playerAffectedResource = default(int);

                                    switch (effect.EffectTarget)
                                    {
                                        case CardEffectTarget.AllyGold:
                                            playerAffectedResource = Player.Gold;
                                            break;

                                        case CardEffectTarget.AllyPopulation:
                                            playerAffectedResource = Player.Population;
                                            break;

                                        case CardEffectTarget.AllyResources:
                                            playerAffectedResource = Player.Resources;
                                            break;

                                        case CardEffectTarget.EnemyGold:
                                            playerAffectedResource = RemotePlayer.Gold;
                                            break;

                                        case CardEffectTarget.EnemyPopulation:
                                            playerAffectedResource = RemotePlayer.Population;
                                            break;

                                        case CardEffectTarget.EnemyResources:
                                            playerAffectedResource = Player.Resources;
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
                                            RemotePlayer.AffectPlayer(operationValue, effect.EffectTarget - 5);
                                        }
                                        else
                                        {
                                            Player.AffectPlayer(operationValue, effect.EffectTarget);
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
                                        RemotePlayer.Draw(effect.EffectValue);
                                    }
                                    else
                                    {
                                        Player.Draw(effect.EffectValue);
                                    }
                                    break;
                            }
                        }

                        targetIndex++;
                    }
                }
              
                //Set eventual playerUse
                card.usage += 1;
            }
            else
            {
                int targetIndex = 0;
                if (card != null)
                {
                    foreach (var effect in card.Effects)
                    {
                        var effectTarget = effect.EffectTarget;
                        if (effect.EffectTarget != CardEffectTarget.None)
                        {
                        EnemyPlayerAttack:
                            switch (effect.EffectType)
                            {
                                //Flat Value Effect     
                                case CardEffectType.ValueIncrement:
                                    switch (effectTarget)
                                    {
                                        case CardEffectTarget.AllyPopulation:
                                        case CardEffectTarget.AllyGold:
                                        case CardEffectTarget.AllyResources:
                                            RemotePlayer.AffectPlayer(effect.EffectValue, effect.EffectTarget);
                                            break;
                                        case CardEffectTarget.EnemyGold:
                                        case CardEffectTarget.EnemyPopulation:
                                        case CardEffectTarget.EnemyResources:
                                            Player.AffectPlayer(effect.EffectValue, effect.EffectTarget - 5);
                                            break;
                                        case CardEffectTarget.AllyStructure:
                                        case CardEffectTarget.AllyUnit:
                                        case CardEffectTarget.EnemyStructure:
                                        case CardEffectTarget.EnemyUnit:
                                            var effectValue = -card.Absolute;
                                            if (targetCards[targetIndex] != null && targetCards[targetIndex].Absolute > 0)
                                            {
                                                targetCards[targetIndex].AffectCard(effectValue);
                                            }

                                            else
                                            {
                                                effectTarget = CardEffectTarget.EnemyPopulation;
                                                goto EnemyPlayerAttack;
                                            }
                                           
                                            break;
                                    }
                                    break;
                                case CardEffectType.PercentualIncrement:

                                    //Percentual Effect
                                    int remotePlayerAffectedResource = default(int);

                                    switch (effect.EffectTarget)
                                    {
                                        case CardEffectTarget.AllyGold:
                                            remotePlayerAffectedResource = RemotePlayer.Gold;
                                            break;

                                        case CardEffectTarget.AllyPopulation:
                                            remotePlayerAffectedResource = RemotePlayer.Population;
                                            break;

                                        case CardEffectTarget.AllyResources:
                                            remotePlayerAffectedResource = RemotePlayer.Resources;
                                            break;

                                        case CardEffectTarget.EnemyGold:
                                            remotePlayerAffectedResource = Player.Gold;
                                            break;

                                        case CardEffectTarget.EnemyPopulation:
                                            remotePlayerAffectedResource = Player.Population;
                                            break;

                                        case CardEffectTarget.EnemyResources:
                                            remotePlayerAffectedResource = Player.Resources;
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
                                            Player.AffectPlayer(operationValue, effect.EffectTarget - 5);
                                        }
                                        else
                                        {
                                            RemotePlayer.AffectPlayer(operationValue, effect.EffectTarget);
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
                                        Player.Draw(effect.EffectValue);
                                    }
                                    else
                                    {
                                        RemotePlayer.Draw(effect.EffectValue);
                                    }
                                    break;
                            }
                        }

                        targetIndex++;
                    }
                }
            }
       
       
    }
}
}
