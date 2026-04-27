/*******************************************************
* Script:      ControllerManager.cs
* Author:      Jarrett Williams
* Description:
*    Finds and separates physical controller devices from
*    the virtual emulator device. This helps the input
*    visualizer decide which controller UI to load.
*******************************************************/

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public static class ControllerManager
{
    public static Gamepad GetPhysicalPad()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad is XInputController)
            {
                return gamepad;
            }

            if (gamepad is DualShockGamepad)
            {
                return gamepad;
            }

            if (gamepad is DualSenseGamepadHID)
            {
                return gamepad;
            }
        }

        return null;
    }

    public static Gamepad GetArtificialPad()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (!(gamepad is XInputController) &&
                !(gamepad is DualShockGamepad) &&
                !(gamepad is DualSenseGamepadHID))
            {
                return gamepad;
            }
        }

        return null;
    }

    public static Gamepad GetActivePad()
    {
        return GetArtificialPad() ?? GetPhysicalPad();
    }
}