using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    [SerializeField]
    int index;

    private void Awake()
    {
        index = transform.parent.GetSiblingIndex();
    }

    private void OnMouseUpAsButton()
    {
        
    }
}
