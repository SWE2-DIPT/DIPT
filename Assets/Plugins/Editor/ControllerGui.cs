/*******************************************************
* Script:      ControllerGUI.cs
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
using System.Collections.Generic;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;


/// <summary>
/// An example plugin.
/// </summary>
public class ControllerGUI : EditorWindow
{
    private static ControllerManager manager;
    private static ControllerComponents components;

    VisualElement R_Trigger, L_Trigger;
    Label R_trigger_value, L_trigger_value;

    Dictionary<string, VisualElement> buttons = new Dictionary<string, VisualElement>();
    VisualElement leftStick, rightStick;

    bool draggingLeft = false;
    bool draggingRight = false;

    VisualElement leftZone, rightZone;
    Vector2 leftClickOffset;
    Vector2 rightClickOffset;
    
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
        UpdateGuiJoysticks();
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
            "up-pad", "down-pad", "left-pad", "right-pad",
            "advanced"
        });

        R_trigger_value = rootVisualElement.Q<Label>("RT-trigger-value-label");
        L_trigger_value = rootVisualElement.Q<Label>("LT-trigger-value-label");

        leftStick = rootVisualElement.Q<VisualElement>("left-joystick").Q(className: "joystick");
        rightStick = rootVisualElement.Q<VisualElement>("right-joystick").Q(className: "joystick");

        leftZone = rootVisualElement.Q<VisualElement>("left-joystick").Q(className: "joystick-zone");
        rightZone = rootVisualElement.Q<VisualElement>("right-joystick").Q(className: "joystick-zone");

        // LEFT JOYSTICK
        leftZone.RegisterCallback<PointerDownEvent>(evt =>
        {
            draggingLeft = true;

            Vector2 center = leftZone.layout.size / 2f;
            Vector2 mousePos = evt.localPosition;

            Vector2 stickPos = new Vector2(
                leftStick.resolvedStyle.translate.x,
                leftStick.resolvedStyle.translate.y
            );

            leftClickOffset = stickPos - (mousePos - center);
        });
        leftZone.RegisterCallback<PointerUpEvent>(evt =>
        {
            draggingLeft = false;
            components.SetLeftJoystick(Vector2.zero); // snap back
        });
        leftZone.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (!draggingLeft) return;

            Vector2 input = GetNormalizedInput(evt, leftZone, leftClickOffset);
            components.SetLeftJoystick(input);
        });

        // RIGHT JOYSTICK
        rightZone.RegisterCallback<PointerDownEvent>(evt =>
        {
            draggingRight = true;

            Vector2 center = rightZone.layout.size / 2f;
            Vector2 mousePos = evt.localPosition;

            Vector2 stickPos = new Vector2(
                rightStick.resolvedStyle.translate.x,
                rightStick.resolvedStyle.translate.y
            );

            rightClickOffset = stickPos - (mousePos - center);
        });
        rightZone.RegisterCallback<PointerUpEvent>(evt =>
        {
            draggingRight = false;
            components.SetRightJoystick(Vector2.zero);
        });
        rightZone.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (!draggingRight) return;

            Vector2 input = GetNormalizedInput(evt, rightZone, rightClickOffset);
            components.SetRightJoystick(input);
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

            // Skip unimplemented names:
            if (button == null)
            {
                Debug.LogWarning($"Button '{name}' not found in UXML!");
                continue;
            }
            
            // Put into dictionary (hash table);
            buttons[name] = button;
            // Assign pressed events:
            button.RegisterCallback<PointerDownEvent>(evt =>
            {
                Debug.Log($"{name}: DOWN");
                SetButtonState(name, true);
            });
            button.RegisterCallback<PointerUpEvent>(evt =>
            {
                Debug.Log($"{name}: UP");
                SetButtonState(name, false);
            });
        }
    }

    void SetButtonState(string name, bool pressed)
    {
        switch (name)
        {
            case "A-button":
                components.SetBottomFaceButton(pressed);
                break;
            case "Y-button":
                components.SetTopFaceButton(pressed);
                break;
            case "B-button":
                components.SetRightFaceButton(pressed);
                break;
            case "X-button":
                components.SetLeftFaceButton(pressed);
                break;
            case "RB-button":
                components.SetRightBumper(pressed);
                break;
            case "LB-button":
                components.SetLeftBumper(pressed);
                break;
            case "up-pad":
                components.SetDpadUp(pressed);
                break;
            case "down-pad":
                components.SetDpadDown(pressed);
                break;
            case "left-pad":
                components.SetDpadLeft(pressed);
                break;
            case "right-pad":
                components.SetDpadRight(pressed);
                break;
        }
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
       
        components.GetComponentState(components.GetLeftBumper(), buttons["LB-button"], "bumper-button-pressed");
        components.GetComponentState(components.GetRightBumper(), buttons["RB-button"], "bumper-button-pressed");

        components.GetComponentState(components.GetDpadUp(), buttons["up-pad"], "DPadPressed");
        components.GetComponentState(components.GetDpadDown(), buttons["down-pad"], "DPadPressed");
        components.GetComponentState(components.GetDpadLeft(), buttons["left-pad"], "DPadPressed");
        components.GetComponentState(components.GetDpadRight(), buttons["right-pad"], "DPadPressed");
        //Y
        components.GetButtonState(components.GetUpFaceButton(), buttons["Y-button"], 
                                                               color_hex("#FFCC00"), color_hex("#1F1F1F"), 
                                                               color_hex("#1F1F1F"), color_hex("#FFCC00"));
        //A
        components.GetButtonState(components.GetBottomFaceButton(), buttons["A-button"],
                                                               color_hex("#107C10"), color_hex("#1F1F1F"),
                                                               color_hex("#1F1F1F"), color_hex("#107C10"));
        //B
        components.GetButtonState(components.GetRightFaceButton(), buttons["B-button"],
                                                               color_hex("#D83B01"), color_hex("#1F1F1F"),
                                                               color_hex("#1F1F1F"), color_hex("#D83B01"));

        //X
        components.GetButtonState(components.GetLeftFaceButton(), buttons["X-button"],
                                                               color_hex("#0078D4"), color_hex("#1F1F1F"),
                                                               color_hex("#1F1F1F"), color_hex("#0078D4"));


        components.GetComponentState(components.GetDpadUp(), buttons["up-pad"], "dpad-pressed");
        components.GetComponentState(components.GetDpadDown(), buttons["down-pad"], "dpad-pressed");
        components.GetComponentState(components.GetDpadRight(), buttons["right-pad"], "dpad-pressed");
        components.GetComponentState(components.GetDpadLeft(), buttons["left-pad"], "dpad-pressed");
    }

    public void UpdateGuiAnalogs()
    {   
        
        float RT = components.GetRightTrigger();
        float LT = components.GetLeftTrigger();
        // These two lines set the trigger width to 0 until pressed. I will fix this later.
        if (R_Trigger != null)
            R_Trigger.style.height = new Length(RT * 100, LengthUnit.Percent);
        if (L_Trigger != null)
            L_Trigger.style.height = new Length(LT * 100, LengthUnit.Percent);

        //buttons["RT-button"].style.height = Length.Percent(RT * 100);
        R_trigger_value.style.fontSize = 20f;
        R_trigger_value.text = $"{RT:F2}";
        

        //buttons["LT-button"].style.height = Length.Percent(LT * 100);
        L_trigger_value.style.fontSize = 20f;
        L_trigger_value.text = $"{LT:F2}";
    }

    public void UpdateGuiJoysticks()
    {
        Vector2 leftInput = components.GetLeftJoystick();
        Vector2 rightInput = components.GetRightJoystick();

        UpdateStick(leftStick, leftInput, 40f);
        UpdateStick(rightStick, rightInput, 40f);
    }
    void UpdateStick(VisualElement stick, Vector2 input, float radius)
    {
        if (stick == null) return;

        if (input.magnitude < 0.1f)
            input = Vector2.zero;

        input = Vector2.ClampMagnitude(input, 1f);

        float x = input.x;
        float y = -input.y;

        stick.style.translate = new Translate(
            x * radius,
            y * radius
        );
    }

    Vector2 GetNormalizedInput(PointerMoveEvent evt, VisualElement zone, Vector2 offset)
    {
        Vector2 center = zone.layout.size / 2f;
        Vector2 localPos = evt.localPosition;

        Vector2 delta = (localPos - center) + offset;

        float radius = zone.layout.width / 2f;

        Vector2 normalized = delta / radius;

        normalized = Vector2.ClampMagnitude(normalized, 1f);
        normalized.y *= -1;

        return normalized;
    }
}
