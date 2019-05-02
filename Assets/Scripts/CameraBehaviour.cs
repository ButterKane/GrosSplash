using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject pointToFollow;
    public float height;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = pointToFollow.transform.position + Vector3.up * height;
        transform.LookAt(pointToFollow.transform.position);
    }
}
