using System.Collections.Generic;

/// <summary>
/// Dictionaries <see langword="for"/> converting between different identifiers that describe similar
/// buttons.  
/// </summary>
public static class Dictionaries
{
    public static readonly Dictionary<string, buttonType> visElToButton = new()
    {
        { "A-button", buttonType.A },
        { "B-button", buttonType.B },
        { "X-button", buttonType.X },
        { "Y-button", buttonType.Y },

        { "RB-button", buttonType.RBumper },
        { "LB-button", buttonType.LBumper },

        { "up-pad", buttonType.Up },
        { "down-pad", buttonType.Down },
        { "left-pad", buttonType.Left },
        { "right-pad", buttonType.Right },

        { "right-stick", buttonType.RightStick },
        { "left-stick", buttonType.LeftStick },

        { "xbox-button", buttonType.Xbox },
        { "menu-button", buttonType.Menu },
        { "view-button", buttonType.View },
        { "share-button", buttonType.Share },

        { "advanced", buttonType.Advanced }
    };
    public static readonly Dictionary<string, triggerType> visElToTrigger = new()
    {
        { "LT-button", triggerType.Left },
        { "RT-button", triggerType.Right }
    };
    public static readonly Dictionary<string, joystickType> visElToJoystick = new()
    {
        { "left-joystick", joystickType.Left },
        { "right-joystick", joystickType.Right }
    };

    public static readonly Dictionary<string, string> visElToEmulatorButton = new()
    {
        { "A-button", "A" },
        { "B-button", "B" },
        { "X-button", "X" },
        { "Y-button", "Y" },

        { "RB-button", "RightShoulder" },
        { "LB-button", "LeftShoulder" },

        { "up-pad", "DpadUp" },
        { "down-pad", "DpadDown" },
        { "left-pad", "DpadLeft" },
        { "right-pad", "DpadRight" },

        { "right-stick", "RightStick" },
        { "left-stick", "LeftStick" },
    };
}