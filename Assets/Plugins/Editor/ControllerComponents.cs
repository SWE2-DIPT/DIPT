/*******************************************************
* Script:      ControllerComponents.cs
* Authors:     Nicholas Stearns
* Description:
*    Tracks controller input changes and sends readable log
*    messages to ControllerDebugLogger. This file compares the
*    current controller state against the previous state so it
*    only logs changes, including button presses, button releases,
*    joystick movement, trigger movement, and PlayStation touchpad
*    input. It also runs automatically in the Unity Editor so the
*    log can work without the input visualizer window being open.
*******************************************************/

using System;
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
    private readonly Dictionary<buttonType, bool> previousButtons = new();
    private readonly Dictionary<triggerType, float> previousTriggers = new();
    private readonly Dictionary<joystickType, Vector2> previousJoysticks = new();
    private readonly Dictionary<joystickType, bool> previousJoystickButtons = new();

    private bool previousTouchpad;

    private const float joystickThreshold = 0.10f;
    private const float triggerThreshold = 0.05f;

    /// <summary>
    /// Initializes previous input values so future updates can detect changes.
    /// </summary>
    public ControllerComponents()
    {
        foreach (buttonType controllerButton in Enum.GetValues(typeof(buttonType)))
        {
            previousButtons[controllerButton] = false;
        }

        foreach (triggerType controllerTrigger in Enum.GetValues(typeof(triggerType)))
        {
            previousTriggers[controllerTrigger] = 0f;
        }

        foreach (joystickType controllerJoystick in Enum.GetValues(typeof(joystickType)))
        {
            previousJoysticks[controllerJoystick] = Vector2.zero;
            previousJoystickButtons[controllerJoystick] = false;
        }

        previousTouchpad = false;
    }

    /// <summary>
    /// Logs joystick movement and joystick button press or release changes.
    /// </summary>
    public void GetJoystickActivity()
    {
        foreach (joystickType controllerJoystick in Enum.GetValues(typeof(joystickType)))
        {
            Vector2 currentPosition = Controller.GetJoystick(controllerJoystick);
            Vector2 previousPosition = previousJoysticks[controllerJoystick];

            if (Vector2.Distance(currentPosition, previousPosition) > joystickThreshold)
            {
                ControllerDebugLogger.LogMovement(
                    $"{GetJoystickName(controllerJoystick)} Joystick → X:{currentPosition.x:F2} | Y:{currentPosition.y:F2}"
                );

                previousJoysticks[controllerJoystick] = currentPosition;
            }

            bool currentPressed = Controller.GetButton(
                controllerJoystick == joystickType.Left ? buttonType.LeftStick : buttonType.RightStick
            );

            bool previousPressed = previousJoystickButtons[controllerJoystick];

            CheckButtonState(
                $"{GetJoystickName(controllerJoystick)} Joystick",
                currentPressed,
                ref previousPressed
            );

            previousJoystickButtons[controllerJoystick] = previousPressed;
        }
    }

    /// <summary>
    /// Logs trigger value changes when the trigger moves past the threshold.
    /// </summary>
    public void GetTriggerActivity()
    {
        foreach (triggerType controllerTrigger in Enum.GetValues(typeof(triggerType)))
        {
            float currentValue = Controller.GetTrigger(controllerTrigger);
            float previousValue = previousTriggers[controllerTrigger];

            if (Mathf.Abs(currentValue - previousValue) > triggerThreshold)
            {
                ControllerDebugLogger.LogMovement(
                    $"{GetTriggerName(controllerTrigger)} Trigger → {currentValue:F2}"
                );

                previousTriggers[controllerTrigger] = currentValue;
            }
        }
    }

    /// <summary>
    /// Logs regular button press and release changes.
    /// </summary>
    public void GetButtonActivity()
    {
        foreach (buttonType controllerButton in Enum.GetValues(typeof(buttonType)))
        {
            if (controllerButton == buttonType.LeftStick || controllerButton == buttonType.RightStick)
            {
                continue;
            }

            bool currentPressed = Controller.GetButton(controllerButton);
            bool previousPressed = previousButtons[controllerButton];

            CheckButtonState(GetButtonName(controllerButton), currentPressed, ref previousPressed);
            previousButtons[controllerButton] = previousPressed;
        }
    }

    /// <summary>
    /// Logs PlayStation touchpad press and release changes when a supported controller is connected.
    /// </summary>
    public void GetTouchpadActivity()
    {
        var gamepad = ControllerManager.GetPhysicalPad();

        if (gamepad is not DualShockGamepad dualShockGamepad)
        {
            return;
        }

        bool currentTouchpad = dualShockGamepad.touchpadButton.isPressed;

        if (currentTouchpad && !previousTouchpad)
        {
            ControllerDebugLogger.LogPressed("Touchpad");
        }
        else if (!currentTouchpad && previousTouchpad)
        {
            ControllerDebugLogger.LogReleased("Touchpad");
        }

        previousTouchpad = currentTouchpad;
    }

    /// <summary>
    /// Compares current and previous button states and logs only when the state changes.
    /// </summary>
    internal void CheckButtonState(string buttonName, bool currentState, ref bool previousState)
    {
        if (currentState && !previousState)
        {
            ControllerDebugLogger.LogPressed(buttonName);
        }
        else if (!currentState && previousState)
        {
            ControllerDebugLogger.LogReleased(buttonName);
        }

        previousState = currentState;
    }

    /// <summary>
    /// Returns the display name for a button based on the connected controller type.
    /// </summary>
    private string GetButtonName(buttonType controllerButton)
    {
        var gamepad = ControllerManager.GetPhysicalPad();

        if (gamepad is XInputController)
        {
            return controllerButton switch
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
                _ => controllerButton.ToString()
            };
        }

        if (gamepad is DualShockGamepad || gamepad is DualSenseGamepadHID)
        {
            return controllerButton switch
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
                _ => controllerButton.ToString()
            };
        }

        return controllerButton.ToString();
    }

    /// <summary>
    /// Returns the display name for a trigger based on the connected controller type.
    /// </summary>
    private string GetTriggerName(triggerType controllerTrigger)
    {
        var gamepad = ControllerManager.GetPhysicalPad();

        if (gamepad is XInputController)
        {
            return controllerTrigger switch
            {
                triggerType.Left => "LT",
                triggerType.Right => "RT",
                _ => controllerTrigger.ToString()
            };
        }

        if (gamepad is DualShockGamepad)
        {
            return controllerTrigger switch
            {
                triggerType.Left => "L2",
                triggerType.Right => "R2",
                _ => controllerTrigger.ToString()
            };
        }

        return controllerTrigger.ToString();
    }

    /// <summary>
    /// Returns the display name for a joystick.
    /// </summary>
    private string GetJoystickName(joystickType controllerJoystick)
    {
        return controllerJoystick switch
        {
            joystickType.Left => "Left",
            joystickType.Right => "Right",
            _ => controllerJoystick.ToString()
        };
    }
}

[InitializeOnLoad]
public static class ControllerComponentsAutoRunner
{
    private static ControllerComponents components;

    /// <summary>
    /// Starts automatic input reading and logging when the Unity Editor loads.
    /// </summary>
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

    /// <summary>
    /// Resets input tracking when Unity switches between edit mode and play mode.
    /// </summary>
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

    /// <summary>
    /// Reads physical controller input into the shared Controller state after Input System updates.
    /// </summary>
    private static void ReadInput()
    {
        ControllerInputReader.ReadPhysicalInputIntoController();
    }

    /// <summary>
    /// Checks for controller activity changes and sends logs when needed.
    /// </summary>
    private static void Update()
    {
        if (components == null)
        {
            components = new ControllerComponents();
        }

        components.GetJoystickActivity();
        components.GetTriggerActivity();
        components.GetButtonActivity();
        components.GetTouchpadActivity();
    }
}