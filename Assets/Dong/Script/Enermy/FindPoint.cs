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

    #region FindTarget변수

    List<GameObject> dislist = new List<GameObject>();
    GameObject[] curVertex;
    Town targetTown;
    List<GameObject> curVectex2 = new List<GameObject>();
    RaycastHit hit;
    Town townScript;
    Town vertex0;
    Town vertex1;
    Vector3 pointVec;
    Collider[] results = new Collider[1120];
    int count;
    float sqrDis;
    float imsiDis;
    Vector3 exPos;
    Vector3 playerPos;

    int curIndexCnt;
    int limmitIndexCnt;

    float dis;
    Vector3 vecDis;
    bool isTrue;

    #endregion

    public void Setting()
    {
        area = Aowner.area;
        playerPos = GameMng.instance.GetPlayerTransform();
    }

    float Ctime;
    public IEnumerator FindTarget()
    {
        EnemyMng.instance.ChangeCandidate();
        targetCandidateDic.Clear();
        curIndexCnt = 0;
        limmitIndexCnt = EnemyMng.instance.targetCandidate.Count;
        Ctime = Time.time;
        foreach (var town in EnemyMng.instance.targetCandidate)
        {
            townScript = town.GetComponent<Town>();
            if (townScript.type == AwnerType.Neutrality)
            {
                triangleCondition.MinimumConditions(area.vlist, town, ref dislist);
                if (dislist.Count < 2)
                    continue;

                exPos = playerPos;
                if (town.tag == "Enermy")
                {
                    dislist.Clear();
                    isTrue = false;
                    foreach (var vec in area.vlist)
                    {
                        dis = Vector3.Distance(vec.GetComponent<Town>().pos, townScript.pos);
                        vecDis = vec.GetComponent<Town>().pos - townScript.pos;
                        Physics.Raycast(townScript.pos + (Vector3.up * 0.1f) + (vecDis.normalized * 2), vecDis.normalized, out hit, dis - 2, LayerMask.GetMask("Hit"));

                        if (hit.collider == null)
                        {
                            isTrue = false;
                            foreach (var node in townScript.nodeList)
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
                    curVertex[1] = query.IntersectObj(curVertex[0].GetComponent<Town>().nodeList, townScript.nodeList);


                    if (null == curVertex[1])
                        continue;
                }
                else
                {

                    curVertex = new GameObject[3];
                    targetTown = townScript;


                    count = 0;
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
                vertex0 = curVertex[0].GetComponent<Town>();
                vertex1 = curVertex[1].GetComponent<Town>();
                if (isRight.isPointInTriangle(vertex1.pos, vertex0.pos, townScript.pos, exPos))
                    continue;
                nodeCount = 0;

                pointVec = isRight.TriangleCenterPoint(vertex1.pos, vertex0.pos, townScript.pos);
                sqrDis = 0; 
                imsiDis = Vector3.Magnitude(townScript.pos - pointVec);
                if (sqrDis < imsiDis)
                sqrDis = imsiDis;
                imsiDis = Vector3.Magnitude(vertex0.pos - pointVec);
                if (sqrDis < imsiDis)
                    sqrDis = imsiDis;
                imsiDis = Vector3.Magnitude(vertex1.pos - pointVec);
                if (sqrDis < imsiDis)
                    sqrDis = imsiDis;
                                
                count = Physics.OverlapSphereNonAlloc(pointVec, sqrDis, results, LayerMask.GetMask("Point"));

                for(int i = 0; i < count; ++i)
                {
                    if (results[i].transform.parent.gameObject == town)
                    {
                        results[i] = null;
                        continue;
                    }
                    if (results[i].transform.parent.GetComponent<Town>().tag != "Untagged")
                    {
                        results[i] = null;
                        continue;
                    }
                    exPos = results[i].transform.parent.GetComponent<Town>().pos;
                    if (isRight.isPointInTriangle(vertex1.pos, vertex0.pos, townScript.pos, exPos))
                    {
                        ++nodeCount;
                    }
                    results[i] = null;
                }

                targetCandidateDic.Add(town, new float[] { nodeCount, Vector3.SqrMagnitude(town.transform.position - area.UnitStartPoint(town)) });
            }
            if (curIndexCnt == 5)//(int)(limmitIndexCnt * 0.25f))
            {
                curIndexCnt = 0;
                yield return null;
            }
            ++curIndexCnt;
        }
        if((Time.time - Ctime) > 7f)
            Debug.Log("실패");
        while((Time.time - Ctime) < 7f)
            yield return new WaitForFixedUpdate();
        SelectTarget();
    }
    public abstract void SelectTarget();

}
