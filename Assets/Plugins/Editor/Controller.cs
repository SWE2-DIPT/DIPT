/*******************************************************
* Script:      Controller.cs
* 
* Description:
*    Stores the shared controller input state for the plugin.
*    Physical controller input and emulated input are stored
*    separately, then merged only when another file reads from
*    this class. This lets the physical controller, GUI emulator,
*    and keyboard mapper work together without overwriting each
*    other.
*******************************************************/

using System.Collections.Generic;
using UnityEngine;

public static class Controller
{
    private static readonly Dictionary<buttonType, bool> physicalButtons = new();
    private static readonly Dictionary<triggerType, float> physicalTriggers = new();
    private static readonly Dictionary<joystickType, Vector2> physicalJoysticks = new();

    private static readonly Dictionary<buttonType, bool> emulatedButtons = new();
    private static readonly Dictionary<triggerType, float> emulatedTriggers = new();
    private static readonly Dictionary<joystickType, Vector2> emulatedJoysticks = new();

    /// <summary>
    /// Stores the current state of a physical controller button.
    /// </summary>
    public static void SetPhysicalButton(buttonType button, bool value)
    {
        physicalButtons[button] = value;
    }

    /// <summary>
    /// Stores the current value of a physical controller trigger.
    /// </summary>
    public static void SetPhysicalTrigger(triggerType trigger, float value)
    {
        physicalTriggers[trigger] = value;
    }

    /// <summary>
    /// Stores the current position of a physical controller joystick.
    /// </summary>
    public static void SetPhysicalJoystick(joystickType joystick, Vector2 value)
    {
        physicalJoysticks[joystick] = value;
    }

    /// <summary>
    /// Stores the current state of an emulated controller button.
    /// </summary>
    public static void SetEmulatedButton(buttonType button, bool value)
    {
        emulatedButtons[button] = value;
    }

    /// <summary>
    /// Stores the current value of an emulated controller trigger.
    /// </summary>
    public static void SetEmulatedTrigger(triggerType trigger, float value)
    {
        emulatedTriggers[trigger] = value;
    }

    /// <summary>
    /// Stores the current position of an emulated controller joystick.
    /// </summary>
    public static void SetEmulatedJoystick(joystickType joystick, Vector2 value)
    {
        emulatedJoysticks[joystick] = value;
    }

    /// <summary>
    /// Clears all emulated input values without affecting physical controller input.
    /// </summary>
    public static void ClearEmulated()
    {
        foreach (var button in new List<buttonType>(emulatedButtons.Keys))
        {
            emulatedButtons[button] = false;
        }

        foreach (var trigger in new List<triggerType>(emulatedTriggers.Keys))
        {
            emulatedTriggers[trigger] = 0f;
        }

        foreach (var joystick in new List<joystickType>(emulatedJoysticks.Keys))
        {
            emulatedJoysticks[joystick] = Vector2.zero;
        }
    }

    /// <summary>
    /// Returns the merged state of a button from physical and emulated input.
    /// </summary>
    public static bool GetButton(buttonType button)
    {
        return physicalButtons.GetValueOrDefault(button) || emulatedButtons.GetValueOrDefault(button);
    }

    /// <summary>
    /// Returns the larger trigger value between physical and emulated input.
    /// </summary>
    public static float GetTrigger(triggerType trigger)
    {
        return Mathf.Max(
            physicalTriggers.GetValueOrDefault(trigger),
            emulatedTriggers.GetValueOrDefault(trigger)
        );
    }

    /// <summary>
    /// Returns the joystick input with the larger movement magnitude.
    /// </summary>
    public static Vector2 GetJoystick(joystickType joystick)
    {
        var physicalInput = physicalJoysticks.GetValueOrDefault(joystick);
        var emulatedInput = emulatedJoysticks.GetValueOrDefault(joystick);

        if (physicalInput.magnitude >= emulatedInput.magnitude)
        {
            return physicalInput;
        }

        return emulatedInput;
    }

    /// <summary>
    /// Returns only the physical state of a button.
    /// </summary>
    public static bool GetPhysicalButton(buttonType button)
    {
        return physicalButtons.GetValueOrDefault(button);
    }

    /// <summary>
    /// Returns only the physical value of a trigger.
    /// </summary>
    public static float GetPhysicalTrigger(triggerType trigger)
    {
        return physicalTriggers.GetValueOrDefault(trigger);
    }

    /// <summary>
    /// Returns only the physical position of a joystick.
    /// </summary>
    public static Vector2 GetPhysicalJoystick(joystickType joystick)
    {
        return physicalJoysticks.GetValueOrDefault(joystick);
    }

    /// <summary>
    /// Returns only the emulated state of a button.
    /// </summary>
    public static bool GetEmulatedButton(buttonType button)
    {
        return emulatedButtons.GetValueOrDefault(button);
    }

    /// <summary>
    /// Returns only the emulated value of a trigger.
    /// </summary>
    public static float GetEmulatedTrigger(triggerType trigger)
    {
        return emulatedTriggers.GetValueOrDefault(trigger);
    }

    /// <summary>
    /// Returns only the emulated position of a joystick.
    /// </summary>
    public static Vector2 GetEmulatedJoystick(joystickType joystick)
    {
        return emulatedJoysticks.GetValueOrDefault(joystick);
    }
}