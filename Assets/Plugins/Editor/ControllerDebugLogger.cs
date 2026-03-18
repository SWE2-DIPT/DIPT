using System;

public static class ControllerDebugLogger
{
    public static event Action<string> OnPressedLog;
    public static event Action<string> OnReleasedLog;
    public static event Action<string> OnMovementLog;

    public static void LogPressed(string message)
    {
        OnPressedLog?.Invoke(FormatMessage(message));
    }

    public static void LogReleased(string message)
    {
        OnReleasedLog?.Invoke(FormatMessage(message));
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