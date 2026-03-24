/*******************************************************
* Script:      ControllerEmulation.cs
* Author(s):   Senny Lu (Add yourselves to this!)
* 
* Description:
*    Controller Emulation only for buttons for now
*    Left and right trigger are set to fully pressed 1 when button is pressed
*******************************************************/

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;
using System;

/// <summary>
/// An example plugin.
/// </summary>
public class ControllerEmulation : EditorWindow
{
    public string controllerType = "";
    private Gamepad emulator;

    [MenuItem("Tools/DIPT/ControllerEmulation")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ControllerEmulation));
    }
    
    public void FindControllerType()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            controllerType = "No Gamepads Detected";
            return;
        }
        controllerType = gamepad.layout;
    }

    void OnEnable()
    {
        emulator = InputSystem.AddDevice<Gamepad>();
    }
    void OnDisable()
    {
        InputSystem.RemoveDevice(emulator);
    }

    void OnGUI()
    {
        var gamepad = emulator;
        if (gamepad == null)
        {
            FindControllerType();
            GUILayout.Label(controllerType, EditorStyles.boldLabel);
            return;
        }
        
        FindControllerType();
        GUILayout.Label(controllerType, EditorStyles.boldLabel);

        uint buttonsPressed = 0;
        int leftTriggerPressBool = 0;
        int rightTriggerPressBool = 0;

        foreach (var control in gamepad.allControls)
        {
            GamepadButton gamepadButton;
            if (control is DpadControl)
            {
                string[] DpadControlNames = {"Up","Down","Left","Right"};
                for (int i = 0; i<4; i++)
                {
                    if (GUILayout.RepeatButton(DpadControlNames[i]))
                    {
                        gamepadButton = (GamepadButton)i;
                    }
                }
            }
            if (control is ButtonControl){
                switch (control.name)
                {
                    case "buttonNorth":
                        gamepadButton = GamepadButton.North;
                        break;
                    case "buttonSouth":
                        gamepadButton = GamepadButton.South;
                        break;
                    case "buttonEast":
                        gamepadButton = GamepadButton.East;
                        break;
                    case "buttonWest":
                        gamepadButton = GamepadButton.West;
                        break;
                    case "leftShoulder":
                        gamepadButton = GamepadButton.LeftShoulder;
                        break;
                    case "rightShoulder":
                        gamepadButton = GamepadButton.RightShoulder;
                        break;
                    case "leftTrigger":
                        gamepadButton = GamepadButton.LeftTrigger;
                        break;
                    case "rightTrigger":
                        gamepadButton = GamepadButton.RightTrigger;
                        break;
                    case "start":
                        gamepadButton = GamepadButton.Start;
                        break;
                    case "select":
                        gamepadButton = GamepadButton.Select;
                        break;
                    case "leftStickPress":
                        gamepadButton = GamepadButton.LeftStick;
                        break;
                    case "rightStickPress":
                        gamepadButton = GamepadButton.RightStick;
                        break;
                    default:
                        continue;
                }

                if (GUILayout.RepeatButton(control.displayName)){
                    // GamepadState.buttons is 32bit while gamepadButton could be 32 or 33
                    if ((int)gamepadButton < 32)
                    {
                        buttonsPressed |= 1u << (int)gamepadButton;
                    }
                    else
                    {
                        if ((int)gamepadButton == 32) leftTriggerPressBool = 1;
                        if ((int)gamepadButton == 33) rightTriggerPressBool = 1;
                    }
                }
            }
        }
        
        InputSystem.QueueStateEvent(gamepad, new GamepadState
        {
            buttons = buttonsPressed,
            leftTrigger = leftTriggerPressBool,
            rightTrigger = rightTriggerPressBool
        });
    }
}