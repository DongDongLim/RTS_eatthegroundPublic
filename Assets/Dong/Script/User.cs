using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    Area area;
    OccupyNode _occupyNode;
    LinkedList<Node> startNode;
    IsAngleRight _isRight;
    [SerializeField]
    Material material;
    Node _selectNode;

    private void Awake()
    {
        area = GetComponent<Area>();
        _occupyNode = GetComponent<OccupyNode>();
        startNode = new LinkedList<Node>();
        _isRight = new IsAngleRight();
    }
    public void AddBase(Node node)
    {
        if (startNode.Count > 2)
            _occupyNode.Occupy(node, material);
        else if (startNode.Count < 2)
        {
            startNode.AddLast(node);
        }
        else
        {
            startNode.AddLast(node);
            GameMng.instance.playerNodePos = _isRight.TriangleCenterPoint
                (startNode.First.Value.transform.position,
                startNode.First.Next.Value.transform.position,
                startNode.First.Next.Next.Value.transform.position);
            _occupyNode.Init(GameMng.instance.playerNodePos);
            _occupyNode.SetStartBase(startNode, material);
        }
    }
    public Vector3 SetTaget(Node taget)
    {
        return _occupyNode.SetTarget(area, taget);
    }
}
