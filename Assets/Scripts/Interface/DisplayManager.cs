using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infissy.Framework;
using UnityEngine.UI;
using System;
using static Infissy.Properties.CardProperties;
using static Infissy.Properties.DisplayProperties;

public class DisplayManager : MonoBehaviour
{
    Field field;

    public GameObject HandCardPrefab;
    public GameObject BuildingCardPrefab;
    public GameObject UnitCardPrefab;

    public Text GameStatus;
    public Text playerDisplayProgress;

  
    public Text remotePlayerDisplayProgress;
    GameObject playerDisplayResources;
    GameObject remotePlayerDisplayResources;

    

    public Button ChangePhaseButton;
    List<Card> cardBufferPhase = new List<Card>();

    List<Card> movedCardBuffer = null;
    List<Card[]> targetCardBuffer = null;

    bool handInteractable;
    bool fieldCardsInteractable;

    string phaseMessage;
    bool fieldUnitInteractable;
    bool enemyCardsSelected;


    public void Start()
    {

        InitializeDisplayResources();
        InitializeChangePhaseReferences();
        ReloadAssets();
        InitializeCards();

    }

    public string PhaseMessage
    {
        set
        {
            GameStatus.text = value;
            phaseMessage = value;
        }
        get
        {
            return phaseMessage;
        }
    }


    //Need fixing, property will do the job
    public void FieldUnitInteractionInitialize()
    {
        var fields = GameObject.FindGameObjectsWithTag("Field");
        var buildings = GameObject.FindGameObjectsWithTag("Buildings");

        if (fields[0].name == "EnemyField")
        {
            fields[0].GetComponent<EnemyDrop>().InitializeEnemyDrop(this, fields[0]);
            if(buildings[0].name == "EnemyBuildings")
            {
                buildings[0].GetComponent<EnemyDrop>().InitializeEnemyDrop(this, fields[0]);
            }
            else
            {
                buildings[1].GetComponent<EnemyDrop>().InitializeEnemyDrop(this, fields[0]);
            }
        }
        else
        {
            fields[1].GetComponent<EnemyDrop>().InitializeEnemyDrop(this, fields[1]);
            if (buildings[0].name == "EnemyBuildings")
            {
                buildings[0].GetComponent<EnemyDrop>().InitializeEnemyDrop(this, fields[1]);
            }
            else
            {
                buildings[1].GetComponent<EnemyDrop>().InitializeEnemyDrop(this, fields[1]);
            }
        }


    }


    public bool HandInteraction { set {
            GameObject Hand = GameObject.FindWithTag("Hand");
            for (int i = 0; i < Hand.transform.childCount; i++)
            {
                Hand.transform.GetChild(i).GetComponent<Draggable>().enabled = value;
            }
            handInteractable = value;
        } get { return handInteractable; } }

    public bool FieldUnitInteraction
    {
        set
        {
            var fields = GameObject.FindGameObjectsWithTag("Field");
            GameObject enemyField;

            if (fields[0].name == "Field")
            {
                enemyField = fields[0];
            }
            else
            {
                enemyField = fields[1];
            }

            for (int i = 0; i < enemyField.transform.childCount; i++)
            {
                enemyField.transform.GetChild(i).GetComponent<CardMoveDraggable>().enabled = value;
            }
            fieldCardsInteractable = value;
        }
        get { return fieldCardsInteractable; }
    }

    public bool EnemyCardsSelected {

        set
        {
            var gameFields = GameObject.FindGameObjectsWithTag("Field");
            GameObject enemyField;
            if (gameFields[0].name == "EnemyField")
            {
                enemyField = gameFields[0];
            }
            else
            {
                enemyField = gameFields[1];
            }

            for (int i = 0; i < enemyField.transform.childCount; i++)
            {
                if(value)
                    enemyField.transform.GetChild(i).GetComponent<CardDisplay>().CardStatus = CardStatus.Selected;
                else
                    enemyField.transform.GetChild(i).GetComponent<CardDisplay>().CardStatus = CardStatus.Normal;
            }


        }
        get {

            return enemyCardsSelected;

        }


    }

    

    public void MoveCard(Card cardMoved, Card[] targetCards) {
        if (movedCardBuffer == null)
            movedCardBuffer = new List<Card>();
        movedCardBuffer.Add(cardMoved);

        if (targetCardBuffer == null)
            targetCardBuffer = new List<Card[]>();
        targetCardBuffer.Add(targetCards);
    }

    private void InitializeChangePhaseReferences()
    {

        GameStatus = GameObject.FindGameObjectWithTag("GameStatusText").GetComponent<Text>();
        (ChangePhaseButton = GameObject.FindGameObjectWithTag("ChangePhaseButton").GetComponent<Button>()).onClick.AddListener(OnChangePhaseButtonClick);
        
    }

    private void InitializeDisplayResources()
    {
        //Fix display reference, use text instead of gameobject

        var resourcesDisplay = GameObject.FindGameObjectsWithTag("Resources");

        if (resourcesDisplay[0].name == "Resources")
        {
            playerDisplayResources = resourcesDisplay[0];
            remotePlayerDisplayResources = resourcesDisplay[1];
        }
        else
        {
            playerDisplayResources = resourcesDisplay[1];
            remotePlayerDisplayResources = resourcesDisplay[0];
        }
        FieldUnitInteractionInitialize();
    }

    private void InitializeCards()
    {

        foreach (var card in field.Player.HandCards)
        {
            GameObject handCard = Instantiate(HandCardPrefab);
            handCard.GetComponent<CardDisplay>().SetCard(card);
            handCard.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.HandCard);
            handCard.transform.SetParent(GameObject.FindGameObjectWithTag("Hand").transform);
        }
      
        
    }

    private void ReloadAssets()
    {
        var gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        field = gameManager.field;

        field.displayManager = this;
        RefreshValues();

       
    }

    public void PlayCard(Draggable d, Transform transformDropzone)
    {
        GameObject prefabIst = null;
        var cardPlayed = d.GetComponent<CardDisplay>().card;
        switch (cardPlayed.Type)
        {
            //Need review, add other types
            case CardType.Attack:
                prefabIst = Instantiate(UnitCardPrefab);
                prefabIst.GetComponent<CardDisplay>().SetCard(cardPlayed);
                prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                break;
            case CardType.Structure:
                prefabIst = Instantiate(BuildingCardPrefab);
                prefabIst.GetComponent<CardDisplay>().SetCard(cardPlayed);
                prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                break;
        }

        cardBufferPhase.Add(cardPlayed);

        prefabIst.transform.SetParent(transformDropzone);
        prefabIst.transform.SetSiblingIndex(transformDropzone.Find("New Game Object").GetSiblingIndex());
        d.DestroyPlaceholder();
        Destroy(d.gameObject);
    
       

    }

    public void OnChangePhaseButtonClick()
    {
        switch (field.GamePhase)
        {  //Fix field displaymanaged doing the same thing
            case Infissy.Properties.GameProperties.GamePhase.DrawPhase:

                ColorBlock buttonColor = ChangePhaseButton.colors;
                buttonColor.normalColor = Color.green;
                ChangePhaseButton.colors = buttonColor;

                field.ChangePhase(null,null,true);

                
                RefreshCards();
               
                PhaseMessage = "Draw";
                break;
            case Infissy.Properties.GameProperties.GamePhase.PlayPhase:

                ColorBlock buttonColorAttack = ChangePhaseButton.colors;
                buttonColorAttack.normalColor = Color.blue;
                ChangePhaseButton.colors = buttonColorAttack;

                
                PhaseMessage = "Attack Phase";
                field.ChangePhase(cardBufferPhase.ToArray(), null, true);
               
               
               
                RefreshValues();
                break;
            case Infissy.Properties.GameProperties.GamePhase.MovePhase:
            case Infissy.Properties.GameProperties.GamePhase.AttackPhase:
                RefreshCards();
                RefreshValues();
             
                if (movedCardBuffer != null)
                {
                    field.ChangePhase(movedCardBuffer.ToArray(), targetCardBuffer.ToArray(), true);
                }
                else
                {
                    field.ChangePhase(null, null, true);
                }
               
              
             
                
               
               

                RefreshCards();
                RefreshValues();
               
                break;
            default:
                break;
        }
        
    }

    public void MoveCardOnPlayer(Card card)
    {
        card.Effects[0].EffectTarget = CardEffectTarget.EnemyPopulation;

        //TODO: Fix card target, it works for only one effect but won't for multiple effects

        MoveCard(card, null);
            
    }

    public void RefreshCards()
    {
        //Do adaptive refresh for less instensive processing power
        //Second method: destroy all gameobject and respawn them following field
        var displayFields = GameObject.FindGameObjectsWithTag("Field");
        var displayBuildings = GameObject.FindGameObjectsWithTag("Buildings");
        var displayHand = GameObject.FindGameObjectWithTag("Hand");

        for (int i = 0; i < displayHand.transform.childCount; i++)
        {
            Destroy(displayHand.transform.GetChild(i).gameObject);

        }
        for (int i = 0; i < displayBuildings[0].transform.childCount; i++)
        {
            Destroy(displayBuildings[0].transform.GetChild(i).gameObject);

        }
        for (int i = 0; i < displayBuildings[1].transform.childCount; i++)
        {
            Destroy(displayBuildings[1].transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < displayFields[0].transform.childCount; i++)
        {
            Destroy(displayFields[0].transform.GetChild(i).gameObject);

        }
        for (int i = 0; i < displayFields[1].transform.childCount; i++)
        {
            
           
            Destroy(displayFields[1].transform.GetChild(i).gameObject);
        }

        foreach (var handCard in field.Player.HandCards)
        {
                var prefabIst = Instantiate(HandCardPrefab);
                prefabIst.GetComponent<CardDisplay>().SetCard(handCard);
                prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.HandCard);
                prefabIst.transform.SetParent(displayHand.transform);
                prefabIst.transform.SetAsLastSibling();
        }


        HandInteraction = handInteractable;

        if (displayFields[0].name == "Field")
        {

            foreach (var fieldCard in field.Player.InFieldCards[(int)CardType.Attack])
                {


                    var prefabIst = Instantiate(UnitCardPrefab);
                    prefabIst.GetComponent<CardDisplay>().SetCard(fieldCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                    prefabIst.transform.SetParent(displayFields[0].transform);
                    prefabIst.transform.SetAsLastSibling();
                }

               
                foreach (var fieldCard in field.RemotePlayer.InFieldCards[(int)CardType.Attack]) { 
                    var prefabIst = Instantiate(UnitCardPrefab);
                    prefabIst.GetComponent<CardDisplay>().SetCard(fieldCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                    prefabIst.transform.SetParent(displayFields[1].transform);
                    prefabIst.transform.SetAsLastSibling();
                }
                

        }
        else
        {
            foreach (var fieldCard in field.Player.InFieldCards[(int)CardType.Attack])
            {
                    var prefabIst = Instantiate(UnitCardPrefab);
                    prefabIst.GetComponent<CardDisplay>().SetCard(fieldCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                    prefabIst.transform.SetParent(displayFields[1].transform);
                    prefabIst.transform.SetAsLastSibling();
            }

            

            foreach (var fieldCard in field.RemotePlayer.InFieldCards[(int)CardType.Attack])
            {
                    var prefabIst = Instantiate(UnitCardPrefab);
                    prefabIst.GetComponent<CardDisplay>().SetCard(fieldCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                    prefabIst.transform.SetParent(displayFields[0].transform);
                    prefabIst.transform.SetAsLastSibling();

            }

        }

        if (displayBuildings[0].name == "Buildings")
        {
          
            foreach (var structureCard in field.Player.InFieldCards[(int)CardType.Structure])
            {
                    var prefabIst = Instantiate(BuildingCardPrefab);
                    prefabIst.GetComponent<CardDisplay>().SetCard(structureCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.StructureCard);
                    prefabIst.transform.SetParent(displayBuildings[0].transform);
                    prefabIst.transform.SetAsLastSibling();
            }
            foreach (var structureCard in field.RemotePlayer.InFieldCards[(int)CardType.Structure])
            {

                var prefabIst = Instantiate(BuildingCardPrefab);
                prefabIst.GetComponent<CardDisplay>().SetCard(structureCard);
                prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.StructureCard);
                prefabIst.transform.SetParent(displayBuildings[1].transform);
                prefabIst.transform.SetAsLastSibling();
            }
        }
        else
        {
            foreach (var structureCard in field.Player.InFieldCards[(int)CardType.Structure])
            {
                    var prefabIst = Instantiate(BuildingCardPrefab);
                    prefabIst.GetComponent<CardDisplay>().SetCard(structureCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.StructureCard);
                    prefabIst.transform.SetParent(displayBuildings[1].transform);
                    prefabIst.transform.SetAsLastSibling();
            }
            foreach (var structureCard in field.RemotePlayer.InFieldCards[(int)CardType.Structure])
            {
                    var prefabIst = Instantiate(BuildingCardPrefab);
                    prefabIst.GetComponent<CardDisplay>().SetCard(structureCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.StructureCard);
                    prefabIst.transform.SetParent(displayBuildings[0].transform);
                    prefabIst.transform.SetAsLastSibling();
            }
        }

    }
    
    public void RefreshValues()
    {

        //Do adaptive refresh for less instensive processing power
        var playerResourcesTextfield = playerDisplayResources.transform.GetComponentsInChildren<Text>();
        var playerRemoteResourcesTextfield = remotePlayerDisplayResources.transform.GetComponentsInChildren<Text>();

        foreach (Text playerResourceTextField in playerResourcesTextfield)
        {
            switch (playerResourceTextField.gameObject.name)
            {
                case "TextPopoulation":

                    playerResourceTextField.text = field.Player.Population.ToString();
                    break;
                case "TextFirstMaterial":

                    playerResourceTextField.text = field.Player.Resources.ToString();
                    break;
                case "TextMoney":

                    playerResourceTextField.text = field.Player.Gold.ToString();
                    break;



            }
        }

        foreach (Text playerRemoteResourceTextField in playerRemoteResourcesTextfield)
        {
            switch (playerRemoteResourceTextField.gameObject.name)
            {
                case "EnemyTextPopoulation":

                    playerRemoteResourceTextField.text = field.RemotePlayer.Population.ToString();
                    break;
                case "EnemyTextFirstMaterial":

                    playerRemoteResourceTextField.text = field.RemotePlayer.Resources.ToString();
                    break;
                case "EnemyTextMoney":

                    playerRemoteResourceTextField.text = field.RemotePlayer.Gold.ToString();
                    break;



            }
        }

        //playerDisplayProgress.text = field.Player.Progress.ToString();
        //remotePlayerDisplayProgress.text = field.RemotePlayer.Progress.ToString();

    }

  
}
    
