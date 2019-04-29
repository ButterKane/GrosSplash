using UnityEngine;

public class Tile : MonoBehaviour
{
    private TileData tileData;

    public void ChangeTileData(TileData newTileData)
    {
        tileData = newTileData;
        UpdateTile();
    }

    public void UpdateTile()
    {
        if (tileData == null)
        {
            GetComponent<SpriteRenderer>().sprite = LevelEditor.i.library.emptyTileTexture;
        } else {
            GetComponent<SpriteRenderer>().sprite = tileData.sprite;
        }
    }
}
