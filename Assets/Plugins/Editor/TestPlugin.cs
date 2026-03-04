/*******************************************************
* Script:      TestPlugin.cs
* Author(s):   Nicholas Johnson (Add yourselves to this as you!)
* 
* Description:
*    A example plugin meant to showcase how to create plugins
*    in Unity.
*******************************************************/

using UnityEngine;
using UnityEditor;

/// <summary>
/// An example plugin.
/// </summary>
public class TestPlugin : EditorWindow
{
    [MenuItem("Tools/DIPT/Project")]
    public static void ShowWindow()
    {
        GetWindow<TestPlugin>();
    }

    void OnGUI()
    {
        GUILayout.Label("Controller Debug");
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Test Button"))
        {
            Debug.Log("Clicked!");
        }
    }
}
