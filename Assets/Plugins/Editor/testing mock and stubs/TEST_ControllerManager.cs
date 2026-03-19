using UnityEngine;

public class TEST_ControllerManager
{
    public DummyController d_controller;

    public bool check_Gamepad()
    {
        return d_controller != null && d_controller.IsConnected;
    }
}
