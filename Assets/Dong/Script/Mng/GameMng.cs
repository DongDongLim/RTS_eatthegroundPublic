using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;

public class GameMng : DontDestroySingleton<GameMng>
{

    bool _isGamePlaying;
    public bool isGamePlaying { private set { _isGamePlaying = value; } get { return _isGamePlaying; } }


    public UnityAction GameStart;

    public UnityAction DayAction;

    [SerializeField]
    CinemachineScript cam;

    [SerializeField]
    GameObject player;

    [SerializeField]
    public User user;

    public Vector3 playerNodePos;

    [SerializeField]
    public GameObject playerObj;

    NavMeshAgent playerNavMesh;

    [SerializeField]
    public GameObject Enemy;

    public Vector3 EnemyNodePos;

    [SerializeField]
    public GameObject EnemyObj;

    public NavMeshAgent EnemyNavMesh;

    [SerializeField]
    GameObject targetTown;

    DayRepeat _dayRepeat;


    public List<Node> occupiedTown = new List<Node>();

    public int m_resource = 500;

    public bool isAttackWin;

    public bool isDefanceWin;

    protected override void OnAwake()
    {
        _dayRepeat = new DayRepeat();
        isGamePlaying = false;
        GameStart += OnSetDay;
        playerNavMesh = playerObj.GetComponent<NavMeshAgent>();
        EnemyNavMesh = EnemyObj.GetComponent<NavMeshAgent>();
    }

    public void OnSetDay()
    {
        StartCoroutine(_dayRepeat.SetDay());
    }

    public int GetDay()
    {
        return _dayRepeat.Day;
    }

    public int GetDayActionStopDate()
    {
        return _dayRepeat.GetDayActionStopDate();
    }

    public float GetDayProgress()
    {
        return _dayRepeat.DayProgress;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public GameObject GetEnemy()
    {
        return Enemy;
    }

    public void GameEnd()
    {
        int playerOccupied = 0;
        int EnemyOccupied = 0;
        foreach(Node town in occupiedTown)
        {
            if (town.type == AwnerType.Player)
                ++playerOccupied;
            else
                ++EnemyOccupied;
        }
        if(playerOccupied > EnemyOccupied)
        {
            GameObject obj = UIMng.instance.uiList["결과"];
            obj.SetActive(true);
            obj.transform.GetChild(0).GetComponent<Text>().text = "승리";
            Debug.Log("승리");
        }
        else if (playerOccupied < EnemyOccupied)
        {
            GameObject obj = UIMng.instance.uiList["결과"];
            obj.SetActive(true);
            obj.transform.GetChild(0).GetComponent<Text>().text = "패배";
            Debug.Log("패배");
        }
        else
        {
            GameObject obj = UIMng.instance.uiList["결과"];
            obj.SetActive(true);
            obj.transform.GetChild(0).GetComponent<Text>().text = "무승부";
            Debug.Log("무승부");
        }
        isGamePlaying = false;
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
    public Vector3 GetEnemyTransform()
    {
        return EnemyNodePos;
    }

    public void SetUser(GameObject playerObj, GameObject EnemyObj)
    {
        player = playerObj;
        Enemy = EnemyObj;
        cam.Fllowing(player);
    }

    public void SetTarget()
    {
        if (null != targetTown)
        {
            return;
        }

        if (PlayMng.instance.curSelectTown.GetComponent<Node>().type != AwnerType.Neutrality)
            return;

        targetTown = PlayMng.instance.curSelectTown;
        playerObj.transform.position = user.SetTaget(targetTown.GetComponent<Node>());
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

        yield return StartCoroutine(targetTown.GetComponent<Node>().Battle(true));

        if (isAttackWin)
        {
            EnemyMng.instance.ai.aiAtkWeight -= 2f;
            user.AddBase(targetTown.GetComponent<Node>());
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

    public void MoveTown()
    {
        if (CameraMng.instance.curCam == CameraMng.instance.camList[0])
            CameraMng.instance.CamSwich(2);
        else
            CameraMng.instance.CamSwich(0);
    }

    public void UnitCreate(SelectUI build)
    {
        if (m_resource < build.m_data.resource)
            return;
        m_resource -= build.m_data.resource;
        UIMng.instance.SetResource();
        ++build.waitingCnt;
        build.SetInfo();
        if (!build.isCreating)
            StartCoroutine(UnitCreateCor(build));
    }

    public IEnumerator UnitCreateCor(SelectUI build)
    {
        build.isCreating = true;
        build.coolDownImg.fillAmount = 1;
        for (float i = 0; i < build.m_data.createTime; i += Time.deltaTime)
        {
            build.coolDownImg.fillAmount = 1 - i / build.m_data.createTime;
            yield return null;
        }
        build.coolDownImg.fillAmount = 0;
        --build.waitingCnt;
        ++UnitMng.instance.UnitCnt[build.m_data];
        if (build.waitingCnt > 0)
            StartCoroutine(UnitCreateCor(build));
        else
            build.isCreating = false;

        build.SetInfo();

    }
}
