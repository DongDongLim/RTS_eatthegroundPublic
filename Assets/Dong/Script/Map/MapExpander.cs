using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExpander : MonoBehaviour
{
    public int range;
    public int rangeExcept;
    int randIndex;
    int direction;
    bool isDis;

    LinkedList<Vector3> creationPoint = new LinkedList<Vector3>();

    Vector3[] creationPointArray;

    public List<Vector3> creationPointList;

    public void init()
    {
        Expander();
        GameMng.instance.DayAction += Expander;
    }

    public void Expander()
    {
        for (int i = 2; i <= range; i += 2)
        {
            isDis = false;
            for (int j = range; j >= 2; j -= 2)
            {
                if (!isDis)
                {
                    if ((new Vector2(i, j) - Vector2.zero).sqrMagnitude > range * range)
                        continue;
                    isDis = true;
                }
                if ((new Vector2(i, j) - Vector2.zero).sqrMagnitude < rangeExcept * rangeExcept)
                {
                    break;
                }
                creationPoint.AddLast(new Vector3(i, 0, j));
            }
        }
        int indexNum = 0;
        creationPointArray = new Vector3[creationPoint.Count];
        foreach (Vector3 vec in creationPoint)
            creationPointArray[indexNum++] = vec;
        creationPointList.Clear();
        creationPointList.AddRange(creationPointArray);        
        creationPoint.Clear();
    }

    public void RangeExpander()
    {
        rangeExcept = range + 2;
        range += 20;
    }
}
