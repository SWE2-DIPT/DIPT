/*******************************************************
* Script:      KeyMapper.cs
* Author(s):   Andrew Bradnao (Add yourselves to this!)
* 
* Description:
*    Handles keyboard-to-controller input mapping for the
*    DIPT controller emulator. This file stores default
*    keyboard bindings, saves and loads custom bindings using
*    Unity EditorPrefs, prevents unsafe keys from being used,
*    and converts keyboard input into emulated controller
*    buttons, triggers, and joystick movement.
*    
* Notes:
*    Saved keybinds are stored in Unity EditorPrefs under:
*    DIPT_KeyBinds_JSON
*    How to find the JSON file to manually inspect, clear, or delete
* -  WINDOWS: Run 'regedit' -> HKEY_CURRENT_USER\Software\Unity Technologies\Unity Editor 5.x
* -  MACOS:   ~/Library/Preferences/com.unity3d.UnityEditor5.x.plist
* -  LINUX:   ~/.config/unity3d/UnityEditor5.x
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public class KeyMapper
{
    private static Dictionary<Key, buttonType> keyboardBinds = new();
    private static Dictionary<Key, triggerType> keyboardTriggers = new();
    private static Dictionary<Key, (joystickType stick, Vector2 direction)> keyboardJoysticks = new();

    private const string SAVE_KEY = "DIPT_KeyBinds_JSON";
    private static bool keyboardWasActive = false;
    private static bool isInitialized = false;

    private static readonly HashSet<Key> ForbiddenKeys = new()
    {
        Key.Escape,
        Key.LeftWindows,
        Key.RightWindows,
        Key.LeftCommand,
        Key.RightCommand,
        Key.Tab,
        Key.PrintScreen,
        Key.Pause,
        Key.Home,
        Key.End,
        Key.Delete,
        Key.Insert,
        Key.Numpad0,
        Key.Numpad1,
        Key.Numpad2,
        Key.Numpad3,
        Key.Numpad4,
        Key.Numpad5,
        Key.Numpad6,
        Key.Numpad7,
        Key.Numpad8,
        Key.Numpad9,
        Key.NumpadEnter,
        Key.NumpadDivide,
        Key.NumpadMultiply,
        Key.NumpadMinus,
        Key.NumpadPlus,
        Key.NumpadPeriod
    };

    [System.Serializable]
    private class ButtonEntry
    {
        public Key key;
        public buttonType action;
    }

    [System.Serializable]
    private class TriggerEntry
    {
        public Key key;
        public triggerType action;
    }

    [System.Serializable]
    private class JoystickEntry
    {
        public Key key;
        public joystickType stick;
        public Vector2 direction;
    }

    [System.Serializable]
    private class BindWrapper
    {
        public List<ButtonEntry> buttonList = new();
        public List<TriggerEntry> triggerList = new();
        public List<JoystickEntry> joystickList = new();
    }

    /// <summary>
    /// Runs when Unity loads the editor and prepares the saved keybind data.
    /// </summary>
    static KeyMapper()
    {
        EnsureInitialized();
    }

    /// <summary>
    /// Returns the keyboard key currently assigned to a controller button.
    /// </summary>
    public static string GetKeyForButton(buttonType action)
    {
        var binding = keyboardBinds.FirstOrDefault(keyBind => keyBind.Value == action);

        if (binding.Key == Key.None)
        {
            return "[ Unassigned ]";
        }

        return binding.Key.ToString();
    }

    /// <summary>
    /// Returns the keyboard key currently assigned to a controller trigger.
    /// </summary>
    public static string GetKeyForTrigger(triggerType action)
    {
        var binding = keyboardTriggers.FirstOrDefault(keyBind => keyBind.Value == action);

        if (binding.Key == Key.None)
        {
            return "[ Unassigned ]";
        }

        return binding.Key.ToString();
    }

    /// <summary>
    /// Returns the keyboard key currently assigned to a joystick direction.
    /// </summary>
    public static string GetKeyForJoystick(joystickType stick, Vector2 direction)
    {
        var binding = keyboardJoysticks.FirstOrDefault(keyBind =>
            keyBind.Value.stick == stick && keyBind.Value.direction == direction
        );

        if (binding.Key == Key.None)
        {
            return "[ Unassigned ]";
        }

        return binding.Key.ToString();
    }

    /// <summary>
    /// Loads saved keybinds once, or creates default keybinds if no save exists.
    /// </summary>
    public static void EnsureInitialized()
    {
        if (isInitialized)
        {
            return;
        }

        if (EditorPrefs.HasKey(SAVE_KEY))
        {
            LoadBinds();
        }
        else
        {
            ResetToDefaults();
        }

        isInitialized = true;
    }

    /// <summary>
    /// Restores all keyboard mappings to the default controller layout and saves them.
    /// </summary>
    public static void ResetToDefaults()
    {
        keyboardBinds.Clear();
        keyboardTriggers.Clear();
        keyboardJoysticks.Clear();

        keyboardBinds.Add(Key.Z, buttonType.Up);
        keyboardBinds.Add(Key.X, buttonType.Left);
        keyboardBinds.Add(Key.C, buttonType.Down);
        keyboardBinds.Add(Key.V, buttonType.Right);

        keyboardBinds.Add(Key.N, buttonType.Y);
        keyboardBinds.Add(Key.M, buttonType.X);
        keyboardBinds.Add(Key.Comma, buttonType.A);
        keyboardBinds.Add(Key.Period, buttonType.B);

        keyboardBinds.Add(Key.Digit1, buttonType.Xbox);
        keyboardBinds.Add(Key.Digit2, buttonType.Menu);
        keyboardBinds.Add(Key.Digit3, buttonType.View);

        keyboardBinds.Add(Key.E, buttonType.LBumper);
        keyboardBinds.Add(Key.U, buttonType.RBumper);

        keyboardBinds.Add(Key.F, buttonType.LeftStick);
        keyboardBinds.Add(Key.H, buttonType.RightStick);

        keyboardTriggers.Add(Key.Q, triggerType.Left);
        keyboardTriggers.Add(Key.O, triggerType.Right);

        keyboardJoysticks.Add(Key.W, (joystickType.Left, Vector2.up));
        keyboardJoysticks.Add(Key.S, (joystickType.Left, Vector2.down));
        keyboardJoysticks.Add(Key.A, (joystickType.Left, Vector2.left));
        keyboardJoysticks.Add(Key.D, (joystickType.Left, Vector2.right));

        keyboardJoysticks.Add(Key.I, (joystickType.Right, Vector2.up));
        keyboardJoysticks.Add(Key.K, (joystickType.Right, Vector2.down));
        keyboardJoysticks.Add(Key.J, (joystickType.Right, Vector2.left));
        keyboardJoysticks.Add(Key.L, (joystickType.Right, Vector2.right));

        SaveBinds();
    }

    /// <summary>
    /// Saves the current button, trigger, and joystick keybinds into Unity EditorPrefs.
    /// </summary>
    public static void SaveBinds()
    {
        BindWrapper wrapper = new BindWrapper();

        foreach (var buttonBind in keyboardBinds)
        {
            wrapper.buttonList.Add(new ButtonEntry
            {
                key = buttonBind.Key,
                action = buttonBind.Value
            });
        }

        foreach (var triggerBind in keyboardTriggers)
        {
            wrapper.triggerList.Add(new TriggerEntry
            {
                key = triggerBind.Key,
                action = triggerBind.Value
            });
        }

        foreach (var joystickBind in keyboardJoysticks)
        {
            wrapper.joystickList.Add(new JoystickEntry
            {
                key = joystickBind.Key,
                stick = joystickBind.Value.stick,
                direction = joystickBind.Value.direction
            });
        }

        string json = JsonUtility.ToJson(wrapper);
        EditorPrefs.SetString(SAVE_KEY, json);
    }

    /// <summary>
    /// Loads saved keybinds from Unity EditorPrefs and falls back to defaults if loading fails.
    /// </summary>
    private static void LoadBinds()
    {
        string json = EditorPrefs.GetString(SAVE_KEY, "");

        if (string.IsNullOrEmpty(json))
        {
            ResetToDefaults();
            return;
        }

        try
        {
            BindWrapper wrapper = JsonUtility.FromJson<BindWrapper>(json);

            if (wrapper == null ||
                (wrapper.buttonList.Count == 0 &&
                wrapper.triggerList.Count == 0 &&
                wrapper.joystickList.Count == 0))
            {
                Debug.LogWarning("DIPT: Saved binds were empty or corrupted. Resetting to defaults.");
                ResetToDefaults();
                return;
            }

            keyboardBinds.Clear();
            keyboardTriggers.Clear();
            keyboardJoysticks.Clear();

            foreach (var entry in wrapper.buttonList)
            {
                keyboardBinds[entry.key] = entry.action;
            }

            foreach (var entry in wrapper.triggerList)
            {
                keyboardTriggers[entry.key] = entry.action;
            }

            foreach (var entry in wrapper.joystickList)
            {
                keyboardJoysticks[entry.key] = (entry.stick, entry.direction);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError($"DIPT: Failed to parse keybinds. Error: {exception.Message}");
            ResetToDefaults();
        }
    }

    /// <summary>
    /// Converts currently pressed keyboard keys into emulated controller input.
    /// </summary>
    public static void UpdateKeyboardEmulation(GamepadEmulator emulator)
    {
        EnsureInitialized();

        if (Keyboard.current == null || emulator == null)
        {
            return;
        }

        bool isCurrentlyActive = CheckKeyboardSignal();

        if (!isCurrentlyActive)
        {
            if (keyboardWasActive)
            {
                ResetAllKeyboardStates();
                emulator.clear();
                keyboardWasActive = false;
            }

            return;
        }

        keyboardWasActive = true;

        foreach (var buttonBind in keyboardBinds)
        {
            bool isPressed = Keyboard.current[buttonBind.Key].isPressed;

            if (isPressed)
            {
                emulator.pressButton((int)buttonBind.Value);
            }
            else
            {
                emulator.releaseButton((int)buttonBind.Value);
            }

            Controller.SetEmulatedButton(buttonBind.Value, isPressed);
        }

        foreach (var triggerBind in keyboardTriggers)
        {
            float triggerValue = Keyboard.current[triggerBind.Key].isPressed ? 1f : 0f;

            if (triggerBind.Value == triggerType.Left)
            {
                emulator.pressLeftTrigger(triggerValue);
            }
            else
            {
                emulator.pressRightTrigger(triggerValue);
            }

            Controller.SetEmulatedTrigger(triggerBind.Value, triggerValue);
        }

        Vector2 leftJoystickTotal = Vector2.zero;
        Vector2 rightJoystickTotal = Vector2.zero;

        foreach (var joystickBind in keyboardJoysticks)
        {
            if (Keyboard.current[joystickBind.Key].isPressed)
            {
                if (joystickBind.Value.stick == joystickType.Left)
                {
                    leftJoystickTotal += joystickBind.Value.direction;
                }
                else
                {
                    rightJoystickTotal += joystickBind.Value.direction;
                }
            }
        }

        leftJoystickTotal = Vector2.ClampMagnitude(leftJoystickTotal, 1f);
        rightJoystickTotal = Vector2.ClampMagnitude(rightJoystickTotal, 1f);

        emulator.moveLeftJoystick(leftJoystickTotal.x, leftJoystickTotal.y);
        emulator.moveRightJoystick(rightJoystickTotal.x, rightJoystickTotal.y);

        Controller.SetEmulatedJoystick(joystickType.Left, leftJoystickTotal);
        Controller.SetEmulatedJoystick(joystickType.Right, rightJoystickTotal);
    }

    /// <summary>
    /// Clears every emulated keyboard input from the shared Controller state.
    /// </summary>
    private static void ResetAllKeyboardStates()
    {
        foreach (var buttonBind in keyboardBinds)
        {
            Controller.SetEmulatedButton(buttonBind.Value, false);
        }

        foreach (var triggerBind in keyboardTriggers)
        {
            Controller.SetEmulatedTrigger(triggerBind.Value, 0f);
        }

        Controller.SetEmulatedJoystick(joystickType.Left, Vector2.zero);
        Controller.SetEmulatedJoystick(joystickType.Right, Vector2.zero);
    }

    /// <summary>
    /// Checks whether any mapped keyboard input is currently pressed.
    /// </summary>
    private static bool CheckKeyboardSignal()
    {
        if (Keyboard.current == null)
        {
            return false;
        }

        foreach (var key in keyboardBinds.Keys)
        {
            if (Keyboard.current[key].isPressed)
            {
                return true;
            }
        }

        foreach (var key in keyboardTriggers.Keys)
        {
            if (Keyboard.current[key].isPressed)
            {
                return true;
            }
        }

        foreach (var key in keyboardJoysticks.Keys)
        {
            if (Keyboard.current[key].isPressed)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Updates keyboard input and applies it to the emulator.
    /// </summary>
    public static void Update(GamepadEmulator emulator)
    {
        UpdateKeyboardEmulation(emulator);
    }

    /// <summary>
    /// Returns whether a key is blocked from being used as a custom binding.
    /// </summary>
    public static bool IsKeyForbidden(Key key)
    {
        return ForbiddenKeys.Contains(key);
    }

    /// <summary>
    /// Removes a key from all binding dictionaries so it can only control one input.
    /// </summary>
    private static void ClearKeyEverywhere(Key key)
    {
        if (keyboardBinds.ContainsKey(key))
        {
            keyboardBinds.Remove(key);
        }

        if (keyboardTriggers.ContainsKey(key))
        {
            keyboardTriggers.Remove(key);
        }

        if (keyboardJoysticks.ContainsKey(key))
        {
            keyboardJoysticks.Remove(key);
        }
    }

    /// <summary>
    /// Assigns a keyboard key to a controller button and removes conflicting bindings.
    /// </summary>
    public static void SetButtonBind(Key newKey, buttonType action)
    {
        if (IsKeyForbidden(newKey))
        {
            return;
        }

        var oldKey = keyboardBinds.FirstOrDefault(keyBind => keyBind.Value == action).Key;

        if (oldKey != Key.None)
        {
            keyboardBinds.Remove(oldKey);
        }

        ClearKeyEverywhere(newKey);
        keyboardBinds.Add(newKey, action);
    }

    /// <summary>
    /// Assigns a keyboard key to a controller trigger and removes conflicting bindings.
    /// </summary>
    public static void SetTriggerBind(Key newKey, triggerType action)
    {
        if (IsKeyForbidden(newKey))
        {
            return;
        }

        var oldKey = keyboardTriggers.FirstOrDefault(keyBind => keyBind.Value == action).Key;

        if (oldKey != Key.None)
        {
            keyboardTriggers.Remove(oldKey);
        }

        ClearKeyEverywhere(newKey);
        keyboardTriggers.Add(newKey, action);
    }

    /// <summary>
    /// Assigns a keyboard key to a joystick direction and removes conflicting bindings.
    /// </summary>
    public static void SetJoystickBind(Key newKey, joystickType stick, Vector2 direction)
    {
        if (IsKeyForbidden(newKey))
        {
            return;
        }

        var oldKey = keyboardJoysticks.FirstOrDefault(keyBind =>
            keyBind.Value.stick == stick && keyBind.Value.direction == direction
        ).Key;

        if (oldKey != Key.None)
        {
            keyboardJoysticks.Remove(oldKey);
        }

        ClearKeyEverywhere(newKey);
        keyboardJoysticks.Add(newKey, (stick, direction));
    }
}