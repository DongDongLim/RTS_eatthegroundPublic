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
public class Town : MonoBehaviour
{
    [SerializeField]
    public Vector3 pos;

    public List<GameObject> nodeList = new List<GameObject> ();

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
                GameMng.instance.isAttackWin = true;
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

    public void AddnodeList(GameObject obj)
    {
        if (null == obj.GetComponent<Town>())
            return;
        nodeList.Add(obj);
        foreach (GameObject obj2 in obj.GetComponent<Town>().nodeList)
        {
            if (obj2 == gameObject)
                return;
        }
        obj.GetComponent<Town>().AddnodeList(gameObject);
    }

    public void RemoveAllnodeList()
    {
        for(int i = 0; i < nodeList.Count;)
        { 
            RemovenodeList(nodeList[i]);
        }
    }

    public void RemovenodeList(GameObject obj)
    {
        if (nodeList.Find(x => x == obj) == null)
            return;

        nodeList.Remove(obj);
        obj.GetComponent<Town>().RemovenodeList(gameObject);
        switch (nodeList.Count)
        {
            case 0:
                MapMng.instance.RemoveVertexList(tag, gameObject);
                break;
            case 1:
                nodeList[0].GetComponent<Town>().RemovenodeList(gameObject);
                MapMng.instance.RemoveVertexList(tag, gameObject);

                break;
            case 2:
                if (nodeList[0].GetComponent<Town>().nodeList.Find(x => x == nodeList[1]) == null)
                {
                    nodeList[0].GetComponent<Town>().RemovenodeList(gameObject);
                    MapMng.instance.RemoveVertexList(tag, gameObject);
                }
                break;
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
            MapMng.instance.curSelectTown = gameObject;
        }
    }

}
