using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using static Infissy.Properties.CardProperties;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public GameObject prefab;


    public void OnPointerEnter(PointerEventData eventData) {
		//Debug.Log("OnPointerEnter");
		if(eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null) {
			d.placeholderParent = this.transform;
		}
	}
	
	public void OnPointerExit(PointerEventData eventData) {
		//Debug.Log("OnPointerExit");
		if(eventData.pointerDrag == null)
        

           
        return;

        
        
        
		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null && d.placeholderParent==this.transform) {
			d.placeholderParent = d.parentToReturnTo;
		}
	}
	
	public void OnDrop(PointerEventData eventData) {
        if(eventData.pointerDrag.GetComponent<CardDisplay>().card.Type == CardType.Attack && gameObject.name.Contains("Field"))
        {
          
                Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);



                Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
                GameObject.FindGameObjectWithTag("DisplayManager").GetComponent<DisplayManager>().PlayCard(d, transform);


                if (d != null)
                {
                    d.parentToReturnTo = this.transform;
                }
            
        }
        if (eventData.pointerDrag.GetComponent<CardDisplay>().card.Type == CardType.Structure && gameObject.name.Contains("Buildings"))
        {
            
                Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);



                Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
                GameObject.FindGameObjectWithTag("DisplayManager").GetComponent<DisplayManager>().PlayCard(d, transform);


                if (d != null)
                {
                    d.parentToReturnTo = this.transform;
                }
            
        }




    }
}
