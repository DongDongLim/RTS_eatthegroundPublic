﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField]
    UnitMove m_Move;

    [SerializeField]
    public Animator[] animator;


    public NavMeshAgent agent;


    public UnitData m_Data;

    private void Start()
    {
        animator = transform.GetChild(0).GetComponentsInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        Setting(gameObject.scene.name);
    }

    private void Setting(string SceneName)
    {
        if (SceneName == "Battle")
        {
            transform.GetChild(0).localPosition = transform.GetChild(2).localPosition;
            m_Move = GetComponent<BattleUnitMove>();
        }
        else if(SceneName == "Town")
        {
            m_Move = GetComponent<TownUnitMove>();
        }
        m_Move.Setting();
        StartCoroutine(m_Move.Move());
    }
}
