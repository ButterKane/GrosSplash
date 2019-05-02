using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TransformEditionTool { Translate, Rotate, Scale}
public class LevelEditor : MonoBehaviour
{
    public static LevelEditor i;

    //Auto found references
    [HideInInspector] public Library library;
    [HideInInspector] public LoaderSaverManager loaderSaverManager;
    [HideInInspector] public ToolSelector toolSelector;
    [HideInInspector] public GlobalFunctions globalFunctions;
    [HideInInspector] public GridManager gridManager;

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
    public Transform selectedGuizmo;

    public TransformEditionTool selectedTransformTool;
    public EditorTool selectedTool;

    public TileData selectedTileData;
    public WallData selectedWallData;
    public GameplayObjData selectedGameplayObjData;
    public PropData selectedPropData;
   

    private void Awake()
    {
        i = this;
        library = FindObjectOfType<Library>();
        loaderSaverManager = FindObjectOfType<LoaderSaverManager>();
        toolSelector = FindObjectOfType<ToolSelector>();
        globalFunctions = FindObjectOfType<GlobalFunctions>();
        gridManager = FindObjectOfType<GridManager>();
        selectedTransformTool = TransformEditionTool.Translate;
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
        layerMask = LayerMask.GetMask("Guizmo");
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            SelectHoveredGuizmo (hit);
        }
        else
        {
            UnselectGuizmo();
        }
        //Input actions
        if (selectedTool == null) { return; }
        //Choose the transform tool
        if (selectedTool.toolName == "TRANSFORM")
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                selectedTransformTool = TransformEditionTool.Translate;
                UpdateAllGuizmos();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                selectedTransformTool = TransformEditionTool.Scale;
                UpdateAllGuizmos();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                selectedTransformTool = TransformEditionTool.Rotate;
                UpdateAllGuizmos();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateHoveredObject();
            }
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
            }
            if (Input.GetMouseButton(1) && hoveredWall != null)
            {
                gridManager.ClearWall(hoveredWall.coordinates);
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
            gridManager.BuildWall(selectedWallData, coordinates);
        }
    }

    public void UpdateAllGuizmos()
    {
        foreach (Prop prop in gridManager.propList)
        {
            prop.ShowGuizmos();
        }
    }

    private void SelectHoveredGuizmo(RaycastHit hit)
    {
        selectedGuizmo = hit.transform;
    }

    private void UnselectGuizmo()
    {
        if (!Input.GetMouseButton(0))
        {
            selectedGuizmo = null;
        }
    }

    private void GeneratePropOverHoveredTile()
    {
        if (hoveredTile != null && selectedPropData != null)
        {
            gridManager.GenerateProp(selectedPropData, hoveredTile.transform.position, Quaternion.Euler(Vector3.zero), Vector3.one);
        }
    }

    
    private void ClearProp(Prop prop)
    {
        gridManager.propList.Remove(prop);
        Destroy(prop.gameObject);
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
            gridManager.ClearWall(coordinates);
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

    public void GenerateGridUsingInputfield()
    {
        Vector2Int gridSize = new Vector2Int(int.Parse(levelSizeXInput.text), int.Parse(levelSizeYInput.text));
        float tileSize = int.Parse(tileSizeInput.text);
        gridManager.GenerateGrid(gridSize, tileSize);
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
        PropInformations[] newPropList = new PropInformations[gridManager.propList.Count];
        int i = 0;
        foreach (Prop prop in gridManager.propList)
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
