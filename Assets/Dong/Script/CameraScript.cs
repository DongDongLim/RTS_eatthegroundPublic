using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    int depthIndex;

    private void Start()
    {
        CameraMng.instance.camList[depthIndex] = GetComponent<Camera>();
        gameObject.SetActive(false);
    }
}
