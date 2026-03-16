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
    private DummyController DummyController;
    private ControllerManager controller;
    
    [SetUp]
    public void Setup()
    {
        window = EditorWindow.GetWindow<TestPlugin>();
        controller = new ControllerManager();
    }

    [TearDown]
    public void TearDown()
    {
     
    }

    [Test]
    public void WindowCreation()
    {
        Assert.IsNotNull(window, "TestPlugin window should be created.");
    }

<<<<<<< Updated upstream

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


=======
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

    // Some more things we should test.
    // 1.) Test that UI elements are created
    //     - Load in all the buttons
    // 2.) 
>>>>>>> Stashed changes
}