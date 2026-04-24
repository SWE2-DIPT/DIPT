/*******************************************************
* Script:      KeyMapper.cs
* Author(s):   Andrew Bradnao (Add yourselves to this!)
* 
* Description:
*    Allows user to map keybinds to interact with GUI.
*    
* Notes: How to find the JSON file to manually inspect, clear, or delete
* - WINDOWS: Run 'regedit' -> HKEY_CURRENT_USER\Software\Unity Technologies\Unity Editor 5.x
* - MACOS:   ~/Library/Preferences/com.unity3d.UnityEditor5.x.plist
* - LINUX:   ~/.config/unity3d/UnityEditor5.x
*   File paths written April 2026
*   Windows path confirmed by Developer, MACOS and LINUX pathways provided by Gemini AI (Google)
*   
*   BE CAREFUL NOT TO EDIT ANYTHING UNLESS YOU KNOW EXACTLY WHAT YOU ARE DOING
*   IF YOU EDIT THE WRONG FILE, OR DELETE THE WRONG FILE, IT MAY AFFECT YOUR MACHINE'S SYSTEM
*   File name of JSON file that saves Keybinds is the string saved in the class variable SAVE_KEY
*******************************************************/
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[InitializeOnLoad] // this allows the static KeyMapper() to run immediately
                   // For debugging, if you want to disconnect this file 
                   // comment out InitializeOnLoad line
public class KeyMapper
{
    private static Dictionary<UnityEngine.InputSystem.Key, buttonType> keyboardBinds = new();
    private static Dictionary<UnityEngine.InputSystem.Key, triggerType> keyboardTriggers = new();
    private static Dictionary<UnityEngine.InputSystem.Key, (joystickType stick, Vector2 direction)> keyboardJoysticks = new();
    // This a tuple ^^ for the Joysticks

    private const string SAVE_KEY = "DIPT_KeyBinds_JSON"; // Type of JSON file for saving custom Keybinds
    private static bool keyboardWasActive = false; // Variable to keep track if the keyboard was active in the last frame
    private static bool isInitialized = false;

    // Prevent users from getting stuck in their editor/plugin
    private static readonly HashSet<Key> ForbiddenKeys = new() // Used to cancel rebinding
    {
        Key.Escape,
        Key.LeftWindows, Key.RightWindows, Key.LeftCommand, Key.RightCommand,
        Key.Tab, Key.PrintScreen, Key.Pause, Key.Home, Key.End, Key.Delete, Key.Insert
    };

    // System.Serializable is an attribute. 
    // Tells Unity this class is safe to save, without this, JsonUtility won't be able 
    // to see or write these variables to save the file.
    [System.Serializable] private class ButtonEntry { public Key key; public buttonType action; }
    [System.Serializable] private class TriggerEntry { public Key key; public triggerType action; }
    [System.Serializable] private class JoystickEntry { public Key key; public joystickType stick; public Vector2 dir; }

    [System.Serializable]
    private class BindWrapper
    {
        public List<ButtonEntry> buttonList = new();
        public List<TriggerEntry> triggerList = new();
        public List<JoystickEntry> joystickList = new();
    }

    static KeyMapper()
    {
        // This runs automatically without you doing anything
        EnsureInitialized();
        // Debug.Log("KeyMapper Initialized and Ready.");
    }


    // Getter functions
    public static string GetKeyForButton(buttonType action)
    {
        var pair = keyboardBinds.FirstOrDefault(x => x.Value == action); // Functino to check everything and stops at first match
        // if finds nothing returns a sort of empty container
        return pair.Key == Key.None ? "[ Unassigned ]" : pair.Key.ToString();
    }

    public static string GetKeyForTrigger(triggerType action)
    {
        var pair = keyboardTriggers.FirstOrDefault(x => x.Value == action);
        return pair.Key == Key.None ? "[ Unassigned ]" : pair.Key.ToString();
    }

    public static string GetKeyForJoystick(joystickType stick, Vector2 direction)
    {
        var pair = keyboardJoysticks.FirstOrDefault(x =>
            x.Value.stick == stick && x.Value.direction == direction);
        return pair.Key == Key.None ? "[ Unassigned ]" : pair.Key.ToString();
    }
    // End of getter functions

    public static void EnsureInitialized()
    {
        if (isInitialized) return;

        if (EditorPrefs.HasKey(SAVE_KEY)) // if the file already exists with keybinds
        {
            LoadBinds();
        }
        else
        {
            ResetToDefaults();
        }

        isInitialized = true;
    }

    public static void ResetToDefaults()
    {
        keyboardBinds.Clear(); // clear the dictionarys
        keyboardTriggers.Clear();
        keyboardJoysticks.Clear();


        // Start of buttonTypes
        keyboardBinds.Add(Key.Z, buttonType.Up);
        keyboardBinds.Add(Key.X, buttonType.Left);
        keyboardBinds.Add(Key.C, buttonType.Down);
        keyboardBinds.Add(Key.V, buttonType.Right);

        keyboardBinds.Add(Key.N, buttonType.Y);
        keyboardBinds.Add(Key.M, buttonType.X);
        keyboardBinds.Add(Key.Comma, buttonType.A); // the , symbol
        keyboardBinds.Add(Key.Period, buttonType.B); // the . symbol

        keyboardBinds.Add(Key.Digit1, buttonType.Xbox); // Number 1 above the Keyboard, not on Numberpad
        keyboardBinds.Add(Key.Digit2, buttonType.Menu); // Number 2 above the Keyboard, not on Numberpad
        keyboardBinds.Add(Key.Digit3, buttonType.View); // Number 3 above the Keyboard, not on Numberpad
        keyboardBinds.Add(Key.Digit4, buttonType.Share); // Number 4 above the Keyboard, not on Numberpad

        keyboardBinds.Add(Key.E, buttonType.LBumper);
        keyboardBinds.Add(Key.U, buttonType.RBumper);

        keyboardBinds.Add(Key.F, buttonType.LeftStick);
        keyboardBinds.Add(Key.H, buttonType.RightStick);
        // End of buttonTypes

        // Start of triggerTypes
        keyboardTriggers.Add(Key.Q, triggerType.Left);
        keyboardTriggers.Add(Key.O, triggerType.Right);
        // End of triggerTypes

        // Start of Joysticks
        // Start of Left Joystick
        keyboardJoysticks.Add(Key.W, (joystickType.Left, Vector2.up));
        keyboardJoysticks.Add(Key.S, (joystickType.Left, Vector2.down));
        keyboardJoysticks.Add(Key.A, (joystickType.Left, Vector2.left));
        keyboardJoysticks.Add(Key.D, (joystickType.Left, Vector2.right));
        // End of Left Joystick

        // Start of Right Joystick
        keyboardJoysticks.Add(Key.I, (joystickType.Right, Vector2.up));
        keyboardJoysticks.Add(Key.K, (joystickType.Right, Vector2.down));
        keyboardJoysticks.Add(Key.J, (joystickType.Right, Vector2.left));
        keyboardJoysticks.Add(Key.L, (joystickType.Right, Vector2.right));
        // End of Right Joystick

        SaveBinds();
        Debug.Log("DIPT: Keybinds reset to factory defaults.");
    }

    public static void SaveBinds()
    {
        BindWrapper wrapper = new BindWrapper(); // A place to hold all the infromation 
        //BindWrapper, ButtonEntry, TriggerEntry, and JoystickEntry are all self declared variables
        // at the top of the file

        foreach (var KeyValuePair in keyboardBinds)
            wrapper.buttonList.Add(new ButtonEntry { key = KeyValuePair.Key, action = KeyValuePair.Value });

        foreach (var KeyValuePair in keyboardTriggers)
            wrapper.triggerList.Add(new TriggerEntry { key = KeyValuePair.Key, action = KeyValuePair.Value });

        foreach (var KeyValuePair in keyboardJoysticks)
            wrapper.joystickList.Add(new JoystickEntry { key = KeyValuePair.Key, stick = KeyValuePair.Value.stick, dir = KeyValuePair.Value.direction });

        string json = JsonUtility.ToJson(wrapper); // turns everything in to JSON
        EditorPrefs.SetString(SAVE_KEY, json);      // Saved within Unity's Editor settings place

    }

    private static void LoadBinds()
    {
        // Get the string from Editor Preferences
        string json = EditorPrefs.GetString(SAVE_KEY, "");

        // saftey check, if string json is empty (no file) load default
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("DIPT: No save found, loading defaults.");
            ResetToDefaults();
            return;
        }

        try
        {
            BindWrapper wrapper = JsonUtility.FromJson<BindWrapper>(json);

            // saftey check to see if any of the lists are empty
            if (wrapper == null || (wrapper.buttonList.Count == 0 &&
                                    wrapper.triggerList.Count == 0 &&
                                    wrapper.joystickList.Count == 0))
            {
                Debug.LogWarning("DIPT: Saved binds were empty or corrupted. Resetting to defaults.");
                ResetToDefaults();
                return;
            }

            // These can probably be more optimized, instead of clearing the whole list, 
            // Use an alternative method
            keyboardBinds.Clear();
            keyboardTriggers.Clear();
            keyboardJoysticks.Clear();

            foreach (var entry in wrapper.buttonList) keyboardBinds[entry.key] = entry.action;
            foreach (var entry in wrapper.triggerList) keyboardTriggers[entry.key] = entry.action;
            foreach (var entry in wrapper.joystickList) keyboardJoysticks[entry.key] = (entry.stick, entry.dir);

            Debug.Log("DIPT: Keybinds loaded successfully from EditorPrefs.");
        }
        catch (Exception e)
        {
            Debug.LogError($"DIPT: Failed to parse keybinds. Error: {e.Message}");
            ResetToDefaults();
        }
    }


    public static void UpdateKeyboardEmulation(GamepadEmulator emulator)
    {
        EnsureInitialized(); // saftey check

        if (Keyboard.current == null || emulator == null) { return; }

        bool isCurrentlyActive = CheckKeyboardSignal();

        if (!isCurrentlyActive) // Goes into this block every time we are not touching the keyboard
        {

            if (keyboardWasActive) // Only goes into this block if we were touching the keyboard in the last frame
            {
                ResetAllKeyboardStates();
                emulator.clear(); // Stop emulating when keys are let go
                keyboardWasActive = false; // Ensures that next time we won't reset because it
                                           // saying the last frame (now the next) we are not touching
            }

            return;
        }


        keyboardWasActive = true; // If we reach here, the keyboard is being used

        foreach (var bind in keyboardBinds)
        {
            bool isDown = Keyboard.current[bind.Key].isPressed;

            if (isDown) emulator.pressButton((int)bind.Value);
            else emulator.releaseButton((int)bind.Value);
        }

        foreach (var trig in keyboardTriggers)
        {
            bool isDown = Keyboard.current[trig.Key].isPressed;

            float val = Keyboard.current[trig.Key].isPressed ? 1f : 0f;

            if (trig.Value == triggerType.Left) emulator.pressLeftTrigger(val);
            else emulator.pressRightTrigger(val);

        }

        Vector2 leftTotal = Vector2.zero;
        Vector2 rightTotal = Vector2.zero;

        foreach (var input in keyboardJoysticks)
        {
            if (Keyboard.current[input.Key].isPressed)
            {
                // Add the direction (up, down, left, right) to the correct bucket
                if (input.Value.stick == joystickType.Left)
                    leftTotal += input.Value.direction;
                else
                    rightTotal += input.Value.direction;
            }
        }

        emulator.moveLeftJoystick(leftTotal.x, leftTotal.y);
        emulator.moveRightJoystick(rightTotal.x, rightTotal.y);
    }

    private static void ResetAllKeyboardStates()
    {   // Function to reset everything back to zero, or unpressed
        // This runs only when the keyboard is not in use AFTER it was (in the last frame)
        foreach (var bind in keyboardBinds)
            XboxController.SetButton(bind.Value, false);

        foreach (var trig in keyboardTriggers)
            XboxController.SetTrigger(trig.Value, 0f);

        XboxController.SetJoystick(joystickType.Left, Vector2.zero);
        XboxController.SetJoystick(joystickType.Right, Vector2.zero);
    }

    private static bool CheckKeyboardSignal() // Checks if the keyboard is being touched
    {
        if (Keyboard.current == null) return false;

        // Check Buttons
        foreach (var key in keyboardBinds.Keys)
            if (Keyboard.current[key].isPressed) { return true; }

        // Check Triggers
        foreach (var key in keyboardTriggers.Keys)
            if (Keyboard.current[key].isPressed) { return true; }

        // Check Joysticks
        foreach (var key in keyboardJoysticks.Keys)
            if (Keyboard.current[key].isPressed) { return true; }

        return false;
    }

    public static void Update(GamepadEmulator emulator)
    {
        UpdateKeyboardEmulation(emulator);
    }

    public static bool IsKeyForbidden(Key key)
    {
        return ForbiddenKeys.Contains(key);
    }

    // This ensures a key is used in only one place
    private static void ClearKeyEverywhere(Key key)
    {
        if (keyboardBinds.ContainsKey(key)) keyboardBinds.Remove(key);
        if (keyboardTriggers.ContainsKey(key)) keyboardTriggers.Remove(key);
        if (keyboardJoysticks.ContainsKey(key)) keyboardJoysticks.Remove(key);
    }

    public static void SetButtonBind(Key newKey, buttonType action)
    {
        if (IsKeyForbidden(newKey)) return;
        
        var oldKey = keyboardBinds.FirstOrDefault(x => x.Value == action).Key; // From the LINQ library 
        if (oldKey != Key.None) keyboardBinds.Remove(oldKey); // Takes out the old keybind

        ClearKeyEverywhere(newKey);
        // E.x. If Z is D-pad up, and they make Z map to Left bumper, D-Pad up becomes unassigned

        keyboardBinds.Add(newKey, action);
    }

    public static void SetTriggerBind(Key newKey, triggerType action)
    { // same as SetButtonBind just with trigger dictionary
        if (IsKeyForbidden(newKey)) return;

        var oldKey = keyboardTriggers.FirstOrDefault(x => x.Value == action).Key;
        if (oldKey != Key.None) keyboardTriggers.Remove(oldKey);

        ClearKeyEverywhere(newKey);

        keyboardTriggers.Add(newKey, action);
    }

    public static void SetJoystickBind(Key newKey, joystickType stick, Vector2 direction)
    { // Basically same as SetButtonBind and SetTriggerBind but with the tuple
        if (IsKeyForbidden(newKey)) return;

        var oldKey = keyboardJoysticks.FirstOrDefault(x =>
            x.Value.stick == stick && x.Value.direction == direction).Key;

        if (oldKey != Key.None) keyboardJoysticks.Remove(oldKey);

        ClearKeyEverywhere(newKey);

        keyboardJoysticks.Add(newKey, (stick, direction));
    }
}