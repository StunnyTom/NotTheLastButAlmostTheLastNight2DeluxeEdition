using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SlimUI.ModernMenu;
using System.Collections.Generic;

public class FixSceneSetup : EditorWindow
{
    [MenuItem("Tools/Menu Setup (Essential)/9. Fix Build Settings & Scenes")]
    public static void FixIt()
    {
        Debug.Log("--- Fixing Scene Setup ---");

        // 1. Add Scenes to Build Settings
        string menuPath = "Assets/SlimUI/Modern Menu 1/Scenes/MainMenu.unity"; // Guessing standard path
        // Try to find active scene path if it's the menu
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            menuPath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
        }

        string gamePath = "Assets/Scenes/The_Viking_Village.unity";

        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        
        // Helper to add if missing
        AddSceneIfNeeded(scenes, menuPath);
        AddSceneIfNeeded(scenes, gamePath);

        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("Build Settings Updated.");

        // 2. Patch "LoadScene" buttons
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager != null)
        {
            Button[] allButtons = manager.GetComponentsInChildren<Button>(true);
            // int patchedCount = 0; // Unused

            foreach (var btn in allButtons)
            {
                // Inspect persistent listeners
                int count = btn.onClick.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    string methodName = btn.onClick.GetPersistentMethodName(i);
                    if (methodName == "LoadScene")
                    {
                        // It's calling LoadScene. Check arguments? Not easily accessible via API in old Unity versions easily without SerializedObject.
                        // But we can blindly assume if it's pointing to UIMenuManager.LoadScene, we might want to update it.
                        // Or better: Just warn?
                        // Let's rely on re-wiring if we know it's "New Game".
                        
                        // Actually, let's just Log it for now so the user knows WHICH button is the culprit.
                        Debug.Log($"Button '{btn.name}' calls LoadScene via {btn.onClick.GetPersistentTarget(i)}");
                    }
                }
            }
        }
    }

    private static void AddSceneIfNeeded(List<EditorBuildSettingsScene> scenes, string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        
        bool exists = scenes.Exists(s => s.path == path);
        if (!exists)
        {
            if (System.IO.File.Exists(path) || AssetDatabase.LoadAssetAtPath<Object>(path) != null)
            {
                scenes.Add(new EditorBuildSettingsScene(path, true));
                Debug.Log($"Added {path} to Build Settings.");
            }
            else
            {
                Debug.LogWarning($"Could not find scene at {path} to add to settings.");
            }
        }
    }
}
