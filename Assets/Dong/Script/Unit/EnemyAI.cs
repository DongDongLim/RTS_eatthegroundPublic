using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Area area;
    OccupyNode _occupyNode;
    LinkedList<Node> startNode;
    IsAngleRight _isRight;
    [SerializeField]
    Material material;

    [SerializeField]
    public FindPoint findPoint;

    public SelectEnemy[] enemyUnit;

    float _aiAtkWeight;
    public float aiAtkWeight
    {
        set { _aiAtkWeight = value; }
        get
        {
            if (_aiAtkWeight > 100)
            {
                _aiAtkWeight = 100;
                return _aiAtkWeight;
            }
            else if (_aiAtkWeight < 0)
            {
                _aiAtkWeight = 0;
                return _aiAtkWeight;
            }
            return _aiAtkWeight;
        }
    }

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
            GameMng.instance.EnemyNodePos = _isRight.TriangleCenterPoint
                (startNode.First.Value.transform.position,
                startNode.First.Next.Value.transform.position,
                startNode.First.Next.Next.Value.transform.position);
            _occupyNode.Init(GameMng.instance.EnemyNodePos);
            _occupyNode.SetStartBase(startNode, material);
        }

    }

    public Vector3 SetTaget(Node taget)
    {
        return _occupyNode.SetTarget(area, taget);
    }
    private void Start()
    {
        aiAtkWeight = 50;
        findPoint.Aowner = this;
        enemyUnit = new SelectEnemy[EnemyMng.instance.m_data.Length];
        for (int i = 0; i < EnemyMng.instance.m_data.Length; ++i)
        {
            enemyUnit[i] = new SelectEnemy();
            enemyUnit[i].Setting(EnemyMng.instance.m_data[i]);
        }
        StartCoroutine(SelectTarget());
    }

    IEnumerator SelectTarget()
    {
        while(!GameMng.instance.isGamePlaying)
        {
            yield return new WaitForSeconds(0.5f);
        }
        findPoint.Setting();
        while (true)
        {
            while (EnemyMng.instance.targetNode != null)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (EnemyMng.instance.m_resource >= 100)
            {
                int index = Random.Range(0, enemyUnit.Length);
                enemyUnit[index].UnitCreateBtn();
                yield return new WaitForSeconds(7f);
            }
            else
                yield return StartCoroutine(findPoint.FindTarget());

            yield return new WaitForSeconds(1f);
        }

    }
}
