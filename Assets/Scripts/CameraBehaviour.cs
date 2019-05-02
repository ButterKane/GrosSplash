using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject[] players;

    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] playerPositions = new Vector3[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerPositions[i] = players[i].transform.position;
        }

        Vector3 middlePos = ((playerPositions[1] - playerPositions[0])/ 2) + playerPositions[0];

        float howFarApart = (playerPositions[1] - playerPositions[0]).magnitude;

        transform.position = middlePos + Vector3.up * howFarApart;
        transform.LookAt(middlePos);
    }
}
