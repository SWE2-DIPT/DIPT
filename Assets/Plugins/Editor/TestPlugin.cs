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

    public void CreateGUI()
    {
        var root = rootVisualElement;

        var label = new Label("Controller Debug");
        label.style.fontSize = 16;
        label.style.marginBottom = 10;
        root.Add(label);

        var button = new Button(() =>
        {
            Debug.Log("Clicked!");
        });
        button.text = "Test Button";
        button.style.backgroundColor = Color.red;
        button.style.height = 40;
        button.style.unityTextAlign = TextAnchor.MiddleCenter;

        root.Add(button);
    }
}
