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


}