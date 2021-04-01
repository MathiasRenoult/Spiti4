using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallStatsManager : MonoBehaviour
{
    public static WallStatsManager singleton;
    EventSystem eventSystem;
    public GameObject specsGameObject;
    public TMPro.TMP_InputField colorInputField;
    public Vector2 colorDefaultPos;
    public TMPro.TMP_InputField widthInputField;
    public Vector2 widthDefaultPos;
    public UnityEngine.UI.Button magnetButton;
    public UnityEngine.UI.Image magnetIcon;
    public UnityEngine.UI.Button strictModeButton;
    public UnityEngine.UI.Image stricModeIcon;
    public Color selectedIconsColor;
    public Color defaultColor;
    public float defaultWidth;

    public bool editingTextFields = false;

    public void Awake()
    {
        singleton = this;
    }
    private void Start()
    {
        colorDefaultPos = colorInputField.transform.position;
        widthDefaultPos = widthInputField.transform.position;
        eventSystem = eventSystem = EventSystem.current;
        SetColorStat(defaultColor);
        SetWidthStat(defaultWidth);
    }
    public void Update()
    {
        if(WallTool.singleton.selected)
        {
            editingTextFields = (colorInputField.isFocused || widthInputField.isFocused);
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
        if(widthInputField.transform.parent.gameObject.activeSelf)
        {
            widthInputField.text = GetWidthStat().ToString();
        }
    }
    public void SetStats()
    {
        if(colorInputField.gameObject.activeSelf)
        {
            SetColorStat(StringToColor(colorInputField.text));
        }
        if(widthInputField.gameObject.activeSelf)
        {
            SetWidthStat(float.Parse(widthInputField.text));
        }
    }
    public Color GetColorStat()
    {
        return WallTool.singleton.defaultColor;
    }
    public float GetWidthStat()
    {
        return WallTool.singleton.defaultWidth;
    }
    public void SetColorStat(Color color)
    {
        WallTool.singleton.defaultColor = color;
    }
    public void SetWidthStat(float width)
    {
        WallTool.singleton.defaultWidth = width;
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

    public void ToggleMagnet()
    {
        WallTool.singleton.magnetButton = !WallTool.singleton.magnetButton;
        if(WallTool.singleton.magnetButton)
        {
            magnetIcon.color = selectedIconsColor;
        }
        else
        {
            magnetIcon.color = Color.white;
        }
    }
    public void ToggleStrictMode()
    {
        WallTool.singleton.strictModeButton = !WallTool.singleton.strictModeButton;
        if(WallTool.singleton.strictModeButton)
        {
            stricModeIcon.color = selectedIconsColor;
        }
        else
        {
            stricModeIcon.color = Color.white;
        }
    }
}
