using UnityEngine;
using UnityEditor;
using System.Linq;

namespace EditorTools
{
    public class SetupBuildScenes : Editor
    {
        [MenuItem("Tools/Antigravity Kit/Recovery/Setup Build Scenes (Fix Load Error)")]
        public static void Setup()
        {
            Debug.Log("--- Setting up Build Scenes ---");
            string[] requiredScenes = new string[] 
            {
                "Assets/Scenes/MainMenu.unity", 
                "Assets/Scenes/Arm/Lobby.unity", // Found previously
                "Assets/Scenes/The_Viking_Village.unity"
            };

            var currentScenes = EditorBuildSettings.scenes.ToList();
            bool changed = false;

            foreach (var path in requiredScenes)
            {
                // Check if valid file
                if (System.IO.File.Exists(path) || AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null)
                {
                    // Check if in build settings
                    if (!currentScenes.Any(s => s.path == path))
                    {
                        Debug.Log($"Adding missing scene: {path}");
                        currentScenes.Add(new EditorBuildSettingsScene(path, true));
                        changed = true;
                    }
                    else
                    {
                        // Ensure enabled
                        var scene = currentScenes.First(s => s.path == path);
                        if (!scene.enabled) { scene.enabled = true; changed = true; }
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not find scene file at: {path}. Trying fuzzy search...");
                    string name = System.IO.Path.GetFileNameWithoutExtension(path);
                    string[] guids = AssetDatabase.FindAssets(name + " t:Scene");
                    if (guids.Length > 0)
                    {
                        string newPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                        Debug.Log($"Found '{name}' at '{newPath}'. Adding.");
                        if (!currentScenes.Any(s => s.path == newPath))
                        {
                            currentScenes.Add(new EditorBuildSettingsScene(newPath, true));
                            changed = true;
                        }
                    }
                    else
                    {
                        Debug.LogError($"CRITICAL: Scene '{name}' not found anywhere!");
                    }
                }
            }

            if (changed)
            {
                EditorBuildSettings.scenes = currentScenes.ToArray();
                Debug.Log("Build Settings Updated! You can now load scenes.");
                EditorUtility.DisplayDialog("Success", "Added Lobby and Game scenes to Build Settings.", "OK");
            }
            else
            {
                Debug.Log("Build Settings were already correct.");
                EditorUtility.DisplayDialog("OK", "Build Settings are already correct.", "OK");
            }
        }
    }
}
