using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using InfissyProperties;
public class Drop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string zone ="";
    public Card Card;
    public int n;

    
    void OnTriggerEnter2D(Collider2D other)
    {
        Card = other.gameObject.GetComponent<CardDisplay>().card;

        Debug.Log(Card.title + n);
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Movement movement = eventData.pointerDrag.GetComponent<Movement>();
        if (movement != null)
        {
            movement.placeholderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Movement movement = eventData.pointerDrag.GetComponent<Movement>();
        if (movement != null && movement.placeholderParent == this.transform)
        {
            movement.placeholderParent = movement.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Movement movement = eventData.pointerDrag.GetComponent<Movement>();
        if (movement != null && Card != null) 
        {
            if ((zone == "BuildingsF" && Card.friendly == true && Card.type == CardProperties.CardType.Structure) || (zone == "UnitsF" && Card.friendly == true && Card.type == CardProperties.CardType.Attack) ) {
            movement.parentToReturnTo = this.transform;
            }                  
            return;
            /*if (zone != "field")
                return;
            movement.parentToReturnTo = this.transform;*/
        }
    }
}
