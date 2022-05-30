using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour, Damaged
{
    Rigidbody rigid;

    Unit Aowner;

    private void Awake()
    {
        Aowner = GetComponent<Unit>();
    }
    
    public void Damaged(int apk)
    {
        if(rigid == null)
            rigid = Aowner.rigid;
        //rigid.velocity = Vector3.zero;
        rigid.AddExplosionForce(apk * 50, transform.forward, 100, apk * 50);
    }
}
