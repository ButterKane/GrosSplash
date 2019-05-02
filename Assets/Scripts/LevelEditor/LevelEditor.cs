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
    [HideInInspector] public ToolSelector toolSelector;
    [HideInInspector] public GlobalFunctions globalFunctions;

    //Manually found references
    [Header("References")]
    public InputField levelSizeXInput;
    public InputField levelSizeYInput;
    public InputField tileSizeInput;

    //Debug values
    [Header("Debug values, don't change them")]
    public Tile hoveredTile;
    public Prop hoveredProp;
    public Wall hoveredWall;

    public EditorTool selectedTool;

    public TileData selectedTileData;
    public WallData selectedWallData;
    public GameplayObjData selectedGameplayObjData;
    public PropData selectedPropData;

    public Tile[,] tileGrid;
    public Wall[,] wallGrid;
    public List<Prop> propList = new List<Prop>();
   
    //Private values
    private Transform gridParent;

    private void Awake()
    {
        i = this;
        library = FindObjectOfType<Library>();
        loaderSaverManager = FindObjectOfType<LoaderSaverManager>();
        toolSelector = FindObjectOfType<ToolSelector>();
        globalFunctions = FindObjectOfType<GlobalFunctions>();
    }

    private void Update()
    {
        //Raycast actions
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Tile");
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            SelectHoveredTile(hit);
        } else
        {
            UnselectTile();
        }
        layerMask = LayerMask.GetMask("Prop");
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            SelectHoveredProp(hit);
        }
        else
        {
            UnselectProp();
        }
        layerMask = LayerMask.GetMask("Wall");
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            SelectHoveredWall(hit);
        }
        else
        {
            UnselectWall();
        }
        //Input actions
        if (selectedTool == null) { return; }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateHoveredObject();
        }
        if (Cursor.lockState != CursorLockMode.Locked) { return; }
        if (selectedTool.toolName == "TILE")
        {
            if (Input.GetMouseButton(0))
            {
                ChangeHoveredTileType();
            }
            if (Input.GetMouseButton(1))
            {
                ClearHoveredTile();
            }
        }
        if (selectedTool.toolName == "WALL")
        {
            if (Input.GetMouseButton(0))
            {
                BuildOverHoveredTile();
                Debug.Log("Building over tile");
            }
            if (Input.GetMouseButton(1) && hoveredWall != null)
            {
                ClearWall(hoveredWall.coordinates);
            }
        }
        if (selectedTool.toolName == "PROP")
        {
            if (Input.GetMouseButtonDown(0))
            {
                GeneratePropOverHoveredTile();
            }
            if (Input.GetMouseButton(1))
            {
                ClearProp(hoveredProp);
            }
        }
    }

    private void RotateHoveredObject()
    {
        switch (selectedTool.toolName)
        {
            case "TILE":
                hoveredTile.Rotate();
                break;
            case "WALL":
                hoveredWall.Rotate();
                break;
        }
    }

    private void ChangeHoveredTileType()
    {
        if (hoveredTile != null && selectedTileData != null)
        {
            hoveredTile.ChangeTileData(selectedTileData);
        }
    }

    private void BuildOverHoveredTile()
    {
        if (hoveredTile != null && selectedWallData != null)
        {
            Vector2Int coordinates = hoveredTile.coordinates;
            BuildWall(selectedWallData, coordinates);
        }
    }

    private void GeneratePropOverHoveredTile()
    {
        if (hoveredTile != null && selectedPropData != null)
        {
            GenerateProp(selectedPropData, hoveredTile.transform.position, Quaternion.Euler(Vector3.zero), Vector3.one);
        }
    }

    private void GenerateProp(PropData propData, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject newProp = new GameObject();
        Prop newPropComponent = newProp.AddComponent<Prop>();
        newPropComponent.ChangePropData(propData);
        newProp.transform.position = position;
        newProp.transform.rotation = rotation;
        newProp.transform.localScale = scale;
        propList.Add(newPropComponent);
    }

    private void BuildWall(WallData wallData, Vector2Int coordinates)
    {
        if (wallGrid[coordinates.x, coordinates.y] != null) { return; } //There is already a wall
        GameObject newWall = new GameObject();
        newWall.transform.SetParent(tileGrid[coordinates.x, coordinates.y].transform, false);
        Wall newWallComponent = newWall.AddComponent<Wall>();
        newWallComponent.ChangeWallData(wallData);
        newWallComponent.coordinates = coordinates;
        wallGrid[coordinates.x, coordinates.y] = newWallComponent;
    }
    
    private void ClearProp(Prop prop)
    {
        propList.Remove(prop);
        Destroy(prop.gameObject);
    }

    private void ClearWall(Vector2Int coordinates)
    {
        if (wallGrid[coordinates.x, coordinates.y] != null)
        {
            Destroy(wallGrid[coordinates.x, coordinates.y].gameObject);
            wallGrid[coordinates.x, coordinates.y] = null;
        }
    }

    private void ClearHoveredTile()
    {
        if (hoveredTile != null)
        {
            hoveredTile.ChangeTileData(library.defaultTile);
        }
    }

    private void ClearWallOverHoveredTile()
    {
        if (hoveredTile != null)
        {
            Vector2Int coordinates = hoveredTile.coordinates;
            ClearWall(coordinates);
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

    private void UnselectTile()
    {
        hoveredTile = null;
    }

    private void SelectHoveredProp(RaycastHit hit)
    {
        Prop potentialHoveredProp = hit.transform.GetComponentInParent<Prop>();
        if (potentialHoveredProp != null)
        {
            hoveredProp = potentialHoveredProp;
        }
    }

    private void UnselectProp()
    {
        hoveredProp = null;
    }

    private void SelectHoveredWall(RaycastHit hit)
    {
        Wall potentialHoveredWall = hit.transform.GetComponentInParent<Wall>();
        if (potentialHoveredWall != null)
        {
            hoveredWall = potentialHoveredWall;
        }
    }

    private void UnselectWall()
    {
        hoveredWall = null;
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
            PropData propData = LevelEditor.i.library.GetPropDataFromID(propInformations.propID);
            Vector3 propPosition = globalFunctions.DeserializeVector3(propInformations.propPosition);
            Vector3 propScale = globalFunctions.DeserializeVector3(propInformations.propScale);
            GenerateProp(propData, propPosition, propInformations.propRotation, propScale);
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
                tileGrid[x, y] = generatedTileScript;
                yPosition++;
            }
            xPosition++;
        }
    }

    public int[][] GetTileRotation(Tile[,] tileGrid)
    {
        int[][] newGrid = new int[tileGrid.GetLength(0)][];
        for (int i = 0; i < tileGrid.GetLength(0); i++)
        {
            newGrid[i] = new int[tileGrid.GetLength(1)];
        }
        for (int x = 0; x < tileGrid.GetLength(0); x++)
        {
            for (int y = 0; y < tileGrid.GetLength(1); y++)
            {
                newGrid[x][y] = tileGrid[x, y].rotationAmount;
            }
        }
        return newGrid;
    }

    public int[][] GetWallRotation(Wall[,] wallGrid)
    {
        int[][] newGrid = new int[wallGrid.GetLength(0)][];
        for (int i = 0; i < wallGrid.GetLength(0); i++)
        {
            newGrid[i] = new int[wallGrid.GetLength(1)];
        }
        for (int x = 0; x < wallGrid.GetLength(0); x++)
        {
            for (int y = 0; y < wallGrid.GetLength(1); y++)
            {
                if (wallGrid[x, y] != null)
                {
                    newGrid[x][y] = wallGrid[x, y].rotationAmount;
                }
            }
        }
        return newGrid;
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

    public int[][] ConvertWallGridToID(Wall[,] wallGrid)
    {
        int[][] newGrid = new int[wallGrid.GetLength(0)][];
        for (int i = 0; i < wallGrid.GetLength(0); i++)
        {
            newGrid[i] = new int[wallGrid.GetLength(1)];
        }
        for (int x = 0; x < wallGrid.GetLength(0); x++)
        {
            for (int y = 0; y < wallGrid.GetLength(1); y++)
            {
                if (wallGrid[x, y] != null)
                {
                    newGrid[x][y] = wallGrid[x, y].GetWallData().ID;
                }
            }
        }
        return newGrid;
    }

    public PropInformations[] GetPropList()
    {
        PropInformations[] newPropList = new PropInformations[propList.Count];
        int i = 0;
        foreach (Prop prop in propList)
        {
            PropInformations newPropInformation = new PropInformations();
            newPropInformation.propID = prop.GetPropData().ID;
            newPropInformation.propPosition = globalFunctions.SerializeVector3(prop.transform.position);
            newPropInformation.propRotation = prop.transform.rotation;
            newPropInformation.propScale = globalFunctions.SerializeVector3(prop.transform.localScale);
            newPropList[i] = newPropInformation;
            i++;
        }
        return newPropList;
    }
}
