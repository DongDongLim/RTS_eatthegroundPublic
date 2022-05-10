using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    [SerializeField]
    int index;

    private void OnEnable()
    {
        transform.GetChild(3).GetComponent<MeshRenderer>().material.color = transform.GetChild(index).GetComponent<MeshRenderer>().material.color;
    }

    public void SetIndex(int num)
    {
        index = num;
    }

    private void OnMouseUpAsButton()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            GameObject ui = UIMng.instance.GetInfoUI(index);
            ui.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            ui.SetActive(true);
        }
    }
}
