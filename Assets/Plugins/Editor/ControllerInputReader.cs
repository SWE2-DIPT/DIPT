using UnityEngine.InputSystem;

public static class ControllerInputReader
{
    public static void ReadPhysicalInputIntoXboxController()
    {
        var gamepad = Gamepad.current;

        if (gamepad == null)
            return;

        XboxController.SetButton(buttonType.A, gamepad.buttonSouth.isPressed);
        XboxController.SetButton(buttonType.B, gamepad.buttonEast.isPressed);
        XboxController.SetButton(buttonType.X, gamepad.buttonWest.isPressed);
        XboxController.SetButton(buttonType.Y, gamepad.buttonNorth.isPressed);

        XboxController.SetButton(buttonType.Up, gamepad.dpad.up.isPressed);
        XboxController.SetButton(buttonType.Down, gamepad.dpad.down.isPressed);
        XboxController.SetButton(buttonType.Left, gamepad.dpad.left.isPressed);
        XboxController.SetButton(buttonType.Right, gamepad.dpad.right.isPressed);

        XboxController.SetButton(buttonType.LBumper, gamepad.leftShoulder.isPressed);
        XboxController.SetButton(buttonType.RBumper, gamepad.rightShoulder.isPressed);

        XboxController.SetButton(buttonType.Menu, gamepad.startButton.isPressed);
        XboxController.SetButton(buttonType.View, gamepad.selectButton.isPressed);

        XboxController.SetJoystickPressed(joystickType.Left, gamepad.leftStickButton.isPressed);
        XboxController.SetJoystickPressed(joystickType.Right, gamepad.rightStickButton.isPressed);

        XboxController.SetTrigger(triggerType.Left, gamepad.leftTrigger.ReadValue());
        XboxController.SetTrigger(triggerType.Right, gamepad.rightTrigger.ReadValue());

        XboxController.SetJoystick(joystickType.Left, gamepad.leftStick.ReadValue());
        XboxController.SetJoystick(joystickType.Right, gamepad.rightStick.ReadValue());
    }
}