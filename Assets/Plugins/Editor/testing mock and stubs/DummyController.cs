using UnityEngine;
using UnityEngine.InputSystem;

public class DummyController
{

    public bool IsConnected { get; set; }
    public Vector2 LeftStick { get; set; }
    public Vector2 RightStick { get; set; }
    public float RightTrigger { get; set; }
    public float LeftTrigger { get; set; }

    public bool button_a, button_b, button_c, button_d;
    public DummyController(bool is_conneced = true) { IsConnected = is_conneced;  }

  
}
