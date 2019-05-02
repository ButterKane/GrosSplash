using UnityEngine;

public class Tile : MonoBehaviour
{
    private TileData tileData;
    public int rotationAmount; //Between 1 and 4, Y axis
    public Vector2Int coordinates;

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
