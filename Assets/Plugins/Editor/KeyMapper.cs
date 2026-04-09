/*******************************************************
* Script:      KeyMapping.cs
* Author(s):   Andrew Bradnao (Add yourselves to this!)
* 
* Description:
*    Allows user to map keybinds to interact with gui.
*******************************************************/
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class KeyMapper
{
    // private Dictionary<KeyCode, buttonType> keyboardBinds = new();
    private Dictionary<UnityEngine.InputSystem.Key, buttonType> keyboardBinds = new();
    private Dictionary<UnityEngine.InputSystem.Key, triggerType> keyboardTriggers = new();
    private Dictionary<UnityEngine.InputSystem.Key, Joystick> keyboardJoysticks = new();
    

    public KeyMapper()
    {
        // WASD for left joystick 
        // IJKL for right joystick

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

    }

    
    public void UpdateKeyboardEmulation()
    {

        if (Keyboard.current == null) return;

        foreach (var bind in keyboardBinds)
        {
            // bool isDown = Keyboard.current.wKey.isPressed;
            bool isDown = Keyboard.current[bind.Key].isPressed;

            // Debug.Log("UP button has been pressed");
            XboxController.SetButton(bind.Value, isDown);
        }

        foreach (var trig in keyboardTriggers)
        {
            bool isDown = Keyboard.current[trig.Key].isPressed;
            int press = isDown ? 1 : 0; // C# doesn't evaluate bool as 1 or 0

            XboxController.SetTrigger(trig.Value, press);
            
            // if (isDown) {Debug.Log($"Trigger {trig.Key} is pressed");}
                
        }

        foreach (var trig in keyboardTriggers)
        {
            bool isDown = Keyboard.current[trig.Key].isPressed;
            int press = isDown ? 1 : 0; // C# doesn't evaluate bool as 1 or 0

            XboxController.SetTrigger(trig.Value, press);

            // if (isDown) {Debug.Log($"Trigger {trig.Key} is pressed");}
        }


        // Process Left Stick (WASD)
        Vector2 leftInput = GetJoystickVector(Key.W, Key.S, Key.A, Key.D);
        XboxController.SetJoystick(joystickType.Left, leftInput);

        // Process Right Stick (IJKL)
        Vector2 rightInput = GetJoystickVector(Key.I, Key.K, Key.J, Key.L);
        XboxController.SetJoystick(joystickType.Right, rightInput);

    }

    private Vector2 GetJoystickVector(Key up, Key down, Key left, Key right)
    {
        float x = 0;
        float y = 0;

        if (Keyboard.current[up].isPressed) y += 1f;
        if (Keyboard.current[down].isPressed) y -= 1f;
        if (Keyboard.current[right].isPressed) x += 1f;
        if (Keyboard.current[left].isPressed) x -= 1f;

        // Returns a vector with a max length of 1
        return Vector2.ClampMagnitude(new Vector2(x, y), 1f);
    }

}