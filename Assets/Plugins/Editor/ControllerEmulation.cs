/*******************************************************
* Script:      ControllerEmulation.cs
* Author(s):   Senny Lu
* 
* Description:
*    Controller Emulation for gamepad
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


public class ControllerEmulation : EditorWindow
{
    private GamepadEmulator emulator;
    private bool mouseDrag = false;
    private bool mouseDown = false;
    private bool inLeftJoystick = false;
    private bool inRightJoystick = false;
    private bool inLeftTrigger = false;
    private bool inRightTrigger = false;
    private string[] buttonNames = 
    {/*0*/ "DpadUp",
     /*1*/ "DpadDown", 
     /*2*/ "DpadLeft", 
     /*3*/ "DpadRight", 
     /*4*/ "North", 
     /*5*/ "East", 
     /*6*/ "South", 
     /*7*/ "West", 
     /*8*/ "LeftStick", 
     /*9*/ "RightStick",
     ///*10*/ "LeftStickButton",
     ///*11*/ "RightStickButton",
     /*12*/ "LeftShoulder", 
     /*13*/ "RightShoulder", 
     /*14*/ "LeftTrigger",
     /*15*/ "RightTrigger",
     /*16*/ "Start", 
     /*17*/ "Select"
    };

    [MenuItem("Tools/DIPT/ControllerEmulation")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ControllerEmulation));
    }

    void OnEnable()
    {
        emulator = new GamepadEmulator();
    }
    void OnDisable()
    {
        emulator.dispose();
    }

    void OnGUI()
    {
        emulator.clear();
        
        // if (GUILayout.RepeatButton("Press All Buttons"))
        //     {
        //         emulator.pressAllButtons();
        //     }

        // // create a button for each controller button input
        // foreach (var button in buttonNames)
        // {
        //     if (GUILayout.RepeatButton(button))
        //     {
        //         emulator.pressButton(button);
        //     }
        // }

        // mouse tracking for joysticks and analog
        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;

        // check mouse button is down
        // if (Mouse.current == null || !Mouse.current.leftButton.isPressed)
        // {
        //     mouseDown = false;
        // }
        if (e.type == EventType.MouseDown && mouseDown)
        {
            mouseDown = false;
        }

        // check mouse dragging
        if (e.type == EventType.MouseDown)
        {
            mouseDrag = true;
            mouseDown = true;
        }

        // check mouse release
        if (mouseDrag && !mouseDown)
        {
            mouseDrag = false;
            inLeftJoystick = false;
            inRightJoystick = false;
            inLeftTrigger = false;
            inRightTrigger = false;
            emulator.clear();
        }
        
        // analog triggers area creation
        Rect analogArea = GUILayoutUtility.GetRect(300, 120);
        float marginBetweenAnalog = 30;
        float analogWidth = 100;
        float analogHeight = 100;
        float topOfTriggerBoxes = analogArea.center.y - 50;
        float bottomOfTriggerBoxes = topOfTriggerBoxes + analogHeight;
        Rect leftTriggerBox = new Rect(analogArea.center.x - (marginBetweenAnalog + analogWidth), topOfTriggerBoxes, analogWidth, analogHeight);
        Rect rightTriggerBox = new Rect(analogArea.center.x + marginBetweenAnalog, topOfTriggerBoxes, analogWidth, analogHeight);
        GUI.Box(leftTriggerBox,"Left Trigger");
        GUI.Box(rightTriggerBox,"Right Trigger");

        Rect activeLeftTriggerBox = leftTriggerBox;
        activeLeftTriggerBox.height = 0;
        Rect activeRightTriggerBox = rightTriggerBox;
        activeRightTriggerBox.height = 0;

        // joystick area creation
        Rect joystickArea = GUILayoutUtility.GetRect(300, 120);
        float joystickRadius = 50;
        float joystickStickRadius = 30;
        float marginBetweenJoysticks = 30;
        Vector2 leftJoystickCenter = new Vector2(joystickArea.center.x - (joystickRadius + marginBetweenJoysticks), joystickArea.center.y);
        Vector2 leftJoystickStick = leftJoystickCenter;

        Vector2 rightJoystickCenter = new Vector2(joystickArea.center.x + (joystickRadius + marginBetweenJoysticks), joystickArea.center.y);
        Vector2 rightJoystickStick = rightJoystickCenter;
        
        // draw joystick pads
        Handles.color = Color.green;
        Handles.DrawSolidDisc(leftJoystickCenter, Vector3.forward, joystickRadius);
        Handles.DrawSolidDisc(rightJoystickCenter, Vector3.forward, joystickRadius);


        // check location of mouse click in which area
        if (e.type == EventType.MouseDown)
        {
            inLeftJoystick = false;
            inRightJoystick = false;
            inLeftTrigger = false;
            inRightTrigger = false;

            if (Vector2.Distance(mousePos, leftJoystickCenter) < joystickRadius)
            {
                inLeftJoystick = true;
            }
            if (Vector2.Distance(mousePos, rightJoystickCenter) < joystickRadius)
            {
                inRightJoystick = true;
            }
            if (leftTriggerBox.Contains(mousePos))
            {
                inLeftTrigger = true;
            }
            if (rightTriggerBox.Contains(mousePos))
            {
                inRightTrigger = true;
            }
        }

        // moving bar if in left trigger
        if (mouseDrag && inLeftTrigger)
        {
            float relativeMouseY = mousePos.y - topOfTriggerBoxes;
            if (mousePos.y < bottomOfTriggerBoxes && mousePos.y > topOfTriggerBoxes)
            {
                activeLeftTriggerBox.y = mousePos.y;
                activeLeftTriggerBox.height = analogHeight - relativeMouseY;
            } else if (mousePos.y > bottomOfTriggerBoxes)
            {
                activeLeftTriggerBox.height = 0;
            } else if (mousePos.y < topOfTriggerBoxes)
            {
                activeLeftTriggerBox.y = topOfTriggerBoxes;
                activeLeftTriggerBox.height = analogHeight;
            }
            emulator.pressLeftTrigger(activeLeftTriggerBox.height / analogHeight);
        }

        // moving bar if in right trigger
        if (mouseDrag && inRightTrigger)
        {
            float relativeMouseY = mousePos.y - topOfTriggerBoxes;
            if (mousePos.y < bottomOfTriggerBoxes && mousePos.y > topOfTriggerBoxes)
            {
                activeRightTriggerBox.y = mousePos.y;
                activeRightTriggerBox.height = analogHeight - relativeMouseY;
            } else if (mousePos.y > bottomOfTriggerBoxes)
            {
                activeRightTriggerBox.height = 0;
            } else if (mousePos.y < topOfTriggerBoxes)
            {
                activeRightTriggerBox.y = topOfTriggerBoxes;
                activeRightTriggerBox.height = analogHeight;
            }
            emulator.pressRightTrigger(activeRightTriggerBox.height / analogHeight);
        }

        // draw trigger bars
        GUI.color = Color.blue;
        GUI.Box(activeLeftTriggerBox,"");
        GUI.Box(activeRightTriggerBox,"");

        
        // moving joystick stick if in left joystick
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
            Vector2 move = (leftJoystickStick - leftJoystickCenter) / joystickRadius;
            emulator.moveLeftJoystick(move.x, -move.y);
        }

        // moving joystick stick if in right joystick
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
            Vector2 move = (rightJoystickStick - rightJoystickCenter) / joystickRadius;
            emulator.moveRightJoystick(move.x, -move.y);
        }

        // draw joystick sticks
        Handles.color = Color.blue;
        Handles.DrawSolidDisc(leftJoystickStick, Vector3.forward, joystickStickRadius);
        Handles.DrawSolidDisc(rightJoystickStick, Vector3.forward, joystickStickRadius);

        // emulate gamepad states for emulated gamepad
        emulator.emulate();
    }

    // refreshes window every tick
    void Update()
    {
        Repaint();
    }
}