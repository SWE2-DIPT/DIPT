using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

public class ControllerComponents
{
    private readonly Dictionary<buttonType, bool> prevButtons = new();
    private readonly Dictionary<triggerType, float> prevTriggers = new();
    private readonly Dictionary<joystickType, Vector2> prevJoysticks = new();
    private readonly Dictionary<joystickType, bool> prevJoystickButtons = new();

    private bool prevTouchpad;

    private const float joystickThreshold = 0.10f;
    private const float triggerThreshold = 0.05f;

    private readonly ControllerManager manager;

    public ControllerComponents()
    {
        manager = new ControllerManager();

        foreach (buttonType button in System.Enum.GetValues(typeof(buttonType)))
            prevButtons[button] = false;

        foreach (triggerType trigger in System.Enum.GetValues(typeof(triggerType)))
            prevTriggers[trigger] = 0f;

        foreach (joystickType joystick in System.Enum.GetValues(typeof(joystickType)))
        {
            prevJoysticks[joystick] = Vector2.zero;
            prevJoystickButtons[joystick] = false;
        }

        prevTouchpad = false;
    }

    public void GetJoystickActivity()
    {
        foreach (joystickType joystick in System.Enum.GetValues(typeof(joystickType)))
        {
            Vector2 currentValue = XboxController.GetJoystick(joystick).position;
            Vector2 previousValue = prevJoysticks[joystick];

            if (Vector2.Distance(currentValue, previousValue) > joystickThreshold)
            {
                ControllerDebugLogger.LogMovement(
                    $"{GetJoystickName(joystick)} Joystick moved to X:{currentValue.x:F2} | Y:{currentValue.y:F2}"
                );

                prevJoysticks[joystick] = currentValue;
            }

            bool currentPressed = XboxController.GetJoystick(joystick).pressed;
            bool previousPressed = prevJoystickButtons[joystick];

            CheckButtonState(
                $"{GetJoystickName(joystick)} Joystick Press",
                currentPressed,
                ref previousPressed
            );

            prevJoystickButtons[joystick] = previousPressed;
        }
    }

    public void GetTriggerActivity()
    {
        foreach (triggerType trigger in System.Enum.GetValues(typeof(triggerType)))
        {
            float currentValue = XboxController.GetTrigger(trigger).pressure;
            float previousValue = prevTriggers[trigger];

            if (Mathf.Abs(currentValue - previousValue) > triggerThreshold)
            {
                ControllerDebugLogger.LogMovement(
                    $"{GetTriggerName(trigger)} Trigger changed to {currentValue:F2}"
                );

                prevTriggers[trigger] = currentValue;
            }
        }
    }

    public void GetButtonActivity()
    {
        foreach (buttonType button in System.Enum.GetValues(typeof(buttonType)))
        {
            bool currentState = XboxController.GetButton(button).pressed;
            bool previousState = prevButtons[button];

            CheckButtonState(GetButtonName(button), currentState, ref previousState);

            prevButtons[button] = previousState;
        }
    }

    public void GetTouchpadActivity()
    {
        var gamepad = Gamepad.current;

        if (gamepad is not DualShockGamepad dualShock)
            return;

        bool currentTouchpad = dualShock.touchpadButton.isPressed;

        if (currentTouchpad && !prevTouchpad)
            ControllerDebugLogger.LogPressed("Touchpad");
        else if (!currentTouchpad && prevTouchpad)
            ControllerDebugLogger.LogReleased("Touchpad");

        prevTouchpad = currentTouchpad;
    }

    internal void CheckButtonState(string buttonName, bool currentState, ref bool previousState)
    {
        if (currentState && !previousState)
            ControllerDebugLogger.LogPressed(buttonName);
        else if (!currentState && previousState)
            ControllerDebugLogger.LogReleased(buttonName);

        previousState = currentState;
    }

    private string GetButtonName(buttonType button)
    {
        var pad = manager.GetPhysicalPad();

        if (pad is XInputController)
        {
            return button switch
            {
                buttonType.A => "A Button",
                buttonType.B => "B Button",
                buttonType.X => "X Button",
                buttonType.Y => "Y Button",
                buttonType.Up => "DPad Up",
                buttonType.Down => "DPad Down",
                buttonType.Left => "DPad Left",
                buttonType.Right => "DPad Right",
                buttonType.LBumper => "LB",
                buttonType.RBumper => "RB",
                buttonType.Xbox => "Xbox Button",
                buttonType.Menu => "Menu Button",
                buttonType.View => "View Button",
                buttonType.Share => "Share Button",
                buttonType.Advanced => "Advanced Button",
                _ => button.ToString()
            };
        }

        if (pad is DualSenseGamepadHID)
        {
             return button switch
            {
                buttonType.A => "Cross Button",
                buttonType.B => "Circle Button",
                buttonType.X => "Square Button",
                buttonType.Y => "Triangle Button",
                buttonType.Up => "DPad Up",
                buttonType.Down => "DPad Down",
                buttonType.Left => "DPad Left",
                buttonType.Right => "DPad Right",
                buttonType.LBumper => "L1",
                buttonType.RBumper => "R1",
                buttonType.Xbox => "PlayStation Button",
                buttonType.Menu => "Options Button",
                buttonType.View => "Share Button",
                buttonType.Advanced => "Advanced Button",
                _ => button.ToString()
            };
        }

        return button.ToString();
    }

    private string GetTriggerName(triggerType trigger)
    {
        var pad = manager.GetPhysicalPad();

        if (pad is XInputController)
        {
            return trigger switch
            {
                triggerType.Left => "LT",
                triggerType.Right => "RT",
                _ => trigger.ToString()
            };
        }

        if (pad is DualShockGamepad)
        {
            return trigger switch
            {
                triggerType.Left => "L2",
                triggerType.Right => "R2",
                _ => trigger.ToString()
            };
        }

        return trigger.ToString();
    }

    private string GetJoystickName(joystickType joystick)
    {
        return joystick switch
        {
            joystickType.Left => "Left",
            joystickType.Right => "Right",
            _ => joystick.ToString()
        };
    }
}

[InitializeOnLoad]
public static class ControllerComponentsAutoRunner
{
    private static ControllerComponents components;

    static ControllerComponentsAutoRunner()
    {
        components = new ControllerComponents();

        EditorApplication.update -= Update;
        EditorApplication.update += Update;

        InputSystem.onAfterUpdate -= ReadInput;
        InputSystem.onAfterUpdate += ReadInput;

        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode ||
            state == PlayModeStateChange.EnteredEditMode)
        {
            components = new ControllerComponents();

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            InputSystem.onAfterUpdate -= ReadInput;
            InputSystem.onAfterUpdate += ReadInput;
        }
    }

    // Reads input in sync with the Input System's own update cycle,
    // Gamepad.current values are always current in both Edit and Play Mode
    private static void ReadInput()
    {
        bool guiOpen = EditorWindow.HasOpenInstances<ControllerGUI>();

        // GUI handles its own reading via its own onAfterUpdate subscription
        if (!guiOpen)
        {
            ControllerInputReader.ReadPhysicalInputIntoXboxController();
        }
    }
    private static void Update()
    {
        if (components == null)
            components = new ControllerComponents();

        components.GetJoystickActivity();
        components.GetTriggerActivity();
        components.GetButtonActivity();
        components.GetTouchpadActivity();
    }
}