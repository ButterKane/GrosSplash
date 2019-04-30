using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GlobalFunctions : MonoBehaviour
{
    public void ChangeLayerRecursively(Transform obj, string layerName)
    {
        obj.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform t in obj)
        {
            ChangeLayerRecursively(t, layerName);
        } 
    }

    public string SerializeVector3(Vector3 Vector)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Vector.x).Append(" ").Append(Vector.y).Append(" ").Append(Vector.z);
        return sb.ToString();
    }
    public Vector3 DeserializeVector3(string aData)
    {
        Vector3 result;
        string[] value = aData.Split(' ');
        if (value.Length != 3)
            throw new System.FormatException("component count mismatch. Expected 3 components but got " + value.Length);
        result = new Vector3(float.Parse(value[0]), float.Parse(value[1]), float.Parse(value[2]));
        return result;
    }
}
