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
using System.Reflection;
using System.Linq.Expressions;
using Unity.VisualScripting;

/// <summary>
/// An example plugin.
/// </summary>
public class ControllerGUI : EditorWindow
{
    private static ControllerManager manager;
    private static ControllerComponents components;

    private static KeyMapper emulator;

    VisualElement RT_Trigger, LT_Trigger;
    Label RT_Label, LT_Label;
    Label R_trigger_value, L_trigger_value;

    Dictionary<string, VisualElement> buttons = new Dictionary<string, VisualElement>();
    Dictionary<string, VisualElement> joysticks = new Dictionary<string, VisualElement>();
    Dictionary<string, VisualElement> triggers = new Dictionary<string, VisualElement>();

    Dictionary<string, buttonType> visElToButton = new()
    {
        { "A-button", buttonType.A },
        { "B-button", buttonType.B },
        { "X-button", buttonType.X },
        { "Y-button", buttonType.Y },

        { "RB-button", buttonType.RBumper },
        { "LB-button", buttonType.LBumper },

        { "up-pad", buttonType.Up },
        { "down-pad", buttonType.Down },
        { "left-pad", buttonType.Left },
        { "right-pad", buttonType.Right },

        { "xbox-button", buttonType.Xbox },
        { "menu-button", buttonType.Menu },
        { "view-button", buttonType.View },
        { "share-button", buttonType.Share },

        { "advanced", buttonType.Advanced }
    };

    Dictionary<string, joystickType> visElToJoystick = new()
    {
        { "left-joystick", joystickType.Left },
        { "right-joystick", joystickType.Right }
    };

    Dictionary<string, triggerType> visElToTrigger = new()
    {
        { "LT-button", triggerType.Left },
        { "RT-button", triggerType.Right }
    };

    private void OnEnable()
    {
        manager = new ControllerManager();
        components = new ControllerComponents();

        emulator = new KeyMapper(components); /* KeyMapper.cs */
    }

    [MenuItem("Tools/DIPT/InputVisualizer")]
    public static void ShowWindow()
    {
        var window = GetWindow<ControllerGUI>();
        window.titleContent = new GUIContent("Xbox");
        window.Show();
    }

    public void CreateGUI()
    {
        LoadUXML();
    }

    void Update()
    {
        foreach (var pair in visElToButton)
        {
            string elementName = pair.Key;
            buttonType button = pair.Value;

            bool isPressed = XboxController.GetButton(button).pressed;
            if (!buttons.TryGetValue(elementName, out VisualElement element))
                continue;
            if (isPressed)
                element.AddToClassList("ButtonPressed");
            else
                element.RemoveFromClassList("ButtonPressed");
        }

        foreach (var pair in visElToJoystick)
        {
            string name = pair.Key;
            joystickType type = pair.Value;

            var joystickRoot = joysticks[name];
            if (joystickRoot == null) continue;

            var stick = joystickRoot.Q<VisualElement>(className: "joystick");
            var labelX = joystickRoot.Q<Label>($"{name}-value_X");
            var labelY = joystickRoot.Q<Label>($"{name}-value_Y");

            Vector2 input = XboxController.GetJoystick(type).position;

            UpdateStick(stick, input, 40f);

            if (labelX != null)
            {
                labelX.style.fontSize = 20f;
                labelX.text = $"X: {input.x:F2}";
            }
            if (labelY != null)
            {
                labelY.style.fontSize = 20f;
                labelY.text = $"Y: {input.y:F2}";
            }
        }

        foreach (var pair in visElToTrigger)
        {
            string name = pair.Key;
            triggerType type = pair.Value;

            if (!triggers.TryGetValue(name, out var triggerRoot))
                continue;

            var fill = triggerRoot.Q<VisualElement>($"{name.Split('-')[0]}-fill");
            var triggerLabel = triggerRoot.Q<VisualElement>($"{name.Split('-')[0]}-label");
            var label = triggerRoot.parent.Q<Label>($"{name.Split('-')[0]}-trigger-value-label");

            float value = XboxController.GetTrigger(type).pressure;

            // Update fill
            if (fill != null)
                fill.style.height = new Length(value * 100, LengthUnit.Percent);

            if(triggerLabel != null)
            {
                var triggerLabelColor = Color.Lerp(color_hex("#FFFFFF"), color_hex("#1F1F1F"), value);
                triggerLabel.style.color = new StyleColor(triggerLabelColor);
            }

            // Update label
            if (label != null)
            {
                label.style.fontSize = 20f;
                label.text = $"VAL: {value:F2}";
            }
        }
        // emulator.UpdateKeyboardEmulation();
    }

    //~LOAD~GUI~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

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
    void LoadUXML(string uxmlPath = "Assets/Plugins/Editor/UI.uxml")
    {
        // Load in the UXML and USS:
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        
        rootVisualElement.Clear();
        visualTree.CloneTree(rootVisualElement);

        // Initialize Buttons with functions:
        InitializeButtons(visElToButton.Keys);
        InitializeJoysticks(visElToJoystick.Keys);
        InitializeTriggers(visElToTrigger.Keys);

        LoadImage("xbox-button-image", "xbox-symbol.png");
        LoadImage("menu-button-image", "menu-symbol.png");
        LoadImage("view-button-image", "view-symbol.png");
        LoadImage("share-button-image", "share-symbol.png");
    }

    void LoadImage(string targetElement, string imageName)
    {
        var imageElement = rootVisualElement.Q<Image>(targetElement);

        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Plugins/Editor/images/{imageName}");
                
        if (texture == null)
        {
            Debug.LogError($"Image {imageName} failed to load!");
        }
        imageElement.image = texture;
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
    void InitializeButtons(IEnumerable<string> buttonNames)
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

                // Set this button's pressed state to true.
                if (visElToButton.TryGetValue(name, out var type))
                    XboxController.SetButton(type, true);

                // Special logic for buttons with images
                var image = button.Q<Image>();
                // If image exists and is not the Xbox button then invert it when pressed.
                bool isInvertable = image != null && image.name != "xbox-button-image";
                if (isInvertable)
                    image.tintColor = Color.black;
            });
            button.RegisterCallback<PointerUpEvent>(evt =>
            {
                Debug.Log($"{name}: UP");

                // Set this button's pressed state to false.
                if (visElToButton.TryGetValue(name, out var type))
                    XboxController.SetButton(type, false);

                // Special logic for buttons with images
                var image = button.Q<Image>();
                // If image exists and is not the Xbox button then invert it when released.
                bool isInvertable = image != null && image.name != "xbox-button-image";
                if (isInvertable)
                    image.tintColor = Color.white;
            });
        }
    }

    void InitializeJoysticks(IEnumerable<string> joystickNames)
    {
        foreach (string name in joystickNames)
        {
            var joystickRoot = rootVisualElement.Q<VisualElement>(name);
            if (joystickRoot == null)
            {
                Debug.LogWarning($"Joystick {name} not found!");
                continue;
            }

            joysticks[name] = joystickRoot;

            var stick = joystickRoot.Q<VisualElement>(className: "joystick");
            var zone = joystickRoot.Q<VisualElement>(className: "joystick-zone");

            var labelX = joystickRoot.Q<Label>($"{name}-value_X");
            var labelY = joystickRoot.Q<Label>($"{name}-value_Y");

            bool dragging = false;
            Vector2 clickOffset = Vector2.zero;

            // Look up joystick type from dictionary
            if (!visElToJoystick.TryGetValue(name, out joystickType type))
            {
                Debug.LogWarning($"Joystick name '{name}' not mapped to a joystickType!");
                continue;
            }

            zone.RegisterCallback<PointerDownEvent>(evt =>
            {
                dragging = true;
                zone.CapturePointer(evt.pointerId);

                Vector2 center = zone.layout.size / 2f;
                Vector2 mousePos = evt.localPosition;

                Vector2 stickPos = new Vector2(
                    stick.resolvedStyle.translate.x,
                    stick.resolvedStyle.translate.y
                );

                clickOffset = stickPos - (mousePos - center);
            });

            zone.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!dragging) return;

                Vector2 input = GetNormalizedInput(evt, zone, clickOffset);

                // Set joystick value in XboxController
                XboxController.SetJoystick(type, input);
            });

            zone.RegisterCallback<PointerUpEvent>(evt =>
            {
                dragging = false;
                zone.ReleasePointer(evt.pointerId);

                XboxController.SetJoystick(type, Vector2.zero);
            });

            zone.RegisterCallback<PointerCaptureOutEvent>(evt =>
            {
                dragging = false;

                XboxController.SetJoystick(type, Vector2.zero);
            });
        }
    }

    void InitializeTriggers(IEnumerable<string> triggerNames)
    {
        foreach (string name in triggerNames)
        {
            // Query each name in buttonNames:
            var trigger = rootVisualElement.Q<VisualElement>(name);

            // Skip unimplemented names:
            if (trigger == null)
            {
                Debug.LogWarning($"Trigger '{name}' not found in UXML!");
                continue;
            }
            
            // Put into dictionary (hash table);
            triggers[name] = trigger;
            // Assign pressed events:
            bool dragging = false;

            trigger.RegisterCallback<PointerDownEvent>(evt =>
            {
                Debug.Log($"{name}: DOWN");

                dragging = true;
                trigger.CapturePointer(evt.pointerId);
            });

            trigger.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!dragging) return;

                float height = trigger.layout.height;
                float y = evt.localPosition.y;

                // Invert so bottom = 0, top = 1
                float normalized = 1f - Mathf.Clamp01(y / height);

                if (visElToTrigger.TryGetValue(name, out var type))
                    XboxController.SetTrigger(type, normalized);
            });

            trigger.RegisterCallback<PointerUpEvent>(evt =>
            {
                Debug.Log($"{name}: UP");

                dragging = false;
                trigger.ReleasePointer(evt.pointerId);

                if (visElToTrigger.TryGetValue(name, out var type))
                    XboxController.SetTrigger(type, 0f);
            });

            trigger.RegisterCallback<PointerCaptureOutEvent>(evt =>
            {
                dragging = false;

                if (visElToTrigger.TryGetValue(name, out var type))
                    XboxController.SetTrigger(type, 0f);
                    
            });
        }
    }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private Color color_hex(string hex)
    {
        Color color;
        UnityEngine.ColorUtility.TryParseHtmlString(hex, out color);
        return color;
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

        // Realistic Squishing (stretch goal)
        /*
        float maxSquish = 0.2f;
        float squishX = 1f - Mathf.Abs(input.x) * maxSquish;
        float squishY = 1f - Mathf.Abs(input.y) * maxSquish;

        stick.style.scale = new Scale(new Vector3(squishX, squishY, 1f));
        */
    }

    Vector2 GetNormalizedInput(PointerMoveEvent evt, VisualElement zone, Vector2 offset)
    {
        Vector2 world = evt.position;
        Vector2 localPos = zone.WorldToLocal(world);

        Vector2 center = zone.layout.size / 2f;
        Vector2 delta = (localPos - center) + offset;

        float radius = zone.layout.width / 2f;

        Vector2 normalized = delta / radius;

        normalized = Vector2.ClampMagnitude(normalized, 1f);
        normalized.y *= -1;

        return normalized;
    }
}
