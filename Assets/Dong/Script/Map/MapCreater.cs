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

    ObjectPooing pooling = new ObjectPooing();


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
        popList = new Dictionary<int, GameObject>[GameMng.instance.GetDayActionStopDate()];
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
        StartCoroutine(PooingCor());
    }

    IEnumerator PooingCor()
    {
        int coolDown = 0;
        for (int i = 0; i < poolingCount; ++i)
        {
            pooling.Push(Instantiate(Town, Map.transform, false));
            ++coolDown;
            if (coolDown == 100)
            {
                coolDown = 0;
                yield return null;
            }
        }
    }

    public GameObject CreateObj(Vector3 pos, int direction, AwnerType type)
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
                obj = pooling.Pop(-pos);
                break;
            case 3:
                obj = pooling.Pop(new Vector3(-pos.x, 0, pos.z));
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
