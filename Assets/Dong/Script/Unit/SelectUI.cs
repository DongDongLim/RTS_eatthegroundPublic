using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectUI : MonoBehaviour
{
    [SerializeField]
    GameObject StateObj;

    [SerializeField]
    GameObject createPanel;

    [SerializeField]
    public UnitData m_data;


    Text hpTxt;

    Text atkTxt;

    Text spdTxt;

    Text rangeTxt;

    Text cntTxt;

    Text resourceTxt;

    Text TimeTxt;

    [SerializeField]
    Text waitingTxt;

    int m_Lv;

    public int waitingCnt = 0;

    public bool isCreating = false;

    [SerializeField]
    public Image coolDownImg;

    bool isStart = false;

    private void Start()
    {
        hpTxt = StateObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        atkTxt = StateObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>();
        spdTxt = StateObj.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
        rangeTxt = StateObj.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>();
        cntTxt = StateObj.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<Text>();
        resourceTxt = StateObj.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<Text>();
        TimeTxt = StateObj.transform.GetChild(6).GetChild(0).GetChild(0).GetComponent<Text>();
        SetInfo();
        isStart = true;
    }

    private void OnEnable()
    {
        if (isStart)
        {
            SetInfo();
        }
    }

    void GetLevel()
    {
        switch (m_data.areaType)
        {
            case UnitTypeArea.POND:
                m_Lv = 0;//TownMng.instance.pondLv;
                break;
            case UnitTypeArea.TREE:
                m_Lv = 0;//TownMng.instance.treeLv;
                break;
            case UnitTypeArea.GRASS:
                m_Lv = 0;//TownMng.instance.grassLv;
                break;
        }
    }

    public void SetInfo()
    {
        GetLevel();
        hpTxt.text = m_data.hp[m_Lv].ToString();
        atkTxt.text = m_data.atk[m_Lv].ToString();
        spdTxt.text = m_data.spd[m_Lv].ToString();
        rangeTxt.text = m_data.range[m_Lv].ToString();
        cntTxt.text = UnitMng.instance.UnitCnt[m_data].ToString();
        resourceTxt.text = m_data.resource.ToString();
        TimeTxt.text = m_data.createTime.ToString();
        if (waitingCnt == 0)
            waitingTxt.text = "";
        else
            waitingTxt.text = waitingCnt.ToString();

        if (TownMng.instance.UnitActivity[m_data])
            createPanel.SetActive(false);
    }

    public void UnitCreateBtn()
    {
        GameMng.instance.UnitCreate(this);
    }


}
