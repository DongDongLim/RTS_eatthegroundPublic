using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleCondition
{
    float dis;
    Vector3 vecDis;
    RaycastHit hit;
    bool isTrue;
    public void MinimumConditions(LinkedList<Node> vecList, Node target, ref LinkedList<Node> dislist)
    {
        dislist.Clear();
        foreach (var vec in vecList)
        {
            dis = Vector3.Distance(vec.pos, target.pos);
            vecDis = vec.pos - target.pos;
            //Physics.Raycast(target.pos + (Vector3.up * 0.1f), vecDis.normalized, out hit, dis, LayerMask.GetMask("Hit"));
            Physics.Raycast(target.pos + (Vector3.up * 0.5f), vecDis.normalized, out hit, dis - 0.1f, LayerMask.GetMask("Hit"));
            if (hit.collider != null)
            {

                foreach (var box in Physics.OverlapBox(target.GetComponent<Node>().pos, new Vector3(1, 1, 1), Quaternion.identity, LayerMask.GetMask("Hit")))
                {
                    if (hit.collider == box)
                    {
                        if (vec != target)
                            dislist.AddLast(vec);
                    }
                }
            }
            else
            {
                if (vec != target)
                    dislist.AddLast(vec);
            }
        }
    }

    public void FillConditions(LinkedList<Node> vecList, Node target, ref LinkedList<Node> dislist)
    {
        isTrue = false;
        dislist.Clear();
        foreach (var vec in dislist)
        {
            dis = Vector3.Distance(vec.pos, target.pos);
            vecDis = vec.pos - target.pos;
            //Physics.Raycast(target.pos + (Vector3.up * 0.1f) + (vecDis.normalized * 2), vecDis.normalized, out hit, dis - 2, LayerMask.GetMask("Hit"));
            Physics.Raycast(target.pos + (Vector3.up * 0.5f), vecDis.normalized, out hit, dis - 0.1f, LayerMask.GetMask("Hit"));
            if (hit.collider == null)
            {
                isTrue = false;
                foreach (var node in target.nodeList)
                {
                    if (node == vec)
                        isTrue = true;
                }

                if (vec != target && !isTrue)
                    dislist.AddLast(target);
            }
        }
    }


}
