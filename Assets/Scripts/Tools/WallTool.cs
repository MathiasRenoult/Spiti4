using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTool : Tool
{
    public static WallTool singleton;
    public Wall currentlyDrawnWall;
    public bool drawing;
    public bool ctrlMode = false;
    public bool shiftMode = false;
    public bool magnet = true;
    public bool magnetButton = true;
    public bool strictModeButton = false;
    [Range(0f,2000f)]
    public float magnetDotRange = 400f;
    [Range(0f,2000f)]
    public float magnetWallRange = 200f;
    public Vector2 oldMousePos;
    public Vector2 mousePos;
    public Color defaultColor;
    public float defaultWidth;

    private void Awake()
    {
        singleton = this;
    }
    private void Start()
    {
        if(magnet || magnetButton)
        {
            WallStatsManager.singleton.magnetIcon.color = WallStatsManager.singleton.selectedIconsColor;
        }
        if(ctrlMode || strictModeButton)
        {
            WallStatsManager.singleton.stricModeIcon.color = WallStatsManager.singleton.selectedIconsColor;
        }
    }
    void Update ()
    {
        if(selected && AppManager.singleton.workZone.selected)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            ctrlMode = AppManager.singleton.leftCtrl || strictModeButton;
            shiftMode = AppManager.singleton.leftShift;
            magnet = magnetButton;

            if(ToolManager.singleton.workZone.selected)
                Draw();
        }
    }

    private void StartDrawing()
    {
        drawing = true;
        currentlyDrawnWall = Instantiate(AppManager.singleton.wallPrefab).GetComponent<Wall>();
        currentlyDrawnWall.SetColor(defaultColor);
        currentlyDrawnWall.SetWidth(defaultWidth);
        currentlyDrawnWall.Refresh();
        AppManager.singleton.objects.Add(currentlyDrawnWall);

        if(!shiftMode)
            AppManager.singleton.DeselectAllSelectedObjects();
        else
            AppManager.singleton.SelectObject(currentlyDrawnWall);

        if(magnet)
        {
            Vector3 closestDot = currentlyDrawnWall.line.GetPosition(0);
            closestDot = ClosestDot(magnetDotRange);
            if(closestDot != new Vector3(-1,-1,-1) && (closestDot - (Vector3)mousePos).sqrMagnitude < magnetDotRange)
                currentlyDrawnWall.line.SetPosition(0, closestDot);
            else
                currentlyDrawnWall.line.SetPosition(0, mousePos);
        }
        else
            currentlyDrawnWall.line.SetPosition(0, mousePos);
    }

    public bool StopDrawing()
    {
        drawing = false;
        if(((currentlyDrawnWall.line.GetPosition(0)-currentlyDrawnWall.line.GetPosition(1)).sqrMagnitude < 10f))
        {
            AppManager.singleton.DestroyObject(currentlyDrawnWall);
            return false;
        }
        else
        {
            currentlyDrawnWall.SetColliders();
            AppManager.singleton.UpdateWall(currentlyDrawnWall);
            if(!shiftMode)
                currentlyDrawnWall.selected = false;
            return true; 
        }
    }

    void Draw()
    {
        if(drawing)
        {
            if(currentlyDrawnWall != null )
            {
                if(ctrlMode)
                {
                    currentlyDrawnWall.line.SetPosition(1, mousePos);
                    AppManager.singleton.UpdateWall(currentlyDrawnWall);
                    currentlyDrawnWall.angle = Mathf.Round(currentlyDrawnWall.angle/45f) * 45f;
                    AppManager.singleton.UpdateWall(currentlyDrawnWall, false, true);
                }
                else
                {
                    if(magnet)
                    {
                        Vector3 closestDot = currentlyDrawnWall.line.GetPosition(0);
                        Vector3 closestWall = currentlyDrawnWall.line.GetPosition(0);

                        if(ClosestDot(magnetDotRange) != (Vector3)mousePos)
                        {
                            currentlyDrawnWall.line.SetPosition(1, ClosestDot(magnetDotRange));
                        }
                        else
                        {
                            currentlyDrawnWall.line.SetPosition(1, ClosestWall(magnetWallRange));
                        }
                    }
                    else
                        currentlyDrawnWall.line.SetPosition(1, mousePos);
                }
            }

            if(Input.GetMouseButtonDown(1))
                AppManager.singleton.DestroyObject(currentlyDrawnWall);

            if(Input.GetMouseButtonUp(0))
                StopDrawing();  
        }
        else
            if(Input.GetMouseButtonDown(0)) StartDrawing();     
    }
    public Vector3 ClosestDot(float maxDist)
    {
        bool clamped = false;
        Vector3 closestDot = new Vector3(-1f, -1f, -1f);
        foreach(Object o in AppManager.singleton.objects)
        {
            if(o is Wall)
            {
                if(o != null && o != currentlyDrawnWall)
                {
                    if(((o as Wall).line.GetPosition(0) - (Vector3)mousePos).sqrMagnitude < maxDist && ((o as Wall).line.GetPosition(0) - (Vector3)mousePos).sqrMagnitude < (closestDot - (Vector3)mousePos).sqrMagnitude)
                    {
                        closestDot = (o as Wall).line.GetPosition(0);
                        clamped = true;
                    }
                
                    if(((o as Wall).line.GetPosition(1) - (Vector3)mousePos).sqrMagnitude < maxDist && ((o as Wall).line.GetPosition(1) - (Vector3)mousePos).sqrMagnitude < (closestDot - (Vector3)mousePos).sqrMagnitude)
                    {
                        closestDot = (o as Wall).line.GetPosition(1);
                        clamped = true;
                    }
                } 
            }
           
        }

        if(clamped)
            return closestDot;
        else
            return mousePos;
    }

    public Vector3 ClosestWall(float maxDist)
    {
        bool clamped = false;
        Vector3 closestWall = new Vector3(9999f, 9999f, 0f);
        foreach(Object o in AppManager.singleton.objects)
        {
            if(o is Wall)
            {
                if(o != null && o != currentlyDrawnWall)
                {
                    if(((o as Wall).polyCollider.ClosestPoint(mousePos) - mousePos).sqrMagnitude < maxDist && ((o as Wall).polyCollider.ClosestPoint(mousePos) - mousePos).sqrMagnitude < ((Vector2)closestWall - mousePos).sqrMagnitude)
                    {
                        closestWall = (o as Wall).polyCollider.ClosestPoint(mousePos);
                        clamped = true;
                    }
                } 
            }
           
        }
        if(clamped)
            return closestWall;
        else
            return mousePos;
    }
}
