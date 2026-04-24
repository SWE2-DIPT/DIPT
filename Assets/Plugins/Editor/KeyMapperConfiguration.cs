/**********************************************************
* Script:      KeyMapperConfiguration.cs
* Author(s):   Andrew Bradnao (Add yourselves to this!)
* 
* Description:
*    Allows user to customize their keybinds for Keymapping
*    This code utlizes an Event Listener concept
************************************************************/

using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System;

public class KeyMapperConfiguration : EditorWindow
{
    private Vector2 scrollPos;
    private bool isEnteringCustomKeybind = false; // Keeps track if the player is entering a keybind or not
    private object whichBtnType = null; // an empty object that is like a "box" that holds anything

    private class JoystickTarget
    {
        public joystickType stick; // Left or Right
        public Vector2 dir; // Up Down Left Right

        // These two lines below ensure that there is not an issue of two objects being made 
        // with the same stick and direction but have different memeory locations
        // this is a C# issue
        public override bool Equals(object obj) => obj is JoystickTarget o && o.stick == stick && o.dir == dir;
        public override int GetHashCode() => HashCode.Combine(stick, dir);
    }


    [MenuItem("Tools/DIPT/KeyboardMapper")]
    public static void ShowWindow()
    {
        GetWindow<KeyMapperConfiguration>("DIPT Keyboard Mapper");
    }

    void OnGUI()
    {

        if (isEnteringCustomKeybind) HandleRebindInput(); // If they click the keybind button to change, handle it

        GUILayout.Label("DIPT Input Configuration", EditorStyles.boldLabel);

        // Top of Bar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Reset to Default", EditorStyles.toolbarButton))
        {
            if (EditorUtility.DisplayDialog("Reset Keybinds?",
                "Are you sure you want to reset all keybinds to the original DIPT default Keybinds?",
                "Yes", "Cancel"))
            {
                KeyMapper.ResetToDefaults();
                Repaint();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);
        DrawRowHeader();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // For the Buttons section
        DrawSectionHeader("Buttons");
        foreach (buttonType action in Enum.GetValues(typeof(buttonType)))
        {
            // Condition: Is the user entering a Keybind, is the whichBtnType object box holding something,
            // and is that something in the box what we are looking for 
            string displayKey = (isEnteringCustomKeybind && whichBtnType is buttonType b && b == action) ? "???" : KeyMapper.GetKeyForButton(action);
            DrawRow(action.ToString(), displayKey, action);
        }

        // For the Triggers section
        EditorGUILayout.Space(10);
        DrawSectionHeader("Triggers");
        foreach (triggerType action in Enum.GetValues(typeof(triggerType)))
        {
            string displayKey = (isEnteringCustomKeybind && whichBtnType is triggerType t && t == action) ? "???" : KeyMapper.GetKeyForTrigger(action);
            DrawRow(action.ToString(), displayKey, action);
        }

        // For the Joystick section
        EditorGUILayout.Space(10);
        DrawSectionHeader("Joysticks");
        foreach (joystickType stick in Enum.GetValues(typeof(joystickType)))
        {
            Vector2[] direction = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            string[] names = { "Up", "Down", "Left", "Right" };
            for (int i = 0; i < 4; i++)
            {
                string label = $"{stick} {names[i]}";
                // This line below is basically saying, is the user putting in a custom keybind, and does the stick
                // match left or right, and the direction?
                bool isThisOne = isEnteringCustomKeybind && whichBtnType is JoystickTarget jt && jt.stick == stick && jt.dir == direction[i];
                string displayKey = isThisOne ? "???" : KeyMapper.GetKeyForJoystick(stick, direction[i]);
                DrawRow(label, displayKey, new JoystickTarget { stick = stick, dir = direction[i] });
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawRowHeader()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ACTION", EditorStyles.boldLabel, GUILayout.Width(150));
        GUILayout.Label("KEYBIND", EditorStyles.boldLabel, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
    }

    private void DrawRow(string left, string right, object action) // the object action holds if it is a button, trigger, or joystick
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(left, GUILayout.Width(150));

        if (isEnteringCustomKeybind && whichBtnType != null && whichBtnType.Equals(action))
        {
            GUI.color = Color.yellow;
        }

        if (GUILayout.Button(right, GUILayout.Width(150)))
        {
            isEnteringCustomKeybind = true;
            whichBtnType = action;
        }
        GUI.color = Color.white; // need to do this because ever row after the clicked one will
        // become yellow 
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSectionHeader(string t)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label(t, EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.EndVertical();
    }

    private void HandleRebindInput()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown) // If a Key, any Key was pressed down
        {
            if (e.keyCode == KeyCode.Escape) // If they press escape, leaves the listening state 
            {
                StopListening();
                e.Use(); // Tells Unity we are done with this current event
                return;
            }

            string keyName = e.keyCode.ToString(); // This converts the Unity enum version 
            // of the KeyCode to a readable string. For example 0 would be "Up"

            if (Enum.IsDefined(typeof(Key), keyName)) 
            {
                Key detectedKey = (Key)Enum.Parse(typeof(Key), keyName);

                if (!KeyMapper.IsKeyForbidden(detectedKey)) // Key's that are not allowed in KeyMapper.cs
                {
                    ApplyNewBind(detectedKey);
                }

                StopListening();
                e.Use();
            }
        }
    }

    private void ApplyNewBind(Key pressedKey)
    {
        // 'is' operator handles casting and checking.
        // in the line 'whichBtnType is buttonType btn'
        // checks if 'whichBtnType' is infact a variable type 'buttonType' and if
        // it is, it stores in the created variable 'btn'
        if (whichBtnType is buttonType btn)
            KeyMapper.SetButtonBind(pressedKey, btn);
        else if (whichBtnType is triggerType trg)
            KeyMapper.SetTriggerBind(pressedKey, trg);
        else if (whichBtnType is JoystickTarget joystick)
            KeyMapper.SetJoystickBind(pressedKey, joystick.stick, joystick.dir);

        KeyMapper.SaveBinds(); // Saves to EditorPrefs

        Debug.Log($"DIPT: Successfully bound {pressedKey} and saved to registry.");
    }

    private void StopListening() { isEnteringCustomKeybind = false; whichBtnType = null; Repaint(); }

    
}