using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    UnitMove m_Move;

    private void Awake()
    {
        m_Move = GetComponent<UnitMove>();
        m_Move.Setting();
    }

    private void Start()
    {
        StartCoroutine(m_Move.Move());
    }
}
