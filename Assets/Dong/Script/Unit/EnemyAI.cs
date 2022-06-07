using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Area area;

    [SerializeField]
    public FindPoint findPoint;

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


            yield return StartCoroutine(findPoint.FindTarget());

            yield return new WaitForSeconds(1f);
        }

    }
}
