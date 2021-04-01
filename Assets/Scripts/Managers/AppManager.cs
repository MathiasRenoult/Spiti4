using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AppManager : MonoBehaviour
{
    public static AppManager singleton;
    public List<Object> objects = new List<Object>();
    public List<Object> selectedObjects = new List<Object>();
    public List<Object> clipboard = new List<Object>();
    public WorkZone workZone;
    public Material gridMat;
    public GameObject wallPrefab;
    public Vector2 mousePos;
    public Vector2 oldMousePos;
    public bool movingCam;
    public bool ctrlKeys;
    public bool leftCtrl;
    public bool leftShift;
    public bool leftAlt;

    private void Awake()
    {
        singleton = this;
    }
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(!leftShift && !leftCtrl)
            Camera.main.orthographicSize -= Input.mouseScrollDelta.y * (Camera.main.orthographicSize/10);
        RefreshObjects();
        CheckKeyboardInputs();
        if(movingCam)
            MoveCam();
    }
    public void AddWall(Vector2 startPos, Vector2 endPos, float width, Color startColor, Color endColor)
    {
        Wall newWall = Instantiate(wallPrefab).GetComponent<Wall>();
        Vector3[] positions = {(Vector3)startPos, (Vector3)endPos};
        newWall.line.SetPositions(positions);
        newWall.line.startColor = startColor;
        newWall.line.endColor = endColor;
        newWall.line.startWidth = width;
        newWall.line.endWidth = width;
        newWall.angle = Mathf.Atan((newWall.line.GetPosition(1).x - newWall.line.GetPosition(0).x)  / (newWall.line.GetPosition(1).y - newWall.line.GetPosition(0).y));
        newWall.length = (newWall.line.GetPosition(0) - newWall.line.GetPosition(1)).magnitude;
        objects.Add(newWall);
    }

    public void AddWall(Wall wall)
    {
        AddWall(wall.line.GetPosition(0), wall.line.GetPosition(1), wall.line.startWidth, wall.line.startColor, wall.line.endColor);
    }

    public void AddWall(Vector2 startPos, float angle, float length, float width, Color startColor, Color endColor)
    {
        Vector2 endPos = new Vector2(startPos.x + Mathf.Sin(angle) * length, startPos.y + Mathf.Cos(angle) * length);
        AddWall(startPos, endPos, width, startColor, endColor);
    }

    public void ModifyWall(Wall oldWall, Vector2 startPos, Vector2 endPos, float width, Color color)
    {
        oldWall.line.SetPosition(0, startPos);
        oldWall.line.SetPosition(1, endPos);
        oldWall.line.startWidth = width;
        oldWall.line.endWidth = width;
        oldWall.line.startColor = color;
        oldWall.line.endColor = color;
    }

    public void ModifyWall(Wall oldWall, Wall newWall)
    {
        ModifyWall(oldWall, newWall.line.GetPosition(0), newWall.line.GetPosition(1), newWall.line.startWidth, newWall.color);
    }

    public void UpdateWall(Wall wall, bool byPositions = true, bool byAngleAndLength = false, bool byEndPoint = false)
    {
        if(byPositions)
        {
            wall.angle = (Mathf.Atan2((wall.line.GetPosition(1).x - wall.line.GetPosition(0).x), (wall.line.GetPosition(1).y - wall.line.GetPosition(0).y)) * (180f / Mathf.PI) + 360f) % 360;
            wall.length = (wall.line.GetPosition(0) - wall.line.GetPosition(1)).magnitude;
            return;
        }

        if(byAngleAndLength)
        {
            if(!byEndPoint)
            {
                wall.line.SetPosition(1, new Vector3(wall.line.GetPosition(0).x + Mathf.Sin(wall.angle * (Mathf.PI / 180f)) * wall.length, wall.line.GetPosition(0).y + Mathf.Cos(wall.angle * (Mathf.PI / 180f)) * wall.length));   
            }
            else
            {
                wall.line.SetPosition(0, new Vector3(wall.line.GetPosition(1).x + Mathf.Sin(wall.angle * (Mathf.PI / 180f)) * wall.length, wall.line.GetPosition(1).y + Mathf.Cos(wall.angle * (Mathf.PI / 180f)) * wall.length));  
            }
            return;
        }
    }

    void RefreshObjects()
    {
        foreach(Object o in selectedObjects)
        {
            if(o != null)
            {
                o.selected = true;
                o.Refresh();
            }
        }
    }
    void CheckKeyboardInputs()
    {
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftWindows) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.RightShift))
            ctrlKeys = true;
        else
            ctrlKeys = false;
        
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            List<Object> copy = new List<Object>();
            foreach(Object o in objects)
            {
                if(o.selected)
                    copy.Add(o);
            }
            selectedObjects.Clear();
            foreach(Object o in copy)
            {
                objects.Remove(o);
                Destroy(o.gameObject);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.LeftControl))
            leftCtrl = true;
        
        if(Input.GetKeyUp(KeyCode.LeftControl))
            leftCtrl = false;

        if(leftCtrl)
        {
            if(Input.GetKeyDown(KeyCode.C)) CopySelected();
            if(Input.GetKeyDown(KeyCode.V)) CopySelected();
            if(Input.GetKeyDown(KeyCode.D)) DeselectAllSelectedObjects();
            if(Input.GetKeyDown(KeyCode.A)) SelectAllObjects();
            if(Input.GetKeyDown(KeyCode.S)) FileSaver.singleton.SaveFile(FileSaver.singleton.currentlyOpenedFile);
            if(Input.GetKeyDown(KeyCode.Q)) WindowManager.singleton.QuitApplication();
            if(Input.GetKeyDown(KeyCode.M)) FileMenuManager.singleton.FileItemClicked(1);
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.W)) ToolSelector.singleton.SelectTool(0);
            if(Input.GetKeyDown(KeyCode.S)) ToolSelector.singleton.SelectTool(1);
            if(Input.GetKeyDown(KeyCode.F)) ToolSelector.singleton.SelectTool(2);
            if(Input.GetKeyDown(KeyCode.D)) ToolSelector.singleton.SelectTool(3);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
            leftShift = true;
        
        if(Input.GetKeyUp(KeyCode.LeftShift))
            leftShift = false;
        
        if(Input.GetMouseButtonDown(2))
        {
            movingCam = true;
            oldMousePos = mousePos;
        }
        if(Input.GetMouseButtonUp(2))
        {
            movingCam = false;
        }
    }

    void MoveCam()
    {
        Camera.main.transform.position += (Vector3)(oldMousePos - mousePos);
    }
    public void SelectObject(Object o)
    {
        if(!selectedObjects.Find(x => x.GetInstanceID() == o.GetInstanceID()))
        {
            selectedObjects.Add(o);
            o.selected = true;            
        }
    }
    public void SelectAllObjects()
    {
        foreach(Object o in objects)
        {
            selectedObjects.Add(o);
        }
    }
    public void DeselectAllSelectedObjects()
    {
        foreach(Object o in selectedObjects)
        {
            if(o != null)
            {
                o.selected = false;
                o.Refresh();
            }
        }
        selectedObjects.Clear();
    }
    public void DeselectObject(Object o)
    {
        o.selected = false;
        selectedObjects.Remove(o);
    }
    public void DestroyObject(Object o)
    {
        objects.Remove(o);
        Destroy(o.gameObject);
    }
    public void DestroyAllObjects()
    {
        int index = objects.Count;
        selectedObjects.Clear();
        for(int i = 0; i<index; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }

    public void CopySelected()
    {
        clipboard.Clear();
        foreach(Object o in selectedObjects)
        {
            clipboard.Add(o);
        }
    }
    public void PasteSelected()
    {
        foreach(Object o in clipboard)
        {
            Object o2 = Instantiate(o.gameObject, o.transform.position + (Vector3.right * 45f), o.transform.rotation).GetComponent<Object>();
            if(o is Wall)
            {
                (o2 as Wall).line.SetPosition(0, (o as Wall).line.GetPosition(0) + (Vector3.right * 45f));
                (o2 as Wall).line.SetPosition(1, (o as Wall).line.GetPosition(1) + (Vector3.right * 45f));
            }
            o.selected = false;
            o2.selected = true;
            objects.Add(o2);
        }
    }
}
