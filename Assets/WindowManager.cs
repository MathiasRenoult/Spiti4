using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
public class WindowManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    public GameObject closingText;
    public static WindowManager singleton;
    private void Awake()
    {
        singleton = this;
    }

    public void QuitApplication()
    {        
        closingText.SetActive(true);
        Application.Quit();
    }

    public void WindowFullscreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
 
    public void Minimize()
    {
        ShowWindow(GetActiveWindow(), 2);
    }
}
