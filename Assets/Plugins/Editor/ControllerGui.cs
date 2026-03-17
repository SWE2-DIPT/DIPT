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
public class ControllerGUI : EditorWindow
{
    private static ControllerManager manager;
    private static ControllerComponents components;

    VisualElement BF_Button, LF_Button, RF_Button, UF_Button;

    private void OnEnable()
    {
        manager = new ControllerManager();
        components = new ControllerComponents();

        
    }

    [MenuItem("Tools/DIPT/Project")]
    public static void ShowWindow()
    {
        var window = GetWindow<ControllerGUI>();
        window.titleContent = new GUIContent("DIPT");
        window.Show();
    }

    public void CreateGUI()
    {
        LoadUXML();
    }

    void Update()
    {
        components.GetJoystickActivity();
        components.GetTriggerActivity();
        components.GetButtonActivity();

        UpdateGui();

        Debug.Log("Ticking");
    }

    /// <summary>
    /// Loads the .uxml at <paramref name="uxmlPath"/> and 
    /// the .uss at <paramref name="ussPath"/> and applies them 
    /// to the plugin's root visual element.
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <c> LoadUXML("Assets/Plugins/Editor/UI.uxml", "Assets/Plugins/Editor/UI.uss"); </c>
    /// </remarks>
    /// <param name="uxmlPath">Path from Project directory to .uxml file</param>
    /// <param name="ussPath">Path from Project directory to .uss file</param>
    /// 
    void LoadUXML(string uxmlPath = "Assets/Plugins/Editor/UI.uxml", string ussPath = "Assets/Plugins/Editor/UI.uss")
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
        
        rootVisualElement.Clear();
        rootVisualElement.styleSheets.Add(styleSheet);
        visualTree.CloneTree(rootVisualElement);

        BF_Button = rootVisualElement.Q<VisualElement>("A-button");
        UF_Button = rootVisualElement.Q<VisualElement>("Y-button");
        RF_Button = rootVisualElement.Q<VisualElement>("B-button");
        LF_Button = rootVisualElement.Q<VisualElement>("X-button");
    }

    private void UpdateGui()
    {
        if (components.get_bottom_face_button())
            BF_Button.AddToClassList("Pressed");
        else
            BF_Button.RemoveFromClassList("Pressed");

        if (components.get_top_face_button())
            UF_Button.AddToClassList("Pressed");
        else
            UF_Button.RemoveFromClassList("Pressed");

        if (components.get_right_face_button())
            RF_Button.AddToClassList("Pressed");
        else
            RF_Button.RemoveFromClassList("Pressed");

        if (components.get_left_face_button())
            LF_Button.AddToClassList("Pressed");
        else
            LF_Button.RemoveFromClassList("Pressed");



    }
}
