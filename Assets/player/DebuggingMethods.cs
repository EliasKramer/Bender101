using System.Collections.Generic;
using UnityEngine;

public static class DebuggingMethods
{
    public static void PrintAllGameObjects(List<GameObject> gameObjects, string msg)
    {
        if(msg == null)
        {
            msg = "";
        }
        foreach(GameObject gameObject in gameObjects)
        {
            msg+=gameObject.name+"|";
        }
        Debug.Log(msg);
    }
    public static void PrintGameObjectDictionary<ValueType>(Dictionary<GameObject,ValueType> dict, string msg)
    {
        if (msg == null)
        {
            msg = "";
        }
        if(dict.Count == 0)
        {
            return;
        }
        bool formattingMode = false;
        if(typeof(ValueType) == typeof(float) || typeof(ValueType) == typeof(double))
        {
            formattingMode = true;
        }
        foreach (KeyValuePair<GameObject, ValueType> curr in dict)
        {
            msg += $"({curr.Key.name}:{curr.Value}) | ";
        }
        Debug.Log(msg);
    }
}

