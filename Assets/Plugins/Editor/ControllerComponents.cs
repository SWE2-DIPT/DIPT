/*******************************************************
* Script:      ControllerComponents.cs
* Author(s):   Nick Stearns, Jarrett Williams (Add yourselves to this!)
* 
* Description:
*    Tracks controller state changes using XboxController
*    and sends logs through ControllerDebugLogger.
*******************************************************/

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

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

    public ControllerComponents()
    {
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
                ControllerDebugLogger.LogMovement($"{GetJoystickName(joystick)} Joystick moved to X:{currentValue.x:F2} | Y:{currentValue.y:F2}"
                );
                prevJoysticks[joystick] = currentValue;
            }

            bool currentPressed = XboxController.GetJoystick(joystick).pressed;
            bool previousPressed = prevJoystickButtons[joystick];

            CheckButtonState($"{GetJoystickName(joystick)} Joystick Press", currentPressed, ref previousPressed);

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
                ControllerDebugLogger.LogMovement($"{GetTriggerName(trigger)} Trigger changed to {currentValue:F2}"
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
            buttonType.LBumper => "Left Bumper",
            buttonType.RBumper => "Right Bumper",
            buttonType.Xbox => "Xbox Button",
            buttonType.Menu => "Menu Button",
            buttonType.View => "View Button",
            buttonType.Share => "Share Button",
            buttonType.Advanced => "Advanced Button",
            buttonType.LeftStick => "Left Joystick Press",
            buttonType.RightStick => "Right Joystick Press",
            _ => button.ToString()
        };
    }

    private string GetTriggerName(triggerType trigger)
    {
        return trigger switch
        {
            triggerType.Left => "Left",
            triggerType.Right => "Right",
            _ => trigger.ToString()
        };
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