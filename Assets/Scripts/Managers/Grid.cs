using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid singleton;
    public LineRenderer line;
    public float squareSize;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        SetupGrid();
    }
    public void SetupGrid()
    {
        List<Vector3> points =  new List<Vector3>();
        Rect rect = AppManager.singleton.workZone.gameObject.GetComponent<RectTransform>().rect;
        print(rect);
        for(int i = 0; rect.size.x*2 > i*squareSize; i++)
        {
            for(int j = 0; rect.size.y*2 > j*squareSize; j++)
            {
                points.Add(new Vector3(i*squareSize, j*squareSize));
                print("Add point");
            }
        }

        print(points);
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
}
