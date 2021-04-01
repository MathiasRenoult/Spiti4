using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;


public class FileSaver : MonoBehaviour
{
    public static FileSaver singleton;
    public string currentlyOpenedFile;
    public string path;
    public string fileExtension;
    private void Awake()
    {
        singleton = this;
    }
    public void SetOpenedFile(string name)
    {
        currentlyOpenedFile = name;
        PlayerPrefs.SetString("openedFile", name);
    }
    private void Start()
    {
        path = Application.persistentDataPath;

        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainScene"))
        {
            currentlyOpenedFile = PlayerPrefs.GetString("openedFile");
            LoadFile(currentlyOpenedFile);
        }
        else
        {
            currentlyOpenedFile = "";
        }
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            SaveFile(currentlyOpenedFile);
        }
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L))
        {
            LoadFile(currentlyOpenedFile);
        }
    }
    public void CreateFile(string name)
    {
        System.IO.File.Create(SetFullPath(name));
    }
    public void SaveFile(string name)
    {
        FileStream stream;

        ObjectDataCollection collectionInstance = new ObjectDataCollection();
        foreach(Object o in AppManager.singleton.objects)
        {
            int objectType = -1;
            int spriteIndex = -1;
            float width = -1;
            Vector2 startPos = Vector2.zero;
            Vector2 endPos = Vector2.zero;

            if(o is Wall)
            {
                objectType = 0;
                startPos = (o as Wall).line.GetPosition(0);
                endPos = (o as Wall).line.GetPosition(1);
                width = (o as Wall).line.startWidth;
            }
            else
            {
                if(o is Furniture)
                {
                    objectType = 1;
                    spriteIndex = (o as Furniture).spriteIndex;  
                } 
                else
                {
                    objectType = 2; 
                    spriteIndex = (o as Door).spriteIndex;
                } 
            }

            ObjectData dataInstance = new ObjectData(objectType, o.transform.position, o.transform.rotation, o.transform.localScale, o.color, o.selected, startPos, endPos, width, spriteIndex);
            collectionInstance.collection.Add(dataInstance);
        }

        string json = JsonHelper.ToJson<ObjectData>(collectionInstance.collection.ToArray());
        stream = new FileStream(SetFullPath(name), FileMode.OpenOrCreate);
        using(var file = new StreamWriter(stream))
        {
            file.Flush();
            file.Write(json);
        }
        stream.Close();
        print("Saved !");
    }
    public void LoadFile(string name)
    {
        string json = File.ReadAllText(SetFullPath(name));
        ObjectData[] loadedData =  JsonHelper.FromJson<ObjectData>(json);
        if(AppManager.singleton) AppManager.singleton.DestroyAllObjects();
        foreach(ObjectData o in loadedData)
        {
            switch(o.objectType)
            {
                case 0:
                    print("Loading wall");
                    Wall newWall = Instantiate(AppManager.singleton.wallPrefab).GetComponent<Wall>();
                    newWall.Construct(o.position, o.rotation, o.color, o.selected, o.startPoint, o.endPoint, o.width);
                    newWall.SetColliders();
                    AppManager.singleton.objects.Add(newWall);
                break;
                case 1:
                    Furniture newFurniture = Instantiate(FurnitureTool.singleton.furniturePrefab).GetComponent<Furniture>();
                    newFurniture.Construct(o.position, o.rotation, o.scale, o.color, o.selected, o.spriteIndex);
                    newFurniture.Place();
                    AppManager.singleton.objects.Add(newFurniture);
                break;
                case 2:
                    Door newDoor = Instantiate(DoorTool.singleton.doorPrefab).GetComponent<Door>();
                    newDoor.Construct(o.position, o.rotation, o.scale, o.color, o.selected, o.spriteIndex);
                    newDoor.Place();
                    AppManager.singleton.objects.Add(newDoor);
                break;
            }
        }  
    }

    private string SetFullPath(string name)
    {
        if(System.IO.Directory.Exists( path + "/Projects"))
        {
            return path + "/Projects/" + name + "." + fileExtension; 
        }
        else
        {
            System.IO.Directory.CreateDirectory(path + "/Projects");
            return path + "/Projects/" + name + "." + fileExtension;
        }
    }
}
