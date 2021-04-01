using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileMenuManager : MonoBehaviour
{
    public GameObject buttonBox;
    public static FileMenuManager singleton;
    private void Awake()
    {
        singleton = this;
    }
    public void FileItemClicked(int index)
    {
        switch(index)
        {
            case 0:
                FileSaver.singleton.SaveFile(FileSaver.singleton.currentlyOpenedFile);
            break;
            case 1:
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            break;
            case 2:
                FileSaver.singleton.SaveFile(FileSaver.singleton.currentlyOpenedFile);
                WindowManager.singleton.QuitApplication();
            break;
        }
    }

    public void FileButtonClicked()
    {
        buttonBox.SetActive(!buttonBox.activeSelf);
    }
}
