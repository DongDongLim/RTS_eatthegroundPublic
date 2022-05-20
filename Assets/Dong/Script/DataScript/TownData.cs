using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DataInt : SerializableDictionary<ScriptableObject, int> { }


[CreateAssetMenu(fileName = "TownData", menuName = "Data/TownData")]
public class TownData : Data
{
    public DataInt unitData;

    public int[] maxOutPut;

    public int[] requiredResources;

}
