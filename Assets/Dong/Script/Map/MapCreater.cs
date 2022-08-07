using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreater : MonoBehaviour
{
    public enum CreationDir
    {
        NONE = -1,
        NORTHEAST,
        SOUTHEAST,
        NORTHWEST,
        SOUTHWEST,
        END
    }


    Dictionary<int, GameObject>[] popList;

    ObjectPooling pooling = new ObjectPooling();


    [SerializeField]
    GameObject Town;

    [SerializeField]
    GameObject Map;

    int poolingCount = 2331;

    int idCnt;

    int registrationNumber;
    private void Awake()
    {
        idCnt = 0;
        pooling.OnRePooing += PooingObj;
        pooling.OnRePooing?.Invoke();
        popList = new Dictionary<int, GameObject>[GameMng.instance.GetDayActionStopDate() + 1];
        for (int i = 0; i < popList.Length; ++i)
            popList[i] = new Dictionary<int, GameObject>();
        GameMng.instance.DayAction += idInit;
    }

    public void idInit()
    {
        idCnt = 0;
    }

    void PooingObj()
    {
        pooling.PoolingObj(Town, Map.transform, poolingCount);
    }

    public GameObject CreateObj(Vector3 pos, int direction, AwnerType type)
    {
        GameObject obj;
        switch (direction)
        {
            case 0:
                obj = pooling.PopObj(pos);
                break;
            case 1:
                obj = pooling.PopObj(new Vector3(pos.x, 0, -pos.z));
                break;
            case 2:
                obj = pooling.PopObj(-pos);
                break;
            case 3:
                obj = pooling.PopObj(new Vector3(-pos.x, 0, pos.z));
                break;
            default:
                return null;
        }
        registrationNumber = ((idCnt++) * 10) + direction;
        obj.GetComponent<Node>().registrationNumber = registrationNumber;
        obj.transform.GetChild((int)type).gameObject.SetActive(true);
        obj.GetComponent<Node>().SetType(type);
        obj.SetActive(true);
        popList[GameMng.instance.GetDay()].Add(registrationNumber, obj);
        return obj;
    }
}
