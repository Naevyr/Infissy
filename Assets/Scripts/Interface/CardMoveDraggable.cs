using Infissy.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMoveDraggable : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler, IDropHandler
{
    Card[] targetBuffer;
    int targetBufferIndex;
    Card cardPointer;
    Vector3 positionBuffer;
    bool IsDragable = false;

    // Start is called before the first frame update
    void Start()
    {
        cardPointer = GetComponent<CardDisplay>().card;
        targetBuffer = new Card[cardPointer.Effects.Length];
           
        if(cardPointer.Type == Infissy.Properties.CardProperties.CardType.Attack)
        {
            IsDragable = true;
        }
    }


    public void SetTarget(Card card, Vector3 cardTargetPosition)
    {
        targetBuffer[targetBufferIndex] = card;
        targetBufferIndex++;
        Vector3[] lineExtremes = new Vector3[] { cardTargetPosition, positionBuffer };
        GetComponent<LineRenderer>().SetPositions(lineExtremes);
        if(targetBuffer.Length == targetBufferIndex)
        {
            var displayManager = GameObject.FindGameObjectWithTag("DisplayManager").GetComponent<DisplayManager>();
            displayManager.MoveCard(this.GetComponent<CardDisplay>().card, targetBuffer);
            
        }
    }




    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        if (IsDragable == false )
            return;
        else
            positionBuffer = transform.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        transform.position = positionBuffer;

    }

    public void OnDrop(PointerEventData eventData)
    {
        

        Debug.Log(transform.position + " " + eventData.pointerDrag.GetComponent<CardDisplay>().card.IDCard + " " + GetComponent<CardDisplay>().card.IDCard);
        eventData.pointerDrag.GetComponent<CardMoveDraggable>().SetTarget(this.GetComponent<CardDisplay>().card, transform.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        GetComponent<LineRenderer>().SetPositions(new Vector3[]{ positionBuffer, eventData.position});
    }
}
