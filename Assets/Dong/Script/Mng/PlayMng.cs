using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayMng : Singleton<PlayMng>
{
    public User user;

    public GameObject curSelectTown;


    public GameObject playerObj;

    public bool isAttackWin;

    [SerializeField]
    Node targetTown;

    NavMeshAgent playerNavMesh;


    protected override void OnAwake()
    {
        playerNavMesh = playerObj.GetComponent<NavMeshAgent>();
    }


    public void SetTarget()
    {
        if (null != targetTown)
        {
            return;
        }

        if (curSelectTown.GetComponent<Node>().type != AwnerType.Neutrality)
            return;

        targetTown = curSelectTown.GetComponent<Node>();
        playerObj.transform.position = user.SetTaget(targetTown);
        playerObj.SetActive(true);
        playerNavMesh.destination = targetTown.transform.position;
        StartCoroutine(PlayerMove());
    }

    float timeCnt;
    IEnumerator PlayerMove()
    {
        timeCnt = 0;
        while (playerNavMesh.velocity == Vector3.zero && timeCnt < 0.2f)
        {
            timeCnt += Time.deltaTime;
            yield return null;
        }

        while (playerNavMesh.velocity != Vector3.zero)
        {
            UIMng.instance.uiList["남은거리"].GetComponent<Text>().text = string.Format("{0:0.0}",
                RemainingDistance(playerNavMesh.path.corners));
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(targetTown.Battle(true));

        if (isAttackWin)
        {
            EnemyMng.instance.ai.aiAtkWeight -= 2f;
            user.AddBase(targetTown);
        }
        else
        {
            EnemyMng.instance.ai.aiAtkWeight += 2f;
        }
        playerObj.SetActive(false);
        targetTown = null;
        UIMng.instance.uiList["남은거리"].GetComponent<Text>().text = "0";
    }



    // 네비메쉬에이전트의 remainingDistance은 마지막 직선경로만 계산하기 때문에 그 전 노드들을 받아와서 직선경로가 2개이상이라면 그 길이를 따로 계산해줌
    public float RemainingDistance(Vector3[] points)
    {
        if (points.Length < 2) return 0;
        float distance = 0;
        for (int i = 0; i < points.Length - 1; i++)
            distance += Vector3.Distance(points[i], points[i + 1]);
        return distance;
    }

}
