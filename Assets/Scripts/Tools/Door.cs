using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Object
{   
    public List<Sprite> doors = new List<Sprite>();
    public Sprite sprite;
    public int spriteIndex;
    public SpriteRenderer spriteRendrerer;
    public Collider2D shapeCollider;
    public List<Transform> editDots = new List<Transform>();
    public GameObject editDotsGameObject;
    public BoxCollider2D outlineCollider;
    public LineRenderer selectionOutline;
    public float originalHeight;
    public float originalWidth;
    public Vector3 offset;

    public void Construct(Vector2 position, Quaternion rotation, Vector3 scale, Color color, bool selected, int spriteIndex)
    {
        this.transform.position = position;
        this.transform.rotation = rotation;
        this.transform.localScale = scale;
        this.color = color;
        this.selected = selected;
        this.spriteIndex = spriteIndex;
        SetDoor(spriteIndex);
        SetColor(color);
        Refresh();
    }
    public override void Refresh()
    {
        if(selected)
        {
            if(!selectionOutline.enabled)
                selectionOutline.enabled = true;
        
            if(!editDotsGameObject.activeSelf)
                editDotsGameObject.SetActive(true);

            selectionOutline.startColor = selectionOutline.endColor = Color.HSVToRGB((Mathf.Sin(Time.time) + 1) / 2, 1f, 1f);  
        }
        else
        {
            if(selectionOutline.enabled)
                selectionOutline.enabled = false;
            
            if(editDotsGameObject.activeSelf)
                editDotsGameObject.SetActive(false);
        }
    }
    public override void SetColor(Color color)
    {
        color.a = 1f;
        this.color = color;
        spriteRendrerer.color = color;
    }
    public void SetDoor(int index)
    {
        if(index >= 0 && index < doors.Count)
        {
            spriteIndex = index;
            sprite = doors[index];
            spriteRendrerer.sprite = sprite;
        }
    }
    public void SetColliders()
    {
        Destroy(shapeCollider);
        shapeCollider = gameObject.AddComponent<PolygonCollider2D>();
    }
    public void SetSelectionSquare()
    {
        BoxCollider2D boxCollider;
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        editDots[0].transform.localPosition = new Vector3(boxCollider.offset.x + boxCollider.size.x/2, boxCollider.offset.y + boxCollider.size.y/2);
        editDots[1].transform.localPosition = new Vector3(boxCollider.offset.x + boxCollider.size.x/2, boxCollider.offset.y - boxCollider.size.y/2);
        editDots[2].transform.localPosition = new Vector3(boxCollider.offset.x - boxCollider.size.x/2, boxCollider.offset.y - boxCollider.size.y/2);
        editDots[3].transform.localPosition = new Vector3(boxCollider.offset.x - boxCollider.size.x/2, boxCollider.offset.y + boxCollider.size.y/2);

        Destroy(boxCollider);

        selectionOutline.positionCount = 4;
        List<Vector3> points = new List<Vector3>();
        foreach(Transform t in editDots)
            points.Add(t.position);

        selectionOutline.SetPositions(points.ToArray());

        Destroy(outlineCollider);
        outlineCollider = gameObject.AddComponent<BoxCollider2D>();
    }

    public void Place()
    {
        SetColliders();
        SetSelectionSquare();   
        originalHeight = sprite.bounds.size.y;
        originalWidth = sprite.bounds.size.x;
    }
    public void SnapToWall()
    {
        mousePos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector2 closestPoint = mousePos;
        PolygonCollider2D closestCollider = null;
        float distance = Mathf.Infinity;
        List<PolygonCollider2D> colliders = new List<PolygonCollider2D>();
        foreach(Object o in AppManager.singleton.objects)
        {
            if(o is Wall) colliders.Add((o as Wall).polyCollider);
        }
        foreach(PolygonCollider2D p in colliders)
        {
            if((mousePos - p.ClosestPoint(mousePos)).sqrMagnitude < distance)
            {
                distance = (mousePos - p.ClosestPoint(mousePos)).sqrMagnitude; 
                closestPoint = p.ClosestPoint(mousePos);
                closestCollider = p;
            } 
        }
        transform.position = closestPoint;

        transform.rotation = Quaternion.Euler(0f, 0f, -closestCollider.gameObject.GetComponent<Wall>().angle);
        if((closestPoint-mousePos).sqrMagnitude < ((Vector2)editDots[0].position - mousePos).sqrMagnitude)
            transform.Rotate(Vector3.forward, 180f);

        this.transform.parent = closestCollider.transform;

        SetSelectionSquare(); 
    }
    public void RotateToMouse()
    {
    }

    public void Resize(Collider2D collider)
    {
       
    }
}
