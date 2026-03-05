using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Tests
{
    private TestPlugin window;

    [SetUp]
    public void Setup()
    {
        window = EditorWindow.GetWindow<TestPlugin>();
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
}