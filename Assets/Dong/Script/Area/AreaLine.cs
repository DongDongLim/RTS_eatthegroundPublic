using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaLine : MonoBehaviour
{
    public  void CreateLine(Vector3[] vertex)
    {
        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = vertex.Length + 1;
        for(int i = 0; i < line.positionCount; ++i)
        {
            line.SetPosition(i, vertex[i % vertex.Length] + (Vector3.up * 0.5f));
        }
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
    }
}
