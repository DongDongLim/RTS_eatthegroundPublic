using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class TownUnitMove : UnitMove
{
    [SerializeField]
    Transform rayStart;

    NavMeshAgent agent;

    RaycastHit hit;

    RaycastHit groundHit;

    Vector3 drawVector = Vector3.down;

    Vector3 startPos;

    [SerializeField]
    float JumpSpd;

    [SerializeField]
    float StdSpd;

    [SerializeField]
    Animator[] animator;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    LayerMask layerMaskEx;

    [SerializeField]
    float MoveCheckTime;

    float CoolDown;

    public override void Setting()
    {
        animator = transform.GetChild(0).GetComponentsInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rayStart = transform.parent.transform;
        startPos = transform.position;
        CoolDown = 0;
    }


    //private void FixedUpdate()
    //{
    //    Debug.DrawRay(hit.collider != null ? hit.point : Vector3.zero, Vector3.up * 3f, Color.red);
    //    Debug.DrawRay(rayStart.transform.position, drawVector * 10f, Color.blue);
    //    AnimationPlay();
    //}

    void AnimationPlay()
    {
        Physics.Raycast(transform.position, Vector3.down, out groundHit, 1f, layerMask.value);
        if (agent.velocity == Vector3.zero)
        {
            foreach (Animator anim in animator)
            {
                anim.Play("Idle A");
                
            }
        }
        else
        {
            if (null == groundHit.collider)
            {
                agent.speed = JumpSpd;
                foreach (Animator anim in animator)
                {
                    anim.Play("Jump");
                }
            }
            else
            {
                agent.speed = StdSpd;
                foreach (Animator anim in animator)
                {
                    if (agent.velocity.sqrMagnitude < 40)
                        anim.Play("Walk");
                    else
                        anim.Play("Run");
                }
            }
        }
       
    }

    public override IEnumerator Move()
    {
        while (true)
        {
            if (Time.time > CoolDown + MoveCheckTime)
            {
                if (agent.velocity.x < 1 && agent.velocity.y < 1 && agent.velocity.z < 1)
                {
                    CoolDown = Time.time;
                    drawVector = (Vector3.down * Random.Range(1f, 0.45f)) + (Vector3.right * Random.Range(-0.7f, 0.71f)) + (Vector3.forward * Random.Range(-0.45f, 0.46f));
                    Physics.Raycast(rayStart.position,
                         drawVector
                         , out hit, Mathf.Infinity, layerMask.value + layerMaskEx.value);
                    if (hit.collider != null)
                    {
                        if (LayerMask.GetMask(LayerMask.LayerToName(hit.collider.gameObject.layer)) == layerMaskEx.value)
                            agent.SetDestination(startPos);
                        else
                            agent.SetDestination(hit.point);
                    }
                    else
                        agent.SetDestination(startPos);
                }
            }

            AnimationPlay();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public Vector3 ReturnStart()
    {
        //transform.position = startPos;
        agent.SetDestination(startPos);
        return startPos;
    }
}
