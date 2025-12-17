using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class FixLobbyLayoutV2 : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/9. Fix Lobby Layout V2 (Overlay Force)")]
    public static void FixLobby()
    {
        // 1. Open Scene
        string scenePath = "Assets/Scenes/LobbyMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);
        
        Debug.Log($"--- FIXING LOBBY SCENE (V2 - OVERLAY) ---");

        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            Undo.RecordObject(c.gameObject, "Fix Lobby V2");
            
            // 1. Force ScreenSpace - Overlay (Safest for pure 2D)
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 2. Fix CanvasScaler
            var scaler = c.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = c.gameObject.AddComponent<CanvasScaler>();

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            // 3. NUCLEAR TRANSFORM RESET (Fix "Floating/Angled" look)
            var rect = c.GetComponent<RectTransform>();
            rect.localRotation = Quaternion.identity; // No rotation
            rect.localScale = Vector3.one;            // Scale 1,1,1
            rect.anchoredPosition = Vector3.zero;     // Center
            
            Debug.Log($"[Fix] Canvas '{c.name}' forced to OVERLAY & Reset Transform.");
        }
        
        // 4. Save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Lobby Fixed (V2)", 
            "I have switched the Lobby to 'Screen Space - Overlay' and reset all rotations.\n\n" +
            "This forces the UI to be flat on the screen.", "OK");
    }
}
