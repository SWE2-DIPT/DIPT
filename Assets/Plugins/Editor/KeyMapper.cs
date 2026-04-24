/*******************************************************
* Script:      KeyMapping.cs
* Author(s):   Andrew Bradnao (Add yourselves to this!)
* 
* Description:
*    Allows user to map keybinds to interact with gui.
*******************************************************/
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;
using System;
using Unity.Collections;
using System.Collections.Generic;
using System.Data;


[InitializeOnLoad] // this allows the static KeyMapper() to run immediately 
public class KeyMapper : EditorWindow
{
    private static Dictionary<UnityEngine.InputSystem.Key, buttonType> keyboardBinds = new();
    private static Dictionary<UnityEngine.InputSystem.Key, triggerType> keyboardTriggers = new();
    private static Dictionary<UnityEngine.InputSystem.Key, (joystickType stick, Vector2 direction)> keyboardJoysticks = new();
    // This a tuple ^^ for the Joysticks

    private static bool keyboardWasActive = false; // Variable to keep track if the keyboard was active in the last frame
    private static bool isInitialized = false;

    static KeyMapper()
    {
        // This runs automatically without you doing anything
        EnsureInitialized();
        // Debug.Log("KeyMapper Initialized and Ready.");
    }

    [MenuItem("Tools/DIPT/KeyboardMapper")]
    public static void ShowWindow()
    {
        GetWindow(typeof(KeyMapper));
    }
    
    void OnGUI()
    {
        GUILayout.Label("Keyboard Mapping Status: ACTIVE", EditorStyles.boldLabel);
        // Future customization UI goes here

        // the GUI is just to make it customizable 
        // base it off the way fortnite does it
        // see if there is a way to make it savable 

    }

    void OnEnable()
    {
        // KeyMapperInitialize();
    }

    void OnDisable()
    {
     // pretty sure just closes the window and save the information
     // 
    }

    public static void EnsureInitialized()
    {
        if (isInitialized) return;

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

        isInitialized = true;
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
                keyboardWasActive = false; // Ensures that next time we won't reset because it saying the last frame (now the next) we are not touching
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
}