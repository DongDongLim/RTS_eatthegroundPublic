using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AwnerType
{
    Player,
    Enermy,
    Neutrality,
}
public class Town : MonoBehaviour
{
    [SerializeField]
    int index;

    [SerializeField]
    public Transform pos;

    public List<GameObject> LinkedTown = new List<GameObject> ();

    public Vector3 verticePos;

    public AwnerType type = AwnerType.Neutrality;

    LineRenderer line;

    List<GameObject> boxList = new List<GameObject>();

    RaycastHit hit;

    public bool isOccupation;

    private void OnEnable()
    {
        switch (type)
        {
            case AwnerType.Player:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case AwnerType.Enermy:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            default:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.white;
                return;
        }
        line.SetPosition(0, pos.position + Vector3.up);
    }

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        isOccupation = false;
    }

    private void Update()
    {
        if (!isOccupation && type == AwnerType.Neutrality)
        {
            Physics.Raycast(pos.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Area"));
            if (null != hit.collider)
                switch(hit.collider.tag)
                {
                    case "Player":
                        SetIndex(0);
                        transform.GetChild(2).gameObject.SetActive(false);
                        transform.GetChild(0).gameObject.SetActive(true);
                        break;
                    case "Enermy":
                        SetIndex(1);
                        transform.GetChild(2).gameObject.SetActive(false);
                        transform.GetChild(1).gameObject.SetActive(true);
                        break;
                }
        }
        else if(!isOccupation && type == AwnerType.Player)
        {
            Physics.Raycast(pos.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Area"));
            if (null != hit.collider)
                switch (hit.collider.tag)
                {
                    case "Enermy":
                        SetIndex(1);
                        transform.GetChild(2).gameObject.SetActive(false);
                        transform.GetChild(1).gameObject.SetActive(true);
                        break;
                }
        }
        else if(!isOccupation && type == AwnerType.Enermy)
        {
            Physics.Raycast(pos.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Area"));
            if (null != hit.collider)
                switch (hit.collider.tag)
                {
                    case "Player":
                        SetIndex(0);
                        transform.GetChild(2).gameObject.SetActive(false);
                        transform.GetChild(0).gameObject.SetActive(true);
                        break;
                }
        }
    }

    public void SetlinePos()
    {
        float dis;
        Vector3 vecDis;
        line.positionCount = LinkedTown.Count * 2;
        boxList.Clear();
        for (int i = 0; i < LinkedTown.Count * 2; ++i)
        {
            line.SetPosition(i, (i) % 2 == 0 ? pos.position + (Vector3.up * 0.2f) : LinkedTown[i / 2].GetComponent<Town>().pos.position + (Vector3.up * 0.2f));
        }
        for(int i = 0; i < LinkedTown.Count; ++i)
        {
            Vector3 vec = LinkedTown[i].GetComponent<Town>().pos.position;
            boxList.Add(new GameObject());
            dis = Vector3.Distance(vec, pos.position);
            vecDis = pos.position - vec;
            BoxCollider col = boxList[boxList.Count - 1].AddComponent<BoxCollider>();
            boxList[boxList.Count - 1].transform.position = pos.position - (vecDis / 2);
            boxList[boxList.Count - 1].transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(vecDis.x, vecDis.z) * Mathf.Rad2Deg, 0));
            boxList[boxList.Count - 1].layer = LayerMask.NameToLayer("Hit");
            boxList[boxList.Count - 1].tag = tag;
            col.size = new Vector3(0.1f, 3f, dis);
        }
    }

    public void AddLinkedTown(GameObject obj)
    {
        if (null == obj.GetComponent<Town>())
            return;
        LinkedTown.Add(obj);
        SetlinePos();
        foreach (GameObject obj2 in obj.GetComponent<Town>().LinkedTown)
        {
            if (obj2 == gameObject)
                return;
        }
        obj.GetComponent<Town>().AddLinkedTown(gameObject);
        switch (type)
        {
            case AwnerType.Player:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case AwnerType.Enermy:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            default:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.white;
                return;
        }
    }

    public void RemoveAllLinkedTown()
    {
        for(int i = 0; i < LinkedTown.Count;)
        { 
            RemoveLinkedTown(LinkedTown[i]);
        }
    }

    public void RemoveLinkedTown(GameObject obj)
    {
        if (LinkedTown.Find(x => x == obj) != null)
        {
            LinkedTown.Remove(obj);
            obj.GetComponent<Town>().RemoveLinkedTown(gameObject);

            switch(LinkedTown.Count)
            {
                case 0:
                    MapMng.instance.RemoveVertexList(type, gameObject);
                    //SetIndex((int)AwnerType.Neutrality);
                    break;
                case 1:
                    LinkedTown[0].GetComponent<Town>().RemoveLinkedTown(gameObject);
                    MapMng.instance.RemoveVertexList(type, gameObject);
                    //type = AwnerType.Neutrality;
                    
                    break;
                case 2:
                    if(LinkedTown[0].GetComponent<Town>().LinkedTown.Find(x => x== LinkedTown[1]) == null)
                    {
                        LinkedTown[0].GetComponent<Town>().RemoveLinkedTown(gameObject);
                        MapMng.instance.RemoveVertexList(type, gameObject);
                        //type = AwnerType.Neutrality;
                    }
                    break;
            }

        }
        SetlinePos();
        switch (type)
        {
            case AwnerType.Player:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case AwnerType.Enermy:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            default:
                transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.white;
                return;
        }
    }

    public void SetIndex(int num)
    {
        index = num;
        type = (AwnerType)num;
    }


    private void OnMouseUpAsButton()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            GameObject ui = UIMng.instance.GetInfoUI(index);
            ui.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            ui.SetActive(true);
            MapMng.instance.curSelectTown = gameObject;
        }
    }

}
