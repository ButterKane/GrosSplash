using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public float gamepadMoveTimer;
    float padMoveDownTimer;
    float padMoveUpTimer;
    int actualMenuOption;
    public GameObject[] backgroundButtons;
    bool controlsAble = true;
    
    void Update()
    {
        if (controlsAble)
        {
            MenuNavigation();
            SelectCategory();
        }
        UpdatePadMoveTimers();
    }

    void UpdatePadMoveTimers()
    {
        if (padMoveUpTimer > 0)
        {
            padMoveUpTimer -= Time.unscaledDeltaTime;
        }
        if (padMoveDownTimer > 0)
        {
            padMoveDownTimer -= Time.unscaledDeltaTime;
        }
    }

    void MenuNavigation()
    {
        if(Input.GetAxisRaw("LStickY") < -0.2f && padMoveDownTimer <= 0)
        {
            padMoveDownTimer = gamepadMoveTimer;
            actualMenuOption = Mathf.Clamp(actualMenuOption-1, 0, 2);
            UpdateVisualOnSelected();
        }
        else if(Input.GetAxisRaw("LStickY") > 0.2f && padMoveUpTimer <= 0)
        {
            padMoveUpTimer = gamepadMoveTimer;
            actualMenuOption = Mathf.Clamp(actualMenuOption + 1, 0, 2);
            UpdateVisualOnSelected();
        }
    }

    void SelectCategory()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            switch (actualMenuOption)
            {
                case 0:
                    print("New Game");
                    break;
                case 1:
                    print("Credits");
                    break;
                case 2:
                    print("Exit");
                    break;
            }
        }
    }

    void UpdateVisualOnSelected()
    {
        for (int i = 0; i < backgroundButtons.Length; i++)
        {
            backgroundButtons[i].SetActive(false);
        }
        backgroundButtons[actualMenuOption].SetActive(true);
    }
}
