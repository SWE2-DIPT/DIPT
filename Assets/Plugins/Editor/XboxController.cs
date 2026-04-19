/*******************************************************
* Script:      Controller.cs
* Author(s):   Nicholas Johnson (Add yourselves to this!)
* 
* Description:
*    A controller object that is the centralized source of truth
*    for input states.
*******************************************************/
using UnityEngine;
using System.Collections.Generic;

public enum buttonType { Up, Down, Left, Right, Y, B, A, X, RightStick, LeftStick, LBumper, RBumper, Menu, View, Share, Advanced, Xbox, }
public enum triggerType { Left, Right }
public enum joystickType { Left, Right }

public static class XboxController
{

    private static Dictionary<buttonType, Button> Buttons = new();
    private static Dictionary<triggerType, Trigger> Triggers = new();
    private static Dictionary<joystickType, Joystick> Joysticks = new();

    static XboxController()  // Constructor
    {
        foreach (buttonType button in System.Enum.GetValues(typeof(buttonType)))
        {
            Buttons[button] = new Button();
        }
        foreach (triggerType trigger in System.Enum.GetValues(typeof(triggerType)))
        {
            Triggers[trigger] = new Trigger();
        }
        foreach (joystickType joystick in System.Enum.GetValues(typeof(joystickType)))
        {
            Joysticks[joystick] = new Joystick();
        }
    }
    //~GETTERS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static Button GetButton(buttonType type)
    {
        return Buttons[type];
    }

    public static Trigger GetTrigger(triggerType type)
    {
        return Triggers[type];
    }

    public static Joystick GetJoystick(joystickType type)
    {
        return Joysticks[type];
    }


    //~SETTERS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static void SetButton(buttonType type, bool pressed)
    {
        Buttons[type].SetPressed(pressed);
    }

    public static void SetTrigger(triggerType type, float value)
    {
        Triggers[type].SetPressure(value);
    }

    public static void SetJoystick(joystickType type, Vector2 value)
    {
        Joysticks[type].SetValue(value);
    }

    public static void SetJoystickPressed(joystickType type, bool pressed)
    {
        Joysticks[type].SetPressed(pressed);
    }

    public static void SetButtonEMU(buttonType type, bool pressed)
    {
        Buttons[type].SetPressed(pressed);
    }
}

public class Button
{
    public bool pressed { get; private set; }
    public void SetPressed(bool value)
    {
        pressed = value;
    }

    /* Function on pressed */
}

public class Joystick
{
    public bool pressed { get; private set; }
    public Vector2 position { get; private set; }

    public void SetPressed(bool value)
    {
        pressed = value;
    }
    public void SetValue(Vector2 value)
    {
        position = Vector2.ClampMagnitude(value, 1f);
    }

    /* Function on move */
}

public class Trigger
{
    public float pressure { get; private set; }

    public void SetPressure(float value)
    {
        pressure = Mathf.Clamp01(value);
    }

    /* Function on pressed */
}
