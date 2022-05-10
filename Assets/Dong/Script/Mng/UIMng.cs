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
    StringObj uiList;

    [SerializeField]
    GameObject[] infoUI;

    protected override void OnAwake()
    {
        SceneMng.instance.SceneEnter += DeActiveUI;
        GameMng.instance.GameStart += OnStartBtnDisable;
    }

    public void DeActiveUI(string scene)
    {
        switch (scene)
        {
            case "FullMap":
                break;
            case "Town":
                break;
        }
    }

    public void OnStartBtnDisable()
    {
        if (uiList["이름설정"].GetComponent<Text>().text == "")
            GameMng.instance.SetPlayerName("NoName");
        else
            GameMng.instance.SetPlayerName(uiList["이름설정"].GetComponent<Text>().text);
        uiList["게임시작"].SetActive(false);   
    }

    public GameObject GetInfoUI(int index)
    {
        return infoUI[index];
    }

}
