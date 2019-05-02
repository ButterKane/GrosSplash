using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    PropData propData;

    private GameObject translationGuizmos;
    private GameObject scaleGuizmos;
    private GameObject rotationGuizmos;

    private void Awake()
    {
        GenerateGuizmos();
    }
    public void GenerateGuizmos()
    {
        //Generates the translation guizmos
        translationGuizmos = new GameObject();
        translationGuizmos.transform.SetParent(this.transform,false);
        translationGuizmos.name = "TranslationGuizmos";
        for (int i = 0; i < 3; i++)
        {
            GameObject newTranslationGuizmo = Instantiate(LevelEditor.i.library.translateGuizmoPrefab, translationGuizmos.transform, false);
            newTranslationGuizmo.transform.localPosition = Vector3.zero;
            switch (i)
            {
                case 0:
                    newTranslationGuizmo.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.xGuizmoMaterial;
                    newTranslationGuizmo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case 1:
                    newTranslationGuizmo.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.yGuizmoMaterial;
                    newTranslationGuizmo.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    break;
                case 2:
                    newTranslationGuizmo.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.zGuizmoMaterial;
                    newTranslationGuizmo.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 90));
                    break;
            }
        }

        //Generates the scale guizmos
        scaleGuizmos = new GameObject();
        scaleGuizmos.transform.SetParent(this.transform, false);
        scaleGuizmos.name = "ScaleGuizmos";
        for (int i = 0; i < 3; i++)
        {
            GameObject newScaleGuizmos = Instantiate(LevelEditor.i.library.scaleGuizmoPrefab, scaleGuizmos.transform, false);
            newScaleGuizmos.transform.localPosition = Vector3.zero;
            switch (i)
            {
                case 0:
                    newScaleGuizmos.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.xGuizmoMaterial;
                    newScaleGuizmos.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case 1:
                    newScaleGuizmos.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.yGuizmoMaterial;
                    newScaleGuizmos.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    break;
                case 2:
                    newScaleGuizmos.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.zGuizmoMaterial;
                    newScaleGuizmos.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 90));
                    break;
            }
        }

        //Generates the rotation guizmos
        rotationGuizmos = new GameObject();
        rotationGuizmos.transform.SetParent(this.transform, false);
        rotationGuizmos.name = "RotationGuizmos";
        for (int i = 0; i < 3; i++)
        {
            GameObject newRotationGuizmo = Instantiate(LevelEditor.i.library.rotateGuizmoPrefab, rotationGuizmos.transform, false);
            newRotationGuizmo.transform.localPosition = Vector3.zero;
            switch (i)
            {
                case 0:
                    newRotationGuizmo.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.xGuizmoMaterial;
                    newRotationGuizmo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case 1:
                    newRotationGuizmo.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.yGuizmoMaterial;
                    newRotationGuizmo.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    break;
                case 2:
                    newRotationGuizmo.GetComponentInChildren<MeshRenderer>().material = LevelEditor.i.library.zGuizmoMaterial;
                    newRotationGuizmo.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 90));
                    break;
            }
        }

        translationGuizmos.SetActive(false);
        scaleGuizmos.SetActive(false);
        rotationGuizmos.SetActive(false);
    }
    
    public void ShowGuizmos()
    {
        DisableGuizmos();
        if (LevelEditor.i.selectedTool.toolName == "TRANSFORM")
        {
            switch (LevelEditor.i.selectedTransformTool)
            {
                case TransformEditionTool.Translate:
                    translationGuizmos.SetActive(true);
                    break;
                case TransformEditionTool.Scale:
                    scaleGuizmos.SetActive(true);
                    break;
                case TransformEditionTool.Rotate:
                    rotationGuizmos.SetActive(true);
                    break;
            }
        }
    }
    
    public void DisableGuizmos()
    {
        translationGuizmos.SetActive(false);
        scaleGuizmos.SetActive(false);
        rotationGuizmos.SetActive(false);
    }

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
            if (t.name == "Visuals")
            {
                Destroy(t);
            }
        }

        //Generates the new visuals
        if (propData.visuals != null)
        {
            GameObject visuals = Instantiate(propData.visuals, this.transform, false);
            visuals.name = "Visuals";
            visuals.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            visuals.transform.localPosition = Vector3.zero;
        }

        //Change the layer
        LevelEditor.i.globalFunctions.ChangeLayerRecursively(this.transform, "Prop");
    }
}
