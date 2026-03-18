using UnityEngine;
using UnityEngine.InputSystem;

public class DummyController : Gamepad
{

    public bool IsConnected { get; set; }
    public Vector2 LeftStick { get; set; }
    public Vector2 RightStick { get; set; }
    public float RightTrigger { get; set; }
    public float LeftTrigger { get; set; }

    public bool button_a, button_b, button_c, button_d;
    public DummyController(bool is_conneced) { IsConnected = is_conneced;  }

    public DummyController(Vector2 left, Vector2 right, bool connected = true)
    {
        LeftStick = left;
        RightStick = right;
        IsConnected = connected;
    }

    public DummyController(float l_trigger, float r_trigger)
    {
        LeftTrigger = l_trigger;
        RightTrigger = r_trigger;
    }

    //can use this for both dpads and face buttons on the gamepad for testing;
    public DummyController(bool _button_a, bool _button_b, bool _button_c, bool _button_d)
    {
       
    } 

    //can use this for R3, bumpers, and select and start buttons
    public DummyController(bool _button_x, bool _button_y)
    {

    }

        

        
    
}
