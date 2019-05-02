using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlotPlayers : MonoBehaviour
{
    public PlayerMenu[] players;
    public Renderer myRend;
    int playersOnSlot;

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

    void ActualizeColor()
    {
        switch (playersOnSlot)
        {
            case 0:
                myRend.material.color = Color.red;
                break;
            case 1:
                myRend.material.color = Color.yellow;
                break;
            case 2:
                myRend.material.color = Color.green;
                break;
        }
    }
}
