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
    private TEST_ControllerManager TEST_CM;
    /*JARRETT - I comment this out*/
    //private ControllerManager controller;
    private ControllerComponents components;
    
    [SetUp]
    public void Setup()
    {
        window = EditorWindow.GetWindow<ControllerGUI>();
        TEST_CM = new TEST_ControllerManager();
    }

    [TearDown]
    public void TearDown()
    {
        if (window != null)
            window.Close();
    }

    [Test]
    public void WindowCreation()
    {
        Assert.IsNotNull(window, "TestPlugin window should be created.");
    }


    [Test]
    public void ControllerCheckTrue()
    {
        var dummy = new DummyController
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
        var dummy = new DummyController
        {
            IsConnected = false
        };

        TEST_CM.d_controller = dummy;

        Debug.Log("controller " + TEST_CM.d_controller.ToString());

        bool result = TEST_CM.check_Gamepad();

        Assert.IsFalse(result);
    }
}