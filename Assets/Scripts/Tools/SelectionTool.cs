using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTool : Tool
{
    public static SelectionTool singleton;

    public bool selecting = false;
    public bool moving = false;
    public bool movingDot = false;
    public bool movingCam = false;
    public bool editMode = false;
    public bool rotating = false;
    public bool handCursorActive = false;
    public bool resizing;

    public GameObject selectRectPrefab;
    public LineRenderer selectRectLine;
    public BoxCollider2D selectRectCollider;
    public Vector2 oldMousePos;
    public Vector2 mousePos;
    public Texture2D standardCursor;
    public Texture2D handCursor;
    public Collider2D pickedCollider;
    public Furniture pickedFurniture;
    public Door pickedDoor;

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        if(selected && AppManager.singleton.workZone.selected)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CheckKeyboardInputs();

            if(editMode)
            {
                Edit();
            }
            else
            {
                CheckSelecting();
                if(rotating)
                    Rotate();
                if(moving)
                    Move(); 
                if(resizing && pickedCollider != null && pickedFurniture != null)
                    ResizingFurniture();
            }

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if(hit.collider != null && hit.collider.CompareTag("FurnitureEditDot") || pickedCollider != null)
            {
                if(!handCursorActive)
                {
                    handCursorActive = true;
                    Cursor.SetCursor(handCursor, new Vector2(32, 0), CursorMode.Auto);
                }
            }
            else
            {
                if(handCursorActive)
                {
                    handCursorActive = false;
                    Cursor.SetCursor(standardCursor, new Vector2(16, 16), CursorMode.Auto);    
                }
            }
        }
    }

    
    public void StartSelectRect()
    {
        oldMousePos = mousePos;
        selecting = true;
        selectRectLine = Instantiate(selectRectPrefab).GetComponent<LineRenderer>();
        selectRectLine.gameObject.SetActive(true);
        selectRectLine.positionCount = 0;
        selectRectLine.positionCount = 4;
        selectRectLine.startWidth = Camera.main.orthographicSize / 120f;
        selectRectLine.endWidth = Camera.main.orthographicSize / 120f;
        selectRectLine.SetPosition(0, oldMousePos);
    }
    public void StopSelectRect()
    {   
        if(selecting)
        {
            selecting = false;
            selectRectCollider = selectRectLine.gameObject.GetComponent<BoxCollider2D>();
            selectRectCollider.offset = new Vector2((mousePos.x + oldMousePos.x) / 2, (mousePos.y + oldMousePos.y) / 2);
            selectRectCollider.size = new Vector2(Mathf.Abs(mousePos.x - oldMousePos.x), Mathf.Abs(mousePos.y - oldMousePos.y));
            if(!AppManager.singleton.leftShift)
                AppManager.singleton.DeselectAllSelectedObjects();

            if((mousePos-oldMousePos).sqrMagnitude < 64f)
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if(hit.collider != null && hit.collider.CompareTag("object"))
                {
                    AppManager.singleton.SelectObject(hit.collider.gameObject.GetComponent<Object>());
                }
                else
                {
                    AppManager.singleton.DeselectAllSelectedObjects();
                }
            }
            else
            {
                List<Collider2D> colliders = new List<Collider2D>();
                ContactFilter2D contactFilter = new ContactFilter2D();
                selectRectCollider.OverlapCollider(contactFilter, colliders);

                foreach(Collider2D c in colliders)
                {
                    if(c.CompareTag("object") && c.gameObject.GetComponent<Object>() != null)
                    {
                        AppManager.singleton.SelectObject(c.gameObject.GetComponent<Object>());
                    }
                }
            }
           
            Destroy(selectRectCollider.gameObject);  
        }
        
    }
    void CheckSelecting()
    {
        if(selecting)
        {
            selectRectLine.SetPosition(1, new Vector3(mousePos.x, oldMousePos.y));
            selectRectLine.SetPosition(2, (Vector3)(mousePos));
            selectRectLine.SetPosition(3, new Vector3(oldMousePos.x, mousePos.y));
        }
    }
    bool CheckMove()
    {
        if(AppManager.singleton.leftShift && AppManager.singleton.leftCtrl)
        {
            moving = true;
            foreach(Object o in AppManager.singleton.selectedObjects)
            {
                if(o is Wall) (o as Wall).MoveToMouse(true);
                else
                {
                    if(o is Furniture) (o as Furniture).MoveToMouse(true); 
                }
            }
            oldMousePos = mousePos;
            return true;
        }
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if(hit.collider != null && hit.collider.CompareTag("object") && hit.collider.GetComponent<Object>().selected)
        {
            moving = true;
            foreach(Object o in AppManager.singleton.selectedObjects)
            {
                if(o is Wall) (o as Wall).MoveToMouse(true);
                else
                {
                    if(o is Furniture) (o as Furniture).MoveToMouse(true);
                }
            }

            oldMousePos = mousePos;
            return true;
        } 
        else
            return false;
    }
    void Move()
    {
        foreach(Object o in AppManager.singleton.selectedObjects)
        {
            if(o is Wall) (o as Wall).MoveToMouse(false);
            else
            {
                if(o is Furniture) (o as Furniture).MoveToMouse(false);
                if(o is Door) (o as Door).SnapToWall(); 
            }
        }
    }
    void Edit()
    {
        if(Input.GetMouseButton(0))
        {
            if(movingDot)
            {
                foreach(Wall w in AppManager.singleton.selectedObjects)
                    if(w.movingDot) w.MoveDot(false, null, true);      
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if(hit.collider != null && hit.collider.CompareTag("wallEditDot"))
                {
                    movingDot = true;
                    foreach(Wall w in AppManager.singleton.selectedObjects)
                        w.MoveDot(true, hit.collider, true);

                    oldMousePos = mousePos;
                } 
            }
        }
        else
            movingDot = false;  
    }
    bool CheckRotate()
    {
        if(!rotating)
        {
            rotating = true;
            foreach(Object o in AppManager.singleton.selectedObjects)
            {
                o.SetRotation();
            }
            oldMousePos = mousePos;
            return true; 
        }
        else
        {
            return false;
        }
    }
    void Rotate()
    {
        foreach(Object o in AppManager.singleton.selectedObjects)
        {
            if(o is Wall) (o as Wall).RotateToMouse();
            else
            {
                if(o is Furniture) (o as Furniture).RotateToMouse();
            }   
        }
    }

    void StartResizeFurniture()
    {
        resizing = true;
        
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if(hit.collider != null && hit.collider.CompareTag("FurnitureEditDot"))
        {
            pickedCollider = hit.collider;
            pickedFurniture = hit.transform.parent.parent.GetComponent<Furniture>();
            if(pickedFurniture == null)
                pickedDoor = hit.transform.parent.parent.GetComponent<Door>();
        }
    }
    void ResizingFurniture()
    {
        if(Input.GetMouseButton(0))
        {
            if(pickedFurniture != null)
                pickedFurniture.Resize(pickedCollider);
        }
        else
        {
            resizing = false;
            pickedCollider = null;
            pickedFurniture = null;
            pickedDoor = null;
        }
    }
    void CheckKeyboardInputs()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(handCursorActive)
            {
                StartResizeFurniture();
            }
            else
            {
               if(!editMode)
                {
                    CheckMove();
                    if(moving)
                        Move();
                    else
                        StartSelectRect();  
                }   
            } 
        }
        if(Input.GetMouseButtonDown(1))
        {
            rotating = CheckRotate();
        }
        if(Input.GetMouseButtonUp(0))
        {
            moving = false;
            if(selecting)
                StopSelectRect();
        }
        if(Input.GetMouseButtonUp(1))
        {
            rotating = false;
        }
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            editMode = true;
            foreach(Wall w in AppManager.singleton.selectedObjects)
                w.SetEditMode(editMode);
        }
        if(Input.GetKeyUp(KeyCode.LeftAlt))
        {
            editMode = false;
            foreach(Wall w in AppManager.singleton.selectedObjects)
                w.SetEditMode(editMode);
        }
    }
}
