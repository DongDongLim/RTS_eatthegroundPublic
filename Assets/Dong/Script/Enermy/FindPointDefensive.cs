using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class FindPointDefensive : FindPoint
{
    public override void SelectTarget()
    {
        var townQuery = from town in targetCandidateDic
                        where town.Value[0] > 0
                        orderby town.Value[1]
                        select town;
        if (townQuery.Count() != 0)
            EnemyMng.instance.SetTarget(townQuery.First().Key);
    }
}
