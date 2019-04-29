using UnityEngine;

public class Tile : MonoBehaviour
{
    private TileData tileData;
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
        GetComponent<SpriteRenderer>().sprite = tileData.sprite;
    }
}
