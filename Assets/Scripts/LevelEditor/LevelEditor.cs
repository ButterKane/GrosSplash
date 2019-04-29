using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor i;
    
    //Auto found references
    [HideInInspector] public Library library;

    //Manually found references
    public InputField levelSizeXInput;
    public InputField levelSizeYInput;
    public InputField tileSizeInput;

    //Public values

   
    //Private values
    private Transform gridParent;

    private void Awake()
    {
        i = this;
        library = FindObjectOfType<Library>();
    }

    public void GenerateGridUsingInputfield()
    {
        Vector2 gridSize = new Vector2(int.Parse(levelSizeXInput.text), int.Parse(levelSizeYInput.text));
        float tileSize = int.Parse(tileSizeInput.text);
        GenerateGrid(gridSize, tileSize);
    }

    private void GenerateGrid(Vector2 gridSize, float tileSize)
    {
        //Check for errors
        if (gridSize == null) { Debug.LogWarning("Tried to create a grid with incorrect size"); }
        if (tileSize == null) { Debug.LogWarning("Tried to create a grid with incorrect tile size"); }

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
        float xPosition = 0;
        for (int x = 0; x < gridSize.x; x++)
        {
            float yPosition = 0;
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 newCellPosition = new Vector3(xPosition * tileSize, 0, yPosition * tileSize);
                GameObject newEmptyCell = Instantiate(emptyCellPrefab, gridParent);
                newEmptyCell.transform.position = newCellPosition;
                if (yPosition <= 0) { yPosition--; }
                yPosition = -yPosition;
            }
            xPosition = -xPosition;
            if (xPosition <= 0) { xPosition--; }
        }
    }
}
