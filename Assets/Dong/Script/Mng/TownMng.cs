using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TownMng : SingletonMini<TownMng>
{
    public int caveLv = 0;

    public UnitData[] m_data;

    public DataInt UnitCnt = new DataInt();

    public Dictionary<ScriptableObject, bool> UnitActivity = new Dictionary<ScriptableObject, bool>();

    [SerializeField]
    GameObject tiger;

    [SerializeField]
    GameObject[] pondObj;
    [SerializeField]
    TownData pondData;
    [SerializeField]
    GameObject pondParant;
    [SerializeField]
    GameObject pondUnitPanel;
    public int pondLv;


    [SerializeField]
    GameObject[] treeObj;
    [SerializeField]
    TownData treeData;
    [SerializeField]
    GameObject treeParant;
    [SerializeField]
    GameObject treeUnitPanel;
    public int treeLv;

    [SerializeField]
    GameObject[] grassObj;
    [SerializeField]
    TownData grassData;
    [SerializeField]
    GameObject grassParant;
    [SerializeField]
    GameObject grassUnitPanel;
    public int grassLv;


    List< ScriptableObject> dataKey = new List<ScriptableObject>();

    protected override void OnAwake()
    {
        pondLv = 0;
        treeLv = 0;
        grassLv = 0;
        foreach(UnitData unit in m_data)
        {
            UnitCnt.Add(unit, 0);
            UnitActivity.Add(unit, false);
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
            SetLevel();
        }
    }

    public void LevelUpPond()
    {
        if (pondLv == caveLv)
        {
            Debug.Log("연못의 레벨은 동굴의 레벨을 초과할 수 없습니다");
            return;
        }
        switch (pondLv)
        {
            case 0:
                pondObj[0].SetActive(false);
                ++pondLv;
                foreach(var data in pondData.unitData)
                {
                    if (data.Value == pondLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, pondParant.transform, false);
                }
                dataKey.Clear();
                break;
            case 1:
                pondObj[1].SetActive(false);
                ++pondLv;
                foreach (var data in pondData.unitData)
                {
                    if (data.Value == pondLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, pondParant.transform, false);
                }
                dataKey.Clear();
                break;
            case 2:
                pondObj[2].SetActive(false);
                ++pondLv;
                foreach (var data in pondData.unitData)
                {
                    if (data.Value == pondLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, pondParant.transform, false);
                }
                dataKey.Clear();
                break;
            default:
                Debug.Log("최대레벨입니다");
                break;
        }
        SetLevel();
    }
    public void LevelUpTree()
    {
        if (treeLv == caveLv)
        {
            Debug.Log("나무의 레벨은 동굴의 레벨을 초과할 수 없습니다");
            return;
        }
        switch (treeLv)
        {
            case 0:
                treeObj[0].SetActive(true);
                ++treeLv;
                foreach (var data in treeData.unitData)
                {
                    if (data.Value == treeLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, treeParant.transform, false);
                }
                dataKey.Clear();
                break;
            case 1:
                treeObj[1].SetActive(true);
                ++treeLv;
                foreach (var data in treeData.unitData)
                {
                    if (data.Value == treeLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, treeParant.transform, false);
                }
                dataKey.Clear();
                break;
            case 2:
                treeObj[2].SetActive(true);
                ++treeLv;
                foreach (var data in treeData.unitData)
                {
                    if (data.Value == treeLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, treeParant.transform, false);
                }
                dataKey.Clear();
                break;
            default:
                Debug.Log("최대레벨입니다");
                break;
        }
        SetLevel();
    }
    public void LevelUpGrass()
    {
        if (grassLv == caveLv)
        {
            Debug.Log("풀의 레벨은 동굴의 레벨을 초과할 수 없습니다");
            return;
        }
        switch (grassLv)
        {
            case 0:
                grassObj[0].SetActive(true);
                ++grassLv;
                foreach (var data in grassData.unitData)
                {
                    if (data.Value == grassLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, grassParant.transform, false);
                }
                dataKey.Clear();
                break;
            case 1:
                grassObj[1].SetActive(true);
                ++grassLv;
                foreach (var data in grassData.unitData)
                {
                    if (data.Value == grassLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, grassParant.transform, false);
                }
                dataKey.Clear();
                break;
            case 2:
                grassObj[2].SetActive(true);
                ++grassLv;
                foreach (var data in grassData.unitData)
                {
                    if (data.Value == grassLv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                    Instantiate((dataKey[i] as UnitData).prefab, grassParant.transform, false);
                }
                dataKey.Clear();
                break;
            default:
                Debug.Log("최대레벨입니다");
                break;
        }
        SetLevel();
    }

    public void UnitCreatePond()
    {
        pondUnitPanel.SetActive(true);
    }
    public void UnitCreateTree()
    {
        treeUnitPanel.SetActive(true);
    }
    public void UnitCreateGrass()
    {
        grassUnitPanel.SetActive(true);
    }

    public void SetTigerPos()
    {
        tiger.SetActive(false);
        tiger.GetComponent<TownUnitMove>().ReturnStart();
        tiger.SetActive(true);
    }

}
