/*******************************************************
* Script:      ControllerDetection.cs
* Author(s):   Senny Lu
* 
* Description:
*    Controller Emulation for gamepad
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

public class ControllerDetection
{
    // returns name of most recently used controller
    public string FindCurrentController()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            return "No Gamepads Detected";
        }
        return gamepad.layout;
    }

    // returns name of controller in parameter
    public string FindControllerType(Gamepad controller)
    {
        var gamepad = controller;
        if (gamepad == null)
        {
            return "No Gamepads Detected";
        }
        return gamepad.layout;
    }
}