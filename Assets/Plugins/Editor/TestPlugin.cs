using UnityEngine;
using UnityEditor;

// For Unity to recognize this file as a plugin, it must be in a folder named "Editor".
// To use this plugin, you must:
//    1.) Open this project (the DIPT directory) in Unity.
//    2.) Go to Tools > DIPT > TestPlugin from the menu bar (the bar with File, Edit, Assets, etc).
// Right now, it does not do anything.

// Below is an XML comment, it will display in VS & VSCode when hovering over
// a field/method. I will use these whenever creating a method that will be called
// in another file.

/// <summary>
/// An example plugin.
/// </summary>

public class TestPlugin : EditorWindow
{
    [MenuItem("Tools/DIPT/Project")]
    public static void InitProjectSetupTool()
    {
        
    }
}
