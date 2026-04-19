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


[InitializeOnLoad]
public class KeyMapper : EditorWindow
{
    private static Dictionary<UnityEngine.InputSystem.Key, buttonType> keyboardBinds = new();
    private static Dictionary<UnityEngine.InputSystem.Key, triggerType> keyboardTriggers = new();
    private static Dictionary<UnityEngine.InputSystem.Key, Joystick> keyboardJoysticks = new();

    private static bool keyboardWasActive = false; // Variable to keep track if the keyboard was active in the last frame
    private static bool isInitialized = false;

    static KeyMapper()
    {
        // 3. This runs AUTOMATICALLY without you doing anything
        EnsureInitialized();
        Debug.Log("KeyMapper Service Initialized and Ready.");
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
        // WASD for left joystick 
        // IJKL for right joystick

        if (isInitialized) return;

        // Start of buttonTypes
        keyboardBinds.Add(Key.Z, buttonType.Up);
        keyboardBinds.Add(Key.X, buttonType.Left);
        keyboardBinds.Add(Key.C, buttonType.Down);
        keyboardBinds.Add(Key.V, buttonType.Right);

        keyboardBinds.Add(Key.N, buttonType.Y);
        keyboardBinds.Add(Key.M, buttonType.X);
        keyboardBinds.Add(Key.Comma, buttonType.A);
        keyboardBinds.Add(Key.Period, buttonType.B); // the , symbol

        keyboardBinds.Add(Key.Digit1, buttonType.Xbox);
        keyboardBinds.Add(Key.Digit2, buttonType.Menu);
        keyboardBinds.Add(Key.Digit3, buttonType.View);
        keyboardBinds.Add(Key.Digit4, buttonType.Share);

        keyboardBinds.Add(Key.E, buttonType.LBumper);
        keyboardBinds.Add(Key.U, buttonType.RBumper);
        // End of buttonTypes

        // Start of triggerTypes
        keyboardTriggers.Add(Key.Q, triggerType.Left);
        keyboardTriggers.Add(Key.O, triggerType.Right);
        // End of triggerTypes

        /* THIS WILL BE IMPLEMENTED ONCE CUSTOMIZATION IS CREATED
         * IDEA IS TO CREATE A PRIVATE FUNCTION THAT DEALS WITH THIS
         * THIS IS JUST THE DEFAULT BUTTONS FOR THE CONSTRUCTOR
        //Start of Joysticks
        // Left Joysticks
        keyboardJoysticks.Add(Key.W, (joystickType.Left, Vector2.up));
        keyboardJoysticks.Add(Key.S, (joystickType.Left, Vector2.down));
        keyboardJoysticks.Add(Key.A, (joystickType.Left, Vector2.left));
        keyboardJoysticks.Add(Key.D, (joystickType.Left, Vector2.right));

        // Right Joysticks
        keyboardJoysticks.Add(Key.I, (joystickType.Right, Vector2.up));
        keyboardJoysticks.Add(Key.K, (joystickType.Right, Vector2.down));
        keyboardJoysticks.Add(Key.J, (joystickType.Right, Vector2.left));
        keyboardJoysticks.Add(Key.L, (joystickType.Right, Vector2.right));
        //End of Joysticks
        */

        isInitialized = true;

    }

    
    public static void UpdateKeyboardEmulation(GamepadEmulator emulator)
    {
        EnsureInitialized(); // saftey check

        if (Keyboard.current == null || emulator == null) { return; }

        bool isCurrentlyActive = CheckKeyboardSignal();

        if (!isCurrentlyActive) // Goes into this block every time we are not touching the keyboard
        {
            // Only reset if we were active in the PREVIOUS frame
            if (keyboardWasActive) // Only goes into this block if we were touching the keyboard in the last frame
            {
                ResetAllKeyboardStates();
                emulator.clear(); // Stop emulating when keys are let go
                emulator.resetLeftJoystick();
                emulator.resetRightJoystick();
                keyboardWasActive = false; // Ensures that next time we won't reset because it saying the last frame (now the next) we are not touching
                // Debug.Log("Keyboard released: Final Reset performed.");
            }

            return;
        }

        
        keyboardWasActive = true; // If we reach here, the keyboard is being used


        //if (Keyboard.current.commaKey.isPressed)
        //{
        //    Debug.Log("KeyMapper is trying to set A-Button to TRUE");
        //}

        foreach (var bind in keyboardBinds)
        {
            bool isDown = Keyboard.current[bind.Key].isPressed;
            XboxController.SetButton(bind.Value, isDown);

            if (isDown) emulator.pressButton((int)bind.Value);
            else emulator.releaseButton((int)bind.Value);

            // XboxController.SetButton(bind.Value, Keyboard.current[bind.Key].isPressed);
        }

        foreach (var trig in keyboardTriggers)
        {
            float val = Keyboard.current[trig.Key].isPressed ? 1f : 0f;
            XboxController.SetTrigger(trig.Value, val);
            if (trig.Value == triggerType.Left) emulator.pressLeftTrigger(val);
            else emulator.pressRightTrigger(val);

            //bool isDown = Keyboard.current[trig.Key].isPressed;
            //int press = isDown ? 1 : 0; // C# doesn't evaluate bool as 1 or 0

            //XboxController.SetTrigger(trig.Value, press);

            // if (isDown) {Debug.Log($"Trigger {trig.Key} is pressed");}

        }


        Vector2 leftStick = GetJoystickVector(Key.W, Key.S, Key.A, Key.D);
        XboxController.SetJoystick(joystickType.Left, leftStick);
        emulator.moveLeftJoystick(leftStick.x, leftStick.y);

        Vector2 rightStick = GetJoystickVector(Key.I, Key.K, Key.J, Key.L);
        XboxController.SetJoystick(joystickType.Right, rightStick);
        emulator.moveRightJoystick(rightStick.x, rightStick.y);

        // Left Stick (WASD)
        // XboxController.SetJoystick(joystickType.Left, GetJoystickVector(Key.W, Key.S, Key.A, Key.D));

        // Right Stick (IJKL)
        // XboxController.SetJoystick(joystickType.Right, GetJoystickVector(Key.I, Key.K, Key.J, Key.L));



    }

    private static Vector2 GetJoystickVector(Key up, Key down, Key left, Key right)
    {
        float x = 0;
        float y = 0;

        if (Keyboard.current[up].isPressed) y += 1f;
        if (Keyboard.current[down].isPressed) y -= 1f;
        if (Keyboard.current[right].isPressed) x += 1f;
        if (Keyboard.current[left].isPressed) x -= 1f;

        return Vector2.ClampMagnitude(new Vector2(x, y), 1f); // Returns a vector with a max length of 1
    }

    private static void ResetAllKeyboardStates() // Function to reset everything back to zero, or unpressed
    {   // This runs only when the keyboard is not in use

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

        bool isAnyKeyPressed = false;

        // Check Buttons
        foreach (var key in keyboardBinds.Keys)
            if (Keyboard.current[key].isPressed) { isAnyKeyPressed = true; break; }

        // Check Triggers
        if (!isAnyKeyPressed)
            foreach (var key in keyboardTriggers.Keys)
                if (Keyboard.current[key].isPressed) { isAnyKeyPressed = true; break; }

        // Check Joysticks
        /* if (!isAnyKeyPressed) // CAN USE THIS FOR WHEN WE MAKE CUSTOMIZATION
            foreach (var key in keyboardJoysticks.Keys)
                if (Keyboard.current[key].isPressed) { isAnyKeyPressed = true; break; }
        */

        bool isJoystickActive =
        Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed ||
        Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed ||
        Keyboard.current.iKey.isPressed || Keyboard.current.jKey.isPressed ||
        Keyboard.current.kKey.isPressed || Keyboard.current.lKey.isPressed;
        // These see if any of the WASD or IJKL buttons have been pressed

        return isAnyKeyPressed || isJoystickActive;
     
    }

    /*public void Update()
    {
        UpdateKeyboardEmulation();
    }
    */
}