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

public enum buttonType { Y, B, A, X, Up, Down, Left, Right, LBumper, RBumper, Advanced }
public enum triggerType { Left, Right }
public enum joystickType { Left, Right }

public static class XboxController
{
    public static Dictionary<buttonType, Button> Buttons {get; private set;} = new();
    public static Dictionary<triggerType, Trigger> Triggers {get; private set;} = new();
    public static Dictionary<joystickType, Joystick> Joysticks {get; private set;} = new();

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
