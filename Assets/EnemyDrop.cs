using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDrop : MonoBehaviour, IDropHandler
{
    DisplayManager displayManager;
    GameObject enemyField;

    public void OnDrop(PointerEventData eventData)
    {


        if (enemyField.transform.childCount > 0)
        {
            displayManager.EnemyCardsSelected = true;
        }

        else
        {

            displayManager.MoveCardOnPlayer(eventData.pointerDrag.GetComponent<CardDisplay>().card);
        }


    }

    // Start is called before the first frame update
    public void InitializeEnemyDrop(DisplayManager displayManager, GameObject EnemyField)
    {
        this.displayManager = displayManager;
        this.enemyField = EnemyField;
        

    }
}
