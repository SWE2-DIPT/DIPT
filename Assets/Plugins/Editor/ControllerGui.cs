/*******************************************************
* Script:      ControllerGUI.cs
* Author(s):   Nicholas Johnson, Nicholas Stearns, Jarrett Williams, Senny Lu (Add yourselves to this!)
* 
* Description:
*    Unity Editor window for the DIPT input visualizer and
*    controller emulator. This window loads the correct
*    controller UI based on the active controller type, connects
*    UI elements to emulator input, updates button, trigger, and
*    joystick visuals in real time, and keeps the virtual gamepad
*    synchronized with the shared Controller input state.
*******************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UIElements;

/// <summary>
/// Displays the controller input visualizer and handles emulator UI input.
/// </summary>
public class ControllerGUI : EditorWindow
{
    private GamepadEmulator emulator;
    private bool controllerDisabled = false;

    Dictionary<string, VisualElement> buttons = new Dictionary<string, VisualElement>();
    Dictionary<string, VisualElement> joysticks = new Dictionary<string, VisualElement>();
    Dictionary<string, VisualElement> triggers = new Dictionary<string, VisualElement>();

    /// <summary>
    /// Creates the emulator and subscribes to Input System and play mode events.
    /// </summary>
    private void OnEnable()
    {
        emulator = new GamepadEmulator();
        InputSystem.onAfterUpdate += OnInputSystemAfterUpdate;

        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    /// <summary>
    /// Unsubscribes from events, re-enables the physical controller if needed, and clears emulator state.
    /// </summary>
    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnControllerCurrentState;
        InputSystem.onAfterUpdate -= OnInputSystemAfterUpdate;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

        if (controllerDisabled)
        {
            var gamepad = ControllerManager.GetPhysicalPad();

            if (gamepad != null)
            {
                InputSystem.EnableDevice(gamepad);
            }

            controllerDisabled = false;
        }

        emulator?.dispose();
        Controller.ClearEmulated();
    }

    /// <summary>
    /// Rebuilds the emulator when switching between edit mode and play mode.
    /// </summary>
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode ||
            state == PlayModeStateChange.EnteredEditMode)
        {
            InputSystem.onDeviceChange -= OnControllerCurrentState;

            controllerDisabled = false;

            emulator?.dispose();
            emulator = new GamepadEmulator();
            Controller.ClearEmulated();

            InputSystem.onDeviceChange += OnControllerCurrentState;
        }
    }

    /// <summary>
    /// Repaints the editor window after the Input System finishes updating.
    /// </summary>
    private void OnInputSystemAfterUpdate()
    {
        Repaint();
    }

    /// <summary>
    /// Opens the controller input visualizer window from the Unity Tools menu.
    /// </summary>
    [MenuItem("Tools/DIPT/InputVisualizer")]
    public static void ShowWindow()
    {
        var window = GetWindow<ControllerGUI>();
        window.titleContent = new GUIContent("gamepad");
        window.Show();

        window.maxSize = new Vector2(441f, 771f);
        window.minSize = new Vector2(441f, 620f);
    }

    /// <summary>
    /// Initializes the visual tree and loads the correct controller UI.
    /// </summary>
    public void CreateGUI()
    {
        InputSystem.onDeviceChange += OnControllerCurrentState;
        ControllerUpdateUI(ControllerManager.GetActivePad());
    }

    /// <summary>
    /// Updates keyboard mapping, controller visuals, emulator state, and window repainting.
    /// </summary>
    void Update()
    {
        KeyMapper.Update(emulator);
        ControllerUpdate();
        Repaint();
    }

    /// <summary>
    /// Responds to gamepad connection changes and reloads the visualizer UI when needed.
    /// </summary>
    void OnControllerCurrentState(InputDevice device, InputDeviceChange deviceChange)
    {
        if (!(device is Gamepad))
        {
            return;
        }

        if (deviceChange == InputDeviceChange.Added || deviceChange == InputDeviceChange.Reconnected)
        {
            ControllerUpdateUI(device);
        }

        if (deviceChange == InputDeviceChange.Removed || deviceChange == InputDeviceChange.Disconnected)
        {
            Debug.Log("Controller disconnected");
            ControllerUpdateUI(ControllerManager.GetArtificialPad());
        }
    }

    /// <summary>
    /// Loads the PlayStation, Xbox, or generic controller UI based on the active physical controller.
    /// </summary>
    void ControllerUpdateUI(InputDevice device)
    {
        Debug.Log("Detected: " + device.displayName + " | Layout: " + device.layout);

        if (ControllerManager.GetPhysicalPad() is DualShockGamepad)
        {
            Debug.Log("playstation 5 controller is active");
            Initializer.LoadUI(rootVisualElement, "Assets/Plugins/Editor/UI_PS.uxml");
            InitializeInput();
        }
        else if (ControllerManager.GetPhysicalPad() is XInputController)
        {
            Debug.Log("Xbox controller is active");
            Initializer.LoadUI(rootVisualElement, "Assets/Plugins/Editor/UI_XBOX.uxml");
            InitializeInput();
        }
        else
        {
            Debug.Log("No connected gamepad!");
            Initializer.LoadUI(rootVisualElement, "Assets/Plugins/Editor/UI_GENERIC.uxml");
            InitializeInput();
        }
    }

    /// <summary>
    /// Converts visual element names into GamepadEmulator button names.
    /// </summary>
    private static string MapElementToButtonString(string elementName) => elementName switch
    {
        "A-button" => "A",
        "B-button" => "B",
        "X-button" => "X",
        "Y-button" => "Y",
        "RB-button" => "RightShoulder",
        "LB-button" => "LeftShoulder",
        "up-pad" => "DpadUp",
        "down-pad" => "DpadDown",
        "left-pad" => "DpadLeft",
        "right-pad" => "DpadRight",
        "menu-button" => "Start",
        "view-button" => "Select",
        "left-stick" => "LeftStick",
        "right-stick" => "RightStick",
        _ => elementName
    };

    /// <summary>
    /// Connects button, joystick, and trigger UI elements to emulator input handlers.
    /// </summary>
    void InitializeInput()
    {
        InitializeButtons(Dictionaries.visElToButton.Keys);
        InitializeJoysticks(Dictionaries.visElToJoystick.Keys);
        InitializeTriggers(Dictionaries.visElToTrigger.Keys);
    }

    /// <summary>
    /// Registers pointer events for button elements and updates the emulated button state.
    /// </summary>
    void InitializeButtons(IEnumerable<string> buttonNames)
    {
        foreach (string elementName in buttonNames)
        {
            var button = rootVisualElement.Q<VisualElement>(elementName);

            if (button == null)
            {
                Debug.LogWarning($"Button '{elementName}' not found in UXML!");
                continue;
            }

            buttons[elementName] = button;

            if (elementName == "advanced")
            {
                button.RegisterCallback<PointerDownEvent>(pointerEvent =>
                {
                    var gamepad = ControllerManager.GetPhysicalPad();

                    if (gamepad == null)
                    {
                        return;
                    }

                    controllerDisabled = !controllerDisabled;

                    if (controllerDisabled)
                    {
                        InputSystem.DisableDevice(gamepad);
                        button.AddToClassList("ButtonPressed");
                        Debug.Log("Physical controller disabled — emulator only");
                    }
                    else
                    {
                        InputSystem.EnableDevice(gamepad);
                        button.RemoveFromClassList("ButtonPressed");
                        Debug.Log("Physical controller re-enabled");
                    }
                });

                continue;
            }

            button.RegisterCallback<PointerDownEvent>(pointerEvent =>
            {
                string mappedButton = MapElementToButtonString(elementName);
                emulator.pressButton(mappedButton);

                if (Dictionaries.visElToButton.TryGetValue(elementName, out var buttonTypeValue))
                {
                    Controller.SetEmulatedButton(buttonTypeValue, true);
                }

                var image = button.Q<Image>();

                if (image != null && image.name != "xbox-button-image")
                {
                    image.tintColor = Color.black;
                }
            });

            button.RegisterCallback<PointerUpEvent>(pointerEvent =>
            {
                string mappedButton = MapElementToButtonString(elementName);
                emulator.releaseButton(mappedButton);

                if (Dictionaries.visElToButton.TryGetValue(elementName, out var buttonTypeValue))
                {
                    Controller.SetEmulatedButton(buttonTypeValue, false);
                }

                var image = button.Q<Image>();

                if (image != null && image.name != "xbox-button-image")
                {
                    image.tintColor = Color.white;
                }
            });
        }
    }

    /// <summary>
    /// Registers pointer drag events for joystick elements and updates the emulated joystick state.
    /// </summary>
    void InitializeJoysticks(IEnumerable<string> joystickNames)
    {
        foreach (string elementName in joystickNames)
        {
            var joystickRoot = rootVisualElement.Q<VisualElement>(elementName);

            if (joystickRoot == null)
            {
                Debug.LogWarning($"Joystick {elementName} not found!");
                continue;
            }

            joysticks[elementName] = joystickRoot;

            var stick = joystickRoot.Q<VisualElement>(className: "joystick");
            var zone = joystickRoot.Q<VisualElement>(className: "joystick-zone");

            bool dragging = false;
            Vector2 clickOffset = Vector2.zero;

            if (!Dictionaries.visElToJoystick.TryGetValue(elementName, out joystickType joystickTypeValue))
            {
                Debug.LogWarning($"Joystick name '{elementName}' not mapped to a joystickType!");
                continue;
            }

            zone.RegisterCallback<PointerDownEvent>(pointerEvent =>
            {
                dragging = true;
                zone.CapturePointer(pointerEvent.pointerId);

                Vector2 center = zone.layout.size / 2f;
                Vector2 mousePosition = pointerEvent.localPosition;
                Vector2 stickPosition = new Vector2(
                    stick.resolvedStyle.translate.x,
                    stick.resolvedStyle.translate.y
                );

                clickOffset = stickPosition - (mousePosition - center);
            });

            zone.RegisterCallback<PointerMoveEvent>(pointerEvent =>
            {
                if (!dragging)
                {
                    return;
                }

                Vector2 input = GetNormalizedInput(pointerEvent, zone, clickOffset);

                switch (elementName)
                {
                    case "left-joystick":
                        emulator.moveLeftJoystick(input.x, input.y);
                        break;

                    case "right-joystick":
                        emulator.moveRightJoystick(input.x, input.y);
                        break;
                }

                Controller.SetEmulatedJoystick(joystickTypeValue, input);
            });

            zone.RegisterCallback<PointerUpEvent>(pointerEvent =>
            {
                dragging = false;
                zone.ReleasePointer(pointerEvent.pointerId);

                emulator.resetLeftJoystick();
                emulator.resetRightJoystick();
                Controller.SetEmulatedJoystick(joystickTypeValue, Vector2.zero);
            });

            zone.RegisterCallback<PointerCaptureOutEvent>(pointerEvent =>
            {
                dragging = false;

                emulator.resetLeftJoystick();
                emulator.resetRightJoystick();
                Controller.SetEmulatedJoystick(joystickTypeValue, Vector2.zero);
            });
        }
    }

    /// <summary>
    /// Registers pointer drag events for trigger elements and updates the emulated trigger state.
    /// </summary>
    void InitializeTriggers(IEnumerable<string> triggerNames)
    {
        foreach (string elementName in triggerNames)
        {
            var trigger = rootVisualElement.Q<VisualElement>(elementName);

            if (trigger == null)
            {
                Debug.LogWarning($"Trigger '{elementName}' not found in UXML!");
                continue;
            }

            triggers[elementName] = trigger;
            bool dragging = false;

            trigger.RegisterCallback<PointerDownEvent>(pointerEvent =>
            {
                dragging = true;
                trigger.CapturePointer(pointerEvent.pointerId);
            });

            trigger.RegisterCallback<PointerMoveEvent>(pointerEvent =>
            {
                if (!dragging)
                {
                    return;
                }

                float triggerHeight = trigger.layout.height;
                float localYPosition = pointerEvent.localPosition.y;
                float normalizedValue = 1f - Mathf.Clamp01(localYPosition / triggerHeight);

                switch (elementName)
                {
                    case "LT-button":
                        emulator.pressLeftTrigger(normalizedValue);
                        break;

                    case "RT-button":
                        emulator.pressRightTrigger(normalizedValue);
                        break;
                }

                if (Dictionaries.visElToTrigger.TryGetValue(elementName, out var triggerTypeValue))
                {
                    Controller.SetEmulatedTrigger(triggerTypeValue, normalizedValue);
                }
            });

            trigger.RegisterCallback<PointerUpEvent>(pointerEvent =>
            {
                dragging = false;
                trigger.ReleasePointer(pointerEvent.pointerId);

                emulator.releaseLeftTrigger();
                emulator.releaseRightTrigger();

                if (Dictionaries.visElToTrigger.TryGetValue(elementName, out var triggerTypeValue))
                {
                    Controller.SetEmulatedTrigger(triggerTypeValue, 0f);
                }
            });

            trigger.RegisterCallback<PointerCaptureOutEvent>(pointerEvent =>
            {
                dragging = false;

                emulator.releaseLeftTrigger();
                emulator.releaseRightTrigger();

                if (Dictionaries.visElToTrigger.TryGetValue(elementName, out var triggerTypeValue))
                {
                    Controller.SetEmulatedTrigger(triggerTypeValue, 0f);
                }
            });
        }
    }

    /// <summary>
    /// Reads the physical gamepad directly and stores the values in the shared Controller state.
    /// </summary>
    private void PublishPhysicalControllerState()
    {
        var gamepad = ControllerManager.GetPhysicalPad();

        if (gamepad == null)
        {
            return;
        }

        Controller.SetPhysicalButton(buttonType.A, gamepad.buttonSouth.isPressed);
        Controller.SetPhysicalButton(buttonType.B, gamepad.buttonEast.isPressed);
        Controller.SetPhysicalButton(buttonType.X, gamepad.buttonWest.isPressed);
        Controller.SetPhysicalButton(buttonType.Y, gamepad.buttonNorth.isPressed);
        Controller.SetPhysicalButton(buttonType.RBumper, gamepad.rightShoulder.isPressed);
        Controller.SetPhysicalButton(buttonType.LBumper, gamepad.leftShoulder.isPressed);
        Controller.SetPhysicalButton(buttonType.Up, gamepad.dpad.up.isPressed);
        Controller.SetPhysicalButton(buttonType.Down, gamepad.dpad.down.isPressed);
        Controller.SetPhysicalButton(buttonType.Left, gamepad.dpad.left.isPressed);
        Controller.SetPhysicalButton(buttonType.Right, gamepad.dpad.right.isPressed);
        Controller.SetPhysicalButton(buttonType.Menu, gamepad.startButton.isPressed);
        Controller.SetPhysicalButton(buttonType.View, gamepad.selectButton.isPressed);
        Controller.SetPhysicalButton(buttonType.LeftStick, gamepad.leftStickButton.isPressed);
        Controller.SetPhysicalButton(buttonType.RightStick, gamepad.rightStickButton.isPressed);

        Controller.SetPhysicalTrigger(triggerType.Left, gamepad.leftTrigger.ReadValue());
        Controller.SetPhysicalTrigger(triggerType.Right, gamepad.rightTrigger.ReadValue());

        Controller.SetPhysicalJoystick(joystickType.Left, gamepad.leftStick.ReadValue());
        Controller.SetPhysicalJoystick(joystickType.Right, gamepad.rightStick.ReadValue());
    }

    /// <summary>
    /// Updates all button, joystick, and trigger visuals using the merged Controller input state.
    /// </summary>
    public void ControllerUpdate()
    {
        foreach (var (elementName, buttonTypeValue) in Dictionaries.visElToButton)
        {
            if (!buttons.TryGetValue(elementName, out var element))
            {
                continue;
            }

            if (Controller.GetButton(buttonTypeValue))
            {
                element.AddToClassList("ButtonPressed");
            }
            else
            {
                element.RemoveFromClassList("ButtonPressed");
            }
        }

        foreach (var (elementName, joystickTypeValue) in Dictionaries.visElToJoystick)
        {
            if (!joysticks.TryGetValue(elementName, out var joystickRoot))
            {
                continue;
            }

            var stick = joystickRoot.Q<VisualElement>(className: "joystick");
            var joystickXLabel = joystickRoot.Q<Label>($"{elementName}-value_X");
            var joystickYLabel = joystickRoot.Q<Label>($"{elementName}-value_Y");

            Vector2 input = Controller.GetJoystick(joystickTypeValue);

            UpdateStick(stick, input, 40f);

            if (joystickXLabel != null)
            {
                joystickXLabel.text = input.x.ToString("F2");
            }

            if (joystickYLabel != null)
            {
                joystickYLabel.text = input.y.ToString("F2");
            }
        }

        foreach (var (elementName, triggerTypeValue) in Dictionaries.visElToTrigger)
        {
            if (!triggers.TryGetValue(elementName, out var triggerRoot))
            {
                continue;
            }

            float value = Controller.GetTrigger(triggerTypeValue);
            string elementPrefix = elementName.Split('-')[0];
            var fill = triggerRoot.Q<VisualElement>($"{elementPrefix}-fill");
            var triggerLabel = triggerRoot.Q<VisualElement>($"{elementPrefix}-label");
            var valueLabel = triggerRoot.parent.Q<Label>($"{elementPrefix}-trigger-value-label");

            if (fill != null)
            {
                fill.style.height = new Length(value * 100, LengthUnit.Percent);
            }

            if (triggerLabel != null)
            {
                triggerLabel.style.color = Color.Lerp(color_hex("#FFFFFF"), color_hex("#1F1F1F"), value);
            }

            if (valueLabel != null)
            {
                valueLabel.style.fontSize = 20f;
                valueLabel.text = $"VAL: {value:F2}";
            }
        }

        emulator.emulate();
    }

    /// <summary>
    /// Converts a hex color string into a Unity Color value.
    /// </summary>
    private Color color_hex(string hex)
    {
        Color color;
        UnityEngine.ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    /// <summary>
    /// Moves the joystick UI handle based on the current joystick input value.
    /// </summary>
    void UpdateStick(VisualElement stick, Vector2 input, float radius)
    {
        if (stick == null)
        {
            return;
        }

        if (input.magnitude < 0.1f)
        {
            input = Vector2.zero;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        float xPosition = input.x;
        float yPosition = -input.y;

        stick.style.translate = new Translate(
            xPosition * radius,
            yPosition * radius
        );
    }

    /// <summary>
    /// Converts pointer movement inside a joystick zone into a normalized joystick input value.
    /// </summary>
    Vector2 GetNormalizedInput(PointerMoveEvent pointerEvent, VisualElement zone, Vector2 offset)
    {
        Vector2 worldPosition = pointerEvent.position;
        Vector2 localPosition = zone.WorldToLocal(worldPosition);

        Vector2 center = zone.layout.size / 2f;
        Vector2 delta = (localPosition - center) + offset;

        float radius = zone.layout.width / 2f;

        Vector2 normalizedInput = delta / radius;

        normalizedInput = Vector2.ClampMagnitude(normalizedInput, 1f);
        normalizedInput.y *= -1;

        return normalizedInput;
    }
}