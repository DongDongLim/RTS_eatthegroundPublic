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

    [SerializeField]
    GameObject enermy;

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

    IEnumerator SetDay()
    {
        while (isGamePlaying)
        {
            yield return new WaitForSeconds(DayRealTime);
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

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    public Mesh GetPlayerMesh()
    {
        return player.transform.GetChild(3).GetComponent<MeshFilter>().mesh;
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
            Debug.Log("아직 공격할 수 없습니다");
            return;
        }

        Debug.Log("타겟 정함");
        playerObj.transform.position = MapMng.instance.PlayerStartPoint();
        playerObj.SetActive(true);
        targetTown = MapMng.instance.curSelectTown;
        playerNavMesh.destination = targetTown.transform.position;
        StartCoroutine("PlayerMove");
    }

    IEnumerator PlayerMove()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("이동중");
        while (playerNavMesh.velocity != Vector3.zero)
        {
            UIMng.instance.uiList["남은거리"].GetComponent<Text>().text = "Dis : " + playerNavMesh.remainingDistance;
            yield return null;
        }

        Debug.Log("도착");
        MapMng.instance.Occupyabase(targetTown);
        playerObj.SetActive(false);
        targetTown = null;
        UIMng.instance.uiList["남은거리"].GetComponent<Text>().text = "Dis : 0";
    }

}
