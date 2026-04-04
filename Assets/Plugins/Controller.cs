/*******************************************************
* Script:      Controller.cs
* Author(s):   Nicholas Johnson (Add yourselves to this!)
* 
* Description:
*    A controller object that is the centralized source of truth
*    for input states.
*******************************************************/
using System.Numerics;
using System.Collections.Generic;

enum buttonType { Y, B, A, X, Up, Down, Left, Right, LBumper, RBumper, Advanced }
enum triggerType { Left, Right }
enum joystickType { Left, Right }

public static class Controller
{
    static Dictionary<buttonType, bool> Buttons {get; set;} = new();
    static Dictionary<triggerType, float> Triggers {get; set;} = new();
    static Dictionary<joystickType, Vector2> Joysticks {get; set;} = new();

    static Controller()  // Constructor
    {
        foreach (buttonType button in System.Enum.GetValues(typeof(buttonType)))
        {
            Buttons[button] = false;
        }
        foreach (triggerType trigger in System.Enum.GetValues(typeof(triggerType)))
        {
            Triggers[trigger] = 0.0f;
        }
        foreach (joystickType joystick in System.Enum.GetValues(typeof(joystickType)))
        {
            Joysticks[joystick] = new Vector2(0.0f, 0.0f);
        }
    }
}

class Button
{
    public bool pressed;

    /* Function on pressed */
}

class Joystick
{
    public bool pressed;
    Vector2 value;

    /* Function on move */
}

class Trigger
{
    public float pressure;

    /* Function on pressed */
}
