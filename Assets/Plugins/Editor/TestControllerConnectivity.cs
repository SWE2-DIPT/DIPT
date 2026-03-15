using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput; //XInput is for xbox


public class TestControllerConnectivity
{
    private bool controller_connected = false;

    private InputDevice current_gamepad;
    public bool check_gamepad()
    {
        foreach(var gamepad in InputSystem.devices)
        {
            if(gamepad is DualShockGamepad)
            {
                Debug.Log("playstation controller detected: " + gamepad.description);
                return controller_connected = true;
            }
            if(gamepad is XInputController)
            {
                Debug.Log("Xbox controller detected: " + gamepad.description);
                return controller_connected = true;
            }
        }

        Debug.Log("No supported controllers connected");

        return controller_connected;
    }

    public InputDevice get_current_gamepad() 
    {
        return Gamepad.current; 
    }

}
