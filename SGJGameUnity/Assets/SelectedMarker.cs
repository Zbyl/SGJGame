using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class SelectedMarker : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public Transform selectedMarker;
    public float horizontalOffset = 0.0f;
    public float verticalOffset = 0.0f;

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was selected");
        selectedMarker.position = gameObject.transform.position + Vector3.right * horizontalOffset + Vector3.down * verticalOffset;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("The cursor entered the selectable UI element.");
        OnSelect(eventData);
    }
}
