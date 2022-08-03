using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyNode : MonoBehaviour
{
    IsAngleRight _isRight;
    LinkedList<Node> _nodeList;
    MeshMaker _meshMaker;
    Node[] _curVertex;
    Occupy _occupy;


    private void Awake()
    {
        _isRight = new IsAngleRight();
        _nodeList = new LinkedList<Node>();
        _meshMaker = new MeshMaker();
        _occupy = new OccupyWayAllRay();
    }

    public void Init(Vector3 expos)
    {
        Debug.Log(expos);
        _curVertex = new Node[2];
        _occupy.Init(expos, ref _nodeList, ref _curVertex);
    }

    public void SetStartBase(LinkedList<Node> startNode, Material material)
    {
        foreach(var node in startNode)
        {
            _nodeList.AddLast(node);
            node.verticePos = node.pos;
            node.isOccupation = true;
            node.tag = tag;
        }

        Node[] startNodeArray = new Node[]
        {
            _nodeList.First.Value,
            _nodeList.First.Next.Value,
            _nodeList.First.Next.Next.Value
        };

        Vector3[] vertices = new Vector3[]
        {
            startNodeArray[0].pos,
            startNodeArray[1].pos,
            startNodeArray[2].pos
        };

        startNodeArray[0].AddnodeList(startNodeArray[1]);
        startNodeArray[0].AddnodeList(startNodeArray[2]);
        startNodeArray[1].AddnodeList(startNodeArray[1]);
       

        int[] indexes = _isRight.isThreeAngleRight(vertices) ?
            new int[] { 0, 1, 2 }
         : new int[] { 0, 2, 1 };

        _meshMaker.MakeMesh(vertices, indexes, tag, material);
    }

    public void Occupy(Node taget, Material material)
    {
        foreach(var iter in _occupy.IsAttackPossible(taget, taget.CompareTag(tag)))
        {
            if (!iter)
                return;
        }

        taget.AddnodeList(_curVertex[0]);
        taget.AddnodeList(_curVertex[1]);

        _nodeList.AddLast(taget);
        taget.isOccupation = true;
        taget.tag = tag;

        Vector3[] vertices = new Vector3[]
           {
               taget.pos,
               taget.nodeList.First.Value.pos,
               taget.nodeList.First.Next.Value.pos
           };

        int[] indexes = _isRight.isThreeAngleRight(vertices) ?
            new int[] { 0, 1, 2 }
         : new int[] { 0, 2, 1 };

        taget.verticePos = vertices[0];

        

        _meshMaker.MakeMesh(vertices, indexes, tag, material);
    }
    public Vector3 SetTarget(Area area, Node taget)
    {
        return area.UnitStartPoint(_nodeList, taget);
    }

}
