using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public int objectType; //0 = wall, 1 = furniture, 2 = door
    public Vector2 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Color color;
    public bool selected;

    //Wall
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float width;

    //Furniture / Door
    public int spriteIndex;

    public ObjectData(int objectType, Vector2 position, Quaternion rotation, Vector3 scale, Color color, bool selected, Vector2 startPoint, Vector2 endPoint, float width, int spriteIndex)
    {
        Debug.Log("Constructing Object Data !");

        this.objectType = objectType;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.color = color;
        this.selected = selected;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.width = width;
        this.spriteIndex = spriteIndex;
    }
}
