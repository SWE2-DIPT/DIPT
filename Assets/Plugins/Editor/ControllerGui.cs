/*******************************************************
* Script:      ControllerGUI.cs
* Author(s):   Nicholas Johnson (Add yourselves to this!)
* 
* Description:
*    A example plugin meant to showcase how to create plugins
*    in Unity.
*******************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UIElements;

/// <summary>
/// An example plugin.
/// </summary>
public class ControllerGUI : EditorWindow
{
    private ControllerManager manager;
    private ControllerComponents components;
    private GamepadEmulator emulator;

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

        { "right-stick", buttonType.RightStick },
        { "left-stick", buttonType.LeftStick },

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
        emulator = new GamepadEmulator();

        InputSystem.onAfterUpdate += OnInputSystemAfterUpdate;
    }

    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= OnInputSystemAfterUpdate;
        InputSystem.onDeviceChange -= OnControllerCurrentState;

        if (emulator != null)
        {
            emulator.dispose();
        }
    }

    private void OnInputSystemAfterUpdate()
    {
        PublishPhysicalControllerState();
        Repaint();
    }

    [MenuItem("Tools/DIPT/InputVisualizer")]
    public static void ShowWindow()
    {
        var window = GetWindow<ControllerGUI>();
        window.titleContent = new GUIContent("gamepad");
        window.Show();
    }

    public void CreateGUI()
    {
        InputSystem.onDeviceChange += OnControllerCurrentState;
        ControllerUpdateUI(manager.GetActivePad());
    }

    void Update()
    {
        KeyMapper.Update(emulator);

        physicalControlellerUpdate();
        emulatedControllerUpdate();
        Repaint();
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
    /// \
    /// 
    /// 
    /// 
    // void LoadUXML(string uxmlPath = "Assets/Plugins/Editor/UI_XBOX.uxml")
    // {
    //     // Load in the UXML and USS:
    //     var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        
    //     rootVisualElement.Clear();
    //     visualTree.CloneTree(rootVisualElement);


    //     // Initialize Buttons with functions:
    //     InitializeButtons(visElToButton.Keys);
    //     InitializeJoysticks(visElToJoystick.Keys);
    //     InitializeTriggers(visElToTrigger.Keys);

    //     LoadImage("xbox-button-image", "xbox/xbox-symbol.png");
    //     LoadImage("menu-button-image", "xbox/menu-symbol.png");
    //     LoadImage("view-button-image", "xbox/view-symbol.png");
    //     LoadImage("share-button-image", "xbox/share-symbol.png");
    // }
 
    // void LoadImage(string targetElement, string imageName)
    // {
    //     var imageElement = rootVisualElement.Q<Image>(targetElement);

    //     var texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Plugins/Editor/images/{imageName}");
                
    //     if (texture == null)
    //     {
    //         Debug.LogError($"Image {imageName} failed to load!");
    //     }
    //     imageElement.image = texture;
    // }

    void OnControllerCurrentState(InputDevice device, InputDeviceChange device_change)
    {
        if (!(device is Gamepad gamepad)) return;

        if (device_change == InputDeviceChange.Added || device_change == InputDeviceChange.Reconnected)
        {
            ControllerUpdateUI(device);
        }
        
        if (device_change == InputDeviceChange.Removed || device_change == InputDeviceChange.Disconnected)
        {
            Debug.Log("Controller disconnected");

            ControllerUpdateUI(manager.GetArtificialPad());
        }

    }

    void ControllerUpdateUI (InputDevice device)
    {
        Debug.Log("Detected: " + device.displayName + " | Layout: " + device.layout);

        if (manager.GetPhysicalPad() is DualShockGamepad || device.layout.Contains("Dual"))
        {
            Debug.Log("playstation controller is active");
            LoadUXML("Assets/Plugins/Editor/UI_PS.uxml");
        }
        else if (manager.GetPhysicalPad() is XInputController)
        {
            Debug.Log("Xbox controller is active");
            LoadUXML("Assets/Plugins/Editor/UI_XBOX.uxml");
        }

        else
        {
            Debug.Log("No conntected gamepad!");
            LoadUXML("Assets/Plugins/Editor/UI_GENERIC.uxml");
        }
    }

    void LoadUXML(string path)
    {
        // Load in the UXML and USS:
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        
        rootVisualElement.Clear();
        visualTree.CloneTree(rootVisualElement);

        rootVisualElement.RemoveFromClassList("xbox");
        rootVisualElement.RemoveFromClassList("ps");
        rootVisualElement.RemoveFromClassList("generic");

        if (path.Contains("XBOX"))
            rootVisualElement.AddToClassList("xbox");
        else if (path.Contains("PS"))
            rootVisualElement.AddToClassList("ps");
        else
            rootVisualElement.AddToClassList("generic");

        // Initialize Buttons with functions:
        InitializeButtons(visElToButton.Keys);
        InitializeJoysticks(visElToJoystick.Keys);
        InitializeTriggers(visElToTrigger.Keys);

        if(path.Contains("XBOX"))
        {
            LoadImage("xbox-button-image", "xbox/xbox-symbol.png");
            LoadImage("menu-button-image", "xbox/menu-symbol.png");
            LoadImage("view-button-image", "xbox/view-symbol.png");
            LoadImage("share-button-image", "xbox/share-symbol.png");
        }
        else if(path.Contains("PS"))
        {
            LoadImage("xbox-button-image", "playstation/playstation-symbol.png");
            LoadImage("menu-button-image", "xbox/menu-symbol.png");
            LoadImage("view-button-image", "xbox/view-symbol.png");
        }
           
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
    /// 
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
            string ButtonName = name;
            // Assign pressed events:;
            button.RegisterCallback<PointerDownEvent>(evt =>
            {
                Debug.Log($"{name}: DOWN");
                //HELL
                
                switch (ButtonName)
                    {
                        case "A-button":
                            ButtonName = "A";
                            break;

                        case "B-button":
                            ButtonName = "B";
                            break;

                        case "X-button":
                            ButtonName = "X";
                            break;

                        case "Y-button":
                            ButtonName = "Y";
                            break;

                        case "RB-button":
                            ButtonName = "RightShoulder";
                            break;

                        case "LB-button":
                            ButtonName = "LeftShoulder";
                            break;

                        case "up-pad":
                            ButtonName = "DpadUp";
                            break;

                        case "down-pad":
                            ButtonName = "DpadDown";
                            break;

                        case "left-pad":
                            ButtonName = "DpadLeft";
                            break;

                        case "right-pad":
                            ButtonName = "DpadRight";
                            break;

                        // case "xbox-button":
                        //     ButtonName = "Xbox";
                        //     break;

                        case "menu-button":
                            ButtonName = "Start";
                            break;

                        case "view-button":
                            ButtonName = "Select";
                            break;

                    // case "share-button":
                    //     ButtonName = "Share";
                    //     break;

                    // case "advanced":
                    //     ButtonName = "Advanced";
                    //     break;

                    case "left-stick":
                        ButtonName = "LeftStick";
                        break;

                    case "right-stick":
                        ButtonName = "RightStick";
                        break;
                }
                //removed "HE"
                emulator.pressButton(ButtonName);

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
                emulator.clear();
                // Debug.Log($"{name}: UP");

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

                string stick_name = name;
                switch (stick_name)
                {
                    case "left-joystick":
                        emulator.moveLeftJoystick(input.x, input.y);
                        break;

                    case "right-joystick":
                        emulator.moveRightJoystick(input.x, input.y);
                        break;
                }

                // Set joystick value in XboxController
                XboxController.SetJoystick(type, input);

            });

            zone.RegisterCallback<PointerUpEvent>(evt =>
            {
                emulator.resetLeftJoystick();
                emulator.resetRightJoystick();

                dragging = false;
                zone.ReleasePointer(evt.pointerId);
                
                XboxController.SetJoystick(type, Vector2.zero);
            });

            zone.RegisterCallback<PointerCaptureOutEvent>(evt =>
            {

                emulator.resetLeftJoystick();
                emulator.resetRightJoystick();
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

                string trigger_name = name;
                switch (trigger_name)
                {
                    case "LT-button":
                        trigger_name = "LeftTrigger";
                        emulator.pressLeftTrigger(normalized);
                        break;

                    case "RT-button":
                        trigger_name = "RightTrigger";
             
                        emulator.pressRightTrigger(normalized);
                        break;
                }

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

                emulator.releaseLeftTrigger();
                emulator.releaseRightTrigger();
            });
        }
    }

    private void PublishPhysicalControllerState()
    {
        if (manager == null)
        {
            return;
        }

        var pad = manager.GetPhysicalPad();

        if (pad == null)
        {
            // XboxController.SetButton(buttonType.A, false);
            // XboxController.SetButton(buttonType.B, false);
            // XboxController.SetButton(buttonType.X, false);
            // XboxController.SetButton(buttonType.Y, false);

            // XboxController.SetButton(buttonType.RBumper, false);
            // XboxController.SetButton(buttonType.LBumper, false);

            // XboxController.SetButton(buttonType.Up, false);
            // XboxController.SetButton(buttonType.Down, false);
            // XboxController.SetButton(buttonType.Left, false);
            // XboxController.SetButton(buttonType.Right, false);

            // XboxController.SetButton(buttonType.LeftStick, false);
            // XboxController.SetButton(buttonType.RightStick, false);

            // XboxController.SetTrigger(triggerType.Left, 0f);
            // XboxController.SetTrigger(triggerType.Right, 0f);

            // XboxController.SetJoystick(joystickType.Left, Vector2.zero);
            // XboxController.SetJoystick(joystickType.Right, Vector2.zero);

            return;
        }

        XboxController.SetButton(buttonType.A, pad.buttonSouth.isPressed);
        XboxController.SetButton(buttonType.B, pad.buttonEast.isPressed);
        XboxController.SetButton(buttonType.X, pad.buttonWest.isPressed);
        XboxController.SetButton(buttonType.Y, pad.buttonNorth.isPressed);

        XboxController.SetButton(buttonType.RBumper, pad.rightShoulder.isPressed);
        XboxController.SetButton(buttonType.LBumper, pad.leftShoulder.isPressed);

        XboxController.SetButton(buttonType.Up, pad.dpad.up.isPressed);
        XboxController.SetButton(buttonType.Down, pad.dpad.down.isPressed);
        XboxController.SetButton(buttonType.Left, pad.dpad.left.isPressed);
        XboxController.SetButton(buttonType.Right, pad.dpad.right.isPressed);

        XboxController.SetButton(buttonType.LeftStick, pad.leftStickButton.isPressed);
        XboxController.SetButton(buttonType.RightStick, pad.rightStickButton.isPressed);

        XboxController.SetTrigger(triggerType.Left, pad.leftTrigger.ReadValue());
        XboxController.SetTrigger(triggerType.Right, pad.rightTrigger.ReadValue());

        XboxController.SetJoystick(joystickType.Left, pad.leftStick.ReadValue());
        XboxController.SetJoystick(joystickType.Right, pad.rightStick.ReadValue());
    }
    public void physicalControlellerUpdate()
    {
        Dictionary<string, buttonType> physElToButton = new()
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

            { "left-stick", buttonType.LeftStick },
            { "right-stick", buttonType.RightStick }
        };

        Dictionary<string, joystickType> physElToJoystick = new()
        {
            { "left-joystick", joystickType.Left },
            { "right-joystick", joystickType.Right }
        };

        Dictionary<string, triggerType> physElToTrigger = new()
        {
            { "RT-button", triggerType.Right },
            { "LT-button", triggerType.Left }
        };

        foreach (var tuple in physElToButton)
        {
            string elementName = tuple.Key;
            buttonType button = tuple.Value;
            bool pressed = XboxController.GetButton(button).pressed;

            if (!buttons.TryGetValue(elementName, out VisualElement element))
            {
                continue;
            }

            if (pressed)
            {
                element.AddToClassList("ButtonPressed");
            }
            else
            {
                element.RemoveFromClassList("ButtonPressed");
            }
        }

        foreach (var tuple in physElToTrigger)
        {
            string name = tuple.Key;
            triggerType type = tuple.Value;
            float value = XboxController.GetTrigger(type).pressure;

            if (!triggers.TryGetValue(name, out var triggerRoot))
            {
                continue;
            }

            var fill = triggerRoot.Q<VisualElement>($"{name.Split('-')[0]}-fill");
            var triggerLabel = triggerRoot.Q<VisualElement>($"{name.Split('-')[0]}-label");
            var label = triggerRoot.parent.Q<Label>($"{name.Split('-')[0]}-trigger-value-label");

            if (fill != null)
            {
                fill.style.height = new Length(value * 100, LengthUnit.Percent);
            }

            if (triggerLabel != null)
            {
                var triggerLabelColor = Color.Lerp(color_hex("#FFFFFF"), color_hex("#1F1F1F"), value);
                triggerLabel.style.color = new StyleColor(triggerLabelColor);
            }

            if (label != null)
            {
                label.style.fontSize = 20f;
                label.text = $"VAL: {value:F2}";
            }
        }

        foreach (var tuple in physElToJoystick)
        {
            string name = tuple.Key;
            joystickType type = tuple.Value;

            if (!joysticks.TryGetValue(name, out var joystickRoot))
            {
                continue;
            }

            var stick = joystickRoot.Q<VisualElement>(className: "joystick");
            var labelXValue = joystickRoot.Q<Label>($"{name}-value_X");
            var labelYValue = joystickRoot.Q<Label>($"{name}-value_Y");

            Vector2 input = XboxController.GetJoystick(type).position;

            UpdateStick(stick, input, 40f);

            if (labelXValue != null && labelYValue != null)
            {
                labelXValue.text = input.x.ToString("F2");
                labelYValue.text = input.y.ToString("F2");
            }
        }
    }

    public void emulatedControllerUpdate()
    {
        foreach (var pair in visElToButton)
        {
            string elementName = pair.Key;
            buttonType button = pair.Value;

            bool isPressed = XboxController.GetButton(button).pressed || emulator.getButtonState(button);

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

            var labelXValue = joystickRoot.Q<Label>($"{name}-value_X");
            var labelYValue = joystickRoot.Q<Label>($"{name}-value_Y");

            Vector2 phys = XboxController.GetJoystick(type).position;
            Vector2 emu = emulator.GetJoysticks(type);

            Vector2 input = phys.magnitude > emu.magnitude ? phys : emu;

            UpdateStick(stick, input, 40f);
            
            if(labelXValue != null && labelYValue != null)
            {
                labelXValue.text = input.x.ToString("F2");
                labelYValue.text = input.y.ToString("F2");
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

            float value = Mathf.Max(XboxController.GetTrigger(type).pressure, emulator.GetTriggers(type));
         
            // Update fill
            if (fill != null)
                fill.style.height = new Length(value * 100, LengthUnit.Percent);

            if (triggerLabel != null)
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

        emulator.emulate();
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
