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
    public GameObject Enemy;

    public Vector3 EnemyNodePos;

    [SerializeField]
    public GameObject EnemyObj;

    public NavMeshAgent EnemyNavMesh;

    DayRepeat _dayRepeat;


    public List<Node> occupiedTown = new List<Node>();

    public int m_resource = 500;

    public bool isDefanceWin;

    protected override void OnAwake()
    {
        _dayRepeat = new DayRepeat();
        isGamePlaying = false;
        GameStart += OnSetDay;
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
