using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelector : MonoBehaviour
{
    public List<EditorTool> toolList;
    public GameObject toolHighlighter;

    public void SelectToolByID(int toolID)
    {
        foreach (EditorTool tool in toolList)
        {
            tool.linkedPanel.gameObject.SetActive(false);
        }
        LevelEditor.i.selectedTool = toolList[toolID];
        toolList[toolID].linkedPanel.gameObject.SetActive(true);
        toolHighlighter.transform.position = toolList[toolID].transform.position;
        LevelEditor.i.library.UpdateSelectedObject();
    }

    public void SelectToolByName(string toolName)
    {
        bool toolFound = false;
        foreach (EditorTool tool in toolList)
        {
            tool.linkedPanel.gameObject.SetActive(false);
            if (tool.toolName == toolName)
            {
                LevelEditor.i.selectedTool = tool;
                toolHighlighter.transform.position = tool.transform.position;
                LevelEditor.i.library.UpdateSelectedObject();
                toolFound = true;
                tool.linkedPanel.gameObject.SetActive(true);
            }
        }
        if (!toolFound)
        {
            Debug.LogWarning("Couldn't find the tool named " + toolName);
        }
    }

    public EditorTool GetToolByName(string name)
    {
        foreach (EditorTool tool in toolList)
        {
            if (tool.toolName == name)
            {
                return tool;
            }
        }
        return null;
    }

    private void Start()
    {
        SelectToolByID(0);
    }

    public void Update()
    {
        //Inputs for tool selection
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectToolByID(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectToolByID(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectToolByID(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectToolByID(3);
        }
    }
}
