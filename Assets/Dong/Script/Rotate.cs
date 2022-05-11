using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    float m_rotateSpd;

    void Update()
    {
        transform.Rotate(0, m_rotateSpd * Time.deltaTime, 0);
    }
}
