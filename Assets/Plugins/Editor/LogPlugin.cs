/*******************************************************
* Script:      LogPlugin.cs
* Author(s):   Nicholas Stearns
* 
* Description:
*    
*******************************************************/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// An example plugin.
/// </summary>
public class LogPlugin : EditorWindow
{
    public enum InputState { Pressed, Released, Movement }
    private InputState activeState = InputState.Pressed;

    private List<string> pressedLogs = new List<string>();
    private List<string> releasedLogs = new List<string>();
    private List<string> movementLogs = new List<string>();

    private Vector2 scrollPosPressed, scrollPosReleased, scrollPosMovement;

    [MenuItem("Tools/DIPT/LogPlugin")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LogPlugin));
    }

    void OnGUI()
    {
        // Header Toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Filter:", GUILayout.Width(50));

        string[] options = { "Show Pressed", "Show Released", "Show Movement" };
        activeState = (InputState)GUILayout.Toolbar((int)activeState, options, EditorStyles.toolbarButton);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);

        // Active Log Area
        switch (activeState)
        {
            case InputState.Pressed:
                DrawLogArea(ref scrollPosPressed, pressedLogs, "PRESSED LOGS");
                break;
            case InputState.Released:
                DrawLogArea(ref scrollPosReleased, releasedLogs, "RELEASED LOGS");
                break;
            case InputState.Movement:
                DrawLogArea(ref scrollPosMovement, movementLogs, "MOVEMENT LOGS");
                break;
        }

        EditorGUILayout.Space(5);

        // Footer Actions
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Dummy Log", GUILayout.Height(30)))
        {
            AddLog(activeState);
        }
        if (GUILayout.Button("Clear Current", GUILayout.Height(30)))
        {
            ClearCurrent();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Renders the scrollable list of logs for a specific state.
    /// </summary>
    void DrawLogArea(ref Vector2 scrollPos, List<string> logs, string title)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        for (int i = 0; i < logs.Count; i++)
        {
            EditorGUILayout.LabelField(logs[i], EditorStyles.helpBox);
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Adds a message to the corresponding list and forces a scroll to bottom.
    /// </summary>
    public void AddLog(InputState state)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string entry = $"[{timestamp}] New {state} entry detected.";

        switch (state)
        {
            case InputState.Pressed:
                pressedLogs.Add(entry);
                scrollPosPressed.y = Mathf.Infinity;
                break;
            case InputState.Released:
                releasedLogs.Add(entry);
                scrollPosReleased.y = Mathf.Infinity;
                break;
            case InputState.Movement:
                movementLogs.Add(entry);
                scrollPosMovement.y = Mathf.Infinity;
                break;
        }

        // Repaint forces the EditorWindow to refresh immediately
        Repaint();
    }

    void ClearCurrent()
    {
        if (activeState == InputState.Pressed) pressedLogs.Clear();
        else if (activeState == InputState.Released) releasedLogs.Clear();
        else movementLogs.Clear();

        Repaint();
    }
}
