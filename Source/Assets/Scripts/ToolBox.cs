using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class ToolBox
{
    public static List<GameObject> FindChildObjects(GameObject parent, string childName)
    {
        List<GameObject> childs = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).transform.name == childName)
            {
                childs.Add(parent.transform.GetChild(i).gameObject);
            }
        }
        return childs;
    }

    public static bool CheckSerializeField(ref GameObject serializeField)
    {
        if (serializeField != null)
        {
            return true;
        }
        else
        {
            serializeField = null;
            Debug.LogError("A Field from " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + " was not initalized");
            return false;
        }
    }

    public static bool CheckSerializeField(ref int serializeField)
    {
        if (serializeField != 0)
        {
            return true;
        }
        else
        {
            serializeField = 0;
            Debug.LogError("A Field from " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + " was not initalized");
            return false;
        }
    }

    public static bool CheckSerializeField(ref float serializeField)
    {
        if (serializeField != 0f)
        {
            return true;
        }
        else
        {
            serializeField = 0f;
            Debug.LogError("A Field from " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + " was not initalized");
            return false;
        }
    }

    public static bool CheckSerializeField(ref LayerMask serializeField)
    {
        if (serializeField.value != 0)
        {
            return true;
        }
        else
        {
            serializeField.value = 0;
            Debug.LogError("A Field from " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + " was not initalized");
            return false;
        }
    }

    public static bool CheckSerializeField(ref AudioClip serializeField)
    {
        if (serializeField != null)
        {
            return true;
        }
        else
        {
            serializeField = null;
            Debug.LogError("A Field from " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + " was not initalized");
            return false;
        }
    }
}