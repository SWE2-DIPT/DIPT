using NUnit.Framework;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Tests
{
    private TestPlugin window;
    private ControllerDectection controllerDectection;
    private DummyController DummyController;
    private ControllerManager controller;
    
    [SetUp]
    public void Setup()
    {
        window = EditorWindow.GetWindow<TestPlugin>();
        controllerDectection = EditorWindow.GetWindow<ControllerDectection>();
        controller = new ControllerManager();
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
        var dummy = new DummyController(true);

        controller.current_gamepad = dummy;

        bool result = controller.check_gamepad();

        Assert.IsTrue(result);
    }
    
    [Test]
    public void ControllerJoystickTest()
    {
        var dummy = new DummyController(true);

        
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