using UnityEngine;
using UnityEngine.InputSystem;

public class DummyController : Gamepad
{

    public bool IsConnected { get; set; }
    public Vector2 LeftStick { get; set; }
    public Vector2 RightStick { get; set; }
    public DummyController(bool is_conneced) { IsConnected = is_conneced;  }

    public DummyController(Vector2 left, Vector2 right, bool connected = true)
    {
        LeftStick = left;
        RightStick = right;
        IsConnected = connected;
    }
}
