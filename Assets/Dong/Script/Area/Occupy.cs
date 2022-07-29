using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Occupy
{
    protected bool _tagetIsMine;
    protected Node _targetNode;
    protected IsAngleRight _isRight;
    protected TriangleCondition _condition;
    protected LinkedList<Node> _disList;
    protected LinkedList<Node> _nodeList;
    protected Vector3 _exPos;
    protected Node[] _curVertex;
    protected Query<Node> _query;
    public void Init(Vector3 exPos, ref LinkedList<Node> nodeList, ref Node[] curVertex)
    {
        _exPos = exPos;
        _isRight = new IsAngleRight();
        _condition = new TriangleCondition();
        _disList = new LinkedList<Node>();
        _nodeList = nodeList;
        _curVertex = curVertex;
        _query = new Query<Node>();
    }
    public abstract IEnumerable<bool> IsAttackPossible(Node taget, bool tagetIsMine);
}
