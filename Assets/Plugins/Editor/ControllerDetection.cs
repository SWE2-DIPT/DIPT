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
    [MenuItem("Tools/DIPT/ControllerDetection")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ControllerDectection));
    }
    
    void OnGUI()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            GUILayout.Label("No Gamepads Detected", EditorStyles.boldLabel);
            return;
        }

        GUILayout.Label(gamepad.layout, EditorStyles.boldLabel);
        foreach (var control in gamepad.allControls)
        {
            GUILayout.Label(control.displayName);
        }
    }

}