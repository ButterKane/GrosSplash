using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "TileData", order = 1)]
public class TileData : ScriptableObject
{
    public int ID;
    public Sprite sprite;
    public bool inflammable = false;
    public bool canStepOnIt = true;
}
