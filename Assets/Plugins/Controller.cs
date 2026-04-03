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

struct Button
{
    bool pressed;

    public Button()
    {
        pressed = false;
    }
    /* Function on pressed */
}

struct Joystick
{
    bool pressed;
    Vector2 value;

    public Joystick()
    {
        pressed = false;
        value = new Vector2(0.0, 0.0);
    }
    /* Function on move */
}

struct Trigger
{
    float pressure;

    public Trigger()
    {
        pressure = 0.0f;
    }
    /* Function on pressed */
}
