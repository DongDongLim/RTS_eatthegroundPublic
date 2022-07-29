using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapMng : Singleton<MapMng>
{
    [SerializeField]
    GameObject Tree;

    public GameObject TreeObj { private set { } get { return Tree; } }

    [SerializeField]
    GameObject Town;
    public GameObject TownObj { private set { } get { return Town; } }

    [SerializeField]
    GameObject Map;
    public GameObject MapObj { private set { } get { return Map; } }

    [SerializeField]
    GameObject playerArea;

    public Area Area_Player;

    [SerializeField]
    GameObject EnemyArea;

    public Area Area_Enemy;

    [SerializeField]
    MapCreater _mapCreater;

    [SerializeField]
    MapExpander _mapExpander;

    [SerializeField]
    MapActivator _mapActivator;


    protected override void OnAwake()
    {
        _mapExpander.init();
        _mapActivator.init(_mapExpander, _mapCreater);
    }
    
    public void EnemyQccupyabase(GameObject target)
    {
        AddVertex(AwnerType.Enemy, target);
    }

    public void RemoveVertexList(string targetTag, GameObject target)
    {
        switch (targetTag)
        {
            case "Player":
                //Area_Player.RemoveVertexList(target);
                break;
            case "Enermy":
                //Area_Enemy.RemoveVertexList(target);
                break;
            default:
                return;
        }
        target.tag = "Untagged";
    }

    public void AddVertex(AwnerType type, GameObject target)
    {
        switch(type)
        {
            case AwnerType.Player:
                //Area_Player.AddVertex(target);
                break;
            case AwnerType.Enemy:
                //Area_Enemy.AddVertex(target);
                break;
            default:
                return;
        }
    }
}
