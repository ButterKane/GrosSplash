using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LosePanel : MonoBehaviour
{
    public void RestartGame()
    {
        GameManager.i.GoToMenu();
    }

    public void GoToNextScene()
    {
        GameManager.i.GoToNextScene();
    }
}
