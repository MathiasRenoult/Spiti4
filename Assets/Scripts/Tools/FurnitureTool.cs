using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureTool : Tool
{
    public static FurnitureTool singleton;
    public GameObject furniturePrefab;
    public Furniture currentlyHeldFurniture;
    public GameObject furniturePanel;
    public bool placing;
    public int heldFurniture;
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
            if(heldFurniture != -1 && placing)
            {
                currentlyHeldFurniture.transform.position = mousePos;
                if(AppManager.singleton.workZone && Input.GetMouseButtonDown(0))
                {
                    PlaceFurniture();
                }
                if(AppManager.singleton.leftShift)
                {
                    currentlyHeldFurniture.transform.Rotate(Vector3.forward, Input.mouseScrollDelta.y*6);
                }
                if(AppManager.singleton.leftCtrl)
                {
                    currentlyHeldFurniture.transform.localScale *= 1 + Input.mouseScrollDelta.y / 10;
                }
                if(AppManager.singleton.workZone && Input.GetMouseButtonDown(1))
                {
                    heldFurniture = -1;
                    placing = false;
                    Destroy(currentlyHeldFurniture.gameObject);
                    currentlyHeldFurniture = null;
                }
            }
        }
    }
    public void SelectFurniture(int index)
    {
        heldFurniture = index;
        placing = true;
        currentlyHeldFurniture = Instantiate(furniturePrefab).GetComponent<Furniture>();
        currentlyHeldFurniture.SetShape(index);
        currentlyHeldFurniture.transform.localScale = Vector3.one * (Camera.main.orthographicSize / 20f);
        currentlyHeldFurniture.SetColor(color);
    }

    public void PlaceFurniture()
    {
        if(AppManager.singleton.leftShift || false)
        {
            Furniture newFurniture = Instantiate(currentlyHeldFurniture.gameObject).GetComponent<Furniture>();
            AppManager.singleton.SelectObject(newFurniture); 
            AppManager.singleton.objects.Add(newFurniture);
            newFurniture.Place();
        }
        else
        {
            AppManager.singleton.objects.Add(currentlyHeldFurniture);
            AppManager.singleton.SelectObject(currentlyHeldFurniture); 
            heldFurniture = -1;
            placing = false;
            currentlyHeldFurniture.Place();
            currentlyHeldFurniture = null;        
        }
        
    }
}
