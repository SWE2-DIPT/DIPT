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
using System.Collections.Generic;


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

    Dictionary<string, VisualElement> buttons = new Dictionary<string, VisualElement>();

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
    void LoadUXML(string uxmlPath = "Assets/Plugins/Editor/UI.uxml", string ussPath = "Assets/Plugins/Editor/UI.uss")
    {
        // Load in the UXML and USS:
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
        
        rootVisualElement.Clear();
        rootVisualElement.styleSheets.Add(styleSheet);
        visualTree.CloneTree(rootVisualElement);

        // Initialize Buttons with functions:
        InitializeButtons(new string[] {
            "A-button", "Y-button", "B-button", "X-button",
            "RB-button", "LB-button",
            "LT-button", "RT-button",
        });
    }

    /// <summary>
    /// Finds all buttons listed in <paramref name="buttonNames"/> and assigns them
    /// their functionalities.
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <c> 
    /// InitializeButtons(new string[] {
    ///    "A-button", "Y-button", "B-button", "X-button",
    ///    "RB-button", "LB-button",
    ///    "LT-button", "RT-button",
    /// }); 
    /// </c>
    /// </remarks>
    /// <param name="buttonNames">Array of names for the buttons you want queried</param>
    void InitializeButtons(string[] buttonNames)
    {
        foreach (string name in buttonNames)
        {
            // Query each name in buttonNames:
            var button = rootVisualElement.Q<VisualElement>(name);

            if (button == null)
            {
                Debug.LogWarning($"Button '{name}' not found in UXML!");
                continue;
            }

            buttons[name] = button;

            button.RegisterCallback<ClickEvent>(evt =>
            {
                components.GetComponentState(true, button, "ButtonPressed");
            });
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                components.GetComponentState(false, button, "ButtonPressed");
            });
        }
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
        // These two lines set the trigger width to 0 until pressed. I will fix this later.
        if (R_Trigger != null)
            R_Trigger.style.height = new Length(components.GetRightTrigger() * 100, LengthUnit.Percent);
        if (L_Trigger != null)
            L_Trigger.style.height = new Length(components.GetLeftTrigger() * 100, LengthUnit.Percent);
    }
}
