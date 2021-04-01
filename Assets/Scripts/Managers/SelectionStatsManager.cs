using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionStatsManager : MonoBehaviour
{
    public static SelectionStatsManager singleton;
    EventSystem eventSystem;
    public GameObject specsGameObject;
    public TMPro.TMP_InputField colorInputField;
    public Vector2 colorDefaultPos;
    public TMPro.TMP_InputField lengthInputField;
    public Vector2 lengthDefaultPos;
    public TMPro.TMP_InputField angleInputField;
    public Vector2 angleDefaultPos;
    public TMPro.TMP_InputField widthInputField;
    public Vector2 widthDefaultPos;
    public int specsMode = -1; //0 = wall, 1 = furniture, 2 = door
    public bool editingTextFields = false;

    public void Awake()
    {
        singleton = this;
    }
    private void Start()
    {
        colorDefaultPos = colorInputField.transform.position;
        lengthDefaultPos = lengthInputField.transform.position;
        angleDefaultPos = angleInputField.transform.position;
        widthDefaultPos = widthInputField.transform.position;
        eventSystem = eventSystem = EventSystem.current;
    }
    public void Update()
    {
        editingTextFields = (colorInputField.isFocused || lengthInputField.isFocused || widthInputField.isFocused || angleInputField.isFocused);
        if(!editingTextFields)
        {
            if(AppManager.singleton.selectedObjects.Count == 1 && SelectionTool.singleton.selected)
            {
                if(AppManager.singleton.selectedObjects[0] is Wall) SetSpecsMode(0);
                if(AppManager.singleton.selectedObjects[0] is Furniture) SetSpecsMode(1);
                if(AppManager.singleton.selectedObjects[0] is Door) SetSpecsMode(2);
            }
            else
            {
                if(specsMode != -1) SetSpecsMode(-1);
            }
        }
    }
    public void SetSpecsMode(int mode)
    {
        specsMode = mode;
        specsGameObject.SetActive(true);
        angleInputField.transform.parent.gameObject.SetActive(true);
        angleInputField.transform.parent.transform.position = new Vector3(angleDefaultPos.x, angleInputField.transform.position.y);
        colorInputField.transform.parent.gameObject.SetActive(true);
        colorInputField.transform.parent.transform.position = new Vector3(colorDefaultPos.x, colorInputField.transform.position.y);
        lengthInputField.transform.parent.gameObject.SetActive(true);
        lengthInputField.transform.parent.transform.position = new Vector3(lengthDefaultPos.x, lengthInputField.transform.position.y);
        widthInputField.transform.parent.gameObject.SetActive(true);
        widthInputField.transform.parent.transform.position = new Vector3(widthDefaultPos.x, widthInputField.transform.position.y);


        switch(mode)
        {
            case -1:
                specsGameObject.SetActive(false);
            break;
            case 0:
            break;
            case 1:
                lengthInputField.transform.parent.gameObject.SetActive(false);
                widthInputField.transform.parent.gameObject.SetActive(false);
                angleInputField.transform.parent.transform.position = new Vector3(lengthDefaultPos.x, angleInputField.transform.position.y);
            break;
            case 2:
                lengthInputField.transform.parent.gameObject.SetActive(false);
                widthInputField.transform.parent.gameObject.SetActive(false);
                angleInputField.transform.parent.transform.position = new Vector3(lengthDefaultPos.x, angleInputField.transform.position.y);
            break;
        }
        if(specsMode != -1)
        {
            GetStats(); 
        }
    }

    public void GetStats()
    {
        if(colorInputField.transform.parent.gameObject.activeSelf)
        {
            colorInputField.text = "#" + ColorToString(GetColorStat());
        }
        if(angleInputField.transform.parent.gameObject.activeSelf)
        {
            angleInputField.text = GetAngleStat().ToString();
        }
        if(lengthInputField.transform.parent.gameObject.activeSelf)
        {
            lengthInputField.text = GetLengthStat().ToString();
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
        if(angleInputField.gameObject.activeSelf)
        {
            SetAngleStat(float.Parse(angleInputField.text));
        }
        if(lengthInputField.gameObject.activeSelf)
        {
            SetLengthStat(float.Parse(lengthInputField.text));
        }
        if(widthInputField.gameObject.activeSelf)
        {
            SetWidthStat(float.Parse(widthInputField.text));
        }
    }
    public Color GetColorStat()
    {
        return AppManager.singleton.selectedObjects[0].color;
    }
    public float GetLengthStat()
    {
        return (AppManager.singleton.selectedObjects[0] as Wall).length;
    }
    public float GetWidthStat()
    {
        return (AppManager.singleton.selectedObjects[0] as Wall).width;
    }
    public float GetAngleStat()
    {   
        return AppManager.singleton.selectedObjects[0].angle;
    }   
    public void SetColorStat(Color color)
    {
        AppManager.singleton.selectedObjects[0].SetColor(color);
    }
    public void SetLengthStat(float length)
    {
        (AppManager.singleton.selectedObjects[0] as Wall).length = length;
        AppManager.singleton.UpdateWall((AppManager.singleton.selectedObjects[0] as Wall), false, true, false);
    }
    public void SetWidthStat(float width)
    {
        (AppManager.singleton.selectedObjects[0] as Wall).width = width;
        AppManager.singleton.ModifyWall((AppManager.singleton.selectedObjects[0] as Wall), (AppManager.singleton.selectedObjects[0] as Wall).line.GetPosition(0), (AppManager.singleton.selectedObjects[0] as Wall).line.GetPosition(1), width, AppManager.singleton.selectedObjects[0].color);
    }
    public void SetAngleStat(float angle)
    {
        AppManager.singleton.selectedObjects[0].angle = angle;
        if(AppManager.singleton.selectedObjects[0] is Wall)
            AppManager.singleton.UpdateWall((AppManager.singleton.selectedObjects[0] as Wall), false, true, false);
        else
        {
            AppManager.singleton.selectedObjects[0].transform.rotation = Quaternion.Euler(0,0,-angle);
            (AppManager.singleton.selectedObjects[0] as Furniture).Place();
        }
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

    public void SetEditVar(TMPro.TMP_InputField inputField)
    {
    }
}
