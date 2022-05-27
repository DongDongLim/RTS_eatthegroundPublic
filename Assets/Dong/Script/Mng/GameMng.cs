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

    public List<Town> occupiedTown = new List<Town>();

    public int m_resource = 500;

    Text DayTxt;
    Image DayFilledImg;

    public float rotateSpd;

    public bool isAttackWin;

    public bool isDefanceWin;

    protected override void OnAwake()
    {
        isGamePlaying = false;
        GameStart += OnSetDay;
        playerNavMesh = playerObj.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        DayTxt = UIMng.instance.uiList["날짜"].transform.GetChild(0).GetComponent<Text>();
        DayFilledImg = UIMng.instance.uiList["날짜"].transform.GetChild(1).GetComponent<Image>();
    }

    private void Update()
    {
        if (isGamePlaying)
        {
            rotateSpd += Time.deltaTime / GameMng.instance.DayRealTime;
            if (rotateSpd >= 1)
                rotateSpd = 0;
            DayFilledImg.fillAmount = rotateSpd;
        }
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
            else
            {
                if (Day == 30)
                    GameEnd();
            }
            ++Day;
            DayTxt.text = Day.ToString();
        }
    }

    void GameEnd()
    {
        int playerOccupied = 0;
        int enermyOccupied = 0;
        foreach(Town town in occupiedTown)
        {
            if (town.type == AwnerType.Player)
                ++playerOccupied;
            else
                ++enermyOccupied;
        }
        if(playerOccupied > enermyOccupied)
        {
            Debug.Log("승리");
        }
        else if (playerOccupied < enermyOccupied)
        {
            Debug.Log("패배");
        }
        else
        {
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
            UIMng.instance.uiList["남은거리"].GetComponent<Text>().text = string.Format("{0:0.0}",
                RemainingDistance(playerNavMesh.path.corners));
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(targetTown.GetComponent<Town>().Battle(true));

        if (isAttackWin)
            MapMng.instance.Occupyabase(targetTown);
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
