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
                switch (gameObject.name) {
                    case "Level1":
                        break;
                    case "Level2":
                        break;
                    case "Credits":
                        break;
                }
            }
        }
        else if (!chargingSlot && lerpValue > 0)
        {
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

    void ChargeLevel()
    {

    }
}
