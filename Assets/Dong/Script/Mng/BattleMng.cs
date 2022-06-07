using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleMng : MonoBehaviour
{
    [SerializeField]
    Transform[] spawnPointPlayer;

    bool[] spawnCheckPlayer;

    [SerializeField]
    Transform[] spawnPointEnemy;

    bool[] spawnCheckEnemy;

    int spawnIndex;

    public Unit playerFlag;

    public Unit EnemyFlag;

    List<GameObject> battleUnit = new List<GameObject>();

    public GameObject PlayerEffect;

    public GameObject PlayerEffect1;

    public GameObject EnemyEffect;

    public GameObject EnemyEffect1;

    private void Start()
    {
        spawnCheckPlayer = new bool[spawnPointPlayer.Length];
        spawnCheckEnemy = new bool[spawnPointEnemy.Length];
        if (gameObject.scene.name == "Battle")
            AttackSet();
        else
            DefanceSet();
    }

    void AttackSet()
    {
        GameObject obj;
        foreach (var unit in TownMng.instance.atkUnit)
        {
            if (UnitMng.instance.UnitCnt[unit] == 0)
                continue;
            do
            {
                spawnIndex = Random.Range(0, spawnPointPlayer.Length);
            }
            while (spawnCheckPlayer[spawnIndex]);

            obj = Instantiate(unit.prefab, spawnPointPlayer[spawnIndex].transform.position, Quaternion.Euler(0, 90, 0));
            obj.transform.SetParent(transform);
            battleUnit.Add(obj);
            spawnCheckPlayer[spawnIndex] = true;
        }
        foreach (var unit in EnemyMng.instance.defUnit)
        {
            if (UnitMng.instance.UnitCnt[unit] == 0)
                continue;
            do
            {
                spawnIndex = Random.Range(0, spawnPointEnemy.Length);
            }
            while (spawnCheckEnemy[spawnIndex]);
            obj = Instantiate(unit.prefab, spawnPointEnemy[spawnIndex].transform.position, Quaternion.Euler(0, 90, 0));
            obj.transform.SetParent(transform);
            battleUnit.Add(obj);
            spawnCheckEnemy[spawnIndex] = true;
        }
    }

    void DefanceSet()
    {
        GameObject obj;
        foreach (var unit in TownMng.instance.defUnit)
        {
            if (UnitMng.instance.UnitCnt[unit] == 0)
                continue;
            do
            {
                spawnIndex = Random.Range(0, spawnPointPlayer.Length);
            }
            while (spawnCheckPlayer[spawnIndex]);
            obj = Instantiate(unit.prefab, spawnPointPlayer[spawnIndex].transform.position, Quaternion.Euler(0, 90, 0));
            obj.transform.SetParent(transform);
            battleUnit.Add(obj);
            spawnCheckPlayer[spawnIndex] = true;
        }
        foreach (var unit in EnemyMng.instance.atkUnit)
        {
            if (UnitMng.instance.UnitCnt[unit] == 0)
                continue;
            do
            {
                spawnIndex = Random.Range(0, spawnPointEnemy.Length);
            }
            while (spawnCheckEnemy[spawnIndex]);
            obj = Instantiate(unit.prefab, spawnPointEnemy[spawnIndex].transform.position, Quaternion.Euler(0, 90, 0));
            obj.transform.SetParent(transform);
            battleUnit.Add(obj);
            spawnCheckEnemy[spawnIndex] = true;
        }
    }

    public void EndBattle()
    {
        foreach (var obj in battleUnit)
            Destroy(obj);
    }

}
