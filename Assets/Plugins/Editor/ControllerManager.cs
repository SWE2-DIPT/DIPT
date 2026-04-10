using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput; //XInput is for xbox


public class ControllerManager
{
    private bool controller_connected;
    public Gamepad current_gamepad;

    public bool check_gamepad()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad is DualShockGamepad)
            {
                current_gamepad = gamepad;
                Debug.Log("PlayStation controller detected");
                controller_connected = true;
                return true;
            }

            if (gamepad is XInputController)
            {
                current_gamepad = gamepad;
                Debug.Log("Xbox controller detected");
                controller_connected = true;
                return true;
            }
        }

        current_gamepad = null;
        controller_connected = false;

        Debug.Log("No supported controllers connected");

        return false;
    }

    public Gamepad get_gamepad()
    {
        return current_gamepad;
    }

    public bool is_gamepad_connected()
    {
        return controller_connected;
    }
}
