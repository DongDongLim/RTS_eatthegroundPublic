using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitMove : MonoBehaviour
{
    public abstract void Setting();
    public abstract IEnumerator Move();
}
