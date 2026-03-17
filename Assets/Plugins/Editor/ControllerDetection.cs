/*******************************************************
* Script:      ControllerDetection.cs
* Author(s):   Senny Lu (Add yourselves to this!)
* 
* Description:
*    
*******************************************************/

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

/// <summary>
/// An example plugin.
/// </summary>
public class ControllerDectection : EditorWindow
{
    public string controllerType = "";

    [MenuItem("Tools/DIPT/ControllerDetection")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ControllerDectection));
    }
    
    public void FindControllerType()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            controllerType = "No Gamepads Detected";
            return;
        }
        controllerType = gamepad.layout;
    }

    void OnGUI()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            FindControllerType();
            GUILayout.Label(controllerType, EditorStyles.boldLabel);
            return;
        }

        FindControllerType();
        GUILayout.Label(controllerType, EditorStyles.boldLabel);
        foreach (var control in gamepad.allControls)
        {
            GUILayout.Label(control.displayName);
        }
    }

}