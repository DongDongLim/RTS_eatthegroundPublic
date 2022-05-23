using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitTypeArea
{
    NONE,
    POND,
    TREE,
    GRASS, 
}

[CreateAssetMenu(fileName = "UnitData", menuName = "Data/UnitData")]
public class UnitData : Data
{
    public UnitTypeArea areaType;

    public int[] hp;
    public int[] atk;
    public int[] spd;
    public int[] range;

    public int resource;
    public int createTime;
}
