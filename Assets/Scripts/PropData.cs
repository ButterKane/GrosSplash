using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropData", menuName = "PropData", order = 1)]
public class PropData : ScriptableObject
{
    public int ID;
    public GameObject visuals;
    public Sprite sprite;
    public bool inflammable = false;
}
