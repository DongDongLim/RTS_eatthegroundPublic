using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class StringInt : SerializableDictionary<string, int> { }

[System.Serializable]
public class StringObj : SerializableDictionary<string, GameObject> { }
public class UIMng : Singleton<UIMng>
{
    [SerializeField]
    public StringObj uiList;

    [SerializeField]
    GameObject[] infoUI;

    protected override void OnAwake()
    {
        //SceneMng.instance.SceneExit += DeActiveUI;
        GameMng.instance.GameStart += OnStartBtnDisable;
        SetResource();
    }

    //public void DeActiveUI(string scene)
    //{
    //    switch (scene)
    //    {
    //        case "FullMap":
    //            break;
    //        case "Town":
    //            uiList["마을정보"].SetActive(false);
    //            break;
    //    }
    //}

    public void OnStartBtnDisable()
    {
        uiList["시작후패널"].SetActive(true);
        if (uiList["이름설정"].GetComponent<Text>().text == "")
        {
            GameMng.instance.SetPlayerName("NoName");
            uiList["플레이어이름"].GetComponentInChildren<Text>().text = "NoName";
        }
        else
        {
            GameMng.instance.SetPlayerName(uiList["이름설정"].GetComponent<Text>().text);
            uiList["플레이어이름"].GetComponentInChildren<Text>().text = uiList["이름설정"].GetComponent<Text>().text;
        }
        uiList["플레이어이름"].transform.position = GameMng.instance.GetPlayerTransform() + new Vector3(0, 2, 0);
        uiList["플레이어이름"].SetActive(true);
        uiList["적이름"].transform.position = GameMng.instance.GetEnermyTransform() + new Vector3(0, 2, 0);
        uiList["적이름"].SetActive(true);
        uiList["게임시작"].SetActive(false);
        uiList["미니맵"].SetActive(true);
    }

    public GameObject GetInfoUI(int index)
    {
        for (int i = 0; i < uiList["마을정보"].transform.childCount; ++i)
            uiList["마을정보"].transform.GetChild(i).gameObject.SetActive(false);
        uiList["마을정보"].SetActive(true);
        return infoUI[index];
    }

    public void OnBtnDisable()
    {
        EventSystem.current.currentSelectedGameObject.SetActive(false);
    }

    public void ActiveMiniMap()
    {
        if (!uiList["미니맵"].transform.GetChild(0).gameObject.activeSelf)
        {
            CameraMng.instance.camList[1].gameObject.SetActive(true);
            uiList["미니맵"].transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            CameraMng.instance.camList[1].gameObject.SetActive(false);
            uiList["미니맵"].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void SetResource()
    {
        uiList["자원"].GetComponent<Text>().text = GameMng.instance.m_resource.ToString();
    }

}
