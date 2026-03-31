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
     /*10*/ "LeftShoulder", 
     /*11*/ "RightShoulder", 
     /*12*/ "Start", 
     /*13*/ "Select"};

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
        emulator.releaseAllButtons();

        foreach (var button in buttonNames)
        {
            if (GUILayout.RepeatButton(button))
            {
                emulator.pressButton(button);
            }
        }

        // mouse tracking for joysticks and analog
        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;

        // mouse dragging
        if (e.type == EventType.MouseDown)
        {
            mouseDrag = true;
        }
        // mouse release
        if (e.type == EventType.MouseUp)
        {
            mouseDrag = false;
            inLeftJoystick = false;
            inRightJoystick = false;
            inLeftTrigger = false;
            inRightTrigger = false;
            emulator.clear();
        }
        
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
        Rect activeRightTriggerBox = rightTriggerBox;

        // check mouse clicked in which analog pad
        if (e.type == EventType.MouseDown)
        {
            if (leftTriggerBox.Contains(mousePos))
            {
                inLeftTrigger = true;
                inRightTrigger = false;
            }
            if (rightTriggerBox.Contains(mousePos))
            {
                inLeftTrigger = false;
                inRightTrigger = true;
            }
        }
        
        if (mouseDrag && inLeftTrigger) // add mouse click
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
        if (mouseDrag && inRightTrigger) // add mouse click
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
        GUI.color = Color.blue;
        GUI.Box(activeLeftTriggerBox,"");
        GUI.Box(activeRightTriggerBox,"");


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
        
        // moving joystick stick
        if (mouseDrag && inLeftJoystick)// add mouse click
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

        if (mouseDrag && inRightJoystick)// add mouse click
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