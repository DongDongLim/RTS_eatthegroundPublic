using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    [SerializeField]
    int index;

    public List<GameObject> LinkedTown = new List<GameObject> ();

    public Vector3 verticePos;

    LineRenderer line;

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

    }

    public void AddLinkedTown(GameObject obj)
    {
        if (null == obj.GetComponent<Town>())
            return;
        LinkedTown.Add(obj);
        line.positionCount = LinkedTown.Count * 2;
        for (int i = 0; i < LinkedTown.Count * 2; ++i)
        {
            line.SetPosition(i, (i)%2 == 0 ? transform.position + Vector3.up : LinkedTown[i / 2].transform.position + Vector3.up);
        }
        foreach(GameObject obj2 in obj.GetComponent<Town>().LinkedTown)
        {
            if (obj2 == gameObject)
                return;
        }
        obj.GetComponent<Town>().AddLinkedTown(gameObject);
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
