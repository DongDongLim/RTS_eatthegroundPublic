using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnitMove : UnitMove
{
    BattleMng battleMng;

    Unit Aowner;

    int m_apk;

    int m_spd;

    int m_range;

    //int m_hp;

    [SerializeField]
    Unit m_target;

    [SerializeField]
    Animator[] animator;

    Rigidbody rigid;

    RaycastHit hit;

    RaycastHit hitGround;

    string targetLayer;

    bool isSetting = false;

    Vector3 startPos;

    public override void Setting()
    {
        battleMng = transform.parent.GetComponent<BattleMng>();
        Aowner = GetComponent<Unit>();
        animator = Aowner.animator;
        rigid = Aowner.rigid;
        startPos = transform.position;
        //int plus = TownMng.instance.UnitCnt[Aowner.m_Data];
        m_apk = Aowner.m_Data.atk[2];// * plus + plus;
        m_spd = Aowner.m_Data.spd[2];// * plus + plus;
        rigid.mass = Aowner.m_Data.hp[2];// * plus + plus;
        m_range = Aowner.m_Data.range[2];
        SetTargetBagic();
        StartCoroutine(Fire());
        isSetting = true;
    }

    void SetTargetBagic()
    {
        if (tag == "Player")
        {
            targetLayer = "BattleUnit";
            m_target = battleMng.playerFlag;            
        }
        else if (tag == "Enermy")
        {
            targetLayer = "Town_Unit";
            m_target = battleMng.EnemyFlag;
        }
        else
        {
            targetLayer = "Null";
            m_target = Aowner;
        }
        if (m_target != Aowner)
        {
            rigid.AddForce((m_target.transform.position - transform.position).normalized * 50, ForceMode.Impulse);
        }
        else
        {
            foreach (var anim in animator)
                anim.Play("Clicked");
        }
    }

    public override IEnumerator Move()
    {
        SetTargetBagic();
        if (m_target != Aowner)
        {
            foreach (var anim in animator)
                anim.Play("Bounce");

            while (true)
            {
                if (transform.position.y < 530)
                    transform.position = startPos;
                yield return null;
                Physics.SphereCast(transform.position, m_range, Vector3.zero, out hit, LayerMask.GetMask(targetLayer));
                m_target = hit.collider?.GetComponent<Unit>();
                if (m_target == null)
                    SetTargetBagic();
                if (hitGround.collider == null)
                    continue;
                float i = 0;
                while (i <= 1)
                {
                    rigid.AddForce((m_target.transform.position - transform.position).normalized * m_spd * 500 * Time.deltaTime, ForceMode.Force);
                    i += Time.deltaTime * 2;
                    yield return null;
                }

                i = 0;
                Vector3 dir = transform.position + transform.forward;
                while (i <= 1)
                {
                    transform.LookAt(Vector3.Slerp(dir, m_target.transform.position, i));
                    i += Time.deltaTime * 0.5f;
                    yield return null;
                }
            }
        }
        else
        {
            while(true)
            {
                yield return null;
            }
        }
    }
    
    IEnumerator Fire()
    {
        while(true)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Town_Unit"))
            {
                if (CameraMng.instance.curCam == CameraMng.instance.camList[3]
                ||
                CameraMng.instance.curCam == CameraMng.instance.camList[4])
                {

                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (m_target != Aowner)
                        {
                            rigid.AddForce((m_target.transform.position - transform.position).normalized * 50, ForceMode.Impulse);
                        }
                        else
                        {
                            foreach (var anim in animator)
                                anim.Play("Clicked");
                        }

                    }
                }
            }
            Physics.Raycast(transform.position, Vector3.down, out hitGround, 1f, LayerMask.GetMask("Default"));

            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isSetting)
        {
            if (tag == "Player")
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("BattleUnit"))
                {
                    collision.gameObject.GetComponent<Damaged>().Damaged(m_apk);
                }
                if (collision.gameObject.tag == "Die")
                    transform.position = startPos;
            }
            else if (tag == "Enermy")
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Town_Unit"))
                {
                    collision.gameObject.GetComponent<Damaged>().Damaged(m_apk);
                }
                if (collision.gameObject.tag == "Die")
                    transform.position = startPos;
            }
        }
    }

}
