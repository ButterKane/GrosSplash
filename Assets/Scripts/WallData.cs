using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallData", menuName = "WallData", order = 1)]
public class WallData : ScriptableObject
{
    public int ID;
    public GameObject visuals;
    public bool inflammable = false;
    public Sprite sprite;
}
