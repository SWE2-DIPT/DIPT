using System.Security.Policy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class ControllerComponents
{
    
    private Vector2 RightJoystick, LeftJoystick;
    private float RightTrigger, LeftTrigger;
    private bool BottomFaceButton, LeftFaceButton, RightFaceButton, TopFaceButton;
    private bool RightBumper, LeftBumper;
    private ControllerManager manager;
    
    public ControllerComponents()
    {
        manager = new ControllerManager();
    }

    public void GetJoystickActivity()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.Log("gamepad disconnected");
            return;
        }

        LeftJoystick = gamepad.leftStick.ReadValue();
        RightJoystick = gamepad.rightStick.ReadValue();

        Debug.Log($"Left Joystick: X:{LeftJoystick.x} | Y:{LeftJoystick.y}");
        Debug.Log($"Right Joystick: X:{RightJoystick.x} | Y:{RightJoystick.y}");
    }

    public void GetTriggerActivity()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.Log("gamepad disconnected");
            return;
        }

        RightTrigger = gamepad.rightTrigger.ReadValue();
        LeftTrigger = gamepad.leftTrigger.ReadValue();

        Debug.Log($"Right Trigger: {RightTrigger}");
        Debug.Log($"Left Trigger: {LeftTrigger}");
    }

    public void GetButtonActivity()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.Log("gamepad disconnected");
            return;
        }

        BottomFaceButton = gamepad.aButton.IsPressed();
        LeftFaceButton = gamepad.xButton.IsPressed();
        RightFaceButton = gamepad.bButton.IsPressed();
        TopFaceButton = gamepad.yButton.IsPressed();
        
        if (BottomFaceButton)
            Debug.Log("Bottom face button is pressed");
        else if (LeftFaceButton)
            Debug.Log("Left face button is pressed");
        else if (RightFaceButton)
            Debug.Log("Right face button is pressed");
        else if (TopFaceButton)
            Debug.Log("Top face button is pressed");
        else
            Debug.Log("no buttons have been pressed");

        RightBumper = gamepad.rightShoulder.IsPressed();
        LeftBumper = gamepad.leftShoulder.IsPressed();

        if (RightBumper)
            Debug.Log("Right Bumper button is pressed");
        if (LeftBumper)
            Debug.Log("Left Bumper button is pressed");

    }
}
