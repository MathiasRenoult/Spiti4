using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    public GameObject projectBoxPrefab;
    public Transform spawnPos;
    public static ProjectManager singleton;
    public string path;
    public List<Vector2> boxPosList = new List<Vector2>();
    public List<GameObject> boxes = new List<GameObject>();
    public string fileExtension;
    void Awake()
    {
        singleton = this;
        path = Application.persistentDataPath + "/Projects";
    }
    void Start()
    {
        if(boxes != null) foreach(GameObject g in boxes) boxPosList.Add(g.transform.position);
        DeleteAllBoxes();
        FindFiles();
    }
    public void Refresh()
    {
        DeleteAllBoxes();
        FindFiles();
    }
    public void SelectPlan(ProjectBox box)
    {
       MenuManager.singleton.LoadPlan(box);
    }
    public void DeletePlan(ProjectBox box)
    {
        print(box.name + "deleted !");
        print(path + "\\" + box.text.text + "." + fileExtension);
        System.IO.File.Delete(path + "\\" + box.text.text + "." + fileExtension);
        Refresh();
    }
    public void FindFiles()
    {
        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();
        int i = 0;
        foreach (string file in System.IO.Directory.GetFiles(path))
        { 
            if(file.Substring(file.Length - fileExtension.Length) == fileExtension)
            {
                ProjectBox currentBox = Instantiate(projectBoxPrefab, boxPosList[i], Quaternion.identity, spawnPos).GetComponent<ProjectBox>();
                boxes.Add(currentBox.gameObject);
                currentBox.text.text = file.Split('\\')[1].Split('.')[0];
                currentBox.image.color = Color.HSVToRGB((Mathf.Abs(currentBox.text.text.GetHashCode())%512)/512f, 1f, 1f);
                currentBox.crossButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() => ProjectManager.singleton.DeletePlan(currentBox)));
                currentBox.imageButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() => ProjectManager.singleton.SelectPlan(currentBox)));
                i++;  
            }
        }
    }
    public void DeleteAllBoxes()
    {
        foreach(GameObject g in boxes)
        {
            Destroy(g);
        }

        boxes.Clear();
    }
}
