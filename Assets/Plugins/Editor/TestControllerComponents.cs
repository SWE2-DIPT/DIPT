using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestControllerComponents
{
    private TestControllerConnectivity ControllerConnectivity;
    public InputDevice gamepad_connected;

    private Vector2 m_LeftJoystick, m_rightJoyStick;
    public TestControllerComponents()
    {
        ControllerConnectivity = new TestControllerConnectivity();
    }
   
    public void controller_joystick_value()
    {
        var gamepad = InputSystem.AddDevice<Gamepad>();

        m_LeftJoystick = gamepad.leftStick.value;
        m_rightJoyStick = gamepad.rightStick.value;
       
    }
}
