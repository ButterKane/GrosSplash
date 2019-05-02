using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;
using System.Xml.Serialization;
using UnityEngine.UI;

[Serializable]
public struct Save
{
    public float tileSize;
    public int[][] tileGrid;
    public int[][] wallGrid;
    public int[][] tileRotation;
    public int[][] wallRotation;
    public PropInformations[] propList;
}

public struct PropInformations
{
    public int propID;
    public string propPosition;
    public Quaternion propRotation;
    public string propScale;
}

public class LoaderSaverManager : MonoBehaviour // the Class
{
    public InputField saveNameInputField;
    public InputField loadNameInputField;

    [HideInInspector] GridManager gridManager;
    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    string path = Application.streamingAssetsPath + "/Levels/";

    public void SaveLevel()
    {
        string fileName = path + saveNameInputField.text + ".xml";
        var save = new Save()
        {
            tileSize = int.Parse(LevelEditor.i.tileSizeInput.text),
            tileGrid = LevelEditor.i.ConvertTileGridToID(gridManager.tileGrid),
            wallGrid = LevelEditor.i.ConvertWallGridToID(gridManager.wallGrid),
            tileRotation = LevelEditor.i.GetTileRotation(gridManager.tileGrid),
            wallRotation = LevelEditor.i.GetWallRotation(gridManager.wallGrid),
            propList = LevelEditor.i.GetPropList()
        };

        XmlSerializer serializer = new XmlSerializer(typeof(Save));

        using (FileStream stream = new FileStream(fileName, FileMode.Create))
        {
            serializer.Serialize(stream, save);
        }
    }

    public void LoadLevel()
    {
        gridManager.GenerateGridUsingSave(GetLevel());
    }

    public void LoadLevel(string levelName)
    {
        gridManager.GenerateGridUsingSave(GetLevelFromString(levelName));
    }

    public Save GetLevelFromString(string levelName)
    {
        string fileName = path + levelName + ".xml";
        XmlSerializer serializer = new XmlSerializer(typeof(Save));

        using (FileStream stream = new FileStream(fileName, FileMode.Open))
        {
            Save loadedSave = (Save)serializer.Deserialize(stream);
            return loadedSave;
        }
    }

    public Save GetLevel()
    {
        string fileName = path + loadNameInputField.text + ".xml";
        XmlSerializer serializer = new XmlSerializer(typeof(Save));

        using (FileStream stream = new FileStream(fileName, FileMode.Open))
        {
            Save loadedSave = (Save)serializer.Deserialize(stream);
            return loadedSave;
        }
    }
}
