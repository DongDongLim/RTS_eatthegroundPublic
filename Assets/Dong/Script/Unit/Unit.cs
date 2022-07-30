using System.Collections;
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

    public Rigidbody rigid;


    public UnitData m_Data;


    private void Start()
    {
        if (transform.GetChild(0).GetComponent<Animator>() == null)
            animator = transform.GetChild(0).GetComponentsInChildren<Animator>();
        else
            animator = new Animator[] { transform.GetChild(0).GetComponent<Animator>() };
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        rigid.drag = 0;
        Setting(gameObject.scene.name);
    }

    private void Setting(string SceneName)
    {
        if (SceneName == "Battle" || SceneName == "BattleDefance")
        {
            agent.enabled = false;
            transform.GetChild(0).localPosition = transform.GetChild(2).localPosition;
            m_Move = GetComponent<BattleUnitMove>();
        }
        else if(SceneName == "Town")
        {
            Destroy(rigid);
            m_Move = GetComponent<TownUnitMove>();
        }
        m_Move.Setting();
        StartCoroutine(m_Move.Move());
    }
}
