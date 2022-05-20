using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfo : MonoBehaviour
{
    [SerializeField]
    public StringObj uiList;

    private void Awake()
    {
        foreach (var item in uiList)
        {
            UIMng.instance.uiList.Add(item.Key, item.Value);
        }
    }

    private void OnDestroy()
    {
        foreach (var item in uiList)
        {
            UIMng.instance.uiList.Remove(item.Key);
        }
    }


}
