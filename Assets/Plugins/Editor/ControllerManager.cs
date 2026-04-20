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

    public Gamepad GetPhysicalPad()
    {
        foreach (var pad in Gamepad.all)
        {
            if (pad is XInputController)
            {
                return pad;
            }

            if (pad is DualShockGamepad)
            {
                return pad;
            }

            if (pad is DualSenseGamepadHID)
            {
                return pad;
            }
        }

        return null;
    }

    public Gamepad GetArtificialPad()
    {
        foreach (var pad in Gamepad.all)
        {
            if (!(pad is XInputController) && !(pad is DualShockGamepad))
            {
                return pad;
            }
        }

        return null;
    }
    // public Gamepad GetPadType()
    // {
    //     physicalGamepad = null;
    //     artificalGamepad = null;

    //     foreach (var pad in Gamepad.all)
    //     {
    //         if (pad is XInputController || pad is DualShockGamepad)
    //         {
    //             physicalGamepad = pad;
    //             artificalGamepad = null;
    //             Debug.Log($"Physical: {pad.displayName}");
    //             break;
    //         }
    //     }

    //     // fallback: anything else that exists
    //     if (physicalGamepad == null)
    //     {
    //         foreach (var pad in Gamepad.all)
    //         {
    //             artificalGamepad = pad;
    //             // Debug.Log($"Fallback: {pad.displayName}");
    //             break;
    //         }
    //     }

    //     return physicalGamepad ?? artificalGamepad;
    // }
}