using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "Tile", order = 1)]
public class TileData : ScriptableObject
{
    public Texture texture;
    public bool inflammable = false;
    public bool canStepOnIt = true;
}
