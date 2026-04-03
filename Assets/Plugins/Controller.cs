/*******************************************************
* Script:      Controller.cs
* Author(s):   Nicholas Johnson (Add yourselves to this!)
* 
* Description:
*    A controller object that is the centralized source of truth
*    for input states.
*******************************************************/
using System.Numerics;

enum buttonType { Y, B, A, X, Up, Down, Left, Right, LBumper, RBumper }

public static class Controller
{
    static Dictionary<buttonType, bool> buttons = new();

    static Controller()
    {
        foreach (buttonType button in System.Enum.GetValues(typeof(buttonType)))
        {
            buttons[button] = false;
        }
    }
}

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