using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectDataCollection
{
    public List<ObjectData> collection;
    public ObjectDataCollection()
    {
        collection = new List<ObjectData>();
    }
}
