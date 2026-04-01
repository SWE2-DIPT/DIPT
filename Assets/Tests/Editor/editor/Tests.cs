using NUnit.Framework;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class Tests
{
    private ControllerGUI window;
    private ControllerDetection controllerDectection;
    private TEST_ControllerManager TEST_CM;
    private ControllerManager controller;
    
    [SetUp]
    public void Setup()
    {
        controllerDectection = new ControllerDetection();
        window = EditorWindow.GetWindow<ControllerGUI>();
        controller = new ControllerManager();
        TEST_CM = new TEST_ControllerManager();
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
        Assert.IsTrue(window == null, "Window should be null.");
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
        string controllerType = controllerDectection.FindControllerType(null);
        Assert.AreEqual("No Gamepads Detected", controllerType);
    }

    // Test for a Controller plugged in
    [Test]
    public void AController()
    {
        Gamepad virtualDevice = InputSystem.AddDevice<Gamepad>();
        string controllerType = controllerDectection.FindCurrentController();
        Assert.AreEqual("Gamepad", controllerType);
        InputSystem.RemoveDevice(virtualDevice);
    }

    // GamepadEmulator Tests
    
    // Test for gamepad creation
    // 1 additional gamepad input device should be created
    [Test]
    public void GamepadEmulatorAddGamePad()
    {
        int initialGamepads = InputSystem.devices.OfType<Gamepad>().Count();

        GamepadEmulator emulator = new GamepadEmulator();
        int numberGamepads = InputSystem.devices.OfType<Gamepad>().Count();

        Assert.AreEqual(initialGamepads+1, numberGamepads);

        emulator.dispose();
    }

    // Test for gamepad removal
    // 1 gamepad input device should be created and removed
    [Test]
    public void GamepadEmulatorDisposeGamePad()
    {
        int initialGamepads = InputSystem.devices.OfType<Gamepad>().Count();

        GamepadEmulator emulator = new GamepadEmulator();
        emulator.dispose();
        int numberGamepads = InputSystem.devices.OfType<Gamepad>().Count();

        Assert.AreEqual(initialGamepads, numberGamepads);
    }

    // Test for pressButton function
    // should simulate button press for each button
    [Test]
    public void GamepadEmulatorPressButton()
    {
        GamepadEmulator emulator = new GamepadEmulator();
        Gamepad inputDevice = emulator.getGamepad();

        // press each button and assert button is pressed
        for (int i = 0; i < 14; i++)
        {
            emulator.pressButton(i);
            emulator.emulate();
            InputSystem.Update();
            Assert.IsTrue(inputDevice[(GamepadButton)i].isPressed, "Button should be pressed");
        }

        emulator.dispose();
    }

    // Test for pressButton function
    // should simulate button release for each button
    [Test]
    public void GamepadEmulatorReleaseButton()
    {
        GamepadEmulator emulator = new GamepadEmulator();
        Gamepad inputDevice = emulator.getGamepad();

        // press all buttons
        for (int i = 0; i < 14; i++)
        {
            emulator.pressButton(i);
        }
        emulator.emulate();
        InputSystem.Update();

        // release each button and assert button is released
        for (int i = 0; i < 14; i++)
        {
            emulator.releaseButton(i);
            emulator.emulate();
            InputSystem.Update();
            Assert.IsFalse(inputDevice[(GamepadButton)i].isPressed, "Button should be released");
        }

        emulator.dispose();
    }

    // Test for releaseAllButtons function
    // should simulate releasing all buttons
    [Test]
    public void GamepadEmulatorReleaseAllButtons()
    {
        GamepadEmulator emulator = new GamepadEmulator();
        Gamepad inputDevice = emulator.getGamepad();

        // press all buttons
        for (int i = 0; i < 14; i++)
        {
            emulator.pressButton(i);
        }
        emulator.emulate();

        // release all buttons
        emulator.releaseAllButtons();
        emulator.emulate();
        InputSystem.Update();

        // assert all buttons are released
        for (int i = 0; i < 14; i++)
        {
            Assert.IsFalse(inputDevice[(GamepadButton)i].isPressed, "Button should be released");
        }

        emulator.dispose();
    }

    // Test for pressLeftTrigger & pressRightTrigger function
    // should simulate analog trigger pressing
    [Test]
    public void GamepadEmulatorPressTriggers()
    {
        GamepadEmulator emulator = new GamepadEmulator();
        Gamepad inputDevice = emulator.getGamepad();
        
        // simulate trigger input using random values
        float randLeft = Random.Range(0f, 1f);
        float randRight = Random.Range(0f, 1f);
        emulator.pressLeftTrigger(randLeft);
        emulator.pressRightTrigger(randRight);
        emulator.emulate();
        InputSystem.Update();
        
        // assert both triggers are correct pressures
        Assert.AreEqual(randLeft, inputDevice.leftTrigger.value, 0.1f);
        Assert.AreEqual(randRight, inputDevice.rightTrigger.value, 0.1f);
        
        emulator.dispose();
    }

    // Test for releaseLeftTrigger & releaseRightTrigger function
    // should simulate analog trigger releasing / no pressure
    [Test]
    public void GamepadEmulatorReleaseTriggers()
    {
        GamepadEmulator emulator = new GamepadEmulator();
        Gamepad inputDevice = emulator.getGamepad();
        
        // simulate trigger input using random values
        float randLeft = Random.Range(0f, 1f);
        float randRight = Random.Range(0f, 1f);
        emulator.pressLeftTrigger(randLeft);
        emulator.pressRightTrigger(randRight);
        emulator.emulate();

        // release left and right triggers
        emulator.releaseLeftTrigger();
        emulator.releaseRightTrigger();
        emulator.emulate();
        InputSystem.Update();

        // assert both triggers are released == 0
        Assert.AreEqual(0f, inputDevice.leftTrigger.value, 0.01f);
        Assert.AreEqual(0f, inputDevice.rightTrigger.value, 0.01f);

        emulator.dispose();
    }

    // Test for moveLeftJoystick & moveRightJoystick function
    // should simulate joystick movement
    [Test]
    public void GamepadEmulatorMoveJoysticks()
    {
        GamepadEmulator emulator = new GamepadEmulator();
        Gamepad inputDevice = emulator.getGamepad();

        // simulate joystick input using random values
        float randLeftX = Random.Range(0f, 1f);
        float randLeftY = Random.Range(0f, 1f);
        float randRightX = Random.Range(0f, 1f);
        float randRightY = Random.Range(0f, 1f);
        emulator.moveLeftJoystick(randLeftX, randLeftY);
        emulator.moveRightJoystick(randRightX, randRightY);
        emulator.emulate();
        InputSystem.Update();
        
        // assert the joysticks move to correct location
        Vector2 expectedLeftStick = new Vector2(randLeftX, randLeftY).normalized;
        Vector2 expectedRightStick = new Vector2(randRightX, randRightY).normalized;
        Assert.AreEqual(expectedLeftStick.x, inputDevice.leftStick.ReadValue().x, 0.01f);
        Assert.AreEqual(expectedLeftStick.y, inputDevice.leftStick.ReadValue().y, 0.01f);
        Assert.AreEqual(expectedRightStick.x, inputDevice.rightStick.ReadValue().x, 0.01f);
        Assert.AreEqual(expectedRightStick.y, inputDevice.rightStick.ReadValue().y, 0.01f);

        emulator.dispose();
    }

    // Test for resetLeftJoystick & resetRightJoystick function
    // should simulate joystick reset / no movement
    [Test]
    public void GamepadEmulatorReleaseJoysticks()
    {
        GamepadEmulator emulator = new GamepadEmulator();
        Gamepad inputDevice = emulator.getGamepad();
        
        // simulate joystick input using random values
        float randLeftX = Random.Range(0f, 1f);
        float randLeftY = Random.Range(0f, 1f);
        float randRightX = Random.Range(0f, 1f);
        float randRightY = Random.Range(0f, 1f);
        emulator.moveLeftJoystick(randLeftX, randLeftY);
        emulator.moveRightJoystick(randRightX, randRightY);
        emulator.emulate();

        // reset left and right joysticks
        emulator.resetLeftJoystick();
        emulator.resetRightJoystick();
        emulator.emulate();
        InputSystem.Update();
        
        // assert both joysticks are at (0,0) = reset
        Assert.AreEqual(0f, inputDevice.leftStick.ReadValue().x, 0.01f);
        Assert.AreEqual(0f, inputDevice.leftStick.ReadValue().y, 0.01f);
        Assert.AreEqual(0f, inputDevice.rightStick.ReadValue().x, 0.01f);
        Assert.AreEqual(0f, inputDevice.rightStick.ReadValue().y, 0.01f);

        emulator.dispose();
    }

    // Test for clear function
    // should simulate empty state of gamepad
    [Test]
    public void GamepadEmulatorClear()
    {
        GamepadEmulator emulator = new GamepadEmulator();
        Gamepad inputDevice = emulator.getGamepad();
        
        // press all buttons
        for (int i = 0; i < 14; i++)
        {
            emulator.pressButton(i);
        }
        emulator.emulate();
        
        // simulate trigger input using random values
        float randLeft = Random.Range(0f, 1f);
        float randRight = Random.Range(0f, 1f);
        emulator.pressLeftTrigger(randLeft);
        emulator.pressRightTrigger(randRight);
        emulator.emulate();

        // simulate joystick input using random values
        float randLeftX = Random.Range(0f, 1f);
        float randLeftY = Random.Range(0f, 1f);
        float randRightX = Random.Range(0f, 1f);
        float randRightY = Random.Range(0f, 1f);
        emulator.moveLeftJoystick(randLeftX, randLeftY);
        emulator.moveRightJoystick(randRightX, randRightY);
        emulator.emulate();
        
        // clear all controller inputs
        emulator.clear();
        emulator.emulate();
        InputSystem.Update();

        // assert all buttons are released
        for (int i = 0; i < 14; i++)
        {
            Assert.IsFalse(inputDevice[(GamepadButton)i].isPressed, "Button should be released");
        }

        // assert both triggers are released == 0
        Assert.AreEqual(0f, inputDevice.leftTrigger.value, 0.01f);
        Assert.AreEqual(0f, inputDevice.rightTrigger.value, 0.01f);
        
        // assert both joysticks are at (0,0) = reset
        Assert.AreEqual(0f, inputDevice.leftStick.ReadValue().x, 0.01f);
        Assert.AreEqual(0f, inputDevice.leftStick.ReadValue().y, 0.01f);
        Assert.AreEqual(0f, inputDevice.rightStick.ReadValue().x, 0.01f);
        Assert.AreEqual(0f, inputDevice.rightStick.ReadValue().y, 0.01f);

        emulator.dispose();
    }

    // Test for creating a button pressed log
    [Test]
    public void LogPressedCreated()
    {
        bool wasCalled = false;

        ControllerDebugLogger.OnPressedLog += (msg) =>
        {
            wasCalled = true;
            Assert.IsTrue(msg.Contains("pressed"));
        };

        ControllerDebugLogger.LogPressed("Test Button pressed");

        Assert.IsTrue(wasCalled);
    }

    
    // Test for creating a button released log
    [Test]
    public void LogReleasedCreated()
    {
        bool wasCalled = false;

        ControllerDebugLogger.OnReleasedLog += (msg) =>
        {
            wasCalled = true;
            Assert.IsTrue(msg.Contains("released"));
        };

        ControllerDebugLogger.LogReleased("Test Button released");

        Assert.IsTrue(wasCalled);
    }

    // Test for creating a movement log
    [Test]
    public void LogMovementCreated()
    {
        bool wasCalled = false;

        ControllerDebugLogger.OnMovementLog += (msg) =>
        {
            wasCalled = true;
            Assert.IsFalse(string.IsNullOrEmpty(msg));
        };

        ControllerDebugLogger.LogMovement("Left Trigger changed to 0.75");

        Assert.IsTrue(wasCalled);
    }

    // Test for holding a button down does not create more than one log
    [Test]
    public void LogPressedReleasedStateChecker()
    {
        var components = new ControllerComponents();
        int pressedCount = 0;
        int releasedCount = 0;
        bool prevState = false;

        ControllerDebugLogger.OnPressedLog += (msg) =>
        {
            pressedCount++;
        };

        ControllerDebugLogger.OnReleasedLog += (msg) =>
        {
            releasedCount++;
        };

        // Simulate press
        components.CheckButtonState("Test Button", true, ref prevState);
        // Simulate hold
        components.CheckButtonState("Test Button", true, ref prevState);
        components.CheckButtonState("Test Button", true, ref prevState);
        // Simulate release
        components.CheckButtonState("Test Button", false, ref prevState);

        Assert.AreEqual(1, pressedCount);
        Assert.AreEqual(1, releasedCount);
    }

    // Some more things we should test.
    // 1.) Test that UI elements are created
    //     - Load in all the buttons
    // 2.)
}
