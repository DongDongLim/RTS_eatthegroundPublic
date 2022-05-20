using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TownMng : SingletonMini<TownMng>
{
    [SerializeField]
    GameObject UnitParant;

    [SerializeField]
    GameObject[] pondObj;
    [SerializeField]
    TownData pondData;
    int pondLv;


    [SerializeField]
    GameObject[] treeObj;
    [SerializeField]
    TownData treeData;
    int treeLv;

    [SerializeField]
    GameObject[] grassObj;
    [SerializeField]
    TownData grassData;
    int grassLv;


    [SerializeField]
    GameObject a;


    [SerializeField]
    GameObject b;


    [SerializeField]
    GameObject c;


    [SerializeField]
    GameObject d;


    [SerializeField]
    GameObject e;


    [SerializeField]
    GameObject f;


    protected override void OnAwake()
    {
        pondLv = 0;
        treeLv = 0;
        grassLv = 0;
    }

    public void LevelUpPond()
    {
        switch(pondLv)
        {
            case 0:
                pondObj[0].SetActive(false);
                ++pondLv;
                a.SetActive(true);
                //Instantiate((pondData.unitData.FirstOrDefault(x => x.Value == pondLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            case 1:
                pondObj[1].SetActive(false);
                ++pondLv;
                b.SetActive(true);
                //Instantiate((pondData.unitData.FirstOrDefault(x => x.Value == pondLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            case 2:
                pondObj[2].SetActive(false);
                ++pondLv;
                //Instantiate((pondData.unitData.FirstOrDefault(x => x.Value == pondLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            default:
                Debug.Log("최대레벨입니다");
                break;
        }
    }
    public void LevelUpTree()
    {
        switch (treeLv)
        {
            case 0:
                treeObj[0].SetActive(true);
                ++treeLv;
                c.SetActive(true);
                //Instantiate((treeData.unitData.FirstOrDefault(x => x.Value == treeLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            case 1:
                treeObj[1].SetActive(true);
                ++treeLv;
                d.SetActive(true);
                //Instantiate((treeData.unitData.FirstOrDefault(x => x.Value == treeLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            case 2:
                treeObj[2].SetActive(true);
                ++treeLv;
                //Instantiate((treeData.unitData.FirstOrDefault(x => x.Value == treeLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            default:
                Debug.Log("최대레벨입니다");
                break;
        }
    }
    public void LevelUpGrass()
    {
        switch (grassLv)
        {
            case 0:
                grassObj[0].SetActive(true);
                ++grassLv;
                e.SetActive(true);
                //Instantiate((grassData.unitData.FirstOrDefault(x => x.Value == grassLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            case 1:
                grassObj[1].SetActive(true);
                ++grassLv;
                f.SetActive(true);
                //Instantiate((grassData.unitData.FirstOrDefault(x => x.Value == grassLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            case 2:
                grassObj[2].SetActive(true);
                ++grassLv;
                //Instantiate((grassData.unitData.FirstOrDefault(x => x.Value == grassLv).Key as UnitData).prefab, UnitParant.transform, false);
                break;
            default:
                Debug.Log("최대레벨입니다");
                break;
        }
    }

}
