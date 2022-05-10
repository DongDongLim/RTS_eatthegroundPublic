using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineScript : MonoBehaviour
{
    public CinemachineVirtualCamera cam;

    private void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        GameMng.instance.GameStart += OnSetPriority;
    }
    public void OnSetPriority()
    {
        switch (cam.Priority)
        {
            case 10:
                cam.Priority = 11;
                break;
            case 11:
                cam.Priority = 10;
                break;
            default:
                break;
        }
    }

    public void OnSetPriority(int priority)
    {
        cam.Priority = priority;
    }

    public void Fllowing(GameObject target)
    {
        cam.Follow = target.transform;
    }
}
