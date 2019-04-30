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
}

public class LoaderSaverManager : MonoBehaviour // the Class
{
    public InputField saveNameInputField;
    public InputField loadNameInputField;

    string path = Application.streamingAssetsPath + "/Levels/";

    public void SaveLevel()
    {
        string fileName = path + saveNameInputField.text + ".xml";
        var save = new Save()
        {
            tileSize = int.Parse(LevelEditor.i.tileSizeInput.text),
            tileGrid = LevelEditor.i.ConvertTileGridToID(LevelEditor.i.tileGrid)
        };

        XmlSerializer serializer = new XmlSerializer(typeof(Save));

        using (FileStream stream = new FileStream(fileName, FileMode.Create))
        {
            serializer.Serialize(stream, save);
        }
    }

    public void LoadLevel()
    {
        LevelEditor.i.GenerateGridUsingSave(GetLevel());
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
