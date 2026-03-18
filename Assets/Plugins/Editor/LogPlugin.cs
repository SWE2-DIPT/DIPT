using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LogPlugin : EditorWindow
{
    public enum InputState { Pressed, Released, Movement }
    private InputState activeState = InputState.Pressed;

    private readonly List<string> pressedLogs = new List<string>();
    private readonly List<string> releasedLogs = new List<string>();
    private readonly List<string> movementLogs = new List<string>();

    private Vector2 scrollPosPressed, scrollPosReleased, scrollPosMovement;

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
        pressedLogs.Add(message);
        scrollPosPressed.y = Mathf.Infinity;
        Repaint();
    }

    private void HandleReleasedLog(string message)
    {
        releasedLogs.Add(message);
        scrollPosReleased.y = Mathf.Infinity;
        Repaint();
    }

    private void HandleMovementLog(string message)
    {
        movementLogs.Add(message);
        scrollPosMovement.y = Mathf.Infinity;
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Filter:", GUILayout.Width(50));

        string[] options = { "Show Pressed", "Show Released", "Show Movement" };
        activeState = (InputState)GUILayout.Toolbar((int)activeState, options, EditorStyles.toolbarButton);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        isPolling = EditorGUILayout.Toggle("Polling Enabled", isPolling);

        EditorGUILayout.Space(5);

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

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Current", GUILayout.Height(30)))
            ClearCurrent();

        if (GUILayout.Button("Clear All", GUILayout.Height(30)))
        {
            pressedLogs.Clear();
            releasedLogs.Clear();
            movementLogs.Clear();
            Repaint();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLogArea(ref Vector2 scrollPos, List<string> logs, string title)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        for (int i = 0; i < logs.Count; i++)
            EditorGUILayout.LabelField(logs[i], EditorStyles.helpBox);

        EditorGUILayout.EndScrollView();
    }

    private void ClearCurrent()
    {
        if (activeState == InputState.Pressed)
            pressedLogs.Clear();
        else if (activeState == InputState.Released)
            releasedLogs.Clear();
        else
            movementLogs.Clear();

        Repaint();
    }
}