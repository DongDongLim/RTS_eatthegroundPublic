using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotate : MonoBehaviour
{
    private void Awake()
    {
        GameMng.instance.GameStart += StartRotate;
    }

    public void StartRotate()
    {
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while(true)
        {
            transform.rotation = Quaternion.Euler(new Vector3(Mathf.Lerp(0, 360, GameMng.instance.GetDayProgress()), 90, 0));
            yield return null;
        }
    }
}
