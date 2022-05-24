using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    void Update()
    {
        if (GameMng.instance.isGamePlaying)
        {
            transform.rotation = Quaternion.Euler(new Vector3(Mathf.Lerp(0, 360, GameMng.instance.rotateSpd), 90, 0));
        }
    }
}
