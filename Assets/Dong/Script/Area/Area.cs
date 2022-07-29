using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public List<GameObject> vlist = new List<GameObject>();

    Mesh mesh;

    AwnerType type;

    Query<Node> query;

    //IsAngleRight isRight = new IsAngleRight();

    //TriangleCondition triangleCondition = new TriangleCondition();

    //RaycastHit hit;

    Dictionary<Vector3[], GameObject> boxList = new Dictionary<Vector3[], GameObject>();

    List<Vector3[]> keyList = new List<Vector3[]>();


    private void Awake()
    {
        query = new Query<Node>();
    }

    //private void Update()
    //{
    //    if (GetComponent<MeshCollider>() == null)
    //        gameObject.AddComponent<MeshCollider>();
    //}

    public void NotOccupyabase()
    {
        Debug.Log("점령 실패");
    }
    public Vector3 UnitStartPoint(LinkedList<Node> nodes, Node target)
    {
        return query.NearestPoint(nodes, target).transform.position;
    }

    public void RemoveVertexList(GameObject target)
    {
        if (vlist.Find(x => x == target) == null)
            return;
        vlist.Remove(target);
        RemoveVertex(target);
    }

    public void RemoveVertex(GameObject target)
    {
        Node targetNode = target.GetComponent<Node>();
        Vector3[] vertices = mesh.vertices;
        int[] indexs = mesh.triangles;
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newIndex = new List<int>();
        int? index = null;
        List<Vector3[]> removeKeyList = new List<Vector3[]>();
        List<Node> removeNodeList = new List<Node>();
        for (int i = 0; i < vertices.Length; ++i)
        {
            if (targetNode.verticePos == vertices[i])
            {
                index = i;
            }
            else
            {
                newVertices.Add(vertices[i]);
            }
        }
        if (index == null)
        {
            return;
        }

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
            else
            {
                Vector3[] vector3s = keyList.Find(x => x[0] == vertices[indexs[i]] && x[1] == vertices[indexs[i + 1]]);

                if (vector3s != null && boxList.ContainsKey(vector3s))
                {
                    Destroy(boxList[vector3s]);
                    boxList.Remove(vector3s);
                    keyList.Remove(vector3s);
                }

                vector3s = keyList.Find(x => x[0] == vertices[indexs[i + 1]] && x[1] == vertices[indexs[i + 2]]);
                if (vector3s != null && boxList.ContainsKey(vector3s))
                {
                    Destroy(boxList[vector3s]);
                    boxList.Remove(vector3s);
                    keyList.Remove(vector3s);
                }

                vector3s = keyList.Find(x => x[0] == vertices[indexs[i + 2]] && x[1] == vertices[indexs[i]]);
                if (vector3s != null && boxList.ContainsKey(vector3s))
                {
                    Destroy(boxList[vector3s]);
                    boxList.Remove(vector3s);
                    keyList.Remove(vector3s);
                }

            }
        }
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newIndex.ToArray();

        Vector2[] uvs = new Vector2[mesh.vertices.Length];



        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
        }
        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        if (GetComponent<MeshCollider>() != null)
            Destroy(GetComponent<MeshCollider>());

        //if (vlist.Count != mesh.vertices.Length)
        //{
        //    List<GameObject> im = vlist;
        //    Vector3[] adsf = mesh.vertices;
        //    Debug.Log("여기 나중에 지워라");
        //}

        foreach (var vec in vlist)
            vec.GetComponent<Node>().NodeCheck();

    }

    //public void CreateLine(Vector3 start, Vector3 end)
    //{
    //    GameObject colObj = new GameObject();
    //    BoxCollider col = colObj.AddComponent<BoxCollider>();
    //    LineRenderer line = colObj.AddComponent<LineRenderer>();
    //    float dis = Vector3.Distance(end, start);
    //    Vector3 vecDis = start - end;
    //    colObj.transform.position = start - (vecDis / 2);
    //    colObj.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(vecDis.x, vecDis.z) * Mathf.Rad2Deg, 0));
    //    colObj.transform.parent = transform;
    //    colObj.layer = LayerMask.NameToLayer("Hit");
    //    colObj.tag = tag;
    //    col.size = new Vector3(0.1f, 4f, dis - 1);
    //    line.positionCount = 2;
    //    line.SetPosition(0, end + (Vector3.up * 0.5f));
    //    line.SetPosition(1, start + (Vector3.up * 0.5f));
    //    line.startWidth = 0.2f;
    //    line.endWidth = 0.2f;
    //    keyList.Add(new Vector3[] { start, end });
    //    boxList.Add(keyList[keyList.Count - 1], colObj);
    //}

    //bool TagetOccupation(GameObject target)
    //{
    //    if (target.GetComponent<Node>().isOccupation)
    //    {
    //        if (target.tag == tag)
    //        {
    //            float dis;
    //            Vector3 vecDis;
    //            List<GameObject> dislist = new List<GameObject>();
    //            bool isTrue = false;
    //            foreach (var vec in vlist)
    //            {
    //                dis = Vector3.Distance(vec.GetComponent<Node>().pos, target.GetComponent<Node>().pos);
    //                vecDis = vec.GetComponent<Node>().pos - target.GetComponent<Node>().pos;
    //                Physics.Raycast(target.GetComponent<Node>().pos + (Vector3.up * 0.1f) + (vecDis.normalized * 2), vecDis.normalized, out hit, dis - 2, LayerMask.GetMask("Hit"));
    //                //Debug.DrawRay(target.GetComponent<Node>().pos + (Vector3.up * 0.1f) + (vecDis.normalized * 2), vecDis.normalized * (dis - 2), Color.red, 10f);
    //                if(hit.collider == null)
    //                {
    //                    isTrue = false;
    //                    foreach (var node in target.GetComponent<Node>().nodeList)
    //                    {
    //                        if (node == vec)
    //                            isTrue = true;
    //                    }

    //                    if (vec != target && !isTrue)
    //                        dislist.Add(vec);
    //                }
    //            }
    //            if(0 == dislist.Count)
    //                return false;

    //            GameObject[] curVertex = new GameObject[2];
    //            curVertex[0] = query.NearestPoint(dislist, target);
    //            curVertex[1] = query.IntersectObj(curVertex[0].GetComponent<Node>().nodeList, target.GetComponent<Node>().nodeList);


    //            if (null == curVertex[1])
    //                return false;

    //            Vector3[] vertices = new Vector3[]
    //            {
    //                target.GetComponent<Node>().pos,
    //                curVertex[0].GetComponent<Node>().pos,
    //                curVertex[1].GetComponent<Node>().pos
    //            };

    //            Vector3 exPos;
    //            switch (type)
    //            {
    //                case AwnerType.Player:
    //                    exPos = GameMng.instance.GetEnemyTransform();
    //                    break;
    //                case AwnerType.Enemy:
    //                    exPos = GameMng.instance.GetPlayerTransform();
    //                    break;
    //                default:
    //                    exPos = Vector3.zero;
    //                    break;
    //            }
    //            if (isRight.isPointInTriangle(vertices[0], vertices[1], vertices[2], exPos))
    //                return false;

    //            int[] indexes = SetVertexIndex(vertices, vlist.FindIndex(vertexIndext => vertexIndext == target),
    //                vlist.FindIndex(vertexIndext => vertexIndext == curVertex[0]),
    //                vlist.FindIndex(vertexIndext => vertexIndext == curVertex[1]));

    //            target.GetComponent<Node>().AddnodeList(curVertex[0]);

    //            MakeTriangle(vertices, indexes, true);

    //            if (GetComponent<MeshCollider>() != null)
    //                Destroy(GetComponent<MeshCollider>());
    //            return false;
    //        }
    //        target.GetComponent<Node>().RemoveAllnodeList();
    //        //MapMng.instance.RemoveVertex(type, target);
    //    }
    //    return true;

    //}

    //void MakeTriangle(Vector3[] vertices, int[] indexes, bool isMine = false)
    //{
    //    List<int> miniIndex = new List<int>();
    //    List<Vector3> miniVertices = new List<Vector3>();
    //    if (mesh.vertices.Length >= 3)
    //    {
    //        miniIndex.AddRange(mesh.triangles);
    //        miniVertices.AddRange(mesh.vertices);
    //        if (!isMine)
    //        {
    //            miniVertices.Add(vertices[0]);
    //        }
    //    }
    //    else
    //    {
    //        miniVertices.AddRange(vertices);
    //    }
    //    miniIndex.AddRange(indexes);

    //    mesh.Clear();

    //    mesh.vertices = miniVertices.ToArray();
    //    mesh.triangles = miniIndex.ToArray();

    //    Vector2[] uvs = new Vector2[mesh.vertices.Length];

    //    for (int i = 0; i < uvs.Length; i++)
    //    {
    //        uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
    //    }
    //    mesh.uv = uvs;

    //    mesh.RecalculateBounds();
    //    mesh.RecalculateNormals();

    //    int[] verticesIndex = SetVertexIndex(vertices, 0, 1, 2);

    //    CreateLine(vertices[verticesIndex[0]], vertices[verticesIndex[1]]);
    //    CreateLine(vertices[verticesIndex[1]], vertices[verticesIndex[2]]);
    //    CreateLine(vertices[verticesIndex[2]], vertices[verticesIndex[0]]);
    //}

    //public void AddVertex(GameObject target)
    //{
    //    List<GameObject> dislist = new List<GameObject>();
    //    triangleCondition.MinimumConditions(vlist, target, ref dislist);

    //    if (vlist.Count < 3)
    //    {
    //        if(!TagetOccupation(target))
    //            return;

    //        target.GetComponent<Node>().isOccupation = true;
    //        target.tag = tag;
    //        vlist.Add(target);
    //        if (vlist.Count != 3)
    //            return;
    //        vlist[0].GetComponent<Node>().AddnodeList(vlist[1]);
    //        vlist[0].GetComponent<Node>().AddnodeList(vlist[2]);
    //        vlist[1].GetComponent<Node>().AddnodeList(vlist[2]);
    //        Vector3[] vertices = new Vector3[]
    //        {
    //            vlist[0].GetComponent<Node>().pos,
    //            vlist[1].GetComponent<Node>().pos,
    //            vlist[2].GetComponent<Node>().pos
    //        };
    //        vlist[0].GetComponent<Node>().verticePos = vertices[0];
    //        vlist[1].GetComponent<Node>().verticePos = vertices[1];
    //        vlist[2].GetComponent<Node>().verticePos = vertices[2];


    //        int[] indexes;
    //        indexes = SetVertexIndex(vertices, 0, 1, 2);

    //        MakeTriangle(vertices, indexes);

    //        if (type == AwnerType.Player)
    //            // 시계방향으로 넣야하기 때문
    //            GameMng.instance.playerNodePos = isRight.TriangleCenterPoint(vertices[indexes[0]], vertices[indexes[1]], vertices[indexes[2]]);
    //        else
    //            GameMng.instance.EnemyNodePos = isRight.TriangleCenterPoint(vertices[indexes[0]], vertices[indexes[1]], vertices[indexes[2]]);

    //    }
    //    else
    //    {
    //        if (dislist.Count < 2)
    //        {
    //            NotOccupyabase();
    //            return;
    //        }

    //        GameObject[] curVertex = new GameObject[3];
    //        Node targetNode = target.GetComponent<Node>();
    //        List<GameObject> curVectex2;

    //        int count = 0;
    //        while (true)
    //        {
    //            curVertex[0] = query.NearestPoint(dislist, target, count);
    //            if (null == curVertex[0])
    //            {
    //                NotOccupyabase();
    //                return;
    //            }
    //            query.IntersectObj(curVertex[0].GetComponent<Node>().nodeList, dislist, out curVectex2);
    //            if (0 != curVectex2.Count)
    //                break;
    //            ++count;
    //        }

    //        if (!TagetOccupation(target))
    //        {
    //            NotOccupyabase();
    //            return;
    //        }

    //        curVertex[1] = query.SmallestAngle(curVectex2, target, curVertex[0]);

    //        Vector3 exPos;
    //        switch (type)
    //        {
    //            case AwnerType.Player:
    //                exPos = GameMng.instance.GetEnemyTransform();
    //                break;
    //            case AwnerType.Enemy:
    //                exPos = GameMng.instance.GetPlayerTransform();
    //                break;
    //            default:
    //                exPos = Vector3.zero;
    //                break;
    //        }

    //        if (isRight.isPointInTriangle(curVertex[1].GetComponent<Node>().pos, curVertex[0].GetComponent<Node>().pos, target.GetComponent<Node>().pos, exPos))
    //        {
    //            NotOccupyabase();
    //            return;
    //        }
    //        targetNode.AddnodeList(curVertex[0]);
    //        targetNode.AddnodeList(curVertex[1]);



    //        vlist.Add(target);

    //        target.GetComponent<Node>().isOccupation = true;
    //        target.tag = tag;

    //        Vector3[] vertices = new Vector3[]
    //        {
    //            target.GetComponent<Node>().pos,
    //            target.GetComponent<Node>().nodeList[0].GetComponent<Node>().pos,
    //            target.GetComponent<Node>().nodeList[1].GetComponent<Node>().pos
    //        };
    //        int[] indexes = SetVertexIndex(vertices, mesh.vertices.Length,
    //            vlist.FindIndex(vertexIndext => vertexIndext == target.GetComponent<Node>().nodeList[0]),
    //            vlist.FindIndex(vertexIndext => vertexIndext == target.GetComponent<Node>().nodeList[1]));

    //        target.GetComponent<Node>().verticePos = vertices[0];

    //        MakeTriangle(vertices, indexes);
    //    }

    //    if (GetComponent<MeshCollider>() != null)
    //        Destroy(GetComponent<MeshCollider>());
    //}

    //int[] SetVertexIndex(Vector3[] vertices, int index0, int index1, int index2)
    //{
    //    int[] indexes;

    //    if (isRight.isThreeAngleRight(vertices))
    //    {
    //        indexes = new int[] { index0, index1, index2 };
    //    }
    //    else
    //    {
    //        indexes = new int[] { index0, index2, index1 };
    //    }
    //    return indexes;
    //}
    

}