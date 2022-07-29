using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapActivator : MonoBehaviour
{
    int index;
    int direction;
    HashSet<int> exNum = new HashSet<int>();

    MapExpander mapExpander;

    MapCreater mapCreater;

    Query query = new Query();

    public void init(MapExpander mapExpander, MapCreater mapCreater)
    {
        this.mapExpander = mapExpander;
        this.mapCreater = mapCreater;
        UserSetting();
        GameMng.instance.DayAction += MapSetting;
    }
    public void UserSetting()
    {
        index = Random.Range(0, mapExpander.creationPointList.Count);
        direction = Random.Range(0, 4);
        int enemyDirection = (direction + 2) % 4;
        GameObject playerObj;
        GameObject enemyObj;
        Vector3 createPos = mapExpander.creationPointList[index] + (Vector3.right * 4);

        GameMng.instance.user.AddBase
            (mapCreater.CreateObj(createPos, direction, AwnerType.Player).GetComponent<Node>());
        exNum.Add(mapExpander.creationPointList.IndexOf(createPos));

        createPos = mapExpander.creationPointList[index] + (Vector3.back * 4); 
        GameMng.instance.user.AddBase
            (mapCreater.CreateObj(createPos, direction, AwnerType.Player).GetComponent<Node>());
        exNum.Add(mapExpander.creationPointList.IndexOf(createPos));

        playerObj = mapCreater.CreateObj(mapExpander.creationPointList[index], direction, AwnerType.Player);
        GameMng.instance.user.AddBase
            (playerObj.GetComponent<Node>());
        exNum.Add(index);

        
        
        
        // 적
        index = query.RandNumber(0, mapExpander.creationPointList.Count, exNum);

        createPos = mapExpander.creationPointList[index] + (Vector3.right * 4);
        EnemyMng.instance.ai.AddBase
            (mapCreater.CreateObj(createPos, enemyDirection, AwnerType.Enemy).GetComponent<Node>());
        exNum.Add(mapExpander.creationPointList.IndexOf(createPos));

        createPos = mapExpander.creationPointList[index] + (Vector3.back * 4);
        EnemyMng.instance.ai.AddBase
            (mapCreater.CreateObj(createPos, enemyDirection, AwnerType.Enemy).GetComponent<Node>());
        exNum.Add(mapExpander.creationPointList.IndexOf(createPos));

        enemyObj = mapCreater.CreateObj(mapExpander.creationPointList[index], enemyDirection, AwnerType.Enemy);
        EnemyMng.instance.ai.AddBase
            (enemyObj.GetComponent<Node>());
        exNum.Add(index);

        GameMng.instance.SetUser(playerObj, enemyObj);
        MapSetting();
    }

    public void MapSetting()
    {
        GameObject obj;
        for (int i = 0; i < mapExpander.range / 2 - (GameMng.instance.GetDay() * 8) - 5; ++i)
        {
            index = query.RandNumber(0, mapExpander.creationPointList.Count, exNum);
            direction = Random.Range(0, 4);
            obj = mapCreater.CreateObj(mapExpander.creationPointList[index], direction, AwnerType.Neutrality);
            exNum.Add(index);
            EnemyMng.instance.addTargetCandidate.Add(obj);
        }
        for (int i = 0; i < mapExpander.creationPointList.Count; ++i)
        {
            if (exNum.Contains(i))
                continue;
            index = Random.Range(0 , 100);
            if (index >= 99)
            {
                index = Random.Range(0, 4);
                switch (index)
                {
                    case 0:
                        Instantiate(MapMng.instance.TreeObj, MapMng.instance.MapObj.transform, false).transform.position = mapExpander.creationPointList[i];
                        break;
                    case 1:
                        Instantiate(MapMng.instance.TreeObj, MapMng.instance.MapObj.transform, false).transform.position = new Vector3(mapExpander.creationPointList[i].x, 0, -mapExpander.creationPointList[i].z);
                        break;
                    case 2:
                        Instantiate(MapMng.instance.TreeObj, MapMng.instance.MapObj.transform, false).transform.position = -mapExpander.creationPointList[i];
                        break;
                    case 3:
                        Instantiate(MapMng.instance.TreeObj, MapMng.instance.MapObj.transform, false).transform.position = new Vector3(-mapExpander.creationPointList[i].x, 0, mapExpander.creationPointList[i].z);
                        break;
                    default:
                        break;
                }
            }

        }
        mapExpander.RangeExpander();
    }
}
