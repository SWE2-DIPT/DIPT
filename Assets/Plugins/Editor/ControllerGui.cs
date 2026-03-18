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
    VisualElement R_Bumper, L_Bumper;
    VisualElement R_Trigger, L_Trigger;

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
        components.GetDpadActivity();

        UpdateGuiButtons();
        UpdateGuiAnalogs();
        

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

        R_Bumper = rootVisualElement.Q<VisualElement>("RB-button");
        L_Bumper = rootVisualElement.Q<VisualElement>("LB-button");

        L_Trigger = rootVisualElement.Q<VisualElement>("LT-button");
        R_Trigger = rootVisualElement.Q<VisualElement>("RT-button");
    }

    private void UpdateGuiButtons()
    {
        if (components.get_bottom_face_button())
            BF_Button.AddToClassList("ButtonPressed");
        else
            BF_Button.RemoveFromClassList("ButtonPressed");

        if (components.get_top_face_button())
            UF_Button.AddToClassList("ButtonPressed");
        else
            UF_Button.RemoveFromClassList("ButtonPressed");

        if (components.get_right_face_button())
            RF_Button.AddToClassList("ButtonPressed");
        else
            RF_Button.RemoveFromClassList("ButtonPressed");

        if (components.get_left_face_button())
            LF_Button.AddToClassList("ButtonPressed");
        else
            LF_Button.RemoveFromClassList("ButtonPressed");

        if (components.get_right_bumper())
            R_Bumper.AddToClassList("bumper-button-pressed");
        else
            R_Bumper.RemoveFromClassList("bumper-button-pressed");


        if (components.get_left_bumper())
            L_Bumper.AddToClassList("bumper-button-pressed");
        else
            L_Bumper.RemoveFromClassList("bumper-button-pressed");

    }

    public void UpdateGuiAnalogs()
    {
        if (components.get_left_trigger() != 0.0f)
            L_Trigger.AddToClassList("trigger-button-triggered");
        else
            L_Trigger.RemoveFromClassList("trigger-button-triggered");

        if (components.get_right_trigger() != 0.0f)
            R_Trigger.AddToClassList("trigger-button-triggered");
        else
            R_Trigger.RemoveFromClassList("trigger-button-triggered");
    }
}
