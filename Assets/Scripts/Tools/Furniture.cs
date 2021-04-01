using System.Collections.Generic;
using UnityEngine;

public class Furniture : Object
{
    public Sprite sprite;
    public int spriteIndex;
    public List<Sprite> shapes = new List<Sprite>();
    public SpriteRenderer shapeRenderer; 
    public PolygonCollider2D shapeCollider;
    public LineRenderer selectionOutline;
    public BoxCollider2D outlineCollider;
    public GameObject editDotsGameObject;
    public List<Transform> editDots = new List<Transform>();
    public Vector2 offset;
    public float originalWidth;
    public float originalHeight;
    public void Construct(Vector2 position, Quaternion rotation, Vector3 scale, Color color, bool selected, int spriteIndex)
    {
        this.transform.position = position;
        this.transform.rotation = rotation;
        this.transform.localScale = scale;
        this.color = color;
        this.selected = selected;
        this.spriteIndex = spriteIndex;
        SetShape(spriteIndex);
        SetColor(color);
        Refresh();
    }

    public override void Refresh()
    {
        if(selected)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            angle = 360 - transform.rotation.eulerAngles.z;
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
        shapeRenderer.color = color;
    }
    
    public void SetShape(int index)
    {
        spriteIndex = index;
        sprite = shapes[index];
        shapeRenderer.sprite = sprite;
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
    public void MoveToMouse(bool start)
    {
        if(start)
        {
            offset = (Vector3)AppManager.singleton.mousePos - transform.position;
        }
        else
        {
            AppManager.singleton.mousePos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
            transform.position = ((Vector3)AppManager.singleton.mousePos - (Vector3)offset);
            SetSelectionSquare();
        }   
    }

    public void RotateToMouse()
    {
        mousePos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
        float mouseDelta = oldMousePos.y > mousePos.y ? ((oldMousePos - mousePos).magnitude) * -1 : (oldMousePos - mousePos).magnitude;
        transform.Rotate(Vector3.forward, mouseDelta);
        transform.position = RotatePoint(transform.position, pivot, mouseDelta);
        oldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SetSelectionSquare();
    }

    public void Resize(Collider2D collider)
    {
        float newHeight = 0;
        float newWidth = 0;
        collider.transform.position = mousePos;
        for(int i = 0; i<editDots.Count; i++)
        {
            if(editDots[i] == collider.transform)
            {
                editDots[i] = collider.transform;
                newHeight = Mathf.Abs(editDots[i].position.y - editDots[(i+2)%4].position.y);
                newWidth = Mathf.Abs(editDots[i].position.x - editDots[(i+2)%4].position.x);
            }
        }

        transform.localScale = new Vector3(newWidth/originalWidth, newHeight/originalHeight, 0f);    
        SetColliders();
        SetSelectionSquare(); 
    }
}
