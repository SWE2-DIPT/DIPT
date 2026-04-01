/*******************************************************
* Script:      KeyMapping.cs
* Author(s):   Andrew Bradnao (Add yourselves to this!)
* 
* Description:
*    Allows user to map keybinds to interact with gui.
*******************************************************/
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyMapper
{
    private ControllerComponents _components;

    public KeyMapper(ControllerComponents components)
    {
        _components = components;
    }

    /// <summary>
    /// Reads the keyboard and updates the ControllerComponents state.
    /// Call this in your EditorWindow's Update() loop.
    /// </summary>
    public void UpdateKeyboardEmulation()
    {
        if (Keyboard.current == null) return;

        // Map 'W' Key to D-Pad Up
        bool wPressed = Keyboard.current.wKey.isPressed;
        _components.SetDpadUp(wPressed);

        // Map 'S' Key to D-Pad Down
        bool sPressed = Keyboard.current.sKey.isPressed;
        _components.SetDpadDown(sPressed);

        // Map 'A' Key to D-Pad Left
        bool aPressed = Keyboard.current.aKey.isPressed;
        _components.SetDpadLeft(aPressed);

        // Map 'D' Key to D-Pad Right
        bool dPressed = Keyboard.current.dKey.isPressed;
        _components.SetDpadRight(dPressed);

        // Map 'Space' to A-Button (Bottom Face)
        bool spacePressed = Keyboard.current.spaceKey.isPressed;
        _components.SetBottomFaceButton(spacePressed);

        // Add more mappings here as needed
    }
}