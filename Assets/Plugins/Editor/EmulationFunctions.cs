/*******************************************************
* Script:      EmulationFunctions.cs
* Author(s):   Senny Lu
* 
* Description:
*    Creates and manages a virtual Gamepad device for the
*    controller emulator. This file stores emulated button,
*    trigger, and joystick values, updates those values when
*    the user interacts with the GUI or keyboard mapper, and
*    sends the final controller state to Unity's Input System.
*******************************************************/

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GamepadEmulator
{
    private Gamepad emulator;

    private uint buttonsPressed = 0;
    private Vector2 leftStickValues = Vector2.zero;
    private Vector2 rightStickValues = Vector2.zero;
    private float leftTriggerValue = 0f;
    private float rightTriggerValue = 0f;

    private string[] validButtonStrings =
    {
        "DpadUp",
        "DpadDown",
        "DpadLeft",
        "DpadRight",
        "Y",
        "Triangle",
        "North",
        "B",
        "Circle",
        "East",
        "South",
        "A",
        "Cross",
        "Square",
        "West",
        "X",
        "LeftStick",
        "RightStick",
        "LeftShoulder",
        "RightShoulder",
        "LeftTrigger",
        "RightTrigger",
        "Start",
        "Select"
    };

    /// <summary>
    /// Creates a virtual gamepad device through Unity's Input System.
    /// </summary>
    public GamepadEmulator()
    {
        emulator = InputSystem.AddDevice<Gamepad>();
    }

    /// <summary>
    /// Removes the virtual gamepad device from Unity's Input System.
    /// </summary>
    public void dispose()
    {
        if (emulator != null)
        {
            InputSystem.RemoveDevice(emulator);
        }
    }

    /// <summary>
    /// Attempts to remove the virtual gamepad device if this object is destroyed.
    /// </summary>
    ~GamepadEmulator()
    {
        dispose();
    }

    /// <summary>
    /// Simulates pressing a button by its GamepadButton number.
    /// </summary>
    public void pressButton(int button)
    {
        if (button > 16 || button < 0)
        {
            throw new Exception("Button must be > 0 and < 16");
        }

        buttonsPressed |= 1u << button;
    }

    /// <summary>
    /// Simulates pressing all supported controller buttons.
    /// </summary>
    public void pressAllButtons()
    {
        buttonsPressed = (1u << 15) - 1;
    }

    /// <summary>
    /// Simulates releasing a button by its GamepadButton number.
    /// </summary>
    public void releaseButton(int button)
    {
        if (button > 16 || button < 0)
        {
            throw new Exception("Button must be > 0 and < 15");
        }

        buttonsPressed &= ~(1u << button);
    }

    /// <summary>
    /// Simulates pressing a button by its string name.
    /// </summary>
    public void pressButton(string buttonName)
    {
        if (!validButtonStrings.Contains(buttonName, StringComparer.OrdinalIgnoreCase))
        {
            throw new Exception("Must be a valid button string");
        }

        buttonsPressed |= 1u << (int)Enum.Parse<GamepadButton>(buttonName, true);
    }

    /// <summary>
    /// Simulates releasing a button by its string name.
    /// </summary>
    public void releaseButton(string buttonName)
    {
        if (!validButtonStrings.Contains(buttonName, StringComparer.OrdinalIgnoreCase))
        {
            throw new Exception("Must be a valid button string");
        }

        buttonsPressed &= ~(1u << (int)Enum.Parse<GamepadButton>(buttonName, true));
    }

    /// <summary>
    /// Returns whether a specific button is currently pressed in the emulator.
    /// </summary>
    public bool getButtonState(buttonType button)
    {
        return (buttonsPressed & (1u << (int)button)) != 0;
    }

    /// <summary>
    /// Releases all currently pressed emulator buttons.
    /// </summary>
    public void releaseAllButtons()
    {
        buttonsPressed = 0;
    }

    /// <summary>
    /// Sets the emulated left trigger pressure.
    /// </summary>
    public void pressLeftTrigger(float value)
    {
        if (value < 0 || value > 1)
        {
            throw new Exception("Value must be > 0 and <= 1");
        }

        leftTriggerValue = value;
    }

    /// <summary>
    /// Resets the emulated left trigger to zero.
    /// </summary>
    public void releaseLeftTrigger()
    {
        leftTriggerValue = 0;
    }

    /// <summary>
    /// Sets the emulated right trigger pressure.
    /// </summary>
    public void pressRightTrigger(float value)
    {
        if (value < 0 || value > 1)
        {
            throw new Exception("Value must be > 0 and <= 1");
        }

        rightTriggerValue = value;
    }

    /// <summary>
    /// Resets the emulated right trigger to zero.
    /// </summary>
    public void releaseRightTrigger()
    {
        rightTriggerValue = 0;
    }

    /// <summary>
    /// Returns the current emulated trigger value.
    /// </summary>
    public float GetTriggers(triggerType trigger)
    {
        if (trigger == triggerType.Left)
        {
            return leftTriggerValue;
        }
        else if (trigger == triggerType.Right)
        {
            return rightTriggerValue;
        }
        else
        {
            return 0f;
        }
    }

    /// <summary>
    /// Sets the emulated left joystick position using normalized values.
    /// </summary>
    public void moveLeftJoystick(float xValue, float yValue)
    {
        leftStickValues = Vector2.ClampMagnitude(new Vector2(xValue, yValue), 1f);
    }

    /// <summary>
    /// Resets the emulated left joystick to the center.
    /// </summary>
    public void resetLeftJoystick()
    {
        leftStickValues = Vector2.zero;
    }

    /// <summary>
    /// Sets the emulated right joystick position using normalized values.
    /// </summary>
    public void moveRightJoystick(float xValue, float yValue)
    {
        rightStickValues = Vector2.ClampMagnitude(new Vector2(xValue, yValue), 1f);
    }

    /// <summary>
    /// Resets the emulated right joystick to the center.
    /// </summary>
    public void resetRightJoystick()
    {
        rightStickValues = Vector2.zero;
    }

    /// <summary>
    /// Returns the current emulated joystick value.
    /// </summary>
    public Vector2 GetJoysticks(joystickType joystick)
    {
        if (joystick == joystickType.Left)
        {
            return leftStickValues;
        }
        else if (joystick == joystickType.Right)
        {
            return rightStickValues;
        }
        else
        {
            return Vector2.zero;
        }
    }

    /// <summary>
    /// Clears all emulated controller values.
    /// </summary>
    public void clear()
    {
        buttonsPressed = 0;
        leftStickValues = Vector2.zero;
        rightStickValues = Vector2.zero;
        leftTriggerValue = 0f;
        rightTriggerValue = 0f;
    }

    /// <summary>
    /// Sends the current emulated controller state to the virtual gamepad.
    /// </summary>
    public void emulate()
    {
        InputSystem.QueueStateEvent(emulator, new GamepadState
        {
            buttons = buttonsPressed,
            leftTrigger = leftTriggerValue,
            rightTrigger = rightTriggerValue,
            leftStick = leftStickValues,
            rightStick = rightStickValues
        });
    }

    /// <summary>
    /// Returns the virtual gamepad device used by the emulator.
    /// </summary>
    public Gamepad getGamepad()
    {
        return emulator;
    }
}