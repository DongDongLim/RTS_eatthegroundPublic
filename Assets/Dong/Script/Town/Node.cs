using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public enum AwnerType
{
    Player,
    Enemy,
    Neutrality,
}
public class Node : MonoBehaviour
{
    [SerializeField]
    public Vector3 pos;

    public int registrationNumber;

    public LinkedList<Node> nodeList = new LinkedList<Node>();

    public Vector3 verticePos;

    public AwnerType type = AwnerType.Neutrality;

    Material material;

    RaycastHit hit;

    public bool isOccupation;

    bool isTarget;

    private void Awake()
    {
        material = transform.GetChild(3).GetComponent<MeshRenderer>().material;
        isTarget = true;
    }
    private void OnEnable()
    {
        pos = transform.position + new Vector3(1, 0, -1);
        isOccupation = false;
        StartCoroutine("AreaCheck");
        switch (type)
        {
            case AwnerType.Player:
                material.color = Color.blue;
                break;
            case AwnerType.Enemy:
                material.color = Color.red;
                break;
            default:
                material.color = Color.black;
                break;
        }

    }
    public void OnDisable()
    {
        StopCoroutine("AreaCheck");
    }

    IEnumerator AreaCheck()
    {
        while (true)
        {
            if (!isOccupation)
            {
                Physics.Raycast(pos + Vector3.up, Vector3.down, out hit, 2f, LayerMask.GetMask("Area"));
                if (null != hit.collider)
                {
                    if(isTarget)
                    {
                        EnemyMng.instance.removeTargetCandidate.Add(gameObject);
                        isTarget = false;
                    }    
                    switch (hit.collider.tag)
                    {
                        case "Player":
                            if (type == AwnerType.Neutrality)
                                GameMng.instance.occupiedTown.Add(this);
                            if (type != AwnerType.Player)
                            {
                                GameMng.instance.m_resource += 100;
                                UIMng.instance.SetResource();
                            }
                            SetType(AwnerType.Player);
                            transform.GetChild(2).gameObject.SetActive(false);
                            transform.GetChild(1).gameObject.SetActive(false);
                            transform.GetChild(0).gameObject.SetActive(true);
                            break;
                        case "Enermy":
                            if (type == AwnerType.Neutrality)
                                GameMng.instance.occupiedTown.Add(this);
                            if (type != AwnerType.Enemy)
                            {
                                EnemyMng.instance.m_resource += 100;
                            }
                            SetType(AwnerType.Enemy);
                            transform.GetChild(2).gameObject.SetActive(false);
                            transform.GetChild(0).gameObject.SetActive(false);
                            transform.GetChild(1).gameObject.SetActive(true);
                            break;
                        default:
                            Debug.Log(hit.collider.name);
                            break;
                    }
                    switch (type)
                    {
                        case AwnerType.Player:
                            material.color = Color.blue;
                            break;
                        case AwnerType.Enemy:
                            material.color = Color.red;
                            break;
                        default:
                            break;
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator Battle(bool isAttack)
    {
        bool isTrue = true;
        if(isAttack)
        {
            if(tag != "Enermy")
            {
                PlayMng.instance.isAttackWin = true;
                yield break;
            }
            SceneMng.instance.SceneStreaming("Battle");
            UIMng.instance.uiList["공격전투"].SetActive(false);
            while (isTrue)
            {
                isTrue = true;
                foreach (var cs in SceneMng.instance.loadScene)
                {
                    if (cs.name == "Battle")
                        isTrue = false;
                }
                yield return null;
            }
            while (!isTrue)
            {
                isTrue = true;
                foreach (var cs in SceneMng.instance.loadScene)
                {
                    if (cs.name == "Battle")
                        isTrue = false;
                }
                yield return null;
            }
            UIMng.instance.uiList["공격전투"].SetActive(true);
        }
        else
        {
            if (tag != "Player")
            {
                GameMng.instance.isDefanceWin = false;
                yield break;
            }
            SceneMng.instance.SceneStreaming("BattleDefance");
            UIMng.instance.uiList["방어전투"].SetActive(false);

            while (isTrue)
            {
                isTrue = true;
                foreach (var cs in SceneMng.instance.loadScene)
                {
                    if (cs.name == "BattleDefance")
                        isTrue = false;
                }
                yield return null;
            }
            while (!isTrue)
            {
                isTrue = true;
                foreach (var cs in SceneMng.instance.loadScene)
                {
                    if (cs.name == "BattleDefance")
                        isTrue = false;
                }
                yield return null;
            }
            UIMng.instance.uiList["방어전투"].SetActive(true);
        }
    }

    public void AddnodeList(Node obj)
    {
        if (null == obj)
            return;
        nodeList.AddLast(obj);
        foreach (Node obj2 in obj.GetComponent<Node>().nodeList)
        {
            if (obj2 == this)
                return;
        }
        obj.AddnodeList(this);
    }

    public void RemoveAllnodeList()
    {
        MapMng.instance.RemoveVertexList(tag, gameObject);
        for (int i = 0; i < nodeList.Count;)
        { 
            RemovenodeList(nodeList.First.Value);
        }
    }

    public void RemovenodeList(Node obj)
    {
        if (!nodeList.Contains(obj))
            return;

        nodeList.Remove(obj);
        obj.RemovenodeList(this);
        switch (nodeList.Count)
        {
            case 0:
                MapMng.instance.RemoveVertexList(tag, gameObject);
                break;
            case 1:
                nodeList.First.Value.RemovenodeList(this);
                MapMng.instance.RemoveVertexList(tag, gameObject);

                break;
            case 2:
                if (nodeList.First.Value.nodeList.Contains(nodeList.First.Next.Value))
                {
                    nodeList.First.Value.RemovenodeList(this);
                    MapMng.instance.RemoveVertexList(tag, gameObject);
                }
                break;
            default:
                break;
        }
    }
    List<GameObject> findStartList = new List<GameObject>();

    public void NodeCheck()
    {
        findStartList.Clear();
        FindStartTown(gameObject);
        if (findStartList.Find(x => x == MapMng.instance.Area_Player.vlist[0] || x == MapMng.instance.Area_Enemy.vlist[0]) == null)
            RemoveAllnodeList();
    }

    public void FindStartTown(GameObject check)
    {
        List<GameObject> findStart = new List<GameObject>();
        foreach (var node in check.GetComponent<Node>().nodeList)
        {
            if (findStartList.Find(x => x == node) == null)
                findStart.Add(node.gameObject);
        }
        foreach(var node in findStart)
        {
            findStartList.Add(node);
            FindStartTown(node);
        }
    }

    public void SetType(AwnerType awnerType)
    {
        type = awnerType;
    }


    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GameObject ui = UIMng.instance.GetInfoUI((int)type);
            ui.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            ui.SetActive(true);
            PlayMng.instance.curSelectTown = gameObject;
        }
    }

}
