using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WorkZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool selected = false;
    // Start is called before the first frame update
    public void OnPointerEnter(PointerEventData eventData)
    {
        selected = true;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        selected = false;
    }
}
