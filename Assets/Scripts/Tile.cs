﻿using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData tileData;
    public int rotationAmount; //Between 1 and 4, Y axis
    public Vector2Int coordinates;
    public GameObject actualFire;
    public float fireValue;



    private void Awake()
    {
        fireValue = 0;
        InvokeRepeating("UpdateFire", 0f, 1f);
        actualFire = null;
    }

    public void UpdateFireScale()
    {
        if (actualFire != null)
        {
            if (fireValue > 0)
            {
                float fireScale = fireValue / 100;
                fireScale = Mathf.Clamp(fireScale, 0, 1);
                foreach (Transform t in actualFire.transform)
                {
                    t.localScale = new Vector3(fireScale, fireScale, fireScale);
                }
            }
        }
    }
    private void UpdateFire()
    {
        if (fireValue > 0)
        {
            fireValue += tileData.fireIncrementationPerSecond;
            TrySpreadFire();
            UpdateFireScale();
        }
        else
        {
            ExtinguishFire();
        }
        if (tileData != null)
        {
            fireValue = Mathf.Clamp(fireValue, tileData.minFireValue, tileData.maxFireValue);
        }
    }

    //Generates the fire on this tile
    public void Ignite()
    {
        if (actualFire != null) { return; }
        if (!GetTileData().inflammable) { return; }
        fireValue++;
        //Generates firevisuals
        if (actualFire == null)
        {
            Debug.Log("igniting");
            GameManager.i.fireManager.fireNumber++;
            actualFire = Instantiate(GameManager.i.library.fireFX, this.transform, true);
            actualFire.transform.localPosition = Vector3.zero;
            UpdateFire();
        }
    }

    //Extinguish the fire on the actual tile
    public void ExtinguishFire()
    {
        fireValue = 0;
        //Removes the firevisuals
        if (actualFire != null)
        {
            GameManager.i.fireManager.fireNumber--;
            Destroy(actualFire.gameObject);
            GameObject extinguishFX = Instantiate(GameManager.i.library.extinguishFX, this.transform, true);
            extinguishFX.transform.localPosition = Vector3.zero;
        }
    }

    //Try to spread the fire to the near tiles
    public void TrySpreadFire()
    {
        //Try to ignite tile north
        Vector2Int coordinatesToCheck = coordinates + new Vector2Int(0, 1);
        if (GameManager.i.gridManager.CheckIfCoordinatesAreInGrid(coordinatesToCheck))
        {
            float fireIsolation = 0;
            if (GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y] != null)
            {
                fireIsolation = GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y].GetWallData().fireIsolation;
            }
            GameManager.i.fireManager.IgniteTile(coordinatesToCheck, fireValue - fireIsolation);
        }

        //Try to ignite tile south
        coordinatesToCheck = coordinates + new Vector2Int(0, -1);
        if (GameManager.i.gridManager.CheckIfCoordinatesAreInGrid(coordinatesToCheck))
        {
            float fireIsolation = 0;
            if (GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y] != null)
            {
                fireIsolation = GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y].GetWallData().fireIsolation;
            }
            GameManager.i.fireManager.IgniteTile(coordinatesToCheck, fireValue - fireIsolation);
        }

        //Try to ignite tile east
        coordinatesToCheck = coordinates + new Vector2Int(1, 0);
        if (GameManager.i.gridManager.CheckIfCoordinatesAreInGrid(coordinatesToCheck))
        {
            float fireIsolation = 0;
            if (GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y] != null)
            {
                fireIsolation = GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y].GetWallData().fireIsolation;
            }
            GameManager.i.fireManager.IgniteTile(coordinatesToCheck, fireValue - fireIsolation);
        }

        //Try to ignite tile west
        coordinatesToCheck = coordinates + new Vector2Int(-1, 0);
        if (GameManager.i.gridManager.CheckIfCoordinatesAreInGrid(coordinatesToCheck))
        {
            float fireIsolation = 0;
            if (GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y] != null)
            {
                fireIsolation = GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y].GetWallData().fireIsolation;
            }
            GameManager.i.fireManager.IgniteTile(coordinatesToCheck, fireValue - fireIsolation);
        }
    }

    public void ChangeTileData(TileData newTileData)
    {
        tileData = newTileData;
        UpdateTile();
    }

    public TileData GetTileData()
    {
        return tileData;
    }

    public void UpdateTile()
    {
        if (tileData == null) { return; }
        //Clear the visuals
        foreach (Transform t in transform)
        {
            if (t.name == "visuals")
            {
                Destroy(t.gameObject);
            }
        }
        if (tileData.visuals != null)
        {
            GameObject newVisual = Instantiate(tileData.visuals, this.transform, false);
            newVisual.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            newVisual.name = "visuals";
        }
        if (!tileData.hasCollisions)
        {
            GetComponent<BoxCollider>().enabled = false;
        } else
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        UpdateRotation();
    }

    public void Rotate()
    {
        rotationAmount++;
        UpdateRotation();
    }

    public void UpdateRotation()
    {
        foreach (Transform t in transform)
        {
            if (t.name == "visuals")
            {
                t.rotation = Quaternion.Euler(new Vector3(0, rotationAmount * 90, 0));
            }
        }
    }
}
