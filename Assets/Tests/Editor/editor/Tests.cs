using NUnit.Framework;
using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class Test 
{
    private ControllerGUI window;
    private DummyController DummyController;
    private ControllerManager controller;
    private ControllerComponents components;
    
    [SetUp]
    public void Setup()
    {
        window = EditorWindow.GetWindow<ControllerGUI>();
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
        var dummy = new DummyController(new Vector2(-0.5f, 0.8f), new Vector2(0.5f, -0.8f));
        controller.current_gamepad = dummy;

        Vector2 left_stick = components.get_left_stick();
        Vector2 right_stick = components.get_right_stick();

        Assert.AreEqual(new Vector2(-0.5f, 0.8f), left_stick);
        Assert.AreEqual(new Vector2(0.5f, -0.8f), right_stick);
    }
}