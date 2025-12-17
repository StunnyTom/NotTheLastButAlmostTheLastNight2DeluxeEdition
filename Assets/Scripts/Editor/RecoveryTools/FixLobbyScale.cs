using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class FixLobbyScale : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/8. Fix Lobby Scale (Tiny UI Fix)")]
    public static void FixLobby()
    {
        // 1. Open Scene
        string scenePath = "Assets/Scenes/LobbyMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);
        
        Debug.Log($"--- FIXING LOBBY SCENE: {scene.name} ---");

        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            Undo.RecordObject(c.gameObject, "Fix Lobby Scale");
            
            // 2. Add/Get Scaler
            var scaler = c.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = c.gameObject.AddComponent<CanvasScaler>();

            // 3. FORCE SCALING (The Fix)
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            // 4. Force Camera Mode (Consistency)
            c.renderMode = RenderMode.ScreenSpaceCamera;
            c.worldCamera = Camera.main;
            if (c.worldCamera == null) c.worldCamera = Object.FindFirstObjectByType<Camera>();
            c.planeDistance = 5;

            Debug.Log($"[Fix] Canvas '{c.name}' updated to ScaleWithScreenSize (1920x1080).");
        }
        
        // 5. Save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Lobby Fixed", 
            "I have forced the Lobby Canvas to 'Scale With Screen Size'.\n\n" +
            "This should prevent it from being tiny in the Build.", "OK");
    }
}
