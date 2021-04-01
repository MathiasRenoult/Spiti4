using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureStatsManager : MonoBehaviour
{
    public static FurnitureStatsManager singleton;
    EventSystem eventSystem;
    public GameObject specsGameObject;
    public TMPro.TMP_InputField colorInputField;
    public Vector2 colorDefaultPos;
    public bool editingTextFields = false;

    public void Awake()
    {
        singleton = this;
    }
    private void Start()
    {
        colorDefaultPos = colorInputField.transform.position;
        eventSystem = eventSystem = EventSystem.current;
    }
    public void Update()
    {
        if(FurnitureTool.singleton.selected)
        {
            editingTextFields = colorInputField.isFocused;
            if(!editingTextFields)
            {
                GetStats();
            } 
        }
    }
    public void GetStats()
    {
        if(colorInputField.transform.parent.gameObject.activeSelf)
        {
            colorInputField.text = "#" + ColorToString(GetColorStat());
        }
    }
    public void SetStats()
    {
        if(colorInputField.gameObject.activeSelf)
        {
            SetColorStat(StringToColor(colorInputField.text));
        }
    }
    public Color GetColorStat()
    {
        return FurnitureTool.singleton.color;
    }
    public void SetColorStat(Color color)
    {
        FurnitureTool.singleton.color = color;
    }
    public Color StringToColor(string colorString)
    {
        Color colorColor;
        ColorUtility.TryParseHtmlString(colorString, out colorColor);
        return colorColor;
    }
    public string ColorToString(Color color)
    {   
        return ColorUtility.ToHtmlStringRGB(color);
    }
}
