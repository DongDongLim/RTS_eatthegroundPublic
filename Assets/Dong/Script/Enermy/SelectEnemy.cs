using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEnemy : MonoBehaviour
{
    public UnitData m_data;


    int m_Lv;

    public int waitingCnt = 0;

    public bool isCreating = false;



    public void Setting(UnitData data)
    {
        m_data = data;
    }

    public void UnitCreateBtn()
    {
        if (!EnemyMng.instance.UnitActivity[m_data])
        {
            EnemyMng.instance.LevelUp(m_data.areaType);
        }
        else
        {
            EnemyMng.instance.UnitCreate(this);
        }

    }
}
