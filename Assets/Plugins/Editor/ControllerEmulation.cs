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
using Unity.Collections;
// using System.Numerics;

/// <summary>
/// An example plugin.
/// </summary>
public class ControllerEmulation : EditorWindow
{
    public string controllerType = "";
    private Gamepad emulator;
    private bool mouseDrag = false;
    private bool inLeftJoystick = false;
    private bool inRightJoystick = false;
    private Vector2 leftStickValues = Vector2.zero;
    private Vector2 rightStickValues = Vector2.zero;

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

        // mouse tracking for joysticks
        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;

        Rect area = GUILayoutUtility.GetRect(300, 120);
        float joystickRadius = 50;
        float joystickStickRadius = 30;
        float marginBetweenJoysticks = 30;
        Vector2 leftJoystickCenter = new Vector2(area.center.x - (joystickRadius + marginBetweenJoysticks), area.center.y);
        Vector2 leftJoystickStick = leftJoystickCenter;

        Vector2 rightJoystickCenter = new Vector2(area.center.x + (joystickRadius + marginBetweenJoysticks), area.center.y);
        Vector2 rightJoystickStick = rightJoystickCenter;
        
        // draw joystick pads
        Handles.color = Color.green;
        Handles.DrawSolidDisc(leftJoystickCenter, Vector3.forward, joystickRadius);
        Handles.DrawSolidDisc(rightJoystickCenter, Vector3.forward, joystickRadius);

        // check mouse clicked in which joystick pad
        if (e.type == EventType.MouseDown)
        {
            if (Vector2.Distance(mousePos, leftJoystickCenter) < joystickRadius)
            {
                inLeftJoystick = true;
                inRightJoystick = false;
            }
            if (Vector2.Distance(mousePos, rightJoystickCenter) < joystickRadius)
            {
                inLeftJoystick = false;
                inRightJoystick = true;
            }
        }

        // mouse dragging joystick stick
        if (e.type == EventType.MouseDrag)
        {
            mouseDrag = true;
        }
        if (e.type == EventType.MouseUp)
        {
            mouseDrag = false;
            inLeftJoystick = false;
            inRightJoystick = false;
            leftStickValues = Vector2.zero;
            rightStickValues = Vector2.zero;
        }
        
        // moving joystick stick
        if (mouseDrag && inLeftJoystick)
        {
            // distance normal for radius from center
            float normalizeValue = joystickRadius / Vector2.Distance(mousePos, leftJoystickCenter);
            // set joystick stick to right distance from joystick pad
            if (Vector2.Distance(mousePos, leftJoystickCenter) > joystickRadius)
            {
                // get distance from center
                Vector2 normMousePos = mousePos - leftJoystickCenter;
                // normalize distance from center to be at most radius
                normMousePos.x *= normalizeValue;
                normMousePos.y *= normalizeValue;

                leftJoystickStick = leftJoystickCenter + normMousePos;
            }
            else
            {
                leftJoystickStick = mousePos;
            }
            // get joystick values
            leftStickValues = (leftJoystickStick - leftJoystickCenter) / joystickRadius;
            leftStickValues.y *= -1;
        }
        if (mouseDrag && inRightJoystick)
        {
            // distance normal for radius from center
            float normalizeValue = joystickRadius / Vector2.Distance(mousePos, rightJoystickCenter);
            // set joystick stick to right distance from joystick pad
            if (Vector2.Distance(mousePos, rightJoystickCenter) > joystickRadius)
            {
                // get distance from center
                Vector2 normMousePos = mousePos - rightJoystickCenter;
                // normalize distance from center to be at most radius
                normMousePos.x *= normalizeValue;
                normMousePos.y *= normalizeValue;

                rightJoystickStick = rightJoystickCenter + normMousePos;
            }
            else
            {
                rightJoystickStick = mousePos;
            }
            // get joystick values
            rightStickValues = (rightJoystickStick - rightJoystickCenter) / joystickRadius;
            rightStickValues.y *= -1;
        }

        // draw joystick sticks
        Handles.color = Color.blue;
        Handles.DrawSolidDisc(leftJoystickStick, Vector3.forward, joystickStickRadius);
        Handles.DrawSolidDisc(rightJoystickStick, Vector3.forward, joystickStickRadius);

        // gamepad states for emulated gamepad
        InputSystem.QueueStateEvent(gamepad, new GamepadState
        {
            buttons = buttonsPressed,
            leftTrigger = leftTriggerPressBool,
            rightTrigger = rightTriggerPressBool,
            leftStick = leftStickValues,
            rightStick = rightStickValues,
        });

    }

    // refreshes window every tick
    void Update()
    {
        Repaint();
    }
}