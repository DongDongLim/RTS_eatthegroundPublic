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
    Camera minimapCamera;

    [SerializeField]
    int cameraSpd;

    bool isCameraPos = false;

    float MoveTime = 0;

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

    public void CameraMoveToMinimap()
    {
        if ((Input.mousePosition - UIMng.instance.uiList["미니맵"].transform.position).sqrMagnitude > 230 * 230)
            return;

        MoveTime = 0;

        StartCoroutine(CameraMove(minimapCamera.ViewportToWorldPoint(minimapCamera.ScreenToViewportPoint(Input.mousePosition)
            - minimapCamera.ScreenToViewportPoint(UIMng.instance.uiList["미니맵"].transform.position - new Vector3(230, 230, 0)))));
    }

    IEnumerator CameraMove(Vector3 end)
    {
        Vector3 start = moveCam.transform.position;
        while (MoveTime <= 1)
        {
            MoveCamSwitch();
            moveCam.transform.position = Vector3.Lerp(start, new Vector3(end.x, moveCam.transform.position.y, end.z), MoveTime);
            MoveTime += Time.deltaTime / 1;
            yield return null;
        }
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
