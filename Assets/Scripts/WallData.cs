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
    public int fireIsolation = 30; //How much fire is isolated (between 0 and 100, 100 means no fire can go through that wall)
}
