using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindPointLow : FindPoint
{
    public override void SelectTarget()
    {
        int limit = (int)(Aowner.aiAtkWeight * 0.1f);
        
        var townQuery = from town in targetCandidateDic
                        where town.Value[0] > limit
                        orderby town.Value[1]
                        select town;
        if (townQuery.Count() != 0)
            EnemyMng.instance.SetTarget(townQuery.First().Key);
        else
        {
            while (limit >= 0)
            {
                --limit;
                townQuery = from town in targetCandidateDic
                            where town.Value[0] > limit
                            orderby town.Value[1]
                            select town;
                if (townQuery.Count() != 0)
                {
                    EnemyMng.instance.SetTarget(townQuery.First().Key);
                    return;
                }
            }
        }
    }

}
