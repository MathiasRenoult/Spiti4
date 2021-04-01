using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelector : MonoBehaviour
{
    public static ToolSelector singleton;
    public ToolSelectorButton[] toolButtons;
    public ToolSelectorButton selectedButton;


    void Awake()
    {
        singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        toolButtons = GetComponentsInChildren<ToolSelectorButton>();
    }

    public void SelectTool(ToolSelectorButton button)
    {
        if(button.associatedTool == null)
            return;

        if(button.specs != null)
            button.specs.SetActive(true);

        foreach(Tool t in ToolManager.singleton.tools)
        {
            t.selected = false;
            t.button.GetComponent<UnityEngine.UI.Image>().color = new Color(0.894f, 0.341f, 0.18f, 1f);
            if(t.button.specs != null && t.button != button)
                t.button.specs.SetActive(false);
        }
        foreach(Tool t in ToolManager.singleton.tools)
        {
            if(button.associatedTool == t)
            {
                t.selected = true;
                selectedButton = button;
                selectedButton.GetComponent<UnityEngine.UI.Image>().color = new Color(0.659f, 0.776f, 0.525f, 1f);
                return;
            }
        }
    }

    public void SelectTool(int index)
    {
        SelectTool(toolButtons[index]);
    }
}
