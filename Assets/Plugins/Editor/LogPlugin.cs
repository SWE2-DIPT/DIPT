using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LogPlugin : EditorWindow
{
    public enum InputState { All, Pressed, Released, Movement }

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

    private Vector2 scrollPos;
    private InputState activeState = InputState.All;
    private readonly List<LogEntry> allLogs = new List<LogEntry>();
    private ControllerComponents controllerReader;
    private bool isPolling = true;

    [MenuItem("Tools/DIPT/LogPlugin")]
    public static void ShowWindow()
    {
        GetWindow<LogPlugin>("Controller Debug Log");
    }

    private void OnEnable()
    {
        controllerReader = new ControllerComponents();

        ControllerDebugLogger.OnPressedLog += HandlePressedLog;
        ControllerDebugLogger.OnReleasedLog += HandleReleasedLog;
        ControllerDebugLogger.OnMovementLog += HandleMovementLog;

        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        ControllerDebugLogger.OnPressedLog -= HandlePressedLog;
        ControllerDebugLogger.OnReleasedLog -= HandleReleasedLog;
        ControllerDebugLogger.OnMovementLog -= HandleMovementLog;

        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate()
    {
        if (!isPolling)
            return;

        if (controllerReader == null)
            return;

        controllerReader.GetJoystickActivity();
        controllerReader.GetTriggerActivity();
        controllerReader.GetButtonActivity();
    }

    private void HandlePressedLog(string message)
    {
        allLogs.Add(new LogEntry(InputState.Pressed, message));
        scrollPos.y = Mathf.Infinity;
        Repaint();
    }

    private void HandleReleasedLog(string message)
    {
        allLogs.Add(new LogEntry(InputState.Released, message));
        scrollPos.y = Mathf.Infinity;
        Repaint();
    }

    private void HandleMovementLog(string message)
    {
        allLogs.Add(new LogEntry(InputState.Movement, message));
        scrollPos.y = Mathf.Infinity;
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Filter:", GUILayout.Width(50));

        string[] options = { "Show All", "Show Pressed", "Show Released", "Show Movement" };
        activeState = (InputState)GUILayout.Toolbar((int)activeState, options, EditorStyles.toolbarButton);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);

        isPolling = EditorGUILayout.Toggle("Polling Enabled", isPolling);

        EditorGUILayout.Space(5);

        DrawLogArea(ref scrollPos, allLogs, GetFilterTitle());

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal(); // DEBUG LINES

        if (GUILayout.Button("Add Pressed Debug", GUILayout.Height(30))) // DEBUG LINES
        {
            HandlePressedLog(GetDebugMessage(InputState.Pressed)); // DEBUG LINES
        }

        if (GUILayout.Button("Add Released Debug", GUILayout.Height(30))) // DEBUG LINES
        {
            HandleReleasedLog(GetDebugMessage(InputState.Released)); // DEBUG LINES
        }

        if (GUILayout.Button("Add Movement Debug", GUILayout.Height(30))) // DEBUG LINES
        {
            HandleMovementLog(GetDebugMessage(InputState.Movement)); // DEBUG LINES
        }

        EditorGUILayout.EndHorizontal(); // DEBUG LINES

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear Current", GUILayout.Height(30)))
            ClearCurrent();

        if (GUILayout.Button("Clear All", GUILayout.Height(30)))
        {
            allLogs.Clear();
            Repaint();
        }

        EditorGUILayout.EndHorizontal();
    }

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

    private string GetDebugMessage(InputState state) // DEBUG LINES
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss"); // DEBUG LINES

        switch (state) // DEBUG LINES
        {
            case InputState.Pressed: // DEBUG LINES
                return $"[{timestamp}] DEBUG pressed log"; // DEBUG LINES

            case InputState.Released: // DEBUG LINES
                return $"[{timestamp}] DEBUG released log"; // DEBUG LINES

            case InputState.Movement: // DEBUG LINES
                return $"[{timestamp}] DEBUG movement log"; // DEBUG LINES

            default: // DEBUG LINES
                return $"[{timestamp}] DEBUG log"; // DEBUG LINES
        }
    }

    private void DrawLogArea(ref Vector2 scrollPos, List<LogEntry> logs, string title)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        for (int i = 0; i < logs.Count; i++)
        {
            if (activeState == InputState.All || logs[i].state == activeState)
            {
                EditorGUILayout.LabelField(logs[i].message, EditorStyles.helpBox);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void ClearCurrent()
    {
        if (activeState == InputState.All)
        {
            allLogs.Clear();
        }
        else
        {
            allLogs.RemoveAll(log => log.state == activeState);
        }

        Repaint();
    }
}