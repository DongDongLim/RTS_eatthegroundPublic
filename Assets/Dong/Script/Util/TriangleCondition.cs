using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleCondition
{
    public void MinimumConditions(List<GameObject> vlist, GameObject target, ref List<GameObject> dislist)
    {
        float dis;
        Vector3 vecDis;
        RaycastHit hit;
        dislist.Clear();
        foreach (var vec in vlist)
        {
            dis = Vector3.Distance(vec.GetComponent<Town>().pos, target.GetComponent<Town>().pos);
            vecDis = vec.GetComponent<Town>().pos - target.GetComponent<Town>().pos;
            Physics.Raycast(target.GetComponent<Town>().pos + (Vector3.up * 0.1f), vecDis.normalized, out hit, dis, LayerMask.GetMask("Hit"));
            if (hit.collider != null)
            {

                foreach (var box in Physics.OverlapBox(target.GetComponent<Town>().pos, new Vector3(1, 1, 1), Quaternion.identity, LayerMask.GetMask("Hit")))
                {
                    if (hit.collider == box)
                    {
                        if (vec != target)
                            dislist.Add(vec);
                    }
                }
            }
            else
            {
                if (vec != target)
                    dislist.Add(vec);
            }
        }
    }
}
