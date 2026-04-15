/*******************************************************
* Script:      EmulationFunctions.cs
* Author(s):   Senny Lu
* 
* Description:
*    Refactored Controller Emulation for controller class
*    for easy use in other files
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
using System.Configuration.Assemblies;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Linq;

// gamepadEmulator class
// creates input device Gamepad which contains all controller inputs
// has functions for creating and removing controller inputs
public class GamepadEmulator
{
    private Gamepad emulator;
    private ControllerManager manager;

    private uint buttonsPressed = 0;
    private Vector2 leftStickValues = Vector2.zero;
    private Vector2 rightStickValues = Vector2.zero;
    private float leftTriggerValue = 0f;
    private float rightTriggerValue = 0f;

    public GamepadEmulator()
    {
        manager = new ControllerManager();
        emulator = InputSystem.AddDevice<Gamepad>();

    }


    // destructor for class: removes emulator input device
    // make sure to call this
    public void dispose()
    {
        if (emulator != null)
        {
            InputSystem.RemoveDevice(emulator);
        }
    }

    // destructor for class: safety net / may not work
    ~GamepadEmulator()
    {
        dispose();
    }


    // refer to link below for button numbers and button numbers in comments of array validButtonStrings below
    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.LowLevel.GamepadButton.html
    private string[] validButtonStrings =
    {/*0*/ "DpadUp",
     /*1*/ "DpadDown", 
     /*2*/ "DpadLeft", 
     /*3*/ "DpadRight", 
     /*4*/ "Y", "Triangle", "North", 
     /*5*/ "B", "Circle", "East", 
     /*6*/ "South", "A", "Cross", 
     /*7*/ "Square", "West", "X",
     /*8*/ "LeftStick", 
     /*9*/ "RightStick", 
     /*10*/ "LeftShoulder", 
     /*11*/ "RightShoulder",
     ///*12*/ "LeftStickButton",
     ///*13*/ "RightStickButton",
     /*12*/ "LeftTrigger",
     /*13*/ "RightTrigger",
     /*14*/ "Start", 
     /*15*/ "Select"
    };

    // simulate press on button
    // param: int of button
    public void pressButton(int button) {
        if (button > 13 || button < 0)
        {
            throw new Exception("Button must be > 0 and < 13");
        }
        buttonsPressed |= 1u << button;
    }

    // simulate press all buttons
    public void pressAllButtons()
    {
        buttonsPressed = (1u << 14) - 1;
    }

    // simulate release on button
    // param: int of button
    public void releaseButton(int button) {
        if (button > 13 || button < 0)
        {
            throw new Exception("Button must be > 0 and < 13");
        }
        buttonsPressed &= ~(1u << button);
    }

    // overloaded pressButton & releaseButton for strings. 
    // Valid button strings in array validButtonStrings above (case insensitive)

    // simulate press on button
    // param: string of button name
    public void pressButton(string button)
    {
        if (!validButtonStrings.Contains(button, StringComparer.OrdinalIgnoreCase))
        {
            throw new Exception("Must be a valid button string");
        }
        buttonsPressed |= 1u << (int)Enum.Parse<GamepadButton>(button, true);
    }

    // simulate release on button
    // param: string of button name
    public void releaseButton(string button)
    {
        if (!validButtonStrings.Contains(button, StringComparer.OrdinalIgnoreCase))
        {
            throw new Exception("Must be a valid button string");
        }
        buttonsPressed &= ~(1u << (int)Enum.Parse<GamepadButton>(button, true));
    }

    public bool getButtonState(buttonType button) 
    {
        return (buttonsPressed & (1u << (int)button)) != 0;
    }

    // release all buttons
    public void releaseAllButtons()
    {
        buttonsPressed = 0;
    }


    // simulate left trigger
    // param: float of analog pressure (0 <= x <= 1)
    public void pressLeftTrigger(float value)
    {
        if (value < 0 || value > 1)
        {
            throw new Exception("Value must be > 0 and <= 1");
        }
        leftTriggerValue = value;
    }
    public void releaseLeftTrigger()
    {
        leftTriggerValue = 0;
    }

    // simulate right trigger
    // param: float of analog pressure (0 <= x <= 1)
    public void pressRightTrigger(float value)
    {
        if (value < 0 || value > 1)
        {
            throw new Exception("Value must be > 0 and <= 1");
        }
        rightTriggerValue = value;
    }
    public void releaseRightTrigger()
    {
        rightTriggerValue = 0;
    }

    public float GetTriggers(triggerType trigger)
    {
       if(trigger == triggerType.Left)
            return leftTriggerValue;
        else if(trigger == triggerType.Right)
            return rightTriggerValue;
        else
            return 0f;
    }

    // simulate left joystick: normalized to 1
    // param: float of x value & float of y value
    public void moveLeftJoystick(float x, float y)
    {
        leftStickValues = new Vector2(x, y).normalized;
    }
    public void resetLeftJoystick()
    {
        leftStickValues = Vector2.zero;
    }

    // simulate right joystick: normalized to 1
    // param: float of x value & float of y value
    public void moveRightJoystick(float x, float y)
    {
        rightStickValues = new Vector2(x, y).normalized;
    }
    public void resetRightJoystick()
    {
        rightStickValues = Vector2.zero;
    }

    public Vector2 GetJoysticks(joystickType joystick)
    {
        if(joystick == joystickType.Left)
            return leftStickValues;
        else if(joystick == joystickType.Right)
            return rightStickValues;
        else
            return Vector2.zero;
    }

    // simulate empty state of gamepad controller
    public void clear()
    {
        buttonsPressed = 0;
        leftStickValues = Vector2.zero;
        rightStickValues = Vector2.zero;
        leftTriggerValue = 0f;
        rightTriggerValue = 0f;
    }
    
    // emulates state of gamepad controller
    // make sure to call to emulate controller values
    public void emulate()
    {
        InputSystem.QueueStateEvent(emulator, new GamepadState
        {
            buttons = buttonsPressed,
            leftTrigger = leftTriggerValue,
            rightTrigger = rightTriggerValue,
            leftStick = leftStickValues,
            rightStick = rightStickValues
        });
    }

    // get gamepad emulator
    public Gamepad getGamepad()
    {
        return emulator;
    }
}