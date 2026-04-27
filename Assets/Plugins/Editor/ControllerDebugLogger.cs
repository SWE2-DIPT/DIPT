/*******************************************************
* Script:      ControllerDebugLogger.cs
* 
* Description:
*    Central logging utility for controller input events.
*    This file formats pressed, released, and movement logs,
*    adds timestamps to each message, tracks how long buttons
*    are held, and sends log messages through events so editor
*    windows like LogPlugin can display them.
*******************************************************/

using System;
using System.Collections.Generic;

public static class ControllerDebugLogger
{
    public static event Action<string> OnPressedLog;
    public static event Action<string> OnReleasedLog;
    public static event Action<string> OnMovementLog;

    private static readonly Dictionary<string, DateTime> pressTimes = new Dictionary<string, DateTime>();

    /// <summary>
    /// Records when a button is pressed and sends a formatted pressed log event.
    /// </summary>
    public static void LogPressed(string buttonName)
    {
        pressTimes[buttonName] = DateTime.Now;

        OnPressedLog?.Invoke(
            FormatMessage($"{buttonName} pressed")
        );
    }

    /// <summary>
    /// Sends a formatted released log event and includes held duration when possible.
    /// </summary>
    public static void LogReleased(string buttonName)
    {
        string finalMessage = $"{buttonName} released";

        if (pressTimes.TryGetValue(buttonName, out DateTime pressTime))
        {
            TimeSpan heldTime = DateTime.Now - pressTime;
            finalMessage = $"{buttonName} released (held for {heldTime.TotalSeconds:F2}s)";
            pressTimes.Remove(buttonName);
        }

        OnReleasedLog?.Invoke(
            FormatMessage(finalMessage)
        );
    }

    /// <summary>
    /// Sends a formatted movement log event for joysticks and triggers.
    /// </summary>
    public static void LogMovement(string message)
    {
        OnMovementLog?.Invoke(FormatMessage(message));
    }

    /// <summary>
    /// Adds a timestamp to a log message.
    /// </summary>
    private static string FormatMessage(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        return $"[{timestamp}] {message}";
    }
}