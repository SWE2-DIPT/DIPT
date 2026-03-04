using UnityEngine;
using UnityEditor;

public class Plugin : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Plugin initialized");
    }

    public void LogMessage(string message)
    {
        Debug.Log(message);
    }
}