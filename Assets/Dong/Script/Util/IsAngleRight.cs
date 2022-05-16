using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsAngleRight
{
    // 점 2가 선 0,1기준으로 오른쪽에 위치하는가
    public bool isThreeAngleRight(Vector3[] vertices)
    {
        return !(((Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                    - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) > 180 &&
                   (Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                    - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) < 360) ||
                    ((Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                    - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) < 0 &&
                    (Mathf.Atan2((vertices[2] - vertices[0]).x, (vertices[2] - vertices[0]).z) * Mathf.Rad2Deg)
                    - (Mathf.Atan2((vertices[1] - vertices[0]).x, (vertices[1] - vertices[0]).z) * Mathf.Rad2Deg) > -180));
    }

    // 선 ab와 cd사이에 교차점이 있는가
    public bool IsIntersects(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3[] ccw1 = new Vector3[] { a, b, d };
        Vector3[] ccw2 = new Vector3[] { a, b, c };
        Vector3[] ccw3 = new Vector3[] { c, d, a };
        Vector3[] ccw4 = new Vector3[] { c, d, b };

        return ((isThreeAngleRight(ccw1) == isThreeAngleRight(ccw3)) && (isThreeAngleRight(ccw2) == isThreeAngleRight(ccw4))) && (isThreeAngleRight(ccw1) != isThreeAngleRight(ccw2));
    }

}
