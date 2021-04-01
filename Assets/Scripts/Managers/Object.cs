using System.Collections.Generic;
using UnityEngine;
using System;

public class Object : MonoBehaviour
{
    public Vector2 pivot;
    public Vector2 mousePos;
    public Vector2 oldMousePos;
    public Color color;
    public float angle;
    public bool selected = false;
    public virtual void Refresh()
    {

    }
    public virtual void SetColor(Color color)
    {

    }
    public void SetRotation()
    {
        List<Vector2> points = new List<Vector2>();

        foreach(Object o in AppManager.singleton.selectedObjects)
        {
            if(o is Wall)
            {
                points.Add((o as Wall).line.GetPosition(0));
                points.Add((o as Wall).line.GetPosition(1));
            }
            else
            {
                if(o is Furniture)
                {
                    points.Add((Vector2)(o as Furniture).transform.position);
                }
            }
        }

        float[] bounds = new float[4];
        bounds[0] = 999999f;
        bounds[1] = 999999f;
        bounds[2] = 0f;
        bounds[3] = 0f;
        foreach(Vector2 v in points)
        {
            if(v.x < bounds[0])
                bounds[0] = v.x;
            
            if(v.y < bounds[1])
                bounds[1] = v.y;

            if(v.x > bounds[2])
                bounds[2] = v.x;

            if(v.y > bounds[3])
                bounds[3] = v.y;
        }

        Vector3[] positions = 
        {
            new Vector3(bounds[0], bounds[1]),
            new Vector3(bounds[0], bounds[3]),
            new Vector3(bounds[2], bounds[3]),
            new Vector3(bounds[2], bounds[1])
        };

        pivot = new Vector2((bounds[0] + bounds[2]) / 2f, (bounds[1] + bounds[3]) / 2f);
        oldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees)
    {
        double angleInRadians = angleInDegrees * (Mathf.PI / 180f);
        double cosTheta = Math.Cos(angleInRadians);
        double sinTheta = Math.Sin(angleInRadians);
        return new Vector2
        {
            x =
                (float)
                (cosTheta * (pointToRotate.x - centerPoint.x) - sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x),
            y =
                (float)
                (sinTheta * (pointToRotate.x - centerPoint.x) + cosTheta * (pointToRotate.y - centerPoint.y) + centerPoint.y)
        };
    }
}
