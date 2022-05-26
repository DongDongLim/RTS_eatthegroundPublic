using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    Area Area_Player;

    [SerializeField]
    GameObject EnermyArea;

    Area Area_Enermy;

    [SerializeField]
    int range;

    [SerializeField]
    int rangeExcept;

    [SerializeField]
    List<GameObject> popList = new List<GameObject>();

    ObjectPooing pooling = new ObjectPooing();

    List<Vector3> creationPoint = new List<Vector3>();

    int poolingCount = 2331;

    bool isDis = false;

    public GameObject curSelectTown;


    protected override void OnAwake()
    {
        pooling.OnRePooing += PooingObj;

        pooling.OnRePooing?.Invoke();
        GameMng.instance.DayAction += MapSetting;
    }

    private void Start()
    {
        Area_Player = playerArea.GetComponent<Area>();
        Area_Enermy = EnermyArea.GetComponent<Area>();
        MapSetting();
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
        return Area_Player.UnitStartPoint();
    }
    public void RemoveVertexList(string targetTag, GameObject target)
    {
        switch (targetTag)
        {
            case "Player":
                Area_Player.RemoveVertexList(target);
                break;
            case "Enermy":
                Area_Enermy.RemoveVertexList(target);
                break;
            default:
                return;
        }
    }
    public void RemoveVertex(AwnerType type, GameObject target)
    {
        target.GetComponent<Town>().RemoveAllnodeList();
        //switch (type)
        //{
        //    case AwnerType.Player:
        //        Area_Enermy.RemoveVertex(target);
        //        break;
        //    case AwnerType.Enermy:
        //        Area_Player.RemoveVertex(target);
        //        break;
        //    default:
        //        return;
        //}
    }   
    public void AddVertex(AwnerType type, GameObject target)
    {
        switch(type)
        {
            case AwnerType.Player:
                Area_Player.AddVertex(target);
                break;
            case AwnerType.Enermy:
                Area_Enermy.AddVertex(target);
                break;
            default:
                return;
        }
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
            obj.GetComponent<Town>().SetType(AwnerType.Player);
            obj.SetActive(true);
            AddVertex(AwnerType.Player, obj);

            obj = CreateObj(creationPoint[index] + (Vector3.back * 4), direction);
            obj.transform.GetChild(0).gameObject.SetActive(true);
            obj.GetComponent<Town>().SetType(AwnerType.Player);
            obj.SetActive(true);
            AddVertex(AwnerType.Player, obj);

            obj = CreateObj(creationPoint[index], direction);
            obj.transform.GetChild(0).gameObject.SetActive(true);
            obj.GetComponent<Town>().SetType(AwnerType.Player);
            obj.SetActive(true);
            AddVertex(AwnerType.Player, obj);
            Vector3 removePos = creationPoint[index];
            if (creationPoint.Find(x => x == (removePos + (Vector3.back * 4))) != null)
                creationPoint.Remove(removePos + (Vector3.down * 4));
            if (creationPoint.Find(x => x == (removePos + (Vector3.back * 2))) != null)
                creationPoint.Remove(removePos + (Vector3.down * 2));
            if (creationPoint.Find(x => x == (removePos + (Vector3.right * 4))) != null)
                creationPoint.Remove(removePos + (Vector3.right * 4));
            if (creationPoint.Find(x => x == (removePos + (Vector3.right * 2))) != null)
                creationPoint.Remove(removePos + (Vector3.right * 2));
            if (creationPoint.Find(x => x == (removePos + (Vector3.right * 2) + (Vector3.back * 2))) != null)
                creationPoint.Remove(removePos + (Vector3.right * 2) + (Vector3.back * 2));
            creationPoint.Remove(removePos);

            // 적

            index = Random.Range(0, creationPoint.Count);
            GameObject obj1;

            obj1 = CreateObj(creationPoint[index] + (Vector3.right * 4), 3 - direction);
            obj1.transform.GetChild(1).gameObject.SetActive(true);
            obj1.GetComponent<Town>().SetType(AwnerType.Enermy);
            obj1.SetActive(true);
            AddVertex(AwnerType.Enermy, obj1);

            obj1 = CreateObj(creationPoint[index] + (Vector3.back * 4), 3 - direction);
            obj1.transform.GetChild(1).gameObject.SetActive(true);
            obj1.GetComponent<Town>().SetType(AwnerType.Enermy);
            obj1.SetActive(true);
            AddVertex(AwnerType.Enermy, obj1);

            obj1 = CreateObj(creationPoint[index], 3 - direction);
            obj1.transform.GetChild(1).gameObject.SetActive(true);
            obj1.GetComponent<Town>().SetType(AwnerType.Enermy);
            obj1.SetActive(true);
            AddVertex(AwnerType.Enermy, obj1);

            removePos = creationPoint[index];
            if (creationPoint.Find(x => x == (removePos + (Vector3.back * 4))) != null)
                creationPoint.Remove(removePos + (Vector3.down * 4));
            if (creationPoint.Find(x => x == (removePos + (Vector3.back * 2))) != null)
                creationPoint.Remove(removePos + (Vector3.down * 2));
            if (creationPoint.Find(x => x == (removePos + (Vector3.right * 4))) != null)
                creationPoint.Remove(removePos + (Vector3.right * 4));
            if (creationPoint.Find(x => x == (removePos + (Vector3.right * 2))) != null)
                creationPoint.Remove(removePos + (Vector3.right * 2));
            if (creationPoint.Find(x => x == (removePos + (Vector3.right * 2) + (Vector3.back * 2))) != null)
                creationPoint.Remove(removePos + (Vector3.right * 2) + (Vector3.back * 2));
            creationPoint.Remove(removePos);
            GameMng.instance.SetUser(obj, obj1);
        }
        GameObject obj2;
        for (int i = 0; i < range/2 - (GameMng.instance.Day * 8) - 5; ++i)
        {
            index = Random.Range(0, creationPoint.Count);
            direction = Random.Range(0, 4);
            obj2 = CreateObj(creationPoint[index], direction);
            obj2.transform.GetChild(2).gameObject.SetActive(true);
            obj2.GetComponent<Town>().SetType(AwnerType.Neutrality);
            creationPoint.RemoveAt(index);
            obj2.SetActive(true);
        }
        for (int i = 0; i < creationPoint.Count; ++i)
        {
            index = Random.Range(0, 100);
            if(index >= 99)
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
