using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayObjectData", menuName = "GameplayObjectData", order = 1)]
public class GameplayObjData : ScriptableObject
{
    public int ID;
    public Sprite sprite;
    public GameObject visuals;
}
