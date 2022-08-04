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
    public bool IsIntersects(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int i = 0)
    {
        Vector3[] ccw1 = new Vector3[] { a, b, d };
        Vector3[] ccw2 = new Vector3[] { a, b, c };
        Vector3[] ccw3 = new Vector3[] { c, d, a };
        Vector3[] ccw4 = new Vector3[] { c, d, b };

        switch (i)
        {
            case 0:
                return ((isThreeAngleRight(ccw1) == isThreeAngleRight(ccw3)) && (isThreeAngleRight(ccw2) == isThreeAngleRight(ccw4))) && (isThreeAngleRight(ccw1) != isThreeAngleRight(ccw2));
            default:
                return ((isThreeAngleRight(ccw1) == isThreeAngleRight(ccw3))) && (isThreeAngleRight(ccw3) != isThreeAngleRight(ccw4));

        }
    }

    public bool isPointInTriangleVer2(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 exPos)
    {
        Vector3[] point1to2 = new Vector3[] { point1, point2, exPos };
        Vector3[] point2to3 = new Vector3[] { point2, point3, exPos };
        Vector3[] point3to1 = new Vector3[] { point3, point1, exPos };
        if (isThreeAngleRight(new Vector3[] { point1, point2, point3 }))
            return (isThreeAngleRight(point1to2) && isThreeAngleRight(point2to3) && isThreeAngleRight(point3to1));
        else
            return !(isThreeAngleRight(point1to2) && isThreeAngleRight(point2to3) && isThreeAngleRight(point3to1));
    }

    // 삼각형 꼭지점 3개, 확인할 점
    public bool isPointInTriangle(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 exPos)
    {
        if ((IsIntersects(point1, point2, exPos, point3, 1))
               &&
               (IsIntersects(point3, point1, exPos, point2, 1))
               &&
              (IsIntersects(point3, point2, exPos, point1, 1))
               )
        {
            return true;
        }
        return false;
    }


    // https://jwmath.tistory.com/105
    // 좌하, 상, 우하의 순서대로 들어오는 삼각형의 무게중심점
    public Vector3 TriangleCenterPoint(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 vec = (b - ((b - (c - ((c - a) * 0.5f))) * 2 / 3));
        return vec;
    }

}
