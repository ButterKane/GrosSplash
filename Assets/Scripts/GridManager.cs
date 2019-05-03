using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

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
        FindGridAndWallsValues();
    }

    public void FindGridAndWallsValues()
    {
        Vector2Int tempGridSize = new Vector2Int(0, 0);
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in allTiles)
        {
            if (tile.coordinates.x+1 > tempGridSize.x)
            {
                tempGridSize.x = tile.coordinates.x+1;
            }
            if (tile.coordinates.y+1 > tempGridSize.y)
            {
                tempGridSize.y = tile.coordinates.y+1;
            }
        }
        tileGrid = new Tile[tempGridSize.x, tempGridSize.y];
        foreach (Tile tile in allTiles)
        {
            tileGrid[tile.coordinates.x, tile.coordinates.y] = tile;
        }
        FindWallsValues(tempGridSize);
    }

    public void FindWallsValues(Vector2Int gridSize)
    {
        Wall[] allWalls = FindObjectsOfType<Wall>();
        wallGrid = new Wall[gridSize.x, gridSize.y];
        foreach (Wall wall in allWalls)
        {
            wallGrid[wall.coordinates.x, wall.coordinates.y] = wall;
        }
    }

    public void GenerateGridUsingSave(Save save)
    {
        tileGrid = null;
        wallGrid = null;
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

    public void Shuffle<T>(System.Random random, T[,] array)
    {
        int lengthRow = array.GetLength(1);

        for (int i = array.Length - 1; i > 0; i--)
        {
            int i0 = i / lengthRow;
            int i1 = i % lengthRow;

            int j = random.Next(i + 1);
            int j0 = j / lengthRow;
            int j1 = j % lengthRow;

            T temp = array[i0, i1];
            array[i0, i1] = array[j0, j1];
            array[j0, j1] = temp;
        }
    }

    public Tile GetRandomExtinguishedTile(int tileID)
    {
        System.Random rnd = new System.Random();
        Tile[,] shuffledTileGrid = (Tile[,])tileGrid.Clone();
        Shuffle(rnd, shuffledTileGrid);
        foreach (Tile tile in shuffledTileGrid)
        {
            if (tile.GetTileData().ID == tileID && tile.fireValue <= 0)
            {
                return tile;
            }
        }
        return null;
    }
}
