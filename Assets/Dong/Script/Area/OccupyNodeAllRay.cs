using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyNodeAllRay : Occupy
{
    int count;
    public override IEnumerable<bool> IsAttackPossible(Node taget, bool tagetIsMine)
    {
        _targetNode = taget;
        _tagetIsMine = tagetIsMine;
        if (tagetIsMine)
            _condition.FillConditions(_nodeList, _targetNode, ref _disList);
        else
            _condition.MinimumConditions(_nodeList, _targetNode, ref _disList);
        // 연결할 거점 수 2개이상 존재
        yield return _disList.Count > 2;
        LinkedList<Node> curVectex2;
        count = 0;
        while (true)
        {
            _curVertex[0] = _query.NearestPoint(_disList, _targetNode, count);
            if (null == _curVertex[0])
            {
                //NotOccupyabase();
                yield return false;
            }
            _query.IntersectObj(_curVertex[0].nodeList, _disList, out curVectex2);
            if (0 != curVectex2.Count)
                break;
            if (tagetIsMine)
                yield return false;
            ++count;
        }

        _curVertex[1] = _query.SmallestAngle(curVectex2, _targetNode, _curVertex[0]);

        if (_isRight.isPointInTriangle(_curVertex[1].pos, _curVertex[0].pos, _targetNode.pos, _exPos))
        {
            //NotOccupyabase();
            yield return false;
        }
    }
}
