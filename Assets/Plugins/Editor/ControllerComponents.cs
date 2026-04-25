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
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

public class ControllerComponents
{
    private Dictionary<ButtonControl, bool> prevButtons = new();
    private Dictionary<ButtonControl, float> prevTriggers = new();
    private Dictionary<StickControl, Vector2> prevJoysticks = new();

    private bool prevTouchpad;

    private const float joystickThreshold = 0.10f;
    private const float triggerThreshold = 0.05f;

    private ControllerManager manager;

    public ControllerComponents()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            return;
        }
        ButtonControl[] buttons =
        {
            gamepad.buttonSouth,
            gamepad.buttonNorth,
            gamepad.buttonEast,
            gamepad.buttonWest,
            gamepad.startButton,
            gamepad.selectButton,
            gamepad.leftShoulder,
            gamepad.rightShoulder,
            gamepad.leftStickButton,
            gamepad.rightStickButton,
            gamepad.dpad.up,
            gamepad.dpad.down,
            gamepad.dpad.left,
            gamepad.dpad.right
        };
        foreach (ButtonControl button in buttons)
            prevButtons[button] = false;

        ButtonControl[] triggers = {gamepad.leftTrigger, gamepad.rightTrigger};
        foreach (ButtonControl trigger in triggers)
        {
            prevTriggers[trigger] = 0f;
        }

        StickControl[] joysticks = {gamepad.leftStick, gamepad.rightStick};
        foreach (StickControl joystick in joysticks)
        {
            prevJoysticks[joystick] = Vector2.zero;
        }

        prevTouchpad = false;
    }

    public void GetJoystickActivity()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            return;
        }
        StickControl[] joysticks = {gamepad.leftStick, gamepad.rightStick};
        
        foreach (StickControl joystick in joysticks)
        {
            Vector2 currentValue = joystick.value;
            // Debug.Log($"{joystick.value.x} {joystick.value.y}");
            Vector2 previousValue = prevJoysticks[joystick];

            if (Vector2.Distance(currentValue, previousValue) > joystickThreshold)
            {
                ControllerDebugLogger.LogMovement($"{joystick.name} Joystick moved to X:{currentValue.x:F2} | Y:{currentValue.y:F2}"
                );
                prevJoysticks[joystick] = currentValue;
            }
        }
    }

    public void GetTriggerActivity()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            return;
        }
        ButtonControl[] triggers = {gamepad.leftTrigger, gamepad.rightTrigger};

        foreach (ButtonControl trigger in triggers)
        {
            float currentValue = trigger.value;
            float previousValue = prevTriggers[trigger];

            if (Mathf.Abs(currentValue - previousValue) > triggerThreshold)
            {
                ControllerDebugLogger.LogMovement($"{trigger.name} Trigger changed to {currentValue:F2}"
                );
                prevTriggers[trigger] = currentValue;
            }
        }
    }

    public void GetButtonActivity()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            return;
        }
        ButtonControl[] buttons =
        {
            gamepad.buttonSouth,
            gamepad.buttonNorth,
            gamepad.buttonEast,
            gamepad.buttonWest,
            gamepad.startButton,
            gamepad.selectButton,
            gamepad.leftShoulder,
            gamepad.rightShoulder,
            gamepad.leftStickButton,
            gamepad.rightStickButton,
            gamepad.dpad.up,
            gamepad.dpad.down,
            gamepad.dpad.left,
            gamepad.dpad.right
        };
        
        foreach (ButtonControl button in buttons)
        {
            bool currentState = button.isPressed;
            bool previousState = prevButtons[button];

            CheckButtonState(button.name, currentState, ref previousState);
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
        manager = new ControllerManager();

        if(manager.GetPhysicalPad() is XInputController)
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
                buttonType.LeftStick => "LS",
                buttonType.RightStick => "RS",
                _ => button.ToString()
            };  
        }
        else if(manager.GetPhysicalPad() is DualShockGamepad)
        {
            return button switch
            {
                buttonType.A => "Cross Button",
                buttonType.B => "Circle Button",
                buttonType.X => "Square Button",
                buttonType.Y => "Trianlge Button",
                buttonType.Up => "DPad Up",
                buttonType.Down => "DPad Down",
                buttonType.Left => "DPad Left",
                buttonType.Right => "DPad Right",
                buttonType.LBumper => "L1",
                buttonType.RBumper => "R1",
                buttonType.Xbox => "Xbox Button",
                buttonType.Menu => "Menu Button",
                buttonType.View => "View Button",
                buttonType.Share => "Share Button",
                buttonType.Advanced => "Advanced Button",
                buttonType.LeftStick => "L3",
                buttonType.RightStick => "R3",
                _ => button.ToString()
            };  
        }

        return button switch
        {
            buttonType.A => "South face Button",
            buttonType.B => "East face Button",
            buttonType.X => "West face Button",
            buttonType.Y => "North face Button",
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
            buttonType.LeftStick => "LS",
            buttonType.RightStick => "RS",
            _ => button.ToString()
        };  
    }

    private string GetTriggerName(triggerType trigger)
    {

        manager = new ControllerManager();

        if(manager.GetPhysicalPad() is XInputController)
        {
            return trigger switch
            {
                triggerType.Left => "LT",
                triggerType.Right => "RT",
                _ => trigger.ToString()
            };
        }

        else if(manager.GetPhysicalPad() is DualShockGamepad)
        {
            return trigger switch
            {
                triggerType.Left => "L3",
                triggerType.Right => "R3",
                _ => trigger.ToString()
            };
        }

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