using UnityEngine;
using UnityEditor;
using System.Linq;

public class EnsureLobbyInBuild : Editor
{
    [MenuItem("Tools/Recovery/Ensure Lobby in Build")]
    public static void Ensure()
    {
        string scenePath = "Assets/Scenes/Arm/Lobby.unity";
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        
        if (sceneAsset == null)
        {
            Debug.LogError($"Could not find scene at {scenePath}");
            return;
        }

        var currentScenes = EditorBuildSettings.scenes.ToList();
        if (currentScenes.Any(s => s.path == scenePath))
        {
            Debug.Log($"Scene {scenePath} is already in Build Settings.");
            return;
        }

        currentScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = currentScenes.ToArray();
        Debug.Log($"Added {scenePath} to Build Settings.");
    }
}
