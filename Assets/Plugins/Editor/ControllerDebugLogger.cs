using System;
using System.Collections.Generic;

public static class ControllerDebugLogger
{
    public static event Action<string> OnPressedLog;
    public static event Action<string> OnReleasedLog;
    public static event Action<string> OnMovementLog;

    private static readonly Dictionary<string, DateTime> pressTimes = new Dictionary<string, DateTime>();

    public static void LogPressed(string buttonName)
    {
        pressTimes[buttonName] = DateTime.Now;
        OnPressedLog?.Invoke(
            FormatMessage($"{buttonName} pressed")
        );
    }

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

    public static void LogMovement(string message)
    {
        OnMovementLog?.Invoke(FormatMessage(message));
    }

    private static string FormatMessage(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        return $"[{timestamp}] {message}";
    }
}