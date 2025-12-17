using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class FixLobbyFinal : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/10. Fix Lobby FINAL (Camera & Stretch)")]
    public static void FixLobby()
    {
        // 1. Open Scene
        string scenePath = "Assets/Scenes/LobbyMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);
        
        Debug.Log("--- FIXING LOBBY (FINAL ATTEMPT) ---");

        // 2. Fix Camera (Make it standard 2D centered)
        var cam = Camera.main;
        if (cam == null) cam = Object.FindFirstObjectByType<Camera>();
        if (cam != null)
        {
            cam.transform.position = new Vector3(0, 0, -10);
            cam.transform.rotation = Quaternion.identity;
            cam.orthographic = true;
            cam.orthographicSize = 5; // Standard Unity 2D size
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black; 
        }

        // 3. Fix Canvas
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            Undo.RecordObject(c.gameObject, "Fix Lobby Final");
            
            // Mode: Scale with Camera
            c.renderMode = RenderMode.ScreenSpaceCamera;
            c.worldCamera = cam;
            c.planeDistance = 5; // In front of camera (-10 + 5 = -5)
            
            // Scaler
            var scaler = c.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = c.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            // 4. FIX CHILDREN ANCHORS (The actual Scaling issue)
            foreach (Transform child in c.transform)
            {
                var rect = child.GetComponent<RectTransform>();
                if (rect)
                {
                    // Heuristic: If it looks like a background or main panel, STRETCH IT
                    // We assume the first child is the "Main Panel" or "Background"
                    if (child.name.ToLower().Contains("panel") || child.name.ToLower().Contains("bg") || child.name.ToLower().Contains("background") || child.GetSiblingIndex() == 0)
                    {
                        Undo.RecordObject(rect, "Strech Panel");
                        rect.anchorMin = Vector2.zero; // Bottom-Left
                        rect.anchorMax = Vector2.one;  // Top-Right
                        rect.pivot = new Vector2(0.5f, 0.5f); // Center
                        rect.offsetMin = Vector2.zero; // Zero margins
                        rect.offsetMax = Vector2.zero;
                        rect.localScale = Vector3.one;
                        rect.localPosition = Vector3.zero;
                        Debug.Log($"[Fix] Stretched Child '{child.name}' to fill screen.");
                    }
                }
            }
        }
        
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Lobby Fixed (Final)", 
            "I have aligned the Camera and forced the Layout Panel to STRETCH.\n\n" +
            "This matches the UI size to the Camera size exactly.", "OK");
    }
}
