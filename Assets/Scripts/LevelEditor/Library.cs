using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Library : MonoBehaviour
{
    public GameObject emptyGridCellPrefab;
    public GameObject panelObjPrefab;
    public TileData defaultTile;



    public List<TileData> tileList;
    public List<WallData> wallList;
    public List<GameplayObjData> gameplayObjList;
    public List<PropData> propList;


    private void Awake()
    {
        GenerateTileListPanel();
        GenerateWallListPanel();
        GenerateGameplayObjListPanel();
        GeneratePropListPanel();
    }

    public TileData GetTileDataFromID(int ID)
    {
        foreach (TileData tile in tileList)
        {
            if (tile.ID == ID)
            {
                return tile;
            }
        }
        return null;
    }

    public WallData GetWallDataFromID(int ID)
    {
        foreach (WallData wall in wallList)
        {
            if (wall.ID == ID)
            {
                return wall;
            }
        }
        return null;
    }

    public PropData GetPropDataFromID(int ID)
    {
        foreach (PropData prop in propList)
        {
            if (prop.ID == ID)
            {
                return prop;
            }
        }
        return null;
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            ScrollPanelUp(LevelEditor.i.selectedTool.objectList.transform);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            ScrollPanelDown(LevelEditor.i.selectedTool.objectList.transform);
        }
    }

    public void GenerateTileListPanel()
    {
        Transform panelParent = LevelEditor.i.toolSelector.GetToolByName("TILE").objectList;
        for (int i = 0; i < tileList.Count; i++)
        {
            GameObject newObj = Instantiate(panelObjPrefab, panelParent);
            newObj.transform.Find("Sprite").GetComponent<Image>().sprite = tileList[i].sprite;
            newObj.name = "Tile " +i;
            TilePanel newObjScript = newObj.AddComponent<TilePanel>();
            newObjScript.linkedData = tileList[i];
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelParent.GetComponent<RectTransform>());
        UpdateSelectedObject();
    }

    public void GenerateWallListPanel()
    {
        Transform panelParent = LevelEditor.i.toolSelector.GetToolByName("WALL").objectList;
        for (int i = 0; i < wallList.Count; i++)
        {
            GameObject newObj = Instantiate(panelObjPrefab, panelParent);
            newObj.transform.Find("Sprite").GetComponent<Image>().sprite = wallList[i].sprite;
            newObj.name = "Wall " + i;
            WallPanel newObjScript = newObj.AddComponent<WallPanel>();
            newObjScript.linkedData = wallList[i];
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelParent.GetComponent<RectTransform>());
        UpdateSelectedObject();
    }

    public void GenerateGameplayObjListPanel()
    {
        Transform panelParent = LevelEditor.i.toolSelector.GetToolByName("GAMEPLAY").objectList;
        for (int i = 0; i < gameplayObjList.Count; i++)
        {
            GameObject newObj = Instantiate(panelObjPrefab, panelParent);
            newObj.transform.Find("Sprite").GetComponent<Image>().sprite = gameplayObjList[i].sprite;
            newObj.name = "Gameplay Object " + i;
            GameplayObjPanel newObjScript = newObj.AddComponent<GameplayObjPanel>();
            newObjScript.linkedData = gameplayObjList[i];
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelParent.GetComponent<RectTransform>());
        UpdateSelectedObject();
    }

    public void GeneratePropListPanel()
    {
        Transform panelParent = LevelEditor.i.toolSelector.GetToolByName("PROP").objectList;
        for (int i = 0; i < propList.Count; i++)
        {
            GameObject newObj = Instantiate(panelObjPrefab, panelParent);
            newObj.transform.Find("Sprite").GetComponent<Image>().sprite = propList[i].sprite;
            newObj.name = "Prop " + i;
            PropPanel newObjScript = newObj.AddComponent<PropPanel>();
            newObjScript.linkedData = propList[i];
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelParent.GetComponent<RectTransform>());
        UpdateSelectedObject();
    }

    public void UpdateSelectedObject()
    {
        if (LevelEditor.i.selectedTool == null) { return; }
        switch (LevelEditor.i.selectedTool.toolName)
        {
            case "TILE":
                LevelEditor.i.selectedTileData = LevelEditor.i.selectedTool.objectList.transform.GetChild(2).GetComponent<TilePanel>().linkedData;
                break;
            case "WALL":
                LevelEditor.i.selectedWallData = LevelEditor.i.selectedTool.objectList.transform.GetChild(2).GetComponent<WallPanel>().linkedData;
                break;
            case "GAMEPLAY":
                LevelEditor.i.selectedGameplayObjData = LevelEditor.i.selectedTool.objectList.transform.GetChild(2).GetComponent<GameplayObjPanel>().linkedData;
                break;
            case "PROP":
                LevelEditor.i.selectedPropData = LevelEditor.i.selectedTool.objectList.transform.GetChild(2).GetComponent<PropPanel>().linkedData;
                break;
        }
    }

    public void ScrollPanelUp(Transform panel)
    {
        panel.GetChild(0).SetAsLastSibling();
        UpdateSelectedObject();
    }

    public void ScrollPanelDown(Transform panel)
    {
        panel.GetChild(panel.childCount-1).SetAsFirstSibling();
        UpdateSelectedObject();
    }
}
