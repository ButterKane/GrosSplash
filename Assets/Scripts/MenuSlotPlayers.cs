using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlotPlayers : MonoBehaviour
{
    public PlayerMenu[] players;
    public Renderer myRend;
    public Renderer myQuadRend;
    public float timeToCharge;
    public float timeToDecharge;
    public Animator canvasAnim;
    int playersOnSlot;
    bool chargingSlot;
    [HideInInspector]
    public float lerpValue;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMenu>() != null)
        {
            playersOnSlot++;
            ActualizeColor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMenu>() != null)
        {
            playersOnSlot--;
            ActualizeColor();
        }
    }

    private void Update()
    {
        ChargingUpdate();
    }

    void ChargingUpdate()
    {
        if (chargingSlot && lerpValue < 1)
        {
            lerpValue = Mathf.Clamp01(lerpValue + (Time.deltaTime / timeToCharge));
            myQuadRend.material.SetFloat("_LerpValue", lerpValue);
            if (lerpValue >= 1)
            {
                DoingSlotEffect();
            }
        }
        else if (!chargingSlot && lerpValue > 0)
        {
            if (lerpValue >= 1)
            {
                UndoSlotEffect();
            }
            lerpValue = Mathf.Clamp01(lerpValue - (Time.deltaTime / timeToDecharge));
            myQuadRend.material.SetFloat("_LerpValue", lerpValue);
        }
    }

    void ActualizeColor()
    {
        switch (playersOnSlot)
        {
            case 0:
                myRend.material.color = Color.red;
                break;
            case 1:
                myRend.material.color = Color.yellow;
                chargingSlot = false;
                break;
            case 2:
                myRend.material.color = Color.green;
                chargingSlot = true;
                break;
        }
    }

    void DoingSlotEffect()
    {
        switch (gameObject.name)
        {
            case "Level1":
                print("load level 1");
                break;
            case "Level2":
                print("load level 2");
                break;
            case "Credits":
                print("Credits");
                canvasAnim.SetBool("In", true);
                break;
            case "Exit":
                print("Exit");
                Application.Quit();
                break;
        }
    }
    
    void UndoSlotEffect()
    {
        if(gameObject.name == "Credits")
        {
            canvasAnim.SetBool("In", false);
        }
    }
}