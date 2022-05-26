using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class StringInt : SerializableDictionary<string, int> { }

[System.Serializable]
public class StringObj : SerializableDictionary<string, GameObject> { }


[System.Serializable]
public class DataInt : SerializableDictionary<ScriptableObject, int> { }


[System.Serializable]
public class TransformBool : SerializableDictionary<Transform, bool> { }