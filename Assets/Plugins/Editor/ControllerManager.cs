using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Windows.Speech; //XInput is for xbox

enum ControllerType { physical, artificial }


public class ControllerManager
{
    
    private Gamepad physicalGamepad;
    private Gamepad artificalGamepad;

    public Gamepad GetPadType()
    {
        physicalGamepad = null;
        artificalGamepad = null;

        foreach (var pad in Gamepad.all)
        {
            if (pad is XInputController || pad is DualShockGamepad)
            {
                physicalGamepad = pad;
                Debug.Log($"Physical: {pad.displayName}");
                break;
            }
        }

        // fallback: anything else that exists
        if (physicalGamepad == null)
        {
            foreach (var pad in Gamepad.all)
            {
                artificalGamepad = pad;
                // Debug.Log($"Fallback: {pad.displayName}");
                break;
            }
        }

        return physicalGamepad ?? artificalGamepad;
    }
}