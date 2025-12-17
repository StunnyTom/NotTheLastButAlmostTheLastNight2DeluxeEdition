using UnityEngine;
using UnityEditor;
using System.Linq;

public class VerifyGameSceneInBuild : Editor
{
    [MenuItem("Tools/Recovery/3. Ensure Game Scene in Build")]
    public static void Ensure()
    {
        Debug.Log("--- Checking Build Settings ---");
        var scenes = EditorBuildSettings.scenes.ToList();
        
        Debug.Log($"Current Scene Count: {scenes.Count}");
        foreach(var s in scenes) Debug.Log($" - '{s.path}' (Enabled: {s.enabled})");

        string targetPath = "Assets/Scenes/The_Viking_Village.unity";
        var existing = scenes.FirstOrDefault(s => s.path == targetPath);

        // 1. Ensure The_Viking_Village is present and enabled
        if (existing != null)
        {
            if (!existing.enabled)
            {
                existing.enabled = true;
                EditorBuildSettings.scenes = scenes.ToArray();
                Debug.Log($"FIXED: Scene '{targetPath}' was disabled. Enabled it.");
            }
            else
            {
                Debug.Log($"OK: Scene '{targetPath}' is already present and enabled.");
            }
        }
        else
        {
            // Try to find it physically to be sure
            var guid = AssetDatabase.AssetPathToGUID(targetPath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"CRITICAL: Physical file not found at '{targetPath}'.");
                // Fallback search
                 var found = AssetDatabase.FindAssets("The_Viking_Village t:Scene");
                 if (found.Length > 0)
                 {
                     targetPath = AssetDatabase.GUIDToAssetPath(found[0]);
                     Debug.LogWarning($"Found scene at different path: '{targetPath}'. Using that.");
                 }
                 else
                 {
                     return;
                 }
            }

            scenes.Add(new EditorBuildSettingsScene(targetPath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"SUCCESS: Added '{targetPath}' to Build Settings.");
            
            EditorUtility.DisplayDialog("Fixed", $"Added '{targetPath}' to build.", "OK");
        }

        // 2. Re-order Scenes (Main Menu must be 0)
        ReorderScenes();
    }

    private static void ReorderScenes()
    {
        // Reload scenes list as it might have changed above
        var scenes = EditorBuildSettings.scenes.ToList();
        var mainMenu = scenes.FirstOrDefault(s => s.path.Contains("MainMenu.unity"));
        
        if (mainMenu != null)
        {
            bool changed = false;
            
            // 1. Ensure Enabled
            if (!mainMenu.enabled)
            {
                mainMenu.enabled = true;
                changed = true;
                Debug.Log("FIXED: Main Menu was disabled. Enabled it.");
            }

            // 2. Ensure Index 0
            int index = scenes.IndexOf(mainMenu);
            if (index != 0)
            {
                Debug.LogWarning($"MainMenu was at index {index}. Moving to 0.");
                scenes.Remove(mainMenu);
                scenes.Insert(0, mainMenu);
                changed = true;
            }

            if (changed)
            {
                EditorBuildSettings.scenes = scenes.ToArray(); // Apply
                Debug.Log("SUCCESS: Main Menu is enabled and at Index 0.");
            }
            else
            {
                Debug.Log("OK: Main Menu is correctly at Index 0 and Enabled.");
            }
        }
        else
        {
             Debug.LogError("CRITICAL: MainMenu.unity not found in build settings! Please add it manually or check the name.");
        }
    }
}
