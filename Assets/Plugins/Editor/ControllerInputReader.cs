/*******************************************************
* Script:      ControllerInputReader.cs
* 
* Description:
*    Reads input from the physical controller and stores it
*    in the shared Controller state.
*******************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public static class ControllerInputReader
{
    /// <summary>
    /// Returns the first physical gamepad and ignores the virtual emulator device.
    /// </summary>
    public static Gamepad GetPhysicalGamepad()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (!string.IsNullOrEmpty(gamepad.description.interfaceName))
            {
                return gamepad;
            }
        }

        return null;
    }

    /// <summary>
    /// Reads all physical gamepad buttons, triggers, and joysticks into the shared Controller state.
    /// </summary>
    public static void ReadPhysicalInputIntoController()
    {
        var gamepad = GetPhysicalGamepad();

        if (gamepad == null)
        {
            return;
        }

        Controller.SetPhysicalButton(buttonType.A, gamepad.buttonSouth.isPressed);
        Controller.SetPhysicalButton(buttonType.B, gamepad.buttonEast.isPressed);
        Controller.SetPhysicalButton(buttonType.X, gamepad.buttonWest.isPressed);
        Controller.SetPhysicalButton(buttonType.Y, gamepad.buttonNorth.isPressed);

        Controller.SetPhysicalButton(buttonType.Up, gamepad.dpad.up.isPressed);
        Controller.SetPhysicalButton(buttonType.Down, gamepad.dpad.down.isPressed);
        Controller.SetPhysicalButton(buttonType.Left, gamepad.dpad.left.isPressed);
        Controller.SetPhysicalButton(buttonType.Right, gamepad.dpad.right.isPressed);

        Controller.SetPhysicalButton(buttonType.LBumper, gamepad.leftShoulder.isPressed);
        Controller.SetPhysicalButton(buttonType.RBumper, gamepad.rightShoulder.isPressed);

        Controller.SetPhysicalButton(buttonType.Menu, gamepad.startButton.isPressed);
        Controller.SetPhysicalButton(buttonType.View, gamepad.selectButton.isPressed);

        Controller.SetPhysicalButton(buttonType.LeftStick, gamepad.leftStickButton.isPressed);
        Controller.SetPhysicalButton(buttonType.RightStick, gamepad.rightStickButton.isPressed);

        Controller.SetPhysicalTrigger(triggerType.Left, gamepad.leftTrigger.ReadValue());
        Controller.SetPhysicalTrigger(triggerType.Right, gamepad.rightTrigger.ReadValue());

        Controller.SetPhysicalJoystick(joystickType.Left, gamepad.leftStick.ReadValue());
        Controller.SetPhysicalJoystick(joystickType.Right, gamepad.rightStick.ReadValue());
    }
}