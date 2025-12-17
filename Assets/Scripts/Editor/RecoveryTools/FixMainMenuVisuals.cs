using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixMainMenuVisuals : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/4. Fix Visuals (Scale & BG)")]
    public static void Fix()
    {
        Debug.Log("--- Fixing Visuals ---");

        // 1. Fix Canvas Scaler
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            Undo.RecordObject(canvas.gameObject, "Fix Visuals");
            
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            
            Undo.RecordObject(scaler, "Fix Scaler");
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f; // Balanced
            
            Debug.Log("SUCCESS: Canvas Scaler set to ScaleWithScreenSize (1920x1080).");
        }
        else
        {
            Debug.LogError("No Canvas found!");
        }

        // 2. Fix Background Anchors
        var bgObj = GameObject.Find("Background");
        if (bgObj == null) bgObj = GameObject.Find("Menu Background");

        if (bgObj != null)
        {
            Undo.RecordObject(bgObj.transform, "Fix BG Anchors");
            var rect = bgObj.GetComponent<RectTransform>();
            
            // Stretch to fill parent
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Debug.Log("SUCCESS: Background RectTransform stretched to fill screen.");
        }
        
        // Force Save
        if (canvas != null) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        
        EditorUtility.DisplayDialog("Visuals Fixed", "1. Canvas Scaler -> ScaleWithScreenSize\n2. Background -> Stretched Fullscreen\n\nTry building now!", "OK");
    }
}
