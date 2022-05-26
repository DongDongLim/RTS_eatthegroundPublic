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
    RectTransform minimapRect;

    [SerializeField]
    int cameraSpd;

    [SerializeField]
    Material[] skyMaterial;

    public Camera[] camList;

    public Camera curCam;

    protected override void OnAwake()
    {
        GameMng.instance.GameStart += SetCameraPoistion;
        //GameMng.instance.GameStart += CamSetting;
        GameMng.instance.DayAction += CameraSize;
        GameMng.instance.DayAction += ChangeSkyBox;
        RenderSettings.skybox = skyMaterial[0];
    }

    private void Start()
    {
        CamSetting();
    }


    public void CamSetting()
    {
        //foreach (var cam in camList)
        //{
        //    if (cam != null)
        //        cam.gameObject.SetActive(false);
        //}
        curCam = camList[0];
        curCam.gameObject.SetActive(true);
    }

    public void CameraSize()
    {
        minimapCamera.orthographicSize += 20;
    }

    private void Update()
    {
        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            moveCamGameObject.transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * cameraSpd * Time.deltaTime, Space.World);            
        }
        if (Input.GetButtonDown("Jump"))
            SetCameraPoistion();

        //if(Input.GetButtonDown("Fire1"))
        //{
        //    Ray mray = curCam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit mmhit;
        //    Physics.Raycast(mray, out mmhit, Mathf.Infinity);
        //    Debug.Log(mmhit.collider.name);

        //}
    }

    public void ChangeSkyBox()
    {
        if (RenderSettings.skybox == skyMaterial[0])
        {
            RenderSettings.skybox = skyMaterial[1];
        }
        else
        {
            RenderSettings.skybox = skyMaterial[0];
        }
    }

    public void SetCameraPoistion()
    {
        moveCamGameObject.transform.position = GameMng.instance.GetPlayerTransform();
    }

    public void CameraMoveToMinimap()
    {
        Ray ray = minimapCamera.ViewportPointToRay(
            Camera.main.ScreenToViewportPoint(new Vector3((Input.mousePosition
            - new Vector3((Screen.width - minimapRect.rect.width) / 2, (Screen.height - minimapRect.rect.height) / 2)).x * ((Screen.width / minimapRect.rect.width))
            , (Input.mousePosition
            - new Vector3((Screen.width - minimapRect.rect.width) / 2, (Screen.height - minimapRect.rect.height) / 2)).y * ((Screen.height / minimapRect.rect.height))
            , 0
           )));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Map"));
        Vector3 movePos = hit.point;
        moveCamGameObject.transform.position = new Vector3(movePos.x, 0, movePos.z);
        UIMng.instance.ActiveMiniMap();
    }

    public void CamSwich(int index)
    {
        curCam.gameObject.SetActive(false);
        curCam = camList[index];
        curCam.gameObject.SetActive(true);
    }

    


}
