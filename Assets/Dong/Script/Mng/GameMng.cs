using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    protected override void OnAwake()
    {
        isGamePlaying = false;
        GameStart += OnSetDay;
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

}
