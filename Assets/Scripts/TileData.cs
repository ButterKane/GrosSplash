using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "TileData", order = 1)]
public class TileData : ScriptableObject
{
    public int ID;
    public Sprite sprite;
    public GameObject visuals;
    public bool inflammable = false;
    public bool hasCollisions = true;
    public int fireStrengthToBeSetOnFire = 50;
    public int maxFireValue = 100;
    public int minFireValue = -20;
    public float fireIncrementationPerSecond = 1;
}
