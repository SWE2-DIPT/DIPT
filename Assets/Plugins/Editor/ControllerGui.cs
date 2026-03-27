/*******************************************************
* Script:      TestPlugin.cs
* Author(s):   Nicholas Johnson (Add yourselves to this!)
* 
* Description:
*    A example plugin meant to showcase how to create plugins
*    in Unity.
*******************************************************/

using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEngine;
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
    VisualElement R_TriggerFilled;

    private void OnEnable()
    {
        manager = new ControllerManager();
        components = new ControllerComponents();

        
    }

    [MenuItem("Tools/DIPT/InputVisualizer")]
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

        UpdateGuiButtons();
        UpdateGuiAnalogs();



        // Debug.Log("Ticking");
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
        R_TriggerFilled = rootVisualElement.Q<VisualElement>("trigger-button-filling");
        
    }

    private void UpdateGuiButtons()
    {
        components.GetComponentState(components.GetBottomFaceButton(),BF_Button, "ButtonPressed");
        components.GetComponentState(components.GetUpFaceButton(),UF_Button, "ButtonPressed");
        components.GetComponentState(components.GetRightFaceButton(),RF_Button, "ButtonPressed");
        components.GetComponentState(components.GetLeftFaceButton(),LF_Button, "ButtonPressed");

        components.GetComponentState(components.GetLeftBumper(), L_Bumper, "bumper-button-pressed");
        components.GetComponentState(components.GetRightBumper(), R_Bumper, "bumper-button-pressed");
    }

    public void UpdateGuiAnalogs()
    {
        R_Trigger.style.height = new Length(components.GetRightTrigger() * 100, LengthUnit.Percent);
        L_Trigger.style.height = new Length(components.GetLeftTrigger() * 100, LengthUnit.Percent);
    }
}
