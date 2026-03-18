using UnityEngine;
using UnityEngine.InputSystem;

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

        if (Vector2.Distance(rightJoystick, prevRightJoystick) > joystickThreshold)
        {
            ControllerDebugLogger.LogMovement(
                $"Right Joystick moved to X:{rightJoystick.x:F2} | Y:{rightJoystick.y:F2}"
            );
            prevRightJoystick = rightJoystick;
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
}