using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    [SerializeField]
    int index;

    public void SetIndex(int num)
    {
        index = num;
    }

    private void OnMouseUpAsButton()
    {
        GameObject ui = UIMng.instance.GetInfoUI(index);
        ui.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        ui.SetActive(true);
    }
}
