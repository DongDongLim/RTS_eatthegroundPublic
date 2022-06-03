using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMng : Singleton<EnemyMng>
{
    #region 마을 변수
    public int caveLv = 0;

    public UnitData[] m_data;

    public Dictionary<ScriptableObject, bool> UnitActivity = new Dictionary<ScriptableObject, bool>();

    public List<UnitData> atkUnit = new List<UnitData>();

    public List<UnitData> defUnit = new List<UnitData>();

    // TODO:이거 쓸일 있는지 확인바람
    [SerializeField]
    UnitData boss;

    [SerializeField]
    TownData mon1Data;

    public int mon1Lv;


    [SerializeField]
    TownData mon2Data;

    public int mon2Lv;

    [SerializeField]
    TownData mon3Data;

    public int mon3Lv;

    List<ScriptableObject> dataKey = new List<ScriptableObject>();

    #endregion

    #region 맵 변수

    [SerializeField]
    public GameObject targetTown;


    #endregion

    #region AI 변수

    public List<GameObject> targetCandidate = new List<GameObject>();

    public List<GameObject> removeTargetCandidate = new List<GameObject>();

    public List<GameObject> addTargetCandidate = new List<GameObject>();

    #endregion

    protected override void OnAwake()
    {
        mon1Lv = 0;
        mon2Lv = 0;
        mon3Lv = 0;
        foreach (UnitData unit in m_data)
        {
            UnitMng.instance.UnitCnt.Add(unit, 0);
            UnitActivity.Add(unit, false);
            if (unit.battleMode == BattleMode.ATTACK)
                atkUnit.Add(unit);
            else if (unit.battleMode == BattleMode.DEFANCE)
                defUnit.Add(unit);
        }
    }

    // 추후 적군의 레벨도 보이게 할지 고민
    //void SetLevel()
    //{
    //    UIMng.instance.uiList["동굴"].GetComponent<Text>().text = string.Format("동굴 Lv.{0}", caveLv);
    //    UIMng.instance.uiList["연못"].GetComponent<Text>().text = string.Format("연못 Lv.{0}", mon1Lv);
    //    UIMng.instance.uiList["나무"].GetComponent<Text>().text = string.Format("나무 Lv.{0}", mon2Lv);
    //    UIMng.instance.uiList["풀"].GetComponent<Text>().text = string.Format("풀 Lv.{0}", mon3Lv);
    //}

    //private void Start()
    //{
    //    SetLevel();
    //}

    #region 마을

    public void LevelUpCave()
    {
        if (caveLv != 3)
        {
            ++caveLv;
        }
    }

    public void LevelUp(UnitTypeArea type)
    {
        int lv;
        TownData townData;

        switch (type)
        {
            case UnitTypeArea.Mon1:
                lv = mon1Lv;
                townData = mon1Data;
                break;
            case UnitTypeArea.Mon2:
                lv = mon2Lv;
                townData = mon2Data;
                break;
            case UnitTypeArea.Mon3:
                lv = mon3Lv;
                townData = mon3Data;
                break;
            default:
                return;
        }

        if (lv == caveLv)
        {
            return;
        }
        switch (lv)
        {
            case 0:
                ++lv;
                foreach (var data in townData.unitData)
                {
                    if (data.Value == lv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                }
                dataKey.Clear();
                break;
            case 1:
                ++lv;
                foreach (var data in townData.unitData)
                {
                    if (data.Value == lv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                }
                dataKey.Clear();
                break;
            case 2:
                ++lv;
                foreach (var data in townData.unitData)
                {
                    if (data.Value == lv)
                        dataKey.Add(data.Key);
                }
                for (int i = 0; i < dataKey.Count; ++i)
                {
                    UnitActivity[dataKey[i]] = true;
                }
                dataKey.Clear();
                break;
            default:
                break;
        }
        switch (type)
        {
            case UnitTypeArea.Mon1:
                mon1Lv = lv;
                mon1Data = townData;
                break;
            case UnitTypeArea.Mon2:
                mon2Lv = lv;
                mon2Data = townData;
                break;
            case UnitTypeArea.Mon3:
                mon3Lv = lv;
                mon3Data = townData;
                break;
            default:
                return;
        }
    }

    public void LevelUpmon1()
    {
        LevelUp(UnitTypeArea.Mon1);
    }
    public void LevelUpmon2()
    {
        LevelUp(UnitTypeArea.Mon2);
    }
    public void LevelUpmon3()
    {
        LevelUp(UnitTypeArea.Mon3);
    }

    #endregion

    #region 맵


    public void SetTarget(GameObject target)
    {
        if (null != targetTown)
        {
            return;
        }

        if (target.GetComponent<Town>().type != AwnerType.Neutrality)
            return;

        targetTown = target;
        GameMng.instance.EnemyObj.transform.position = MapMng.instance.EnemyStartPoint(targetTown);
        GameMng.instance.EnemyObj.SetActive(true);
        GameMng.instance.EnemyNavMesh.destination = targetTown.transform.position;
        StartCoroutine("EnemyMove");
    }

    float timeCnt;
    IEnumerator EnemyMove()
    {
        timeCnt = 0;
        while (GameMng.instance.EnemyNavMesh.velocity == Vector3.zero && timeCnt < 0.2f)
        {
            timeCnt += Time.deltaTime;
            yield return null;
        }

        while (GameMng.instance.EnemyNavMesh.velocity != Vector3.zero)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(targetTown.GetComponent<Town>().Battle(false));

        if (!GameMng.instance.isDefanceWin)
            MapMng.instance.EnemyQccupyabase(targetTown);

        GameMng.instance.EnemyObj.SetActive(false);
        targetTown = null;
    }


    #endregion

    #region AI
    public void ChangeCandidate()
    {
        //foreach (var remove in addTargetCandidate)
        //    targetCandidate.Add(remove);
        targetCandidate.AddRange(addTargetCandidate);
        addTargetCandidate.Clear();
        foreach (var remove in removeTargetCandidate)
            targetCandidate.Remove(remove);
        removeTargetCandidate.Clear();
    }
    #endregion
}
