using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DefausseZone : MonoBehaviour, IDropHandler
{
    [SerializeField] public Image image;
    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<DragDrop>().OnDefausseZone();
            this.image.sprite = eventData.pointerDrag.GetComponent<DragDrop>().getSprite();
        }
    }
}
