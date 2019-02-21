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

    public GameObject HandCard;
    public GameObject BuildingCard;
    public GameObject UnitCard;



    public Text playerDisplayProgress;
    public Text remotePlayerDisplayProgress;
    GameObject playerDisplayResources;
    GameObject remotePlayerDisplayResources;



    public Button ChangePhaseButton;
    List<Card> cardBufferPhase = new List<Card>();
    
    private void Start()
    {


        InitializeDisplayResources();
        InitializeChangePhaseReferences();
        ReloadAssets();
        InitializeCards();


    }

    private void InitializeChangePhaseReferences()
    {
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


    }

    private void InitializeCards()
    {
        

        foreach (var card in field.Player.HandCards)
        {
            GameObject handCard = Instantiate(HandCard);
            handCard.GetComponent<CardDisplay>().SetCard(card);
            handCard.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.HandCard);
            handCard.transform.SetParent(GameObject.FindGameObjectWithTag("Hand").transform);
        }
        
      
        
    }

    private void ReloadAssets()
    {
        var gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        field = gameManager.field;
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
                prefabIst = Instantiate(UnitCard);
                prefabIst.GetComponent<CardDisplay>().SetCard(cardPlayed);
                prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                break;
            case CardType.Structure:
                prefabIst = Instantiate(BuildingCard);
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
        {
            case Infissy.Properties.GameProperties.GamePhase.DrawPhase:


                field.ChangePhase(null, null, true);
                
                RefreshCards();
                
                break;
            case Infissy.Properties.GameProperties.GamePhase.PlayPhase:
                field.ChangePhase(cardBufferPhase.ToArray(), null, true);
                RefreshCards();
                RefreshValues();

                break;
            case Infissy.Properties.GameProperties.GamePhase.MovePhase:
                break;
            case Infissy.Properties.GameProperties.GamePhase.AttackPhase:
                break;
            default:
                break;
        }
        
        
    }
    public void RefreshCards()
    {
        //Do adaptive refresh for less instensive processing power
        //Second method: destroy all gameobject and respawn them following field
        var displayFields = GameObject.FindGameObjectsWithTag("Field");
        var displayBuildings = GameObject.FindGameObjectsWithTag("Buildings");
        var displayHand = GameObject.FindGameObjectWithTag("Hand");

        foreach (var handCard in field.Player.HandCards)
        {
            bool cardFound = false;
            foreach (var cardDisplay in displayHand.GetComponentsInChildren<CardDisplay>())
            {
                if (cardDisplay.card == handCard)
                {
                    cardFound = true;
                    break;
                }

            }
            if (cardFound == false)
            {
                var prefabIst = Instantiate(HandCard);
                prefabIst.GetComponent<CardDisplay>().SetCard(handCard);
                prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.HandCard);
                prefabIst.transform.SetParent(displayHand.transform);
                prefabIst.transform.SetAsLastSibling();
            }
        }
       




        if (displayFields[0].name == "Field")
        {
            
                foreach (var fieldCard in field.Player.InFieldCards[(int)CardType.Attack])
                {
                    bool cardFound = false;
                    foreach (var cardDisplay in displayFields[0].GetComponentsInChildren<CardDisplay>())
                    {
                        if (cardDisplay.card == fieldCard)
                        {
                            cardDisplay.RefreshValues(CardDisplayType.StructureCard);
                            cardFound = true;
                            break;
                        }
                           
                    }
                    if (cardFound == false)
                    {
                        var prefabIst = Instantiate(UnitCard);
                        prefabIst.GetComponent<CardDisplay>().SetCard(fieldCard);
                        prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                        prefabIst.transform.SetParent(displayFields[0].transform);
                        prefabIst.transform.SetAsLastSibling();
                    }
                }

                //////
                foreach (var fieldCard in field.RemotePlayer.InFieldCards[(int)CardType.Attack])
                {
                bool cardFound = false;
                foreach (var cardDisplay in displayFields[1].GetComponentsInChildren<CardDisplay>())
                {
                    if (cardDisplay.card == fieldCard)
                    {
                        cardDisplay.RefreshValues(CardDisplayType.StructureCard);
                        cardFound = true;
                        break;
                    }

                }
                if (cardFound == false)
                {
                    var prefabIst = Instantiate(UnitCard);
                    prefabIst.GetComponent<CardDisplay>().SetCard(fieldCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                    prefabIst.transform.SetParent(displayFields[1].transform);
                    prefabIst.transform.SetAsLastSibling();
                }

                }


        }
        else
        {
            foreach (var fieldCard in field.Player.InFieldCards[(int)CardType.Attack])
            {
                bool cardFound = false;
                foreach (var cardDisplay in displayFields[1].GetComponentsInChildren<CardDisplay>())
                {
                    if (cardDisplay.card == fieldCard)
                    {
                        cardDisplay.RefreshValues(CardDisplayType.StructureCard);
                        cardFound = true;
                        break;
                    }

                }
                if (cardFound == false)
                {
                    var prefabIst = Instantiate(UnitCard);
                    prefabIst.GetComponent<CardDisplay>().SetCard(fieldCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                    prefabIst.transform.SetParent(displayFields[1].transform);
                    prefabIst.transform.SetAsLastSibling();
                }
            }

            ////////
            

            foreach (var fieldCard in field.RemotePlayer.InFieldCards[(int)CardType.Attack])
            {
                bool cardFound = false;
                foreach (var cardDisplay in displayFields[0].GetComponentsInChildren<CardDisplay>())
                {
                    if (cardDisplay.card == fieldCard)
                    {
                        cardDisplay.RefreshValues(CardDisplayType.StructureCard);
                        cardFound = true;
                        break;
                    }

                }
                if (cardFound == false)
                {
                    var prefabIst = Instantiate(UnitCard);
                    prefabIst.GetComponent<CardDisplay>().SetCard(fieldCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.UnitCard);
                    prefabIst.transform.SetParent(displayFields[0].transform);
                    prefabIst.transform.SetAsLastSibling();
                }

            }


        }

        if (displayBuildings[0].name == "Buildings")
        {

            foreach (var structureCard in field.Player.InFieldCards[(int)CardType.Structure])
            {
                bool cardFound = false;
                foreach (var cardDisplay in displayBuildings[0].GetComponentsInChildren<CardDisplay>())
                {
                    if (cardDisplay.card == structureCard)
                    {
                        cardDisplay.RefreshValues(CardDisplayType.StructureCard);
                        cardFound = true;
                        break;
                    }

                }
                if (cardFound == false)
                {
                    var prefabIst = Instantiate(BuildingCard);
                    prefabIst.GetComponent<CardDisplay>().SetCard(structureCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.StructureCard);
                    prefabIst.transform.SetParent(displayBuildings[0].transform);
                    prefabIst.transform.SetAsLastSibling();
                }
            }
            foreach (var structureCard in field.Player.InFieldCards[(int)CardType.Structure])
            {
                bool cardFound = false;
                foreach (var cardDisplay in displayBuildings[1].GetComponentsInChildren<CardDisplay>())
                {
                    if (cardDisplay.card == structureCard)
                    {
                        cardDisplay.RefreshValues(CardDisplayType.StructureCard);
                        cardFound = true;
                        break;
                    }

                }
                if (cardFound == false)
                {
                    var prefabIst = Instantiate(BuildingCard);
                    prefabIst.GetComponent<CardDisplay>().SetCard(structureCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.StructureCard);
                    prefabIst.transform.SetParent(displayBuildings[1].transform);
                    prefabIst.transform.SetAsLastSibling();
                }
            }


        }
        else
        {

            foreach (var structureCard in field.Player.InFieldCards[(int)CardType.Structure])
            {
                bool cardFound = false;
                foreach (var cardDisplay in displayBuildings[1].GetComponentsInChildren<CardDisplay>())
                {
                    if (cardDisplay.card == structureCard)
                    {
                        cardDisplay.RefreshValues(CardDisplayType.StructureCard);
                        cardFound = true;
                        break;
                    }

                }
                if (cardFound == false)
                {
                    var prefabIst = Instantiate(BuildingCard);
                    prefabIst.GetComponent<CardDisplay>().SetCard(structureCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.StructureCard);
                    prefabIst.transform.SetParent(displayBuildings[1].transform);
                    prefabIst.transform.SetAsLastSibling();
                }
            }
            foreach (var structureCard in field.RemotePlayer.InFieldCards[(int)CardType.Structure])
            {
                bool cardFound = false;
                foreach (var cardDisplay in displayBuildings[0].GetComponentsInChildren<CardDisplay>())
                {
                    if (cardDisplay.card == structureCard)
                    {
                        cardDisplay.RefreshValues(CardDisplayType.StructureCard);
                        cardFound = true;
                        break;
                    }

                }
                if (cardFound == false)
                {
                    var prefabIst = Instantiate(BuildingCard);
                    prefabIst.GetComponent<CardDisplay>().SetCard(structureCard);
                    prefabIst.GetComponent<CardDisplay>().RefreshValues(CardDisplayType.StructureCard);
                    prefabIst.transform.SetParent(displayBuildings[0].transform);
                    prefabIst.transform.SetAsLastSibling();
                }
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
    
