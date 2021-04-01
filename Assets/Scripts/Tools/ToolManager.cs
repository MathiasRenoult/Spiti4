using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolManager : MonoBehaviour
{
    public static ToolManager singleton;
    public Tool[] tools;
    public WorkZone workZone;

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        tools = GetComponentsInChildren<Tool>();
    }
}
