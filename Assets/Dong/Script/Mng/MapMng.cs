﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapMng : SingletonMini<MapMng>
{
    [SerializeField]
    GameObject Tree;

    [SerializeField]
    GameObject Town;

    [SerializeField]
    GameObject Map;

    [SerializeField]
    GameObject playerArea;

    [SerializeField]
    GameObject EnermyArea;

    [SerializeField]
    int range;

    [SerializeField]
    int rangeExcept;

    [SerializeField]
    List<GameObject> popList = new List<GameObject>();

    ObjectPooing pooling = new ObjectPooing();

    List<Vector3> creationPoint = new List<Vector3>();

    int poolingCount = 2331;

    int direction;

    bool isDis = false;

    [SerializeField]
    List<GameObject> vertexList = new List<GameObject>();

    [SerializeField]
    List<GameObject> vertexEnermyList = new List<GameObject>();

    public GameObject curSelectTown;

    Mesh meshPlayer;


    Mesh meshEnermy;

    RaycastHit hit;

    protected override void OnAwake()
    {
        pooling.OnRePooing += PooingObj;

        pooling.OnRePooing?.Invoke();
        GameMng.instance.DayAction += MapSetting;
        meshPlayer = new Mesh();
        meshEnermy = new Mesh();
    }

    private void Start()
    {
        MapSetting();
    }

    private void Update()
    {

    }

    public void Occupyabase(GameObject target)
    {
        AddVertex(AwnerType.Player, target);
    }

    public void EnermyQccupyabase()
    {
        AddVertex(AwnerType.Enermy, curSelectTown);
    }

    public Vector3 PlayerStartPoint()
    {
        return NearestPoint(vertexList, curSelectTown).transform.position;
    }

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

    public void RemoveVertexList(AwnerType type, GameObject target)
    {
        switch (type)
        {
            case AwnerType.Player:
                vertexList.Remove(target);
                break;
            case AwnerType.Enermy:
                vertexEnermyList.Remove(target);
                break;
            default:
                return;
        }
    }

    public void RemoveVertex(AwnerType type, GameObject target)
    {
        List<GameObject> vlist;
        Mesh mesh;
        switch (type)
        {
            case AwnerType.Player:
                vlist = vertexList;
                mesh = meshPlayer;
                break;
            case AwnerType.Enermy:
                vlist = vertexEnermyList;
                mesh = meshEnermy;
                break;
            default:
                return;
        }
        
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
            }
            else
            {
                newVertices.Add(vertices[i]);

            }
        }
        if (index == null)
            return;
        targetTown.RemoveAllLinkedTown();
        for (int i = 0; i < indexs.Length; i += 3)
        {
            if(indexs[i] != index && indexs[i + 1] != index && indexs[i + 2] != index)
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
        if(vlist.Count == 0)
        {
            switch (type)
            {
                case AwnerType.Player:
                    AddVertex(AwnerType.Player, GameMng.instance.GetPlayer());
                    break;
                case AwnerType.Enermy:
                    AddVertex(AwnerType.Enermy, GameMng.instance.GetEnermy());
                    break;
                default:
                    return;
            }
        }

    }

    public void NotOccupyabase()
    {
        Debug.Log("점령 실패");
    }

    // 플러드 필알고리즘
    // https://www.crocus.co.kr/1288
    // https://ko.wikipedia.org/wiki/%EB%B3%B4%EB%A1%9C%EB%85%B8%EC%9D%B4_%EB%8B%A4%EC%9D%B4%EC%96%B4%EA%B7%B8%EB%9E%A8

    // https://minok-portfolio.tistory.com/17
    public void AddVertex(AwnerType type, GameObject target)
    {
        List<GameObject> vlist;
        GameObject meshAwner;
        Mesh mesh;
        switch (type)
        {
            case AwnerType.Player:
                vlist = vertexList;
                mesh = meshPlayer;
                meshAwner = playerArea;
                break;
            case AwnerType.Enermy:
                vlist = vertexEnermyList;
                mesh = meshEnermy;
                meshAwner = EnermyArea;
                break;
            default:
                return;
        }

        float dis;
        Vector3 vecDis;
        
        foreach (Vector3 vec in mesh.vertices)
        {
            dis = Vector3.Distance(vec, target.GetComponent<Town>().pos.position);
            vecDis = vec - target.GetComponent<Town>().pos.position;
            Physics.Raycast(target.GetComponent<Town>().pos.position + (Vector3.up * 0.1f), vecDis.normalized, out hit, dis, LayerMask.GetMask("Hit"));
           
            switch(type)
            {
                case AwnerType.Player:
                    if (hit.collider?.gameObject.tag == "Enermy")
                    {
                        NotOccupyabase();
                        return;
                    }
                    break;
                case AwnerType.Enermy:
                    if (hit.collider?.gameObject.tag == "Player")
                    {
                        NotOccupyabase();
                        return;
                    }
                    break;
            }
        }


        //foreach (Vector3 vec in mesh.vertices)
        //{
        //    dis = Vector3.Distance(vec, target.GetComponent<Town>().pos.position);
        //    vecDis = target.GetComponent<Town>().pos.position - vec;
        //    if (Physics.BoxCast(target.GetComponent<Town>().pos.position + Vector3.up - (vecDis / 2)
        //         , new Vector3(0.1f, 0.1f, dis)
        //         , Vector3.down
        //         , out hit
        //         , Quaternion.Euler(new Vector3(0, Mathf.Atan2(vecDis.z, vecDis.x) * Mathf.Rad2Deg, 0))
        //         , Mathf.Infinity
        //         , LayerMask.GetMask("Area")))
        //    {
        //        switch (type)
        //        {
        //            case AwnerType.Player:
        //                if (hit.collider?.tag == "Enermy")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            case AwnerType.Enermy:
        //                if (hit.collider?.tag == "Player")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            default:
        //                return;
        //        }
        //    }
        //}
        //if (vlist.Count == 1)
        //{
        //    float dis = Vector3.Distance(vlist[0].GetComponent<Town>().pos.position, target.GetComponent<Town>().pos.position);
        //    Vector3 vecDis = vlist[0].GetComponent<Town>().pos.position - target.GetComponent<Town>().pos.position;
        //    if (Physics.BoxCast(target.GetComponent<Town>().pos.position + Vector3.up + (vecDis / 2)
        //        , new Vector3(0.1f, 0.1f, dis)
        //        , Vector3.down
        //        , out hit
        //        , Quaternion.Euler(new Vector3(0, Mathf.Atan2(vecDis.y, vecDis.x), 0))
        //        , Mathf.Infinity
        //        , LayerMask.GetMask("Area")))
        //    {
        //        switch (type)
        //        {
        //            case AwnerType.Player:
        //                if (hit.collider?.tag == "Enermy")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            case AwnerType.Enermy:
        //                if (hit.collider?.tag == "Player")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            default:
        //                return;
        //        }
        //    }
        //}
        //else if (vlist.Count == 2)
        //{
        //    float dis = Vector3.Distance(vlist[0].GetComponent<Town>().pos.position, target.GetComponent<Town>().pos.position);
        //    Vector3 vecDis = vlist[0].GetComponent<Town>().pos.position - target.GetComponent<Town>().pos.position;
        //    if (Physics.BoxCast(target.GetComponent<Town>().pos.position + Vector3.up + (vecDis / 2)
        //           , new Vector3(0.1f, 0.1f, Vector3.Distance(vlist[0].GetComponent<Town>().pos.position, target.GetComponent<Town>().pos.position))
        //           , Vector3.down
        //           , out hit
        //           , Quaternion.Euler(new Vector3(0, Mathf.Atan2((target.GetComponent<Town>().pos.position - vlist[0].GetComponent<Town>().pos.position).y, (target.GetComponent<Town>().pos.position - vlist[0].GetComponent<Town>().pos.position).x), 0))
        //           , Mathf.Infinity
        //           , LayerMask.GetMask("Area")))
        //    {
        //        switch (type)
        //        {
        //            case AwnerType.Player:
        //                if (hit.collider?.tag == "Enermy")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            case AwnerType.Enermy:
        //                if (hit.collider?.tag == "Player")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            default:
        //                return;
        //        }
        //    }
        //    dis = Vector3.Distance(vlist[1].GetComponent<Town>().pos.position, target.GetComponent<Town>().pos.position);
        //    vecDis = vlist[1].GetComponent<Town>().pos.position - target.GetComponent<Town>().pos.position;
        //    if (Physics.BoxCast(target.GetComponent<Town>().pos.position + Vector3.up + (vecDis / 2)
        //           , new Vector3(0.1f, 0.1f, dis)
        //           , Vector3.down
        //           , out hit
        //           , Quaternion.Euler(new Vector3(0, Mathf.Atan2(vecDis.y, vecDis.x), 0))
        //           , Mathf.Infinity
        //           , LayerMask.GetMask("Area")))
        //    {
        //        switch (type)
        //        {
        //            case AwnerType.Player:
        //                if (hit.collider?.tag == "Enermy")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            case AwnerType.Enermy:
        //                if (hit.collider?.tag == "Player")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            default:
        //                return;
        //        }
        //    }
        //}
        //else if (vlist.Count >= 3)
        //{
        //    GameObject obj = NearestPoint(vlist, target);
        //    float dis = Vector3.Distance(obj.GetComponent<Town>().pos.position, target.GetComponent<Town>().pos.position);
        //    Vector3 vecDis = obj.GetComponent<Town>().pos.position - target.GetComponent<Town>().pos.position;
        //    if (Physics.BoxCast(target.GetComponent<Town>().pos.position + Vector3.up + (vecDis / 2)
        //           , new Vector3(0.1f, 0.1f, dis)
        //           , Vector3.down
        //           , out hit
        //           , Quaternion.Euler(new Vector3(0, Mathf.Atan2(vecDis.y, vecDis.x), 0))
        //           , Mathf.Infinity
        //           , LayerMask.GetMask("Area")))
        //    {
        //        switch (type)
        //        {
        //            case AwnerType.Player:
        //                if(hit.collider?.tag == "Enermy")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            case AwnerType.Enermy:
        //                if (hit.collider?.tag == "Player")
        //                {
        //                    NotOccupyabase();
        //                    return;
        //                }
        //                break;
        //            default:
        //                return;
        //        }
        //    }
        //    foreach (GameObject obj1 in obj.GetComponent<Town>().LinkedTown)
        //    {
        //        dis = Vector3.Distance(obj1.GetComponent<Town>().pos.position, target.GetComponent<Town>().pos.position);
        //        vecDis = obj1.GetComponent<Town>().pos.position - target.GetComponent<Town>().pos.position;
        //        if (Physics.BoxCast(target.GetComponent<Town>().pos.position + Vector3.up + (vecDis / 2)
        //             , new Vector3(0.1f, 0.1f, dis)
        //             , Vector3.down
        //             , out hit
        //             , Quaternion.Euler(new Vector3(0, Mathf.Atan2(vecDis.y, vecDis.x), 0))
        //             , Mathf.Infinity
        //             , LayerMask.GetMask("Area")))
        //        {
        //            switch (type)
        //            {
        //                case AwnerType.Player:
        //                    if (hit.collider?.tag == "Enermy")
        //                    {
        //                        NotOccupyabase();
        //                        return;
        //                    }
        //                    break;
        //                case AwnerType.Enermy:
        //                    if (hit.collider?.tag == "Player")
        //                    {
        //                        NotOccupyabase();
        //                        return;
        //                    }
        //                    break;
        //                default:
        //                    return;
        //            }
        //        }
        //    }
        //}
        vlist.Add(target);

        if (target.GetComponent<Town>().isOccupation)
        {
            switch (type)
            {
                case AwnerType.Player:
                    RemoveVertex(AwnerType.Enermy, target);
                    break;
                case AwnerType.Enermy:
                    RemoveVertex(AwnerType.Player, target);
                    break;
                default:
                    break;
            }
        }

        //switch(target.GetComponent<Town>().type)
        //{
        //    case AwnerType.Player:
        //        RemoveVertex(AwnerType.Player, target);
        //        break;
        //    case AwnerType.Enermy:
        //        RemoveVertex(AwnerType.Enermy, target);
        //        break;
        //    default:
        //        break;
        //}
        //target.GetComponent<Town>().SetIndex((int)type);
        target.GetComponent<Town>().isOccupation = true;
        switch (type)
        {
            case AwnerType.Player:
                target.tag = "Player";
                break;
            case AwnerType.Enermy:
                target.tag = "Enermy";
                break;
            default:
                break;
        }
        if (vlist.Count == 3)
        {

            vlist[0].GetComponent<Town>().AddLinkedTown(vlist[1]);
            vlist[0].GetComponent<Town>().AddLinkedTown(vlist[2]);
            vlist[1].GetComponent<Town>().AddLinkedTown(vlist[2]);
            Vector3[] vertices = new Vector3[]
            {
                vlist[0].GetComponent<Town>().pos.position,
                vlist[1].GetComponent<Town>().pos.position,
                vlist[2].GetComponent<Town>().pos.position
                //Vector3.zero,
                //vlist[1].transform.position - vlist[0].transform.position,
                //vlist[2].transform.position - vlist[0].transform.position
            };
            vlist[0].GetComponent<Town>().verticePos = vertices[0];
            vlist[1].GetComponent<Town>().verticePos = vertices[1];
            vlist[2].GetComponent<Town>().verticePos = vertices[2];
            int[] indexes;
            indexes = SetVertexIndex(vertices, 0, 1, 2);
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = indexes;

            MeshFilter meshFilter = meshAwner.GetComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            //vlist[0].transform.GetChild(3).transform.localScale = Vector3.one;
            //vertexList.RemoveRange(1, vlist.Count-1);
        }
        else if (vlist.Count > 3)
        {

            GameObject[] curVertex = new GameObject[3];

            curVertex[0] = NearestPoint(vlist, target);

            var vertexQuery1 = from vertex in curVertex[0].GetComponent<Town>().LinkedTown
                               orderby Mathf.Abs((Mathf.Atan2((target.transform.position - curVertex[0].transform.position).x
                               , (target.transform.position - curVertex[0].transform.position).z) * Mathf.Rad2Deg)
                  - (Mathf.Atan2((vertex.transform.position - curVertex[0].transform.position).x
                  , (vertex.transform.position - curVertex[0].transform.position).z) * Mathf.Rad2Deg))
                               /*orderby Vector3.SqrMagnitude(target.transform.position
                               - vertex.transform.position)*/
                               select vertex;
            foreach (var vertex in vertexQuery1)
            {
                curVertex[1] = vertex;
                break;
            }
            var vertexQuery2 = from vertex in curVertex[0].GetComponent<Town>().LinkedTown.Intersect(curVertex[1].GetComponent<Town>().LinkedTown)
                               select vertex;
            foreach (var vertex in vertexQuery2)
            {
                curVertex[2] = vertex;
                break;
            }
            if (sementIntersects(curVertex[0].transform.position, curVertex[1].transform.position, curVertex[2].transform.position, target.transform.position))
            {
                target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                target.GetComponent<Town>().AddLinkedTown(curVertex[1]);
            }
            else if (sementIntersects(curVertex[2].transform.position, curVertex[1].transform.position, curVertex[0].transform.position, target.transform.position))
            {
                target.GetComponent<Town>().AddLinkedTown(curVertex[1]);
                target.GetComponent<Town>().AddLinkedTown(curVertex[2]);
            }
            else if (sementIntersects(curVertex[2].transform.position, curVertex[0].transform.position, curVertex[1].transform.position, target.transform.position))
            {
                target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                target.GetComponent<Town>().AddLinkedTown(curVertex[2]);
            }
            else
            {
                target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                target.GetComponent<Town>().AddLinkedTown(curVertex[1]);
                //    방법 2
                //    if (sementIntersects1(curVertex[0].transform.position, curVertex[1].transform.position, curVertex[2].transform.position, target.transform.position))
                //    {
                //        if (Vector3.Cross(curVertex[2].transform.position - target.transform.position
                //            , curVertex[1].transform.position - target.transform.position).sqrMagnitude >
                //            Vector3.Cross(curVertex[2].transform.position - target.transform.position
                //            , curVertex[0].transform.position - target.transform.position).sqrMagnitude)
                //        {
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[2]);
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[1]);
                //        }
                //        else
                //        {
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[2]);
                //        }
                //    }
                //    else if (sementIntersects1(curVertex[2].transform.position, curVertex[1].transform.position, curVertex[0].transform.position, target.transform.position))
                //    {
                //        if (Vector3.Cross(curVertex[0].transform.position - target.transform.position
                //           , curVertex[1].transform.position - target.transform.position).sqrMagnitude >
                //           Vector3.Cross(curVertex[0].transform.position - target.transform.position
                //           , curVertex[2].transform.position - target.transform.position).sqrMagnitude)
                //        {
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[1]);
                //        }
                //        else
                //        {
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[2]);
                //        }
                //    }
                //    else if (sementIntersects(curVertex[2].transform.position, curVertex[0].transform.position, curVertex[1].transform.position, target.transform.position))
                //    {
                //        if (Vector3.Cross(curVertex[1].transform.position - target.transform.position
                //           , curVertex[0].transform.position - target.transform.position).sqrMagnitude >
                //           Vector3.Cross(curVertex[1].transform.position - target.transform.position
                //           , curVertex[2].transform.position - target.transform.position).sqrMagnitude)
                //        {
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[1]);
                //        }
                //        else
                //        {
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[1]);
                //            target.GetComponent<Town>().AddLinkedTown(curVertex[2]);
                //        }
                //    }

                // 방법 1
                //if (Vector3.Cross(curVertex[0].transform.position - target.transform.position
                //    , curVertex[1].transform.position - target.transform.position).sqrMagnitude >
                //    Vector3.Cross(curVertex[0].transform.position - target.transform.position
                //    , curVertex[2].transform.position - target.transform.position).sqrMagnitude)
                //{
                //    target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                //    target.GetComponent<Town>().AddLinkedTown(curVertex[1]);
                //}
                //else
                //{
                //    target.GetComponent<Town>().AddLinkedTown(curVertex[0]);
                //    target.GetComponent<Town>().AddLinkedTown(curVertex[2]);
                //}
            }
            /*
            int i = 0;
            GameObject curvetex = null;
            foreach (var vertex in vertexQuery)
            {
                if(i == 0)
                {
                    target.GetComponent<Town>().AddLinkedTown(vertex);
                    curvetex = vertex;
                    ++i;
                }
                if (i == 1)
                {
                    if (curvetex.GetComponent<Town>().LinkedTown.FindIndex(x => x == vertex) != -1)
                    {
                        target.GetComponent<Town>().AddLinkedTown(vertex);
                        break;
                    }
                }
            }
            */


            Vector3[] vertices = new Vector3[]
            {
                target.GetComponent<Town>().pos.position,
                target.GetComponent<Town>().LinkedTown[0].GetComponent<Town>().pos.position,
                target.GetComponent<Town>().LinkedTown[1].GetComponent<Town>().pos.position
                //target.transform.position - vlist[0].transform.position,
                //target.GetComponent<Town>().LinkedTown[0].transform.position - vlist[0].transform.position,
                //target.GetComponent<Town>().LinkedTown[1].transform.position - vlist[0].transform.position
            };
            List<int> miniIndex = new List<int>();
            miniIndex.AddRange(mesh.triangles);
            miniIndex.AddRange(SetVertexIndex(vertices, mesh.vertices.Length,
                vlist.FindIndex(vertexIndext => vertexIndext == target.GetComponent<Town>().LinkedTown[0]),
                vlist.FindIndex(vertexIndext => vertexIndext == target.GetComponent<Town>().LinkedTown[1])));
            List<Vector3> miniVertices = new List<Vector3>();
            miniVertices.AddRange(mesh.vertices);
            miniVertices.Add(vertices[0]);
            target.GetComponent<Town>().verticePos = vertices[0];
            mesh.Clear();
            mesh.vertices = miniVertices.ToArray();
            mesh.triangles = miniIndex.ToArray();
        }
        Destroy(meshAwner.GetComponent<MeshCollider>());
        meshAwner.AddComponent<MeshCollider>();
    }



    // 세개짜리만 가능
    bool isThreeAngleRight(Vector3[] vertices)
    {
        if (((Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                  - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) > 180 &&
                 (Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                  - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) < 360) ||
                  ((Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                  - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) < 0 &&
                  (Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                  - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) > -180))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // https://bowbowbow.tistory.com/17
    //bool sementIntersects1(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    //{
    //    Vector3[] ccw1 = new Vector3[] { a, b, d };
    //    Vector3[] ccw2 = new Vector3[] { a, b, c };
    //    Vector3[] ccw3 = new Vector3[] { c, d, a };
    //    Vector3[] ccw4 = new Vector3[] { c, d, b };

    //    return isThreeAngleRight(ccw3) != isThreeAngleRight(ccw4);
    //}

    bool sementIntersects(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3[] ccw1 = new Vector3[] { a, b, d };
        Vector3[] ccw2 = new Vector3[] { a, b, c };
        Vector3[] ccw3 = new Vector3[] { c, d, a };
        Vector3[] ccw4 = new Vector3[] { c, d, b };

        return ((isThreeAngleRight(ccw1) == isThreeAngleRight(ccw3)) && (isThreeAngleRight(ccw2) == isThreeAngleRight(ccw4))) && (isThreeAngleRight(ccw1) != isThreeAngleRight(ccw2));
    }


    int[] SetVertexIndex(Vector3[] vertices, int index0, int index1, int index2)
    {
        int[] indexes;
        if (!isThreeAngleRight(vertices))
        {
            indexes = new int[] { index0, index2, index1 };
        }
        else
        {
            indexes = new int[] { index0, index1, index2 };
        }
        return indexes;
    }

    public void MapSetting()
    {
        for(int i = 2; i <= range; i += 2)
        {
            isDis = false;
            for (int j = range; j >= 2; j -= 2)
            {
                if (!isDis)
                {
                    if ((new Vector2(i, j) - Vector2.zero).sqrMagnitude > range * range)
                        continue;
                    isDis = true;
                }
                if ((new Vector2(i, j) - Vector2.zero).sqrMagnitude < rangeExcept * rangeExcept)
                {
                    break;
                }
                creationPoint.Add(new Vector3(i, 0, j));
            }
        }
        int index = Random.Range(0, creationPoint.Count);
        int direction = Random.Range(0, 4);
        if (!GameMng.instance.isGamePlaying)
        {
            // 플레이어
            GameObject obj;
            obj = CreateObj(creationPoint[index] + (Vector3.right * 4), direction);
            obj.transform.GetChild(0).gameObject.SetActive(true);
            obj.GetComponent<Town>().SetIndex(0);
            AddVertex(AwnerType.Player, obj);
            obj.SetActive(true);

            obj = CreateObj(creationPoint[index] + (Vector3.back * 4), direction);
            obj.transform.GetChild(0).gameObject.SetActive(true);
            obj.GetComponent<Town>().SetIndex(0);
            AddVertex(AwnerType.Player, obj);
            obj.SetActive(true);

            obj = CreateObj(creationPoint[index], direction);
            obj.transform.GetChild(0).gameObject.SetActive(true);
            obj.GetComponent<Town>().SetIndex(0);
            //curSelectTown = obj;
            AddVertex(AwnerType.Player, obj);
            obj.SetActive(true);

            if (creationPoint.Find(x => x == (creationPoint[index] + (Vector3.back * 4))) != null)
                creationPoint.Remove(creationPoint[index] + (Vector3.down * 4));
            if (creationPoint.Find(x => x == (creationPoint[index] + (Vector3.right * 4))) != null)
                creationPoint.Remove(creationPoint[index] + (Vector3.right * 4));
            creationPoint.RemoveAt(index);

            // 적

            index = Random.Range(0, creationPoint.Count);
            GameObject obj1;

            obj1 = CreateObj(creationPoint[index] + (Vector3.right * 4), 3 - direction);
            obj1.transform.GetChild(1).gameObject.SetActive(true);
            obj1.GetComponent<Town>().SetIndex(1);
            AddVertex(AwnerType.Enermy, obj1);
            obj1.SetActive(true);

            obj1 = CreateObj(creationPoint[index] + (Vector3.back * 4), 3 - direction);
            obj1.transform.GetChild(1).gameObject.SetActive(true);
            obj1.GetComponent<Town>().SetIndex(1);
            AddVertex(AwnerType.Enermy, obj1);
            obj1.SetActive(true);

            obj1 = CreateObj(creationPoint[index], 3 - direction);
            obj1.transform.GetChild(1).gameObject.SetActive(true);
            obj1.GetComponent<Town>().SetIndex(1);
            //curSelectTown = obj1;
            AddVertex(AwnerType.Enermy, obj1);
            obj1.SetActive(true);
            if (creationPoint.Find(x => x == (creationPoint[index] + (Vector3.back * 4))) != null)
                creationPoint.Remove(creationPoint[index] + (Vector3.down * 4));
            if (creationPoint.Find(x => x == (creationPoint[index] + (Vector3.right * 4))) != null)
                creationPoint.Remove(creationPoint[index] + (Vector3.right * 4));
            creationPoint.RemoveAt(index);
            GameMng.instance.SetUser(obj, obj1);
        }
        GameObject obj2;
        for (int i = 0; i < range/2 - (GameMng.instance.Day * 8) - 10; ++i)
        {
            index = Random.Range(0, creationPoint.Count);
            direction = Random.Range(0, 4);
            obj2 = CreateObj(creationPoint[index], direction);
            obj2.transform.GetChild(2).gameObject.SetActive(true);
            obj2.GetComponent<Town>().SetIndex(2);
            creationPoint.RemoveAt(index);
            obj2.SetActive(true);
        }
        for (int i = 0; i < creationPoint.Count; ++i)
        {
            index = Random.Range(0, 100);
            if(index >= 90)
            {
                index = Random.Range(0, 4);
                switch (index)
                {
                    case 0:
                        Instantiate(Tree, Map.transform, false).transform.position = creationPoint[i];
                        break;
                    case 1:
                        Instantiate(Tree, Map.transform, false).transform.position = new Vector3(creationPoint[i].x, 0, -creationPoint[i].z);
                        break;
                    case 2:
                        Instantiate(Tree, Map.transform, false).transform.position = -creationPoint[i];
                        break;
                    case 3:
                        Instantiate(Tree, Map.transform, false).transform.position = new Vector3(-creationPoint[i].x, 0, creationPoint[i].z);
                        break;
                    default:
                        break;
                }
            }
            
        }
        creationPoint.Clear();
        rangeExcept = range + 2;
        range += 20;
    }


    void PooingObj()
    {
        for (int i = 0; i < poolingCount; ++i)
        {
            pooling.Push(Instantiate(Town, Map.transform, false));
        }
    }
        

    public GameObject CreateObj(Vector3 pos, int direction)
    {
        GameObject obj;
        switch (direction)
        {
            case 0:
                obj = pooling.Pop(pos);
                break;
            case 1:
                obj = pooling.Pop(new Vector3(pos.x, 0, -pos.z));
                break;
            case 2:
                obj = pooling.Pop(new Vector3(-pos.x, 0, pos.z));
                break;
            case 3:
                obj = pooling.Pop(-pos);
                break;
            default:
                return null;
        }
        popList.Add(obj);
        return obj;
    }

    public void RemoveObj(GameObject obj)
    {
        pooling.Push(obj);
        popList.Remove(obj);
    }

    void RemoveAllObj()
    {
        int cnt = popList.Count;
        for (int i = 0; i < cnt; ++i)
            RemoveObj(popList[0]);
    }

}
