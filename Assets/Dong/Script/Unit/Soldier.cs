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
        rigid.AddExplosionForce(apk * 1000, transform.forward, 10, apk * 10000);
    }
}
