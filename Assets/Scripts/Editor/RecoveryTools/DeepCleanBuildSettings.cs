using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class DeepCleanBuildSettings : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/7. Deep Clean Build Settings (Fix Failures)")]
    public static void Clean()
    {
        Debug.Log("--- DEEP CLEAING BUILD SETTINGS ---");

        var newScenes = new List<EditorBuildSettingsScene>();

        // DEFINING THE EXPECTED SCENES (In Order)
        string[] targetScenes = new string[]
        {
            "Assets/Scenes/MainMenu.unity",         // INDEX 0 (Startup)
            "Assets/Scenes/LobbyMenu.unity",
            "Assets/Scenes/The_Viking_Village.unity",
            "Assets/Scenes/Arm/Lobby.unity"
        };

        foreach (var path in targetScenes)
        {
            if (File.Exists(path))
            {
                newScenes.Add(new EditorBuildSettingsScene(path, true));
                Debug.Log($"[Clean] Added Valid Scene: {path}");
            }
            else
            {
                Debug.LogError($"[Clean] SKIPPING MISSING SCENE: {path} (This was likely breaking the build!)");
            }
        }

        // FORCE APPLY
        EditorBuildSettings.scenes = newScenes.ToArray();

        string msg = $"Build Settings have been RESET.\n\nTotal Scenes: {newScenes.Count}\nMissing Scenes Removed.\n\nTry Building now.";
        Debug.Log("SUCCESS: " + msg);
        EditorUtility.DisplayDialog("Build Settings Cleaned", msg, "OK");
    }
}
