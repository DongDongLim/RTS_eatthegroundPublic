using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AwnerType
{
    Neutrality,
    Player,
    Enermy
}
public class Town : MonoBehaviour
{
    [SerializeField]
    int index;

    public List<GameObject> LinkedTown = new List<GameObject> ();

    public Vector3 verticePos;

    public AwnerType type = AwnerType.Neutrality;

    LineRenderer line;

    RaycastHit hit;

    private void OnEnable()
    {
        transform.GetChild(3).GetComponent<MeshRenderer>().material.color = transform.GetChild(index).GetComponent<MeshRenderer>().material.color;
        line.SetPosition(0, transform.position + Vector3.up);
    }

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (LinkedTown.Count == 0)
        {
            Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Area"));
            if (null != hit.collider)
                MapMng.instance.RemoveObj(gameObject);
        }
    }

    public void SetlinePos()
    {
        line.positionCount = LinkedTown.Count * 2;
        for (int i = 0; i < LinkedTown.Count * 2; ++i)
        {
            line.SetPosition(i, (i) % 2 == 0 ? transform.position + Vector3.up : LinkedTown[i / 2].transform.position + Vector3.up);
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
                    type = AwnerType.Neutrality;
                    break;
                case 1:
                    LinkedTown[0].GetComponent<Town>().RemoveLinkedTown(gameObject);
                    MapMng.instance.RemoveVertexList(type, gameObject);
                    type = AwnerType.Neutrality;
                    
                    break;
                case 2:
                    if(LinkedTown[0].GetComponent<Town>().LinkedTown.Find(x => x== LinkedTown[1]) == null)
                    {
                        LinkedTown[0].GetComponent<Town>().RemoveLinkedTown(gameObject);
                        MapMng.instance.RemoveVertexList(type, gameObject);
                        type = AwnerType.Neutrality;
                    }
                    break;
            }

        }
        SetlinePos();
    }

    public void SetIndex(int num)
    {
        index = num;
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
