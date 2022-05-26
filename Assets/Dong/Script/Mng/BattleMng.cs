﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleMng : MonoBehaviour
{
    [SerializeField]
    Transform[] spawnPointPlayer;

    bool[] spawnCheckPlayer;

    [SerializeField]
    Transform[] spawnPointEnermy;

    bool[] spawnCheckEnermy;

    int spawnIndex;

    public Unit playerFlag;

    public Unit EnermyFlag;

    private void Start()
    {
        spawnCheckPlayer = new bool[spawnPointPlayer.Length];
        spawnCheckEnermy = new bool[spawnPointEnermy.Length];
        if (GameMng.instance.isAttack)
            AttackSet();
        else
            DefanceSet();
    }

    void AttackSet()
    {
        foreach (var unit in TownMng.instance.atkUnit)
        {
            do
            {
                spawnIndex = Random.Range(0, spawnPointPlayer.Length);
            }
            while (spawnCheckPlayer[spawnIndex]);
            Instantiate(unit.prefab, spawnPointPlayer[spawnIndex].transform.position, Quaternion.Euler(0, 90, 0)).transform.SetParent(transform);
            spawnCheckPlayer[spawnIndex] = true;
        }
    }

    void DefanceSet()
    {
        foreach (var unit in TownMng.instance.defUnit)
        {
            do
            {
                spawnIndex = Random.Range(0, spawnPointPlayer.Length);
            }
            while (spawnCheckPlayer[spawnIndex]);
            Instantiate(unit.prefab, spawnPointPlayer[spawnIndex].transform.position, Quaternion.Euler(0, 90, 0)).transform.SetParent(transform);
            spawnCheckPlayer[spawnIndex] = true;
        }
    }

}
