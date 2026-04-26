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

    /*
    public static Dictionary<string, VisualElement> LoadButtonFunctions(IEnumerable<string> buttonNames)
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
            string ButtonName = name;
            // Assign pressed events:;
            button.RegisterCallback<PointerDownEvent>(evt =>
            {
                Debug.Log($"{name}: DOWN");
                //HELL
                
                switch (ButtonName)
                {
                    case "A-button":
                        ButtonName = "A";
                        break;

                    case "B-button":
                        ButtonName = "B";
                        break;

                    case "X-button":
                        ButtonName = "X";
                        break;

                    case "Y-button":
                        ButtonName = "Y";
                        break;

                    case "RB-button":
                        ButtonName = "RightShoulder";
                        break;

                    case "LB-button":
                        ButtonName = "LeftShoulder";
                        break;

                    case "up-pad":
                        ButtonName = "DpadUp";
                        break;

                    case "down-pad":
                        ButtonName = "DpadDown";
                        break;

                    case "left-pad":
                        ButtonName = "DpadLeft";
                        break;

                    case "right-pad":
                        ButtonName = "DpadRight";
                        break;

                    // case "xbox-button":
                    //     ButtonName = "Xbox";
                    //     break;

                    case "menu-button":
                        ButtonName = "Start";
                        break;

                    case "view-button":
                        ButtonName = "Select";
                        break;

                    // case "share-button":
                    //     ButtonName = "Share";
                    //     break;

                    // case "advanced":
                    //     ButtonName = "Advanced";
                    //     break;

                    case "left-stick":
                        ButtonName = "LeftStick";
                        break;

                    case "right-stick":
                        ButtonName = "RightStick";
                        break;
                }
                //removed "HE"
                emulator.pressButton(ButtonName);

                // Set this button's pressed state to true.
                if (visElToButton.TryGetValue(name, out var type))
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
                if (visElToButton.TryGetValue(name, out var type))
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
    */
}
