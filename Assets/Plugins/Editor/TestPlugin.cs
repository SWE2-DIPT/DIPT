/*******************************************************
* Script:      TestPlugin.cs
* Author(s):   Nicholas Johnson (Add yourselves to this!)
* 
* Description:
*    A example plugin meant to showcase how to create plugins
*    in Unity.
*******************************************************/

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Diagnostics;

/// <summary>
/// An example plugin.
/// </summary>
public class TestPlugin : EditorWindow
{
    [MenuItem("Tools/DIPT/Project")]
    public static void ShowWindow()
    {
        var window = GetWindow<TestPlugin>();
        window.titleContent = new GUIContent("DIPT");
        window.Show();
    }

    public void CreateGUI()
    {
        LoadUXML("Assets/Plugins/Editor/UI.uxml");
    }

    /// <summary>
    /// Loads the .uxml at <paramref name="path"/> by:
    /// <list type="number">
    ///     <item>
    ///         Loading the .uxml file into a VisualTreeAsset (a blueprint for the GUI).
    ///     </item>
    ///     <item>
    ///         Cloning that visual tree and attaching it to the window's rootVisualElement.
    ///     </item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <c> LoadUXML("Assets/Plugins/Editor/UI.uxml"); </c>
    /// </remarks>
    /// <param name="path">Path from Project directory to .uxml file</param>
    void LoadUXML(string path)
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);

        rootVisualElement.Clear();
        visualTree.CloneTree(rootVisualElement);
    }
}
