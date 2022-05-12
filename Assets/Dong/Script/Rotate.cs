using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    float m_rotateSpd;

    void Update()
    {
        if (GameMng.instance.isGamePlaying)
        {
            m_rotateSpd += Time.deltaTime / GameMng.instance.DayRealTime;
            if (m_rotateSpd >= 1)
                m_rotateSpd = 0;
            transform.rotation = Quaternion.Euler(new Vector3(Mathf.Lerp(0, 360, m_rotateSpd), 90, 0));
        }
    }
}
