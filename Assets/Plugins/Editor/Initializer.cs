using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

/// <summary>
/// Initializes UI on desired window.
/// </summary>
public static class Initializer
{
    public static void LoadUI(VisualElement root, string uxmlPath)
    {
        if (root == null)
        {
            Debug.Log($"InitializeUI failed: root '{root}' is null.");
            return;
        }
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);  // Loads UI.
        if (visualTree == null)
        {
            Debug.Log($"InitializeUI failed: No tree found at '{uxmlPath}'.");
            return;           
        }

        root.Clear();  // Clear previous UI.
        visualTree.CloneTree(root);  // Clones UI to root.
    }
}