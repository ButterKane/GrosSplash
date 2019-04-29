using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Library : MonoBehaviour
{
    public GameObject emptyGridCellPrefab;
    public GameObject tilePanelPrefab;
    public Transform tileSelectionPanelParent;
    public Sprite emptyTileTexture;

    private List<Transform> tilesPanel = new List<Transform>();



    public List<TileData> tileList;


    private void Awake()
    {
        GenerateTileListPanel();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            ScrollTilePanelUp();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            ScrollTilePanelDown();
        }
    }

    public void GenerateTileListPanel()
    {
        tilesPanel.Clear();
        for (int i = 0; i < tileList.Count; i++)
        {
            GameObject newTile = Instantiate(tilePanelPrefab, tileSelectionPanelParent);
            newTile.transform.Find("TileSprite").GetComponent<Image>().sprite = tileList[i].sprite;
            newTile.GetComponent<TilePanel>().linkedData = tileList[i];
            tilesPanel.Add(newTile.transform);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(tileSelectionPanelParent.GetComponent<RectTransform>());
        UpdateSelectedTileType();
    }

    public void UpdateSelectedTileType()
    {
        if (tilesPanel == null) { return; }
        if (tilesPanel.Count > 3)
        {
            LevelEditor.i.selectedTileData = tilesPanel[2].GetComponent<TilePanel>().linkedData;
        } else
        {
            LevelEditor.i.selectedTileData = tilesPanel[0].GetComponent<TilePanel>().linkedData;
        }
    }

    public void ScrollTilePanelUp()
    {
        Transform[] tempTilesPanel = new Transform[tilesPanel.Count];
        for (int i = tilesPanel.Count-1; i >= 0; i--)
        {
            int newTileIndex = i - 1;
            if (newTileIndex < 0)
            {
                newTileIndex = tilesPanel.Count-1;
            }
            tempTilesPanel[newTileIndex] = tilesPanel[i];
            tilesPanel[i].SetSiblingIndex(newTileIndex);
        }
        tilesPanel = tempTilesPanel.ToList();
        UpdateSelectedTileType();
    }

    public void ScrollTilePanelDown()
    {
        Transform[] tempTilesPanel = new Transform[tilesPanel.Count];
        for (int i = 0; i < tilesPanel.Count; i++)
        {
            int newTileIndex = i + 1;
            if (newTileIndex >= tilesPanel.Count)
            {
                newTileIndex = 0;
            }
            tempTilesPanel[newTileIndex] = tilesPanel[i];
            tilesPanel[i].SetSiblingIndex(newTileIndex);
        }
        tilesPanel = tempTilesPanel.ToList();
        UpdateSelectedTileType();
    }
}
