using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TownMng : SingletonMini<TownMng>
{
    public int caveLv = 0;

    public UnitData[] m_data;

    public Dictionary<ScriptableObject, bool> UnitActivity = new Dictionary<ScriptableObject, bool>();

    public List<UnitData> atkUnit = new List<UnitData>();

    public List<UnitData> defUnit = new List<UnitData>();

    [SerializeField]
    GameObject tiger;

    public Text caveTxt;

    [SerializeField]
    GameObject[] pondObj;
    [SerializeField]
    TownData pondData;
    [SerializeField]
    GameObject pondParant;
    public int pondLv;
    public Text pondTxt;

    [SerializeField]
    GameObject[] treeObj;
    [SerializeField]
    TownData treeData;
    [SerializeField]
    GameObject treeParant;
    public int treeLv;
    public Text treeTxt;

    [SerializeField]
    GameObject[] grassObj;
    [SerializeField]
    TownData grassData;
    [SerializeField]
    GameObject grassParant;
    public int grassLv;
    public Text grassTxt;


    List< ScriptableObject> dataKey = new List<ScriptableObject>();

    protected override void OnAwake()
    {
        pondLv = 0;
        treeLv = 0;
        grassLv = 0;
        foreach(UnitData unit in m_data)
        {
            UnitMng.instance.UnitCnt.Add(unit, 0);
            UnitActivity.Add(unit, false);
            if(unit.battleMode == BattleMode.ATTACK)
                atkUnit.Add(unit);
            else if(unit.battleMode == BattleMode.DEFANCE)
                defUnit.Add(unit);
        }
    }

    void SetLevel()
    {
        UIMng.instance.uiList["동굴"].GetComponent<Text>().text = string.Format("동굴 Lv.{0}", caveLv);
        UIMng.instance.uiList["연못"].GetComponent<Text>().text = string.Format("연못 Lv.{0}", pondLv);
        UIMng.instance.uiList["나무"].GetComponent<Text>().text = string.Format("나무 Lv.{0}", treeLv);
        UIMng.instance.uiList["풀"].GetComponent<Text>().text = string.Format("풀 Lv.{0}", grassLv);
    }

    private void Start()
    {
        SetLevel();
    }

    public void CavePanel()
    {
        UIMng.instance.uiList["빌드업"].SetActive(true);
    }

    public void LevelUpCave()
    {
        if (caveLv != 3)
        {
            ++caveLv;
            caveTxt.text = caveLv.ToString();
            SetLevel();
        }
    }

    public void LevelUp(UnitTypeArea type)
    {
        int lv;
        GameObject[] objArray;
        TownData townData;
        GameObject parent;
        Text lvTxt;
        bool isActive;

        switch (type)
        {
            case UnitTypeArea.POND:
                lv = pondLv;
                objArray = pondObj;
                townData = pondData;
                parent = pondParant;
                lvTxt = pondTxt;
                isActive = false;
                break;
            case UnitTypeArea.TREE:
                lv = treeLv;
                objArray = treeObj;
                townData = treeData;
                parent = treeParant;
                lvTxt = treeTxt;
                isActive = true;
                break;
            case UnitTypeArea.GRASS:
                lv = grassLv;
                objArray = grassObj;
                townData = grassData;
                parent = grassParant;
                lvTxt = grassTxt;
                isActive = true;
                break;
            default:
                return;
        }

        if (lv == caveLv)
        {
            Debug.Log("건물의 레벨은 동굴의 레벨을 초과할 수 없습니다");
            return;
        }
        switch (lv)
        {
            case 0:
                objArray[0].SetActive(isActive);
                ++lv;
                foreach (var data in townData.unitData)
                {
                    if (data.Value == lv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, parent.transform, false);
                }
                dataKey.Clear();
                break;
            case 1:
                objArray[1].SetActive(isActive);
                ++lv;
                foreach (var data in townData.unitData)
                {
                    if (data.Value == lv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, parent.transform, false);
                }
                dataKey.Clear();
                break;
            case 2:
                objArray[2].SetActive(isActive);
                ++lv;
                foreach (var data in townData.unitData)
                {
                    if (data.Value == lv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, parent.transform, false);
                }
                dataKey.Clear();
                break;
            default:
                Debug.Log("최대레벨입니다");
                break;
        }
        lvTxt.text = lv.ToString();
        switch (type)
        {
            case UnitTypeArea.POND:
                pondLv = lv;
                pondObj = objArray;
                pondData = townData;
                pondParant = parent;
                break;
            case UnitTypeArea.TREE:
                treeLv = lv;
                treeObj = objArray;
                treeData = townData;
                treeParant = parent;
                break;
            case UnitTypeArea.GRASS:
                grassLv = lv;
                grassObj = objArray;
                grassData = townData;
                grassParant = parent;
                break;
            default:
                return;
        }
        SetLevel();
    }

    public void LevelUpPond()
    {
        LevelUp(UnitTypeArea.POND);
    }
    public void LevelUpTree()
    {
        LevelUp(UnitTypeArea.TREE);
    }
    public void LevelUpGrass()
    {
        LevelUp(UnitTypeArea.GRASS);
    }

    public void UnitCreatePond()
    {
        UIMng.instance.uiList["연못패널"].SetActive(true);
    }
    public void UnitCreateTree()
    {
        UIMng.instance.uiList["나무패널"].SetActive(true);
    }
    public void UnitCreateGrass()
    {
        UIMng.instance.uiList["풀패널"].SetActive(true);
    }

    public void SetTigerPos()
    {
        Vector3 pos = tiger.GetComponent<TownUnitMove>().ReturnStart();
        tiger.SetActive(false);
        tiger.transform.position = pos;
        tiger.SetActive(true);
        StartCoroutine(tiger.GetComponent<TownUnitMove>().Move());
    }

}
