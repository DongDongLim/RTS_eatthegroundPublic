﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public List<GameObject> vlist = new List<GameObject>();

    Mesh mesh;

    public AwnerType type;

    public Query query = new Query();

    IsAngleRight isRight = new IsAngleRight();

    RaycastHit hit;

    Dictionary<Vector3[], GameObject> boxList = new Dictionary<Vector3[], GameObject>();

    List<Vector3[]> keyList = new List<Vector3[]>();

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        if (GetComponent<MeshCollider>() == null)
            gameObject.AddComponent<MeshCollider>();
    }

    public void NotOccupyabase()
    {
        Debug.Log("점령 실패");
    }
    public Vector3 UnitStartPoint()
    {
        return query.NearestPoint(vlist, MapMng.instance.curSelectTown).transform.position;
    }

    public void RemoveVertexList(GameObject target)
    {
        vlist.Remove(target);
    }

    public void RemoveVertex(GameObject target)
    {
        Town targetTown = target.GetComponent<Town>();
        Vector3[] vertices = mesh.vertices;
        int[] indexs = mesh.triangles;
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newIndex = new List<int>();
        int? index = null;
        for (int i = 0; i < vertices.Length; ++i)
        {
            if (targetTown.verticePos == vertices[i])
            {
                index = i;
                foreach (GameObject obj in targetTown.nodeList)
                {
                    foreach(var veclist in keyList)
                    {
                        if(((veclist[0] == targetTown.verticePos) && (veclist[1] == obj.GetComponent<Town>().verticePos))
                            ||
                        ((veclist[1] == targetTown.verticePos) && (veclist[0] == obj.GetComponent<Town>().verticePos)))
                        {
                            Destroy(boxList[veclist]);
                            boxList.Remove(veclist);
                        }
                    }
                }
            }
            else
            {
                newVertices.Add(vertices[i]);
            }
        }
        if (index == null)
        {
            Debug.Log("여기 예외 난듯");
            return;
        }
        targetTown.RemoveAllnodeList();
        for (int i = 0; i < indexs.Length; i += 3)
        {
            if (indexs[i] != index && indexs[i + 1] != index && indexs[i + 2] != index)
            {
                if (indexs[i] > index)
                    newIndex.Add(indexs[i] - 1);
                else
                    newIndex.Add(indexs[i]);
                if (indexs[i + 1] > index)
                    newIndex.Add(indexs[i + 1] - 1);
                else
                    newIndex.Add(indexs[i + 1]);
                if (indexs[i + 2] > index)
                    newIndex.Add(indexs[i + 2] - 1);
                else
                    newIndex.Add(indexs[i + 2]);
            }
        }
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newIndex.ToArray();
    }

    public void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject colObj = new GameObject();
        BoxCollider col = colObj.AddComponent<BoxCollider>();
        LineRenderer line = colObj.AddComponent<LineRenderer>();
        float dis = Vector3.Distance(end, start);
        Vector3 vecDis = start - end;
        colObj.transform.position = start - (vecDis / 2);
        colObj.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(vecDis.x, vecDis.z) * Mathf.Rad2Deg, 0));
        colObj.transform.parent = transform;
        colObj.layer = LayerMask.NameToLayer("Hit");
        colObj.tag = tag;
        col.size = new Vector3(0.1f, 1f, dis - 1);
        line.positionCount = 2;
        line.SetPosition(0, end);
        line.SetPosition(1, start);
        keyList.Add(new Vector3[] { start, end });
        boxList.Add(keyList[keyList.Count - 1], colObj);
    }

    // 플러드 필알고리즘
    // https://www.crocus.co.kr/1288
    // https://ko.wikipedia.org/wiki/%EB%B3%B4%EB%A1%9C%EB%85%B8%EC%9D%B4_%EB%8B%A4%EC%9D%B4%EC%96%B4%EA%B7%B8%EB%9E%A8

    // https://minok-portfolio.tistory.com/17
    public void AddVertex(GameObject target)
    {
        float dis;
        Vector3 vecDis;

        foreach (Vector3 vec in mesh.vertices)
        {
            dis = Vector3.Distance(vec, target.GetComponent<Town>().pos);
            vecDis = vec - target.GetComponent<Town>().pos;
            Physics.Raycast(target.GetComponent<Town>().pos + (Vector3.up * 0.1f), vecDis.normalized, out hit, dis, LayerMask.GetMask("Hit"));
            if (hit.collider != null && hit.collider.gameObject.tag != tag)
            {
                NotOccupyabase();
                return;
            }
        }
        vlist.Add(target);

        if (target.GetComponent<Town>().isOccupation)
        {
            if (target.tag != tag)
            {
                MapMng.instance.RemoveVertex(type, target);
            }
        }

        target.GetComponent<Town>().isOccupation = true;
        target.tag = tag;

        if (vlist.Count == 3)
        {
            vlist[0].GetComponent<Town>().AddnodeList(vlist[1]);
            vlist[0].GetComponent<Town>().AddnodeList(vlist[2]);
            vlist[1].GetComponent<Town>().AddnodeList(vlist[2]);
            Vector3[] vertices = new Vector3[]
            {
                vlist[0].GetComponent<Town>().pos,
                vlist[1].GetComponent<Town>().pos,
                vlist[2].GetComponent<Town>().pos
            };
            vlist[0].GetComponent<Town>().verticePos = vertices[0];
            vlist[1].GetComponent<Town>().verticePos = vertices[1];
            vlist[2].GetComponent<Town>().verticePos = vertices[2];
            int[] indexes;
            indexes = SetVertexIndex(vertices, 0, 1, 2);
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = indexes;
            CreateLine(vertices[0], vertices[1]);
            CreateLine(vertices[1], vertices[2]);
            CreateLine(vertices[2], vertices[0]);

        }
        else if (vlist.Count > 3)
        {
            GameObject[] curVertex = new GameObject[3];
            Town targetTown = target.GetComponent<Town>();
            curVertex[0] = query.NearestPoint(vlist, target);

            curVertex[1] = query.SmallestAngle(curVertex[0].GetComponent<Town>().nodeList, target, curVertex[0]);

            curVertex[2] = query.IntersectObj(curVertex[0].GetComponent<Town>().nodeList, curVertex[1].GetComponent<Town>().nodeList);

            if (isRight.IsIntersects(curVertex[0].transform.position, curVertex[1].transform.position, curVertex[2].transform.position, target.transform.position))
            {
                targetTown.AddnodeList(curVertex[0]);
                targetTown.AddnodeList(curVertex[1]);
            }
            else if (isRight.IsIntersects(curVertex[2].transform.position, curVertex[1].transform.position, curVertex[0].transform.position, target.transform.position))
            {
                targetTown.AddnodeList(curVertex[1]);
                targetTown.AddnodeList(curVertex[2]);
            }
            else if (isRight.IsIntersects(curVertex[2].transform.position, curVertex[0].transform.position, curVertex[1].transform.position, target.transform.position))
            {
                targetTown.AddnodeList(curVertex[0]);
                targetTown.AddnodeList(curVertex[2]);
            }
            else
            {
                targetTown.AddnodeList(curVertex[0]);
                targetTown.AddnodeList(curVertex[1]);
            }

            Vector3[] vertices = new Vector3[]
            {
                target.GetComponent<Town>().pos,
                target.GetComponent<Town>().nodeList[0].GetComponent<Town>().pos,
                target.GetComponent<Town>().nodeList[1].GetComponent<Town>().pos
            };
            List<int> miniIndex = new List<int>();
            miniIndex.AddRange(mesh.triangles);
            miniIndex.AddRange(SetVertexIndex(vertices, mesh.vertices.Length,
                vlist.FindIndex(vertexIndext => vertexIndext == target.GetComponent<Town>().nodeList[0]),
                vlist.FindIndex(vertexIndext => vertexIndext == target.GetComponent<Town>().nodeList[1])));
            List<Vector3> miniVertices = new List<Vector3>();
            miniVertices.AddRange(mesh.vertices);
            miniVertices.Add(vertices[0]);
            target.GetComponent<Town>().verticePos = vertices[0];
            mesh.Clear();
            mesh.vertices = miniVertices.ToArray();
            mesh.triangles = miniIndex.ToArray();

            CreateLine(vertices[0], vertices[1]);
            CreateLine(vertices[1], vertices[2]);
            CreateLine(vertices[2], vertices[0]);
        }

        if (GetComponent<MeshCollider>() != null)
            Destroy(GetComponent<MeshCollider>());
    }

    int[] SetVertexIndex(Vector3[] vertices, int index0, int index1, int index2)
    {
        int[] indexes;

        if (isRight.isThreeAngleRight(vertices))
        {
            indexes = new int[] { index0, index1, index2 };
        }
        else
        {
            indexes = new int[] { index0, index2, index1 };
        }
        return indexes;
    }


}