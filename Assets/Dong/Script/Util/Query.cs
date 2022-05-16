using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Query
{
    // 가장 가까운 거리의 오브젝트
    public GameObject NearestPoint(List<GameObject> list, GameObject target)
    {
        //https://hijuworld.tistory.com/56
        var vertexQuery = from vertex in list
                          where vertex != target
                          orderby Vector3.SqrMagnitude(
                              target.transform.position
                              - vertex.transform.position)
                          select vertex;
        foreach (var vertex in vertexQuery)
        {
            return vertex;
        }
        return null;
    }

    // 가장 작은 각도의 오브젝트
    public GameObject SmallestAngle(List<GameObject> list, GameObject target, GameObject comparison)
    {
        var vertexQuery = from vertex in list
                           orderby Mathf.Abs((Mathf.Atan2((target.transform.position - comparison.transform.position).x
                           , (target.transform.position - comparison.transform.position).z) * Mathf.Rad2Deg)
              - (Mathf.Atan2((vertex.transform.position - comparison.transform.position).x
              , (vertex.transform.position - comparison.transform.position).z) * Mathf.Rad2Deg))
                           select vertex;
        foreach (var vertex in vertexQuery)
        {
            return vertex;
        }
        return null;
    }

    // 두 리스트가 모두 포함하는 오브젝트
    public GameObject IntersectObj(List<GameObject> list, List<GameObject> targetList)
    {
        var vertexQuery = from vertex in list.Intersect(targetList)
                           select vertex; 
        foreach (var vertex in vertexQuery)
        {
            return vertex;
        }
        return null;
    }

}
