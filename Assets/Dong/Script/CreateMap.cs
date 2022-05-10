using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMap : MonoBehaviour
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
    private void Awake()
    {
        pooling.OnRePooing += PooingObj;

        pooling.OnRePooing?.Invoke();
        GameMng.instance.DayAction += MapSetting;
    }

    private void Start()
    {
        MapSetting();
    }

    private void Update()
    {

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
        for (int i = 0; i < range/2 - (GameMng.instance.Day * 30); ++i)
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
