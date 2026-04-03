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
    static Dictionary<buttonType, bool> buttons = new();
    static Dictionary<triggerType, float> triggers = new();
    static Dictionary<joystickType, Vector2> joysticks = new();

    static Controller()  // Constructor
    {
        foreach (buttonType button in System.Enum.GetValues(typeof(buttonType)))
        {
            buttons[button] = false;
        }
        foreach (triggerType trigger in System.Enum.GetValues(typeof(triggerType)))
        {
            triggers[trigger] = 0.0f;
        }
        foreach (joystickType joystick in System.Enum.GetValues(typeof(joystickType)))
        {
            joysticks[joystick] = new Vector2(0.0f, 0.0f);
        }
    }
}

/*
public struct Joysticks
{
    public Vector2 Left;
    public Vector2 Right;
}

public struct Triggers
{
    public float Left;
    public struct Right;
}
*/