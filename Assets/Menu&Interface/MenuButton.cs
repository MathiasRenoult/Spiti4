using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    Button button;
    TMPro.TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        if(!button)
            button = gameObject.GetComponent<Button>();
        if(!text)
            text = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeTextColor(new Color(0.953f, 0.655f, 0.071f, 1f));
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeTextColor(new Color(0.659f, 0.776f, 0.525f, 1f));
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ChangeTextColor(new Color(0.953f, 0.655f, 0.071f, 1f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeTextColor(new Color(0.161f, 0.2f, 0.361f, 1f));
    }

    public void ChangeTextColor(Color color)
    {
        string[] splits = text.text.Split('>');
        text.text = "<color=" + HexConverter(color) + ">" + splits[splits.Length-1];
    }

    private string HexConverter(Color c)
    {
        //print(c.r + " " + c.g + " " + c.b);
        return "#" + ((int)(c.r * 255f)).ToString("X2") + ((int)(c.g * 255f)).ToString("X2") + ((int)(c.b * 255f)).ToString("X2");
    }
}

