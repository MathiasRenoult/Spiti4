using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager singleton;
    public GameObject NewPlanPanel;
    public GameObject OptionPanel;
    public GameObject CreditsPanel;
    public GameObject LoadPlansPanel;
    public TMPro.TMP_InputField inputField;
    public MenuButton[] buttons;
    private void Awake()
    {
        singleton = this;
        buttons = gameObject.GetComponentsInChildren<MenuButton>();
    }
    public void NewPlanClick()
    {
        if(!NewPlanPanel.activeSelf)
            NewPlanPanel.SetActive(true);
        else
            NewPlanPanel.SetActive(false);
    }
    public void LoadPlan(ProjectBox box)
    {
        StartCoroutine(LoadYourAsyncScene(box.text.text));
    }
    public void CreateNewPlan()
    {
        FileSaver.singleton.CreateFile(inputField.text);
        PlayerPrefs.SetString("openedFile", inputField.text);
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);   
    }
    public void LoadPlanClick()
    {
        if(!LoadPlansPanel.activeSelf)
            LoadPlansPanel.SetActive(true);
        else
            LoadPlansPanel.SetActive(false);
    }

    public void OptionsClick()
    {
        if(!OptionPanel.activeSelf)
            OptionPanel.SetActive(true);
        else
            OptionPanel.SetActive(false);
    }

    public void CreditsClick()
    {
        if(!CreditsPanel.activeSelf)
            CreditsPanel.SetActive(true);
        else
            CreditsPanel.SetActive(false);
    }

    IEnumerator LoadYourAsyncScene(string name)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        PlayerPrefs.SetString("openedFile", name);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
