using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBetweenPlayers : MonoBehaviour
{
    public Transform[] playersT;

    void Update()
    {
        transform.position = (playersT[0].position + playersT[1].position) / 2;
    }
}
