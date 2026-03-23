/*******************************************************
* Script:      ControllerDetection.cs
* Author(s):   Senny Lu (Add yourselves to this!)
* 
* Description:
*    
*******************************************************/

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// An example plugin.
/// </summary>
public class ControllerDectection : EditorWindow
{
    public string controllerType = "";
    private Gamepad emulator;

    [MenuItem("Tools/DIPT/ControllerDetection")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ControllerDectection));
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

        foreach (var control in gamepad.allControls)
        {
            if (control is ButtonControl){
                GamepadButton gamepadButton;
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
                    buttonsPressed |= 1u << (int)gamepadButton;
                }
            }
        }
        
        InputSystem.QueueStateEvent(gamepad, new GamepadState
        {
            buttons = buttonsPressed
        });
    }
}