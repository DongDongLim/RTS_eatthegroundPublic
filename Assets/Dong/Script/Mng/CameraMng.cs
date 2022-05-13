using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMng : SingletonMini<CameraMng>
{
    [SerializeField]
    CinemachineScript moveCam;

    [SerializeField]
    GameObject moveCamGameObject;

    [SerializeField]
    CinemachineScript fixCam;

    [SerializeField]
    Camera minimapCamera;

    [SerializeField]
    int cameraSpd;

    bool isCameraPos = false;


    protected override void OnAwake()
    {
        GameMng.instance.GameStart += SetCameraPoistion;
        GameMng.instance.DayAction += CameraSize;
    }

    public void CameraSize()
    {
        minimapCamera.orthographicSize += 20;
    }

    private void Update()
    {
        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            MoveCamSwitch();
            moveCamGameObject.transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * cameraSpd * Time.deltaTime, Space.World);
            //moveCam.transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * cameraSpd * Time.deltaTime, Space.World);
        }
        if (Input.GetButtonDown("Jump"))
            FixCamSwitch();
    }

    public void SetCameraPoistion()
    {
        moveCamGameObject.transform.position = GameMng.instance.GetPlayerTransform().position;
        isCameraPos = true;
    }

    public void CameraMoveToMinimap()
    {
        MoveCamSwitch();
        Vector3 movePos = minimapCamera.ScreenToWorldPoint(Input.mousePosition);
        moveCamGameObject.transform.position = new Vector3(movePos.x, 0, movePos.z);
        Debug.Log(movePos);
        UIMng.instance.ActiveMiniMap();
    }


    void FixCamSwitch()
    {
        moveCam.OnSetPriority(9);
        fixCam.OnSetPriority(10);
        isCameraPos = false;
    }

    void MoveCamSwitch()
    {
        if(!isCameraPos)
        SetCameraPoistion();

        moveCam.OnSetPriority(10);
        fixCam.OnSetPriority(9);
    }
}
