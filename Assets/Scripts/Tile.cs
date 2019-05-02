using UnityEngine;

public class Tile : MonoBehaviour
{
    private TileData tileData;
    public int rotationAmount; //Between 1 and 4, Y axis
    public Vector2Int coordinates;
    float fireValue;

    private void Awake()
    {
        fireValue = 0;
    }

    private void Update()
    {
        if (fireValue > 0)
        {
            fireValue += tileData.fireIncrementationPerSecond;
            TrySpreadFire();
        } else
        {
            ExtinguishFire();
        }
    }

    //Generates the fire on this tile
    public void Ignite()
    {
        fireValue++;
        //Generates firevisuals
    }

    //Extinguish the fire on the actual tile
    public void ExtinguishFire()
    {
        fireValue = 0;
        //Removes the firevisuals
    }

    //Try to spread the fire to the near tiles
    public void TrySpreadFire()
    {
        //Try to ignite tile north
        Vector2Int coordinatesToCheck = coordinates + new Vector2Int(0, 1);
        float fireIsolation = GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y].GetWallData().fireIsolation;
        GameManager.i.fireManager.IgniteTile(coordinatesToCheck, fireValue - fireIsolation);

        //Try to ignite tile south
        coordinatesToCheck = coordinates + new Vector2Int(0, -1);
        fireIsolation = GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y].GetWallData().fireIsolation;
        GameManager.i.fireManager.IgniteTile(coordinatesToCheck, fireValue - fireIsolation);

        //Try to ignite tile east
        coordinatesToCheck = coordinates + new Vector2Int(1, 0);
        fireIsolation = GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y].GetWallData().fireIsolation;
        GameManager.i.fireManager.IgniteTile(coordinatesToCheck, fireValue - fireIsolation);

        //Try to ignite tile west
        coordinatesToCheck = coordinates + new Vector2Int(-1, 0);
        fireIsolation = GameManager.i.gridManager.wallGrid[coordinatesToCheck.x, coordinatesToCheck.y].GetWallData().fireIsolation;
        GameManager.i.fireManager.IgniteTile(coordinatesToCheck, fireValue - fireIsolation);
    }

    public void ChangeTileData(TileData newTileData)
    {
        tileData = newTileData;
        UpdateTile();
    }

    public void UpdateFireValue(float incrementationValue)
    {

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
