using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Tests
{
    private TestPlugin window;
    private TestControllerConnectivity controller;

    [SetUp]
    public void Setup()
    {
        window = EditorWindow.GetWindow<TestPlugin>();
        controller = new TestControllerConnectivity();
            
    }

    [TearDown]
    public void TearDown()
    {
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
    public void ControllerCheck()
    {
        controller.check_gamepad();
    }
}