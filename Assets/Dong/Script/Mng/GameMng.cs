using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;

public class GameMng : Singleton<GameMng>
{
    int _Day = 0;
    public int Day { private set { _Day = value; } get { return _Day; } }

    int _DayRealTime = 10;
    public int DayRealTime { get { return _DayRealTime; } }

    bool _isGamePlaying;
    public bool isGamePlaying { private set { _isGamePlaying = value; } get { return _isGamePlaying; } }


    public UnityAction GameStart;

    public UnityAction DayAction;

    [SerializeField]
    CinemachineScript cam;

    [SerializeField]
    GameObject player;

    public Vector3 playerNodePos;

    [SerializeField]
    GameObject enermy;

    public Vector3 enermyNodePos;

    [SerializeField]
    public GameObject playerObj;

    [SerializeField]
    GameObject targetTown;

    NavMeshAgent playerNavMesh;

    protected override void OnAwake()
    {
        isGamePlaying = false;
        GameStart += OnSetDay;
        playerNavMesh = playerObj.GetComponent<NavMeshAgent>();
    }

    public void OnSetDay()
    {
        StartCoroutine("SetDay");
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public GameObject GetEnermy()
    {
        return enermy;
    }

    IEnumerator SetDay()
    {
        while (isGamePlaying)
        {
            yield return new WaitForSeconds(DayRealTime);
            if (Day <= 20)
                DayAction?.Invoke();
            ++Day;
        }
    }

    public void StartBtn()
    {
        isGamePlaying = true;
        GameStart?.Invoke();
    }

    public void SetPlayerName(string name)
    {
        player.name = name;
    }

    public Vector3 GetPlayerTransform()
    {
        return playerNodePos;
    }
    public Vector3 GetEnermyTransform()
    {
        return enermyNodePos;
    }

    public void SetUser(GameObject obj , GameObject obj1)
    {
        player = obj;
        enermy = obj1;
        cam.Fllowing(player);
    }

    public void SetTarget()
    {
        if (null != targetTown)
        {
            return;
        }

        if (MapMng.instance.curSelectTown.GetComponent<Town>().type != AwnerType.Neutrality)
            return;

        playerObj.transform.position = MapMng.instance.PlayerStartPoint();
        playerObj.SetActive(true);
        targetTown = MapMng.instance.curSelectTown;
        playerNavMesh.destination = targetTown.transform.position;
        StartCoroutine("PlayerMove");
    }

    IEnumerator PlayerMove()
    {
        while(playerNavMesh.velocity == Vector3.zero)
        {
            yield return null;
        }

        while (playerNavMesh.velocity != Vector3.zero)
        {
            UIMng.instance.uiList["남은거리"].GetComponent<Text>().text = "Dis : " + string.Format("{0:0.0}",
                RemainingDistance(playerNavMesh.path.corners));
            yield return new WaitForSeconds(0.1f);
        }

        MapMng.instance.Occupyabase(targetTown);
        playerObj.SetActive(false);
        targetTown = null;
        UIMng.instance.uiList["남은거리"].GetComponent<Text>().text = "Dis : 0";
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

    public void MoveTown()
    {
        SceneMng.instance.SceneStreaming("Town");
    }
}
