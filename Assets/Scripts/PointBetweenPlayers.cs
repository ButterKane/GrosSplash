using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBetweenPlayers : MonoBehaviour
{
    public Transform[] playersT;

    private void Start()
    {
        GameObject[] tempPlayers = GameObject.FindGameObjectsWithTag("Player");
        print(tempPlayers[0]);
        print(tempPlayers[1]);

        playersT = new Transform[tempPlayers.Length];
        for (int i = 0; i < tempPlayers.Length; i++)
        {
            playersT[i] = tempPlayers[i].transform;
        }
    }

    void Update()
    {
        transform.position = (playersT[0].position + playersT[1].position) / 2;
    }
}