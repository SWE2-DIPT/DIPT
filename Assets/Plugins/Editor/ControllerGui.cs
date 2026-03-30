/*******************************************************
* Script:      TestPlugin.cs
* Author(s):   Nicholas Johnson (Add yourselves to this!)
* 
* Description:
*    A example plugin meant to showcase how to create plugins
*    in Unity.
*******************************************************/

using Codice.Client.BaseCommands;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;


/// <summary>
/// An example plugin.
/// </summary>
public class ControllerGUI : EditorWindow
{
    private static ControllerManager manager;
    private static ControllerComponents components;

    VisualElement BF_Button, LF_Button, RF_Button, UF_Button;
    VisualElement Dpad_up, Dpad_down, Dpad_right, Dpad_left;
    VisualElement R_Bumper, L_Bumper;
    VisualElement R_Trigger, L_Trigger;
    VisualElement R_TriggerFill, L_TriggerFill;
    Label RT_TriggerValue, LT_TriggerValue;

    Color UFB_idle_color, UFB_idle_color_background;
    Color UFB_pressed_color, UFB_prsesed_color_background;



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
        R_TriggerFill = rootVisualElement.Q<VisualElement>("RT-fill");
        L_TriggerFill = rootVisualElement.Q<VisualElement>("LT-fill");

        RT_TriggerValue = rootVisualElement.Q<Label>("RT-trigger-value-label");
        LT_TriggerValue = rootVisualElement.Q<Label>("LT-trigger-value-label");

        Dpad_up = rootVisualElement.Q<VisualElement>("up-pad");
        Dpad_down = rootVisualElement.Q <VisualElement>("down-pad");
        Dpad_left = rootVisualElement.Q<VisualElement>("left-pad");
        Dpad_right = rootVisualElement.Q<VisualElement>("right-pad");
    }

    //getter for colors
    private Color color_hex (string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    private void UpdateGuiButtons()
    {
        //Y
        components.GetButtonState(components.GetUpFaceButton(), UF_Button, 
                                                               color_hex("#FFCC00"), color_hex("#1F1F1F"), 
                                                               color_hex("#1F1F1F"), color_hex("#FFCC00"));
        //A
        components.GetButtonState(components.GetBottomFaceButton(), BF_Button,
                                                               color_hex("#107C10"), color_hex("#1F1F1F"),
                                                               color_hex("#1F1F1F"), color_hex("#107C10"));
        //B
        components.GetButtonState(components.GetRightFaceButton(), RF_Button,
                                                               color_hex("#D83B01"), color_hex("#1F1F1F"),
                                                               color_hex("#1F1F1F"), color_hex("#D83B01"));

        //X
        components.GetButtonState(components.GetLeftFaceButton(), LF_Button,
                                                               color_hex("#0078D4"), color_hex("#1F1F1F"),
                                                               color_hex("#1F1F1F"), color_hex("#0078D4"));


        components.GetComponentState(components.GetLeftBumper(), L_Bumper, "bumper-button-pressed");
        components.GetComponentState(components.GetRightBumper(), R_Bumper, "bumper-button-pressed");

        components.GetComponentState(components.GetDpadUp(), Dpad_up, "dpad-pressed");
        components.GetComponentState(components.GetDpadDown(), Dpad_down, "dpad-pressed");
        components.GetComponentState(components.GetDpadRight(), Dpad_right, "dpad-pressed");
        components.GetComponentState(components.GetDpadLeft(), Dpad_left, "dpad-pressed");
    }

    public void UpdateGuiAnalogs()
    {
        float prev_val = 0.0f;

        float RT = components.GetRightTrigger();
        float LT = components.GetLeftTrigger();

        //R_TriggerFill.style.height = Length.Percent(RT * 100);
        RT_TriggerValue.style.fontSize = 20f;
        RT_TriggerValue.text = $"Value: {RT:F2}";

        //L_TriggerFill.style.height = Length.Percent(LT * 100);
        LT_TriggerValue.style.fontSize = 20f;
        LT_TriggerValue.text = $"Value: {LT:F2}";
    }
}
