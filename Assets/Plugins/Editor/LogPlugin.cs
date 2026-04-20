/*******************************************************
* Script:      LogPlugin.cs
* Author(s):   Nick Stearns (Add yourselves to this!)
* 
* Description:
*    A example plugin meant to showcase how to create plugins
*    in Unity.
*******************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class LogPlugin : EditorWindow
{
    public enum InputState { All, Pressed, Released, Movement }

    Color grey = new Color(0.8f, 0.8f, 0.8f);


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
    private InputState activeState;
    private readonly List<LogEntry> allLogs = new List<LogEntry>();
    private Gamepad currentController;
    private ControllerComponents controllerReader;
    private bool isPolling = true;

    [MenuItem("Tools/DIPT/LogPlugin")]
    public static void ShowWindow()
    {
        GetWindow<LogPlugin>("Controller Debug Log");
    }

    private void OnEnable()
    {
        activeState = InputState.All;
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

        if (currentController != Gamepad.current)
        {
            currentController = Gamepad.current;
            controllerReader = new ControllerComponents();
        }

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
        GUILayout.Label("Filter", GUILayout.Width(50));
        DrawFilterButton(InputState.All, "All");
        DrawFilterButton(InputState.Pressed, "Pressed");
        DrawFilterButton(InputState.Released, "Released");
        DrawFilterButton(InputState.Movement, "Movement");
        EditorGUILayout.EndHorizontal();

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
        DrawFilterTitle(ref scrollPos, title);

        for (int i = 0; i < logs.Count; i++)
        {
            if (activeState == InputState.All || logs[i].state == activeState)
            {
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = GetColorForState(logs[i].state);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(logs[i].message);
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = originalColor;
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
    private Color GetColorForState(InputState state)
    {
        switch (state)
        {
            case InputState.Pressed:
                return new Color(0.6f, 1f, 0.6f); // light green
            case InputState.Released:
                return new Color(1f, 0.6f, 0.6f); // light red
            case InputState.Movement:
                return new Color(0.6f, 0.8f, 1f); // light blue

            default:
                return Color.white;
        }
    }

    private void DrawFilterTitle(ref Vector2 scrollPos, string title)
    {
        Color oColor = GUI.color;
        GUI.color = GetColorForState(activeState);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        GUI.color = oColor;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
    }

    private void DrawFilterButton(InputState state, string label)
    {
        Color originalTextColor = GUI.contentColor;
        Color originalBackground = GUI.backgroundColor;

        GUI.contentColor = GetColorForState(state);
        // Active button text color
        if (activeState == state)
        {
            GUI.backgroundColor = grey;
        }
        else
        {
            //GUI.contentColor = Color.white;
            GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
        }

        if (GUILayout.Button(label, EditorStyles.toolbarButton))
        {
            activeState = state;
        }

        // Restore colors
        GUI.contentColor = originalTextColor;
        GUI.backgroundColor = originalBackground;
    }

    private void SaveLogsToFile()
    {
        string path = EditorUtility.SaveFilePanel("Save Controller Logs", "", "ControllerLogs.txt", "txt");

        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        try
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
            {
                for (int i = 0; i < allLogs.Count; i++)
                {
                    if (activeState == InputState.All || allLogs[i].state == activeState)
                    {
                        writer.WriteLine($"[{allLogs[i].state}] {allLogs[i].message}");
                    }
                }
            }

            Debug.Log("Logs saved to: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save logs: " + e.Message);
        }
    }
}