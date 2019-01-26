using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 offset = new Vector2(0, 0);
    public Transform parentBack = null;
    public Transform placeholderParent = null;

    GameObject placeholder = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag" + eventData.position);
        Vector2 ep = eventData.position;
        Vector2 tp = this.transform.position;
        offset.Set(ep.x - tp.x, ep.y - tp.y);
        parentBack = this.transform.parent;
        this.transform.SetParent(parentBack.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
        placeholderParent = parentBack;

        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 ep = eventData.position;
        this.transform.position = new Vector2(ep.x - offset.x, ep.y - offset.y);
        if (placeholder.transform.parent != placeholderParent)
        {
            placeholder.transform.SetParent(placeholderParent);
        }
        int signIndex = placeholderParent.childCount;
        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            float x = placeholderParent.GetChild(i).position.x;
            if (eventData.position.x < x)
            {
                signIndex = i;
                if(placeholder.transform.GetSiblingIndex() < signIndex)
                {
                    signIndex--;
                }
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(signIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        this.transform.SetParent(parentBack);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(placeholder);
    }
}
