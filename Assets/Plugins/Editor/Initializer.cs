using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// <see langword="Object"/> that initializes. 
/// </summary>
public static class Initializer
{
    /// <summary>
    /// Initializes the UI.
    /// </summary>
    public static void LoadUI(VisualElement root, string uxmlPath)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        if (string.IsNullOrWhiteSpace(uxmlPath))
            throw new ArgumentException("Path cannot be null or empty", nameof(uxmlPath));

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);  // Loads UI.
        if (visualTree == null)
        {
            Debug.Log($"InitializeUI failed: No tree found at '{uxmlPath}'.");
            return;           
        }

        root.Clear();  // Clear previous UI.
        string[] uiTypes = { "xbox", "ps", "generic" };
        foreach (string type in uiTypes)
            root.RemoveFromClassList(type);  // Remove any type classes.

        string styleClass = uxmlPath.Contains("XBOX") ? "xbox" : uxmlPath.Contains("PS") ? "ps" : "generic";
        root.AddToClassList(styleClass);
        visualTree.CloneTree(root);  // Clones UI to root.
    }



    /// <summary>
    /// Finds all buttons listed in <paramref name="buttonNames"/> and assigns them
    /// their functionalities.
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <c> 
    /// InitializeButtons(new string[] {
    ///    "A-button", "Y-button", "B-button", "X-button",
    ///    "RB-button", "LB-button",
    ///    "LT-button", "RT-button",
    /// }); 
    /// </c>
    /// </remarks>
    /// <param name="buttonNames">Array of names for the buttons you want queried</param>
    public static Dictionary<string, VisualElement> LoadButtonFunctions(VisualElement rootVisualElement, IEnumerable<string> buttonNames, GamepadEmulator emulator)
    {
        Dictionary<string, VisualElement> buttons = new();

        foreach (string name in buttonNames)
        {
            // Query each name in buttonNames:
            var button = rootVisualElement.Q<VisualElement>(name);

            // Skip unimplemented names:
            if (button == null)
            {
                Debug.LogWarning($"Button '{name}' not found in UXML!");
                continue;
            }

            // Put into dictionary (hash table);
            buttons[name] = button;
            // Assign pressed events:;
            button.RegisterCallback<PointerDownEvent>(evt =>
            {
                emulator.pressButton(Dictionaries.visElToEmulatorButton[name]);

                // Set this button's pressed state to true.
                if (Dictionaries.visElToButton.TryGetValue(name, out var type))
                    XboxController.SetButton(type, true);

                // Special logic for buttons with images
                var image = button.Q<Image>();
                // If image exists and is not the Xbox button then invert it when pressed.
                bool isInvertable = image != null && image.name != "xbox-button-image";
                if (isInvertable)
                    image.tintColor = Color.black;
            });
            
            button.RegisterCallback<PointerUpEvent>(evt =>
            {
                emulator.clear();
                Debug.Log($"{name}: UP");

                // Set this button's pressed state to false.
                if (Dictionaries.visElToButton.TryGetValue(name, out var type))
                    XboxController.SetButton(type, false);

                // Special logic for buttons with images
                var image = button.Q<Image>();
                // If image exists and is not the Xbox button then invert it when released.
                bool isInvertable = image != null && image.name != "xbox-button-image";
                if (isInvertable)
                    image.tintColor = Color.white;
            });
        }
        return buttons;
    }
}
