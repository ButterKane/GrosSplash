using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSoundsClass
{

    public string name;
    public AudioClip clip;

    public PlayerSoundsClass(string newName, AudioClip newClip)
    {
        name = newName;
        clip = newClip;
    }

}
