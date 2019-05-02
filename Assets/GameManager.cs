using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    [HideInInspector] public GridManager gridManager;
    [HideInInspector] public FireManager fireManager;
    [HideInInspector] LoaderSaverManager loadSaverManager;

    public GameObject player1Prefab;

    void Awake()
    {
        i = this;
        gridManager = FindObjectOfType<GridManager>();
        fireManager = FindObjectOfType<FireManager>();
        loadSaverManager = FindObjectOfType<LoaderSaverManager>();
    }

    public void PlayLevel(string levelName)
    {
        loadSaverManager.LoadLevel(levelName);
        ConvertLevelToPlayable();
    }

    public void PlayLevel()
    {
        loadSaverManager.LoadLevel();
        ConvertLevelToPlayable();
    }


    //Convert a level from the editor to a playable one
    public void ConvertLevelToPlayable()
    {
        //Removes the air tiles
        for (int x = 0; x < gridManager.tileGrid.GetLength(0); x++)
        {
            for (int y = 0; y < gridManager.tileGrid.GetLength(1); y++)
            {
                if (gridManager.tileGrid[x,y].GetTileData().ID == 0)
                {
                    Destroy(gridManager.tileGrid[x, y].gameObject);
                }
            }
        }

        //Spawns the players
        Instantiate(player1Prefab);

        //Creates the camera

    }
}
