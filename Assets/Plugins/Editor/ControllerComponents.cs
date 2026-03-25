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
    private Vector2 rightJoystick;
    private Vector2 leftJoystick;
    private float rightTrigger;
    private float leftTrigger;

    private bool bottomFaceButton;
    private bool leftFaceButton;
    private bool rightFaceButton;
    private bool topFaceButton;
    private bool rightBumper;
    private bool leftBumper;

    private bool dpadUp;
    private bool dpadDown;
    private bool dpadLeft;
    private bool dpadRight;

    private bool prevBottomFaceButton;
    private bool prevLeftFaceButton;
    private bool prevRightFaceButton;
    private bool prevTopFaceButton;
    private bool prevRightBumper;
    private bool prevLeftBumper;

    private Vector2 prevLeftJoystick;
    private Vector2 prevRightJoystick;
    private float prevLeftTrigger;
    private float prevRightTrigger;

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
    
}
