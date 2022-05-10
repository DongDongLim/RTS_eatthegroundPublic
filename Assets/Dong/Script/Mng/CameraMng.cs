using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMng : SingletonMini<CameraMng>
{
    [SerializeField]
    CinemachineScript moveCam;

    [SerializeField]
    CinemachineScript fixCam;

    [SerializeField]
    int cameraSpd;

    bool isCameraPos = false;
    protected override void OnAwake()
    {
        GameMng.instance.GameStart += SetCameraPoistion;
    }

    private void Update()
    {
        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            MoveCamSwitch();
            moveCam.transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * cameraSpd * Time.deltaTime, Space.World);
        }
        if (Input.GetButtonDown("Jump"))
            FixCamSwitch();
    }

    public void SetCameraPoistion()
    {
        moveCam.transform.position = fixCam.transform.position;
        isCameraPos = true;
    }

    void FixCamSwitch()
    {
        moveCam.OnSetPriority(9);
        fixCam.OnSetPriority(11);
        isCameraPos = false;
    }

    void MoveCamSwitch()
    {
        if(!isCameraPos)
        SetCameraPoistion();

        moveCam.OnSetPriority(11);
        fixCam.OnSetPriority(9);
    }
}
