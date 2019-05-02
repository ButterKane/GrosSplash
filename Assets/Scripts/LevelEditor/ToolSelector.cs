using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelector : MonoBehaviour
{
    public List<EditorTool> toolList;
    public GameObject toolHighlighter;

    public void SelectToolByID(int toolID)
    {
        LevelEditor.i.UpdateAllGuizmos();
        foreach (EditorTool tool in toolList)
        {
            if (tool.linkedPanel != null)
            {
                tool.linkedPanel.gameObject.SetActive(false);
            }
        }
        LevelEditor.i.selectedTool = toolList[toolID];
        if (toolList[toolID].linkedPanel != null)
        {
            toolList[toolID].linkedPanel.gameObject.SetActive(true);
        }
        toolHighlighter.transform.position = toolList[toolID].transform.position;
        LevelEditor.i.library.UpdateSelectedObject();
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
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectToolByID(4);
        }
    }
}
