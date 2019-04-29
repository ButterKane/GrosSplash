using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor i;
    
    //Auto found references
    [HideInInspector] public Library library;
    [HideInInspector] public LoaderSaverManager loaderSaverManager;

    //Manually found references
    [Header("References")]
    public InputField levelSizeXInput;
    public InputField levelSizeYInput;
    public InputField tileSizeInput;

    //Debug values
    [Header("Debug values, don't change them")]
    public Tile hoveredTile;
    public TileData selectedTileData;

    public Tile[,] tileGrid;
   
    //Private values
    private Transform gridParent;

    private void Awake()
    {
        i = this;
        library = FindObjectOfType<Library>();
    }

    private void Update()
    {
        //Raycast actions
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            SelectHoveredTile(hit);
        } else
        {
            UnselectedTile();
        }
        //Input actions
        if (Input.GetMouseButtonDown(0))
        {
            ChangeHoveredTileType();
        }
        if (Input.GetMouseButton(1))
        {
            ClearHoveredTile();
        }
    }

    private void ChangeHoveredTileType()
    {
        if (hoveredTile != null && selectedTileData != null)
        {
            hoveredTile.ChangeTileData(selectedTileData);
        }
    }

    private void ClearHoveredTile()
    {
        if (hoveredTile != null)
        {
            hoveredTile.ChangeTileData(library.defaultTile);
        }
    }

    private void SelectHoveredTile(RaycastHit hit)
    {
        Tile potentialHoveredTile = hit.transform.GetComponent<Tile>();
        if (potentialHoveredTile != null)
        {
            hoveredTile = potentialHoveredTile;
        }
    }

    private void UnselectedTile()
    {
        hoveredTile = null;
    }

    public void GenerateGridUsingSave(Save save)
    {
        Vector2Int gridSize = new Vector2Int(save.tileGrid[0].Length, save.tileGrid[1].Length);
        GenerateGrid(gridSize, save.tileSize);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                tileGrid[x, y].ChangeTileData(library.GetTileDataFromID(save.tileGrid[x][y]));
            }
        }
    }

    public void GenerateGridUsingInputfield()
    {
        Vector2Int gridSize = new Vector2Int(int.Parse(levelSizeXInput.text), int.Parse(levelSizeYInput.text));
        float tileSize = int.Parse(tileSizeInput.text);
        GenerateGrid(gridSize, tileSize);
    }

    private void GenerateGrid(Vector2Int gridSize, float tileSize)
    {
        tileGrid = new Tile[gridSize.x, gridSize.y];
        //Check for errors
        if (gridSize == null) { Debug.LogWarning("Tried to create a grid with incorrect size"); }
        if (tileSize <= 0) { Debug.LogWarning("Tried to create a grid with incorrect tile size"); }

        //Generate a gameobject to be a parent of the tiles, clear the grid if it's already existing
        if (gridParent != null)
        {
            Destroy(gridParent.gameObject);
        }
        GameObject gridParentObj = new GameObject();
        gridParentObj.name = "Grid parent";
        gridParent = gridParentObj.transform;

        //Generates the tiles
        GameObject emptyCellPrefab = library.emptyGridCellPrefab;
        int xPosition = Mathf.RoundToInt(-gridSize.x / 2);
        for (int x = 0; x < gridSize.x; x++)
        {
            int yPosition = Mathf.RoundToInt(-gridSize.y / 2);
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 newCellPosition = new Vector3(xPosition * tileSize, 0, yPosition * tileSize);
                GameObject newEmptyCell = Instantiate(emptyCellPrefab, gridParent);
                Tile generatedTileScript = newEmptyCell.GetComponent<Tile>();
                generatedTileScript.coordinates = new Vector2Int(x, y);
                generatedTileScript.ChangeTileData(library.defaultTile);
                newEmptyCell.transform.position = newCellPosition;
                tileGrid[x, y] = generatedTileScript;
                yPosition++;
            }
            xPosition++;
        }
    }

    public int[][] ConvertTileGridToID(Tile[,] tileGrid)
    {
        int[][] newGrid = new int[tileGrid.GetLength(0)][];
        for (int i = 0; i < tileGrid.GetLength(0) ; i++) {
            newGrid[i] = new int[tileGrid.GetLength(1)];
        }
        for (int x = 0; x < tileGrid.GetLength(0); x++)
        {
            for (int y = 0; y < tileGrid.GetLength(1); y++)
            {
                newGrid[x][y] = tileGrid[x, y].GetTileData().ID;
            }
        }
        return newGrid;
    }
}
