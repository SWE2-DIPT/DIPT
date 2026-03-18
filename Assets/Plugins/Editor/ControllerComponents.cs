using Codice.Client.Common.GameUI;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

enum buttons
{
    BottomFace,
    TopFace,
    RightFace,
    LeftFace,
    RightBumper,
    LeftBumper
};
public class ControllerComponents
{
    private Vector2 RightJoystick, LeftJoystick;
    private float RightTrigger, LeftTrigger;
    private bool current_button_active;
    private bool DpadUp, DpadDown, DpadLeft, DpadRight;
    private bool BottomFaceButton, LeftFaceButton, RightFaceButton, TopFaceButton;
    private bool RightBumper, LeftBumper;
    
    private ControllerManager manager;
    private Gamepad gamepad;
    
    public ControllerComponents()
    {
        manager = new ControllerManager();

        gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.Log("gamepad disconnected");
            return;
        }
    }

    public void GetJoystickActivity()
    { 

        LeftJoystick = gamepad.leftStick.ReadValue();
        RightJoystick = gamepad.rightStick.ReadValue();

        Debug.Log($"Left Joystick: X:{LeftJoystick.x} | Y:{LeftJoystick.y}");
        Debug.Log($"Right Joystick: X:{RightJoystick.x} | Y:{RightJoystick.y}");

    }

    public void GetTriggerActivity()
    {
      
        RightTrigger = gamepad.rightTrigger.ReadValue();
        LeftTrigger = gamepad.leftTrigger.ReadValue();

        Debug.Log($"Right Trigger: {RightTrigger}");
        Debug.Log($"Left Trigger: {LeftTrigger}");
    }

    public void GetButtonActivity()
    {
        
        BottomFaceButton = gamepad.aButton.IsPressed();
        LeftFaceButton = gamepad.xButton.IsPressed();
        RightFaceButton = gamepad.bButton.IsPressed();
        TopFaceButton = gamepad.yButton.IsPressed();

        if (BottomFaceButton)
        {
            Debug.Log("bottom face button is pressed");
        }
        else if (LeftFaceButton)
        {
          
            Debug.Log("Left face button is pressed");
        }
        else if (RightFaceButton)
        {
            
            Debug.Log("Right face button is pressed");
        }
        else if (TopFaceButton)
        {
            current_button_active = TopFaceButton;
            Debug.Log("Top face button is pressed");
        }
        else
            Debug.Log("no buttons have been pressed");

        RightBumper = gamepad.rightShoulder.IsPressed();
        LeftBumper = gamepad.leftShoulder.IsPressed();

        if (RightBumper) Debug.Log("Right Bumper button is pressed");
        if (LeftBumper)  Debug.Log("Left Bumper button is pressed");
    }

    public void GetDpadActivity()
    {
        DpadLeft = gamepad.dpad.left.IsPressed();
        DpadRight = gamepad.dpad.right.IsPressed();
        DpadUp = gamepad.dpad.up.IsPressed();
        DpadDown = gamepad.dpad.down.IsPressed();

        if (DpadLeft) Debug.Log("Dpad Left");
        if (DpadRight) Debug.Log("Dpad right");
        if (DpadUp) Debug.Log("Dpad up");
        if (DpadDown) Debug.Log("Dpad down");
            
    }

    public Vector2 get_right_stick()
    {
        return RightJoystick;
    }
    public Vector2 get_left_stick()
    {
        return LeftJoystick;
    }

    public float get_right_trigger()
    {
        return RightTrigger;
    }

    public float get_left_trigger()
    {
        return LeftTrigger;
    }

    public bool get_bottom_face_button()
    {
        return BottomFaceButton;
    }
    
    public bool get_top_face_button()
    {
        return TopFaceButton;
    }

    public bool get_right_face_button()
    {
        return RightFaceButton;
    }

    public bool get_left_face_button()
    {
        return LeftFaceButton;
    }

    public bool get_right_bumper()
    {
        return RightBumper;
    }

    public bool get_left_bumper()
    {
        return LeftBumper;
    }
  
}
