using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTool : Tool
{
    public static DoorTool singleton;
    public GameObject doorPrefab;
    public GameObject doorPanel;
    public Door currentlyHeldDoor;
    public int heldDoor = -1;
    public bool placing;
    public Vector2 mousePos;
    public Color color;

    private void Awake()
    {
        singleton = this;
    }
    private void Update()
    {
        if(selected && AppManager.singleton.workZone.selected)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(heldDoor != -1 && placing)
            {
                currentlyHeldDoor.SnapToWall();
                if(AppManager.singleton.workZone && Input.GetMouseButtonDown(0))
                {
                    PlaceDoor();
                }
                if(AppManager.singleton.leftShift)
                {
                    currentlyHeldDoor.transform.Rotate(Vector3.forward, Input.mouseScrollDelta.y*6);
                }
                if(AppManager.singleton.leftCtrl)
                {
                    currentlyHeldDoor.transform.localScale *= 1 + Input.mouseScrollDelta.y / 10;
                }
                if(AppManager.singleton.workZone && Input.GetMouseButtonDown(1))
                {
                    heldDoor = -1;
                    placing = false;
                    Destroy(currentlyHeldDoor.gameObject);
                    currentlyHeldDoor = null;
                }  
            } 
        }
    } 
    public void SelectDoor(int index)
    {
        heldDoor = index;
        placing = true;
        currentlyHeldDoor = Instantiate(doorPrefab).GetComponent<Door>();
        currentlyHeldDoor.SetDoor(index);
        currentlyHeldDoor.SetColor(Color.white);
        currentlyHeldDoor.transform.localScale = Vector3.one * (Camera.main.orthographicSize / 30f);
        currentlyHeldDoor.SetColor(color);
    }
    public void PlaceDoor()
    {
        if(AppManager.singleton.leftShift || false)
        {
            Door newDoor = Instantiate(currentlyHeldDoor.gameObject).GetComponent<Door>();
            AppManager.singleton.objects.Add(newDoor);
            AppManager.singleton.SelectObject(newDoor); 
            newDoor.Place();
        }
        else
        {
            AppManager.singleton.objects.Add(currentlyHeldDoor);
            AppManager.singleton.SelectObject(currentlyHeldDoor); 
            heldDoor = -1;
            placing = false;
            currentlyHeldDoor.Place();
            currentlyHeldDoor = null;        
        }
    }
}
