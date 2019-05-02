using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    [HideInInspector] GridManager gridManager;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    public void IgniteTile(Vector2Int coordinates, float fireStrength)
    {
        if (fireStrength > gridManager.tileGrid[coordinates.x, coordinates.y].GetTileData().fireStrengthToBeSetOnFire)
        {
            gridManager.tileGrid[coordinates.x, coordinates.y].Ignite();
        }
    }
}
