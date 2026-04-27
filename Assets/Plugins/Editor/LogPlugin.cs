/*******************************************************
* Script:      LogPlugin.cs
* Author(s):   Nick Stearns (Add yourselves to this!)
* 
* Description:
*    Unity Editor window that displays controller input logs
*    from ControllerDebugLogger. The window separates pressed,
*    released, and movement events, allows filtering by input
*    type, supports clearing logs, and can save the current
*    filtered log view to a text file.
*******************************************************/

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LogPlugin : EditorWindow
{
    public enum InputState
    {
        All,
        Pressed,
        Released,
        Movement
    }

    private Color selectedFilterColor = new Color(0.8f, 0.8f, 0.8f);

    private class LogEntry
    {
        public InputState state;
        public string message;

        public LogEntry(InputState state, string message)
        {
            this.state = state;
            this.message = message;
        }
    }

    private Vector2 scrollPosition;
    private InputState activeState;
    private readonly List<LogEntry> allLogs = new List<LogEntry>();

    /// <summary>
    /// Opens the controller debug log window from the Unity Tools menu.
    /// </summary>
    [MenuItem("Tools/DIPT/LogPlugin")]
    public static void ShowWindow()
    {
        GetWindow<LogPlugin>("Controller Debug Log");
    }

    /// <summary>
    /// Sets the default filter and subscribes to controller log events.
    /// </summary>
    private void OnEnable()
    {
        activeState = InputState.All;

        ControllerDebugLogger.OnPressedLog += HandlePressedLog;
        ControllerDebugLogger.OnReleasedLog += HandleReleasedLog;
        ControllerDebugLogger.OnMovementLog += HandleMovementLog;
    }

    /// <summary>
    /// Unsubscribes from controller log events when the window closes or reloads.
    /// </summary>
    private void OnDisable()
    {
        ControllerDebugLogger.OnPressedLog -= HandlePressedLog;
        ControllerDebugLogger.OnReleasedLog -= HandleReleasedLog;
        ControllerDebugLogger.OnMovementLog -= HandleMovementLog;
    }

    /// <summary>
    /// Adds a pressed input message to the log list.
    /// </summary>
    private void HandlePressedLog(string message)
    {
        allLogs.Add(new LogEntry(InputState.Pressed, message));
        scrollPosition.y = Mathf.Infinity;
        Repaint();
    }

    /// <summary>
    /// Adds a released input message to the log list.
    /// </summary>
    private void HandleReleasedLog(string message)
    {
        allLogs.Add(new LogEntry(InputState.Released, message));
        scrollPosition.y = Mathf.Infinity;
        Repaint();
    }

    /// <summary>
    /// Adds a movement input message to the log list.
    /// </summary>
    private void HandleMovementLog(string message)
    {
        allLogs.Add(new LogEntry(InputState.Movement, message));
        scrollPosition.y = Mathf.Infinity;
        Repaint();
    }

    /// <summary>
    /// Draws the log window controls, filter buttons, log area, and action buttons.
    /// </summary>
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Filter", GUILayout.Width(50));
        DrawFilterButton(InputState.All, "All");
        DrawFilterButton(InputState.Pressed, "Pressed");
        DrawFilterButton(InputState.Released, "Released");
        DrawFilterButton(InputState.Movement, "Movement");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        DrawLogArea(ref scrollPosition, allLogs, GetFilterTitle());

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear Current", GUILayout.Height(30)))
        {
            ClearCurrent();
        }

        if (GUILayout.Button("Clear All", GUILayout.Height(30)))
        {
            allLogs.Clear();
            Repaint();
        }

        if (GUILayout.Button("Save", GUILayout.Height(30)))
        {
            SaveLogsToFile();
        }

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Returns the heading text that matches the active log filter.
    /// </summary>
    private string GetFilterTitle()
    {
        switch (activeState)
        {
            case InputState.All:
                return "ALL LOGS";

            case InputState.Pressed:
                return "PRESSED LOGS";

            case InputState.Released:
                return "RELEASED LOGS";

            case InputState.Movement:
                return "MOVEMENT LOGS";

            default:
                return "LOGS";
        }
    }

    /// <summary>
    /// Draws the filtered log messages inside a scrollable area.
    /// </summary>
    private void DrawLogArea(ref Vector2 scrollPosition, List<LogEntry> logs, string title)
    {
        DrawFilterTitle(ref scrollPosition, title);

        for (int logIndex = 0; logIndex < logs.Count; logIndex++)
        {
            if (activeState == InputState.All || logs[logIndex].state == activeState)
            {
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = GetColorForState(logs[logIndex].state);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(logs[logIndex].message);
                EditorGUILayout.EndVertical();

                GUI.backgroundColor = originalColor;
            }
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Clears either the current filtered log group or all logs.
    /// </summary>
    private void ClearCurrent()
    {
        if (activeState == InputState.All)
        {
            allLogs.Clear();
        }
        else
        {
            allLogs.RemoveAll(logEntry => logEntry.state == activeState);
        }

        Repaint();
    }

    /// <summary>
    /// Returns the display color used for each log type.
    /// </summary>
    private Color GetColorForState(InputState state)
    {
        switch (state)
        {
            case InputState.Pressed:
                return new Color(0.6f, 1f, 0.6f);

            case InputState.Released:
                return new Color(1f, 0.6f, 0.6f);

            case InputState.Movement:
                return new Color(0.6f, 0.8f, 1f);

            default:
                return Color.white;
        }
    }

    /// <summary>
    /// Draws the current filter title and starts the scroll view.
    /// </summary>
    private void DrawFilterTitle(ref Vector2 scrollPosition, string title)
    {
        Color originalColor = GUI.color;

        GUI.color = GetColorForState(activeState);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        GUI.color = originalColor;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
    }

    /// <summary>
    /// Draws one toolbar filter button and updates the active filter when clicked.
    /// </summary>
    private void DrawFilterButton(InputState state, string label)
    {
        Color originalTextColor = GUI.contentColor;
        Color originalBackground = GUI.backgroundColor;

        GUI.contentColor = GetColorForState(state);

        if (activeState == state)
        {
            GUI.backgroundColor = selectedFilterColor;
        }
        else
        {
            GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
        }

        if (GUILayout.Button(label, EditorStyles.toolbarButton))
        {
            activeState = state;
        }

        GUI.contentColor = originalTextColor;
        GUI.backgroundColor = originalBackground;
    }

    /// <summary>
    /// Saves the current filtered log view to a text file.
    /// </summary>
    private void SaveLogsToFile()
    {
        string path = EditorUtility.SaveFilePanel(
            "Save Controller Logs",
            "",
            "ControllerLogs.txt",
            "txt"
        );

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        try
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
            {
                for (int logIndex = 0; logIndex < allLogs.Count; logIndex++)
                {
                    if (activeState == InputState.All || allLogs[logIndex].state == activeState)
                    {
                        writer.WriteLine($"[{allLogs[logIndex].state}] {allLogs[logIndex].message}");
                    }
                }
            }

            Debug.Log("Logs saved to: " + path);
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Failed to save logs: " + exception.Message);
        }
    }
}