using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class FixBuildSettings : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/FIX BUILD SCENES (Critical)")]
    public static void FixScenes()
    {
        Debug.Log("--- FIXING BUILD SETTINGS ---");

        var currentScenes = EditorBuildSettings.scenes.ToList();
        var newScenes = new List<EditorBuildSettingsScene>();

        string correctMenuPath = "Assets/Scenes/MainMenu.unity"; // The good one
        string oldMenuPath = "Assets/Scenes/MainMenu_OLD.unity"; // The bad one

        // 1. Add Correct Menu FIRST
        newScenes.Add(new EditorBuildSettingsScene(correctMenuPath, true));
        Debug.Log($"[Fix] Set '{correctMenuPath}' as Scene #0.");

        // 2. Add others (preserving order but skipping OLD menu and duplicates)
        foreach (var s in currentScenes)
        {
            if (s.path == correctMenuPath) continue; // Already added
            if (s.path == oldMenuPath)
            {
                Debug.Log($"[Fix] REMOVED '{oldMenuPath}' from build.");
                continue; 
            }
            newScenes.Add(s);
        }

        // 3. Apply
        EditorBuildSettings.scenes = newScenes.ToArray();
        
        Debug.Log("SUCCESS: Build Settings updated. Please check File > Build Settings.");
        EditorUtility.DisplayDialog("Build Settings Fixed", 
            "I found the problem!\n\n" +
            "The build was loading 'MainMenu_OLD' instead of 'MainMenu'.\n\n" +
            "I have fixed it. 'MainMenu.unity' is now Scene #0.\n\n" +
            "PLEASE BUILD AND RUN NOW.", "OK");
    }
}
