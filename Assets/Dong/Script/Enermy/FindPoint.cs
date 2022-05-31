using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FindPoint : MonoBehaviour
{
    public EnemyAI Aowner;

    Area area;


    Query query = new Query();

    IsAngleRight isRight = new IsAngleRight();

    TriangleCondition triangleCondition = new TriangleCondition();

    public Dictionary<GameObject, float[]> targetCandidateDic = new Dictionary<GameObject, float[]>();

    int nodeCount;

    public void Setting()
    {
        area = Aowner.area;
        StartCoroutine(FindTarget());
    }

    IEnumerator FindTarget()
    {
        while (true)
        {
            List<GameObject> dislist = new List<GameObject>();
            GameObject[] curVertex;
            Town targetTown;
            List<GameObject> curVectex2 = new List<GameObject>();
            RaycastHit hit;
            targetCandidateDic.Clear();
            foreach (var town in MapMng.instance.popList)
            {
                if (town.GetComponent<Town>().type == AwnerType.Neutrality)
                {
                    triangleCondition.MinimumConditions(area.vlist, town, ref dislist);
                    if (dislist.Count < 2)
                        continue;

                    Vector3 exPos = GameMng.instance.GetEnemyTransform();
                    if (town.tag == "Enermy")
                    {
                        float dis;
                        Vector3 vecDis;
                        dislist.Clear();
                        bool isTrue = false;
                        foreach (var vec in area.vlist)
                        {
                            dis = Vector3.Distance(vec.GetComponent<Town>().pos, town.GetComponent<Town>().pos);
                            vecDis = vec.GetComponent<Town>().pos - town.GetComponent<Town>().pos;
                            Physics.Raycast(town.GetComponent<Town>().pos + (Vector3.up * 0.1f) + (vecDis.normalized * 2), vecDis.normalized, out hit, dis - 2, LayerMask.GetMask("Hit"));

                            if (hit.collider == null)
                            {
                                isTrue = false;
                                foreach (var node in town.GetComponent<Town>().nodeList)
                                {
                                    if (node == vec)
                                        isTrue = true;
                                }

                                if (vec != town && !isTrue)
                                    dislist.Add(vec);
                            }
                        }
                        if (0 == dislist.Count)
                            continue;

                        curVertex = new GameObject[2];
                        curVertex[0] = query.NearestPoint(dislist, town);
                        curVertex[1] = query.IntersectObj(curVertex[0].GetComponent<Town>().nodeList, town.GetComponent<Town>().nodeList);


                        if (null == curVertex[1])
                            continue;
                    }
                    else
                    {

                        curVertex = new GameObject[3];
                        targetTown = town.GetComponent<Town>();


                        int count = 0;
                        while (true)
                        {
                            curVertex[0] = query.NearestPoint(dislist, town, count);
                            if (null == curVertex[0])
                            {
                                break;
                            }
                            query.IntersectObj(curVertex[0].GetComponent<Town>().nodeList, dislist, out curVectex2);
                            if (0 != curVectex2.Count)
                                break;
                            ++count;
                        }
                        if (null == curVertex[0])
                        {
                            continue;
                        }

                        curVertex[1] = query.SmallestAngle(curVectex2, town, curVertex[0]);
                    }
                    if (isRight.isPointInTriangle(curVertex[1].GetComponent<Town>().pos, curVertex[0].GetComponent<Town>().pos, town.GetComponent<Town>().pos, exPos))
                        continue;
                    nodeCount = 0;

                    foreach (var town1 in MapMng.instance.popList)
                    {
                        if (town1 == town)
                            continue;
                        exPos = town1.GetComponent<Town>().pos;
                        if (isRight.isPointInTriangle(curVertex[1].GetComponent<Town>().pos, curVertex[0].GetComponent<Town>().pos, town.GetComponent<Town>().pos, exPos))
                        {
                            ++nodeCount;
                        }
                    }

                    targetCandidateDic.Add(town, new float[] { nodeCount, Vector3.SqrMagnitude(town.transform.position - area.UnitStartPoint(town)) });
                }
            }
            yield return null;
        }
    }

    public abstract void SelectTarget();
    
}
