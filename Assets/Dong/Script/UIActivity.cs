using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActivity : MonoBehaviour
{
    private void Awake()
    {
        UIMng.instance.uiList.Add(name, gameObject);
    }
}
