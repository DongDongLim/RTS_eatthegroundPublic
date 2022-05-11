using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapMng : SingletonMini<MapMng>
{
    [SerializeField]
    GameObject Town;

    [SerializeField]
    GameObject Map;

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

    public GameObject curSelectTown;

    Mesh mesh;

    protected override void OnAwake()
    {
        pooling.OnRePooing += PooingObj;

        pooling.OnRePooing?.Invoke();
        GameMng.instance.DayAction += MapSetting;
        mesh = new Mesh();
    }

    private void Start()
    {
        MapSetting();
    }

    private void Update()
    {

    }

    // 플러드 필알고리즘
    // https://minok-portfolio.tistory.com/17
    public void AddVertex()
    {
        vertexList.Add(curSelectTown);
        if(vertexList.Count == 3)
        {
            vertexList[0].GetComponent<Town>().AddLinkedTown(vertexList[1]);
            vertexList[0].GetComponent<Town>().AddLinkedTown(vertexList[2]);
            vertexList[1].GetComponent<Town>().AddLinkedTown(vertexList[2]);
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0, 0.5f),
                vertexList[1].transform.position - vertexList[0].transform.position,
                vertexList[2].transform.position - vertexList[0].transform.position
            };
            int[] indexes;
            indexes = SetVertexIndex(vertices, 0, 1, 2);
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = indexes;

            MeshFilter meshFilter = vertexList[0].transform.GetChild(3).GetComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            vertexList[0].transform.GetChild(3).transform.localScale = Vector3.one;
            //vertexList.RemoveRange(1, vertexList.Count-1);
        }
        else if(vertexList.Count > 3)
        {
            //https://hijuworld.tistory.com/56
            var vertexQuery = from vertex in vertexList
                              where vertex != vertexList[vertexList.Count - 1]
                              orderby Vector3.SqrMagnitude(
                                  vertexList[vertexList.Count - 1].transform.position
                                  - vertex.transform.position)
                              select vertex;

            GameObject[] curVertex = new GameObject[3];

            foreach (var vertex in vertexQuery)
            {
                curVertex[0] = vertex;
                break;
            }
            var vertexQuery1 = from vertex in curVertex[0].GetComponent<Town>().LinkedTown
                               orderby Vector3.SqrMagnitude(vertexList[vertexList.Count - 1].transform.position
                               - vertex.transform.position)
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
            if(sementIntersects(curVertex[0].transform.position, curVertex[1].transform.position, curVertex[2].transform.position, vertexList[vertexList.Count - 1].transform.position))
            {
                Debug.Log(1);
                vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(curVertex[0]);
                vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(curVertex[1]);
            }
            else if(sementIntersects(curVertex[2].transform.position, curVertex[1].transform.position, curVertex[0].transform.position, vertexList[vertexList.Count - 1].transform.position))
            {
                Debug.Log(2);
                vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(curVertex[1]);
                vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(curVertex[2]);
            }
            else if (sementIntersects(curVertex[2].transform.position, curVertex[0].transform.position, curVertex[1].transform.position, vertexList[vertexList.Count - 1].transform.position))
            {
                Debug.Log(3);
                vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(curVertex[0]);
                vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(curVertex[2]);
            }
            else
            {
                Debug.Log(4);
                vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(curVertex[0]);
                vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(curVertex[2]);
            }
            /*
            int i = 0;
            GameObject curvetex = null;
            foreach (var vertex in vertexQuery)
            {
                if(i == 0)
                {
                    vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(vertex);
                    curvetex = vertex;
                    ++i;
                }
                if (i == 1)
                {
                    if (curvetex.GetComponent<Town>().LinkedTown.FindIndex(x => x == vertex) != -1)
                    {
                        vertexList[vertexList.Count - 1].GetComponent<Town>().AddLinkedTown(vertex);
                        break;
                    }
                }
            }
            */


            Vector3[] vertices = new Vector3[]
            {
                vertexList[vertexList.Count - 1].transform.position - vertexList[0].transform.position,
                vertexList[vertexList.Count - 1].GetComponent<Town>().LinkedTown[0].transform.position - vertexList[0].transform.position,
                vertexList[vertexList.Count - 1].GetComponent<Town>().LinkedTown[1].transform.position - vertexList[0].transform.position
            };
            List<int> miniIndex = new List<int>();
            miniIndex.AddRange(mesh.triangles);
            miniIndex.AddRange(SetVertexIndex(vertices, mesh.vertices.Length,
                vertexList.FindIndex(vertexIndext => vertexIndext == vertexList[vertexList.Count - 1].GetComponent<Town>().LinkedTown[0]),
                vertexList.FindIndex(vertexIndext => vertexIndext == vertexList[vertexList.Count - 1].GetComponent<Town>().LinkedTown[1])));
            List<Vector3> miniVertices = new List<Vector3>();
            miniVertices.AddRange(mesh.vertices);
            miniVertices.Add(vertices[0]);
            mesh.Clear();
            mesh.vertices = miniVertices.ToArray();
            mesh.triangles = miniIndex.ToArray();
        }
    }



    // 세개짜리만 가능
    bool isThreeAngle(Vector3[] vertices)
    {
        if ((Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                  - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) > 180 ||
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
    bool sementIntersects(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3[] ccw1 = new Vector3[] { a, b, d };
        Vector3[] ccw2 = new Vector3[] { a, b, c };
        Vector3[] ccw3 = new Vector3[] { c, d, a };
        Vector3[] ccw4 = new Vector3[] { c, d, b };

        return isThreeAngle(ccw1) && isThreeAngle(ccw3) && !isThreeAngle(ccw2) && !isThreeAngle(ccw4);
    }


    int[] SetVertexIndex(Vector3[] vertices, int index0, int index1, int index2)
    {
        int[] indexes;
        if (!isThreeAngle(vertices))
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
        if (!GameMng.instance.isGamePlaying)
        {
            GameObject obj = CreateObj(creationPoint[index]);
            obj.transform.GetChild(0).gameObject.SetActive(true);
            obj.GetComponent<Town>().SetIndex(0);
            curSelectTown = obj;
            AddVertex();
            creationPoint.RemoveAt(index);
            obj.SetActive(true);
            index = Random.Range(0, creationPoint.Count);
            GameObject obj1 = CreateObj(creationPoint[index]);
            obj1.transform.GetChild(1).gameObject.SetActive(true);
            obj1.GetComponent<Town>().SetIndex(1);
            creationPoint.RemoveAt(index);
            obj1.SetActive(true);
            GameMng.instance.SetUser(obj, obj1);
        }
        GameObject obj2;
        for (int i = 0; i < range/2 - (GameMng.instance.Day * 8) - 10; ++i)
        {
            index = Random.Range(0, creationPoint.Count);
            obj2 = CreateObj(creationPoint[index]);
            obj2.transform.GetChild(2).gameObject.SetActive(true);
            obj2.GetComponent<Town>().SetIndex(2);
            creationPoint.RemoveAt(index);
            obj2.SetActive(true);
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

    public GameObject CreateObj(Vector3 pos)
    {
        direction = Random.Range(0, 4);

        GameObject obj;
        switch (direction)
        {
            case 0:
                obj = pooling.Pop(pos);
                break;
            case 1:
                obj = pooling.Pop(-pos);
                break;
            case 2:
                obj = pooling.Pop(new Vector3(pos.x, 0, -pos.z));
                break;
            case 3:
                obj = pooling.Pop(new Vector3(-pos.x, 0, pos.z));
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
