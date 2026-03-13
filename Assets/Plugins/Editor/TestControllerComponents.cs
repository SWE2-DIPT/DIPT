using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class TestControllerComponents
{
    private TestControllerConnectivity ControllerConnectivity;
    private InputDevice gamepad;

    public void controller_joystick_value()
    {
        gamepad = ControllerConnectivity.get_current_gamepad();
        
        
    }
}
