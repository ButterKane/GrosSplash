using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    PropData propData;

    public PropData GetPropData()
    {
        return propData;
    }

    public void ChangePropData(PropData newPropData)
    {
        propData = newPropData;
        UpdateProp();
    }

    public void UpdateProp()
    {
        if (propData == null) { return; }
        //Destroys the visuals
        foreach (Transform t in transform)
        {
            Destroy(t);
        }

        //Generates the new visuals
        if (propData.visuals != null)
        {
            GameObject visuals = Instantiate(propData.visuals, this.transform, false);
            visuals.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            visuals.transform.localPosition = Vector3.zero;
        }

        //Change the layer
        LevelEditor.i.globalFunctions.ChangeLayerRecursively(this.transform, "Prop");
    }
}
