using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Area area;

    [SerializeField]
    FindPoint findPoint;

    private void Start()
    {
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
        yield return new WaitForSeconds(1f);
        while (true)
        {
            yield return new WaitForSeconds(1f);
            while(findPoint.targetCandidateDic == null)
            {
                yield return null;
            }

            findPoint.SelectTarget();
        }

    }
}
