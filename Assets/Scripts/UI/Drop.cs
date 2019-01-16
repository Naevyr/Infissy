using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

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

        Movement d = eventData.pointerDrag.GetComponent<Movement>();
        if (d != null)
        {
            d.placeholderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Movement d = eventData.pointerDrag.GetComponent<Movement>();
        if (d != null && d.placeholderParent == this.transform)
        {
            d.placeholderParent = d.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Movement d = eventData.pointerDrag.GetComponent<Movement>();
        if (d != null && Card != null) 
        {
            if ((zone == "BuildingsF" && Card.friendly == 1 && Card.type == 2) || (zone == "UnitsF" && Card.friendly == 1 && Card.type == 1) ) {
            d.parentToReturnTo = this.transform;
            }                  
            return;
            /*if (zone != "field")
                return;
            d.parentToReturnTo = this.transform;*/
        }
    }
}
