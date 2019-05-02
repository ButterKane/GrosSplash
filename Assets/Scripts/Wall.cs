using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public WallData wallData;
    public Vector2Int coordinates;
    public int rotationAmount; //Between 1 and 4

    public void ChangeWallData(WallData newWallData)
    {
        wallData = newWallData;
        UpdateWall();
    }

    public WallData GetWallData()
    {
        return wallData;
    }

    public void UpdateWall()
    {
        if (wallData == null) { return; }
        //Destroys the visuals
        foreach (Transform t in transform)
        {
            Destroy(t);
        }

        //Generates the new visuals
        if (wallData.visuals != null)
        {
            GameObject visuals = Instantiate(wallData.visuals, this.transform, false);
            visuals.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            visuals.transform.localPosition = Vector3.zero;
        }

        //Change the layer
        Debug.Log("Changing the layer");
        LevelEditor.i.globalFunctions.ChangeLayerRecursively(this.transform, "Wall");

        UpdateRotation();
    }

    public void Rotate()
    {
        rotationAmount++;
        UpdateRotation();
    }

    public void UpdateRotation()
    {
        foreach (Transform t in transform)
        {
            t.rotation = Quaternion.Euler(new Vector3(0, rotationAmount * 90, 0));
        }
    }
}
