using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnitMove : UnitMove
{
    BattleMng battleMng;

    Unit Aowner;

    int m_atk;

    int m_spd;

    int m_range;

    int m_hp;

    Unit m_target;

    [SerializeField]
    Animator[] animator;

    NavMeshAgent agent;

    public override void Setting()
    {
        battleMng = transform.parent.GetComponent<BattleMng>();
        Aowner = GetComponent<Unit>();
        animator = Aowner.animator;
        agent = Aowner.agent;
        int plus = TownMng.instance.UnitCnt[Aowner.m_Data];
        m_atk = Aowner.m_Data.atk[0] * plus + plus;
        m_spd = Aowner.m_Data.spd[0] * plus + plus;
        m_hp = Aowner.m_Data.hp[0] * plus + plus;
        m_range = Aowner.m_Data.range[0];
        SetTargetBagic();
    }

    void SetTargetBagic()
    {
        if (tag == "Player")
            m_target = battleMng.playerFlag;
        else if (tag == "Enermy")
            m_target = battleMng.EnermyFlag;
        else
            m_target = Aowner;
    }

    public override IEnumerator Move()
    {
        while(true)
        {
            
            if (m_target == null)
                SetTargetBagic();

            agent.destination = m_target.transform.position;
            yield return null;
        }
    }

}
