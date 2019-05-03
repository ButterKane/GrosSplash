using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Tile[,] tileGrid;
    public Wall[,] wallGrid;
    public List<Prop> propList = new List<Prop>();
    private Transform gridParent;

    [HideInInspector] public Library library;
    [HideInInspector] public GlobalFunctions globalFunctions;

    private void Awake()
    {
        library = FindObjectOfType<Library>();
        globalFunctions = FindObjectOfType<GlobalFunctions>();
    }

    public void GenerateGridUsingSave(Save save)
    {
        propList.Clear();
        Vector2Int gridSize = new Vector2Int(save.tileGrid[0].Length, save.tileGrid[1].Length);
        GenerateGrid(gridSize, save.tileSize);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                //Generates the tiles
                tileGrid[x, y].ChangeTileData(library.GetTileDataFromID(save.tileGrid[x][y]));
                //Apply rotation to the tile
                tileGrid[x, y].rotationAmount = save.tileRotation[x][y];
                tileGrid[x, y].UpdateRotation();
                if (save.wallGrid[x][y] > 0)
                {
                    //Generates the walls
                    BuildWall(library.GetWallDataFromID(save.wallGrid[x][y]), new Vector2Int(x, y));
                    //Apply the rotation to the walls
                    wallGrid[x, y].rotationAmount = save.wallRotation[x][y];
                    wallGrid[x, y].UpdateRotation();
                }
            }
        }
        //Generates the props
        for (int i = 0; i < save.propList.Length; i++)
        {
            PropInformations propInformations = save.propList[i];
            PropData propData = library.GetPropDataFromID(propInformations.propID);
            Vector3 propPosition = globalFunctions.DeserializeVector3(propInformations.propPosition);
            Vector3 propScale = globalFunctions.DeserializeVector3(propInformations.propScale);
            GenerateProp(propData, propPosition, propInformations.propRotation, propScale);
        }
    }

    public bool CheckIfCoordinatesAreInGrid(Vector2Int coordinates)
    {
        if (coordinates.x < 0 || coordinates.y < 0 || coordinates.x >= tileGrid.GetLength(0) || coordinates.y >= tileGrid.GetLength(1))
        {
            return false;
        }
        return true;
    }

    public void GenerateGrid(Vector2Int gridSize, float tileSize)
    {
        foreach (Prop prop in propList)
        {
            Destroy(prop.gameObject);
        }
        propList.Clear();
        tileGrid = new Tile[gridSize.x, gridSize.y];
        wallGrid = new Wall[gridSize.x, gridSize.y];
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
                newEmptyCell.name = "Cell[" + x + "," + y + "]";
                tileGrid[x, y] = generatedTileScript;
                yPosition++;
            }
            xPosition++;
        }
    }

    public void GenerateProp(PropData propData, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject newProp = new GameObject();
        Prop newPropComponent = newProp.AddComponent<Prop>();
        newPropComponent.ChangePropData(propData);
        newProp.transform.position = position;
        newProp.transform.rotation = rotation;
        newProp.transform.localScale = scale;
        propList.Add(newPropComponent);
    }

    public void BuildWall(WallData wallData, Vector2Int coordinates)
    {
        if (wallGrid[coordinates.x, coordinates.y] != null) { return; } //There is already a wall
        GameObject newWall = new GameObject();
        newWall.transform.SetParent(tileGrid[coordinates.x, coordinates.y].transform, false);
        Wall newWallComponent = newWall.AddComponent<Wall>();
        newWallComponent.ChangeWallData(wallData);
        newWallComponent.coordinates = coordinates;
        wallGrid[coordinates.x, coordinates.y] = newWallComponent;
    }

    public void ClearWall(Vector2Int coordinates)
    {
        if (wallGrid[coordinates.x, coordinates.y] != null)
        {
            Destroy(wallGrid[coordinates.x, coordinates.y].gameObject);
            wallGrid[coordinates.x, coordinates.y] = null;
        }
    }
}
