using System.Collections.Generic;
using UnityEngine;

public class Wall : Object
{
    public LineRenderer line;
    public PolygonCollider2D polyCollider; 
    public bool editmode = false;
    public bool movingDot = false;
    public Vector2 pt1;
    public Vector2 pt2;
    public Transform pivotPoint;
    public Transform startEditDot;
    public Transform endEditDot;
    [Range(0,10f)]
    public float editDotSize = 2f;
    public int currentlyMovingDot = -1;
    [Range(0f,359f)]
    public float colorOffset;
    public float length;
    public float width;

    public void Construct(Vector2 position, Quaternion rotation, Color color, bool selected, Vector2 startPoint, Vector2 endPoint, float width)
    {
        this.transform.position = position;
        this.transform.rotation = rotation;
        this.color = color;
        this.selected = selected;
        this.line.SetPosition(0, startPoint);
        this.line.SetPosition(1, endPoint);
        this.width = width;
        SetWidth(width);
        SetColor(color);
        Refresh();
    }
    
    public override void Refresh()
    {
        if(selected)
        {
            AppManager.singleton.UpdateWall(this);
            line.startColor = Color.HSVToRGB((Mathf.Sin(Time.time) + 1) / 2, 1f, 1f);
            line.endColor = Color.HSVToRGB((Mathf.Sin(Time.time+colorOffset) + 1) / 2, 1f, 1f);
        }
        else
        {
            line.startColor = color;
            line.endColor = color;
        }

        if(editmode)
        {
            startEditDot.localPosition = line.GetPosition(0);
            endEditDot.localPosition = line.GetPosition(1);
        }
    }
    public override void SetColor(Color color)
    {
        color.a = 1f;
        this.color = color;
        line.startColor = color;
        line.endColor = color;
    }
    public void SetWidth(float width)
    {
        this.width = width;
        line.startWidth = width;
        line.endWidth = width;
    }
    public void MoveToMouse(bool start)
    {
        if(start)
        {
            pt1 = line.GetPosition(0);
            pt2 = line.GetPosition(1);
            oldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            mousePos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
            line.SetPosition(0, (Vector3)pt1 +  (Vector3)(mousePos - oldMousePos));
            line.SetPosition(1, (Vector3)pt2 + (Vector3)(mousePos - oldMousePos));
            SetColliders();
        }   
    }
    public void RotateToMouse()
    {
        mousePos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
        float mouseDelta = oldMousePos.y > mousePos.y ? ((oldMousePos - mousePos).magnitude) * -1 : (oldMousePos - mousePos).magnitude;
        line.SetPosition(0, RotatePoint(line.GetPosition(0), pivot, mouseDelta));
        line.SetPosition(1, RotatePoint(line.GetPosition(1), pivot, mouseDelta));
        SetColliders();
        oldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public void MoveDot(bool start, Collider2D collider2d = null, bool magnet = false)
    {
        if(start)
        {
            movingDot = collider2d == startEditDot.GetComponent<CircleCollider2D>() || endEditDot.GetComponent<CircleCollider2D>() ? true : false;
            if(!movingDot)
                return;
            currentlyMovingDot = collider2d == startEditDot.GetComponent<CircleCollider2D>() ? 0 : 1;
            pt1 = line.GetPosition(currentlyMovingDot); 
            oldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        else
        {
            if(!movingDot)
                return;

            mousePos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if(magnet)
            {
                Vector3 closestDot = WallTool.singleton.ClosestDot(WallTool.singleton.magnetDotRange);
                if(closestDot != new Vector3(-1f, -1f, -1f) && (closestDot - (Vector3)mousePos).sqrMagnitude < WallTool.singleton.magnetDotRange)
                {
                    line.SetPosition(currentlyMovingDot, closestDot);
                }
                else
                {
                    line.SetPosition(currentlyMovingDot, (Vector3)pt1 +  (Vector3)(mousePos - oldMousePos));
                }
            }
            else 
            {
                line.SetPosition(currentlyMovingDot, (Vector3)pt1 +  (Vector3)(mousePos - oldMousePos));
            }

            SetColliders();
        }
    }

    public void SetColliders()
    {
        if(line != null)
        {
        float angle = -Mathf.Atan2((line.GetPosition(1).y - line.GetPosition(0).y), (line.GetPosition(1).x - line.GetPosition(0).x));
        float halfWidth = line.startWidth/2;
        List<Vector2> points = new List<Vector2>();

        points.Add(GetEndPointVector(angle, halfWidth, line.GetPosition(0).x, line.GetPosition(0).y, true));
        points.Add(GetEndPointVector(angle, halfWidth, line.GetPosition(0).x, line.GetPosition(0).y, false));
        points.Add(GetEndPointVector(angle, halfWidth, line.GetPosition(1).x, line.GetPosition(1).y, false));
        points.Add(GetEndPointVector(angle, halfWidth, line.GetPosition(1).x, line.GetPosition(1).y, true));

        polyCollider.SetPath(0, points);
        }
    }

    private Vector2 GetEndPointVector(float angle, float halfWitdth, float positionX, float positionY, bool addition) {
        float deltaX = Mathf.Sin(angle) * halfWitdth;
        float deltaY = Mathf.Cos(angle) * halfWitdth;
        float x = addition ? positionX + deltaX : positionX - deltaX;
        float y = addition ? positionY + deltaY : positionY - deltaY;

        return new Vector2(x, y);
    }

    public void SetEditMode(bool active)
    {
        editmode = active;
        startEditDot.gameObject.SetActive(active);
        endEditDot.gameObject.SetActive(active);
    }
}
