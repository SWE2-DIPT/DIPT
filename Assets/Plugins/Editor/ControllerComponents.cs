using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    // Joysticks
    private Vector2 rightJoystick, leftJoystick;
    // Triggers
    private float rightTrigger, leftTrigger;

    private bool bottomFaceButton, leftFaceButton, rightFaceButton, topFaceButton;
    private bool rightBumper, leftBumper;

    // D-Pad
    private bool dpadUp, dpadDown, dpadLeft, dpadRight;

    private bool prevBottomFaceButton, prevLeftFaceButton, prevRightFaceButton, prevTopFaceButton;
    private bool prevRightBumper, prevLeftBumper;

    private Vector2 prevLeftJoystick, prevRightJoystick;
    private float prevLeftTrigger, prevRightTrigger;

    private const float joystickThreshold = 0.10f;
    private const float triggerThreshold = 0.05f;

    public void GetJoystickActivity()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
            return;

        leftJoystick = gamepad.leftStick.ReadValue();
        rightJoystick = gamepad.rightStick.ReadValue();

        if (Vector2.Distance(leftJoystick, prevLeftJoystick) > joystickThreshold)
        {
            ControllerDebugLogger.LogMovement(
                $"Left Joystick moved to X:{leftJoystick.x:F2} | Y:{leftJoystick.y:F2}"
            );
            prevLeftJoystick = leftJoystick;
        }
    }

    public void GetTriggerActivity()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
            return;

        leftTrigger = gamepad.leftTrigger.ReadValue();
        rightTrigger = gamepad.rightTrigger.ReadValue();

        if (Mathf.Abs(leftTrigger - prevLeftTrigger) > triggerThreshold)
        {
            ControllerDebugLogger.LogMovement($"Left Trigger changed to {leftTrigger:F2}");
            prevLeftTrigger = leftTrigger;
        }

        if (Mathf.Abs(rightTrigger - prevRightTrigger) > triggerThreshold)
        {
            ControllerDebugLogger.LogMovement($"Right Trigger changed to {rightTrigger:F2}");
            prevRightTrigger = rightTrigger;
        }
    }

    public void GetButtonActivity()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
            return;

        bottomFaceButton = gamepad.buttonSouth.isPressed;
        leftFaceButton = gamepad.buttonWest.isPressed;
        rightFaceButton = gamepad.buttonEast.isPressed;
        topFaceButton = gamepad.buttonNorth.isPressed;
        rightBumper = gamepad.rightShoulder.isPressed;
        leftBumper = gamepad.leftShoulder.isPressed;

        dpadUp = gamepad.dpad.up.isPressed;
        dpadDown = gamepad.dpad.down.isPressed;
        dpadLeft = gamepad.dpad.left.isPressed;
        dpadRight = gamepad.dpad.right.isPressed;


        CheckButtonState("Bottom Face Button", bottomFaceButton, ref prevBottomFaceButton);
        CheckButtonState("Left Face Button", leftFaceButton, ref prevLeftFaceButton);
        CheckButtonState("Right Face Button", rightFaceButton, ref prevRightFaceButton);
        CheckButtonState("Top Face Button", topFaceButton, ref prevTopFaceButton);
        CheckButtonState("Right Bumper", rightBumper, ref prevRightBumper);
        CheckButtonState("Left Bumper", leftBumper, ref prevLeftBumper);

        
    }

    private void CheckButtonState(string buttonName, bool currentState, ref bool previousState)
    {
        if (currentState && !previousState)
        {
            ControllerDebugLogger.LogPressed($"{buttonName} pressed");
        }
        else if (!currentState && previousState)
        {
            ControllerDebugLogger.LogReleased($"{buttonName} released");
        }

        previousState = currentState;
    }

    public void GetComponentState(bool buttonPressed, VisualElement element, string state)
    {
        if (element == null)
            return;
        if(buttonPressed)
        {
            element.AddToClassList(state);
        }
        else
        {
            element.RemoveFromClassList(state);
        }
    }

    public float GetRightTrigger()
    {
        return rightTrigger;
    }

    public float GetLeftTrigger()
    {
        return leftTrigger;
    }

    public bool GetBottomFaceButton()
    {
        return bottomFaceButton;
    }
    public bool GetUpFaceButton()
    {
        return topFaceButton;
    }
    public bool GetRightFaceButton()
    {
        return rightFaceButton;
    }
    public bool GetLeftFaceButton()
    {
        return leftFaceButton;
    }
    
    public bool GetRightBumper()
    {
        return rightBumper;
    }

    public bool GetLeftBumper()
    {
        return leftBumper;
    }

    public bool GetDpadUp()
    {
        Debug.Log("up dpad");
        return dpadUp;
    }

    public bool GetDpadDown()
    {
        Debug.Log("down dpad");
        return dpadDown;
    }

    public bool GetDpawRight()
    {
        Debug.Log("right dpad");
        return dpadRight;
    }

    public bool GetDpadLeft()
    {
        Debug.Log("left dpad");
        return dpadLeft;
    }

    public void SetBottomFaceButton(bool value)
    {
        bottomFaceButton = value;
    }

    public void SetTopFaceButton(bool value)
    {
        topFaceButton = value;
    }

    public void SetRightFaceButton(bool value)
    {
        rightFaceButton = value;
    }

    public void SetLeftFaceButton(bool value)
    {
        leftFaceButton = value;
    }

    public void SetRightBumper(bool value)
    {
        rightBumper = value;
    }

    public void SetLeftBumper(bool value)
    {
        leftBumper = value;
    }
}
