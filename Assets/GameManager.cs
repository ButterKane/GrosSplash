using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    [HideInInspector] public GridManager gridManager;
    [HideInInspector] public FireManager fireManager;
    [HideInInspector] public Library library;
    [HideInInspector] LoaderSaverManager loadSaverManager;

    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject player1Prefab;
    public int EnemiesToWin;
    public int EnemiesKilled;
    public int fireCountToLose = 100;

    public bool gameStop;

    void Awake()
    {
        i = this;
        library = FindObjectOfType<Library>();
        gridManager = FindObjectOfType<GridManager>();
        fireManager = FindObjectOfType<FireManager>();
        loadSaverManager = FindObjectOfType<LoaderSaverManager>();
    }

    private void Update()
    {
        if(EnemiesKilled == EnemiesToWin && !gameStop)
        {
            GameWin();
            gameStop = true;
        }
        if (!gameStop && GameManager.i.fireManager.fireNumber >= fireCountToLose) // Add the condition for loss
        {
            GameLost();
            gameStop = true;
        }

    }

    public void GameWin()
    {
        Instantiate(winPanel);
    }

    public void GameLost()
    {
        Instantiate(losePanel);
    }

    public void GoToNextScene()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
