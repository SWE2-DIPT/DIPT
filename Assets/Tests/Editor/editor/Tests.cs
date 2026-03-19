using NUnit.Framework;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Tests
{
    private ControllerGUI window;
    private ControllerDectection controllerDectection;
    private TEST_ControllerManager TEST_CM;
    private ControllerManager controller;
    
    [SetUp]
    public void Setup()
    {
        window = EditorWindow.GetWindow<ControllerGUI>();
        controllerDectection = EditorWindow.GetWindow<ControllerDectection>();
        controller = new ControllerManager();
        TEST_CM = new TEST_ControllerManager();
        
    }

    [TearDown]
    public void TearDown()
    {
        if (controllerDectection != null)
        {
            controllerDectection.Close();
        }
        if (window != null)
        {
            window.Close();
        }
    }

    [Test]
    public void WindowCreation()
    {
        Assert.IsNotNull(window, "TestPlugin window should be created.");
    }
    [Test]
    public void ControllerCheckTrue()
    {
        var dummy = new DummyController()
        {
            IsConnected = true
        };

        TEST_CM.d_controller = dummy;

        Debug.Log("controller " + TEST_CM.d_controller.ToString());

        bool result = TEST_CM.check_Gamepad();

        Assert.IsTrue(result);
    }

    [Test]
    public void ControlelrCheckFalse()
    {
        var dummy = new DummyController()
        {
            IsConnected = false
        };

        TEST_CM.d_controller = dummy;

        Debug.Log("controller " + TEST_CM.d_controller.ToString());

        bool result = TEST_CM.check_Gamepad();

        Assert.IsFalse(result);
    }
   
    [Test]
    public void WindowClosing()
    {
        window.Close();
    }

    [Test]
    public void WindowNull()
    {
        window = null;
        Assert.IsNull(window, "Window should be null.");
    }

    // Test for no Controller plugged in
    [Test]
    public void NoController()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad)
            {
                InputSystem.RemoveDevice(device);
            }
        }
        controllerDectection.FindControllerType();
        Assert.AreEqual("No Gamepads Detected", controllerDectection.controllerType);
    }

    // Test for a Controller plugged in
    [Test]
    public void AController()
    {
        Gamepad virtualDevice = InputSystem.AddDevice<Gamepad>();
        controllerDectection.FindControllerType();
        Assert.AreEqual("Gamepad", controllerDectection.controllerType);
        InputSystem.RemoveDevice(virtualDevice);
    }

    // Some more things we should test.
    // 1.) Test that UI elements are created
    //     - Load in all the buttons
    // 2.)
}