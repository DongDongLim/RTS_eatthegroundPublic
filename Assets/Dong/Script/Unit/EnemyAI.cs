using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Area area;

    [SerializeField]
    public FindPoint findPoint;

    public SelectEnemy[] enemyUnit;

    float _aiAtkWeight;
    public float aiAtkWeight
    {
        set { _aiAtkWeight = value; }
        get
        {
            if (_aiAtkWeight > 100)
            {
                _aiAtkWeight = 100;
                return _aiAtkWeight;
            }
            else if (_aiAtkWeight < 0)
            {
                _aiAtkWeight = 0;
                return _aiAtkWeight;
            }
            return _aiAtkWeight;
        }
    }

    private void Start()
    {
        aiAtkWeight = 50;
        findPoint.Aowner = this;
        enemyUnit = new SelectEnemy[EnemyMng.instance.m_data.Length];
        for (int i = 0; i < EnemyMng.instance.m_data.Length; ++i)
        {
            enemyUnit[i] = new SelectEnemy();
            enemyUnit[i].Setting(EnemyMng.instance.m_data[i]);
        }
        StartCoroutine(SelectTarget());
    }

    IEnumerator SelectTarget()
    {
        while(!GameMng.instance.isGamePlaying)
        {
            yield return new WaitForSeconds(0.5f);
        }
        findPoint.Setting();
        while (true)
        {
            while(EnemyMng.instance.targetTown != null)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (EnemyMng.instance.m_resource >= 100)
            {
                EnemyMng.instance.m_resource -= 100;
                int index = Random.Range(0, enemyUnit.Length);
                enemyUnit[index].UnitCreateBtn();
                yield return new WaitForSeconds(2f);
            }
            else
                yield return StartCoroutine(findPoint.FindTarget());

            yield return new WaitForSeconds(1f);
        }

    }
}
