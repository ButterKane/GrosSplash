﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
