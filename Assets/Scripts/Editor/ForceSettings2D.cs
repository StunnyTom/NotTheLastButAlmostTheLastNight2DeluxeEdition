using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ForceSettings2D : EditorWindow
{
    [MenuItem("Tools/Force Settings to 2D (FINAL FIX)")]
    public static void Force2D()
    {
        Debug.Log("=== FORCING SETTINGS TO 2D ===");
        
        // Find SettingsCanvas
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] all = scene.GetRootGameObjects();
        GameObject settingsCanvas = System.Array.Find(all, obj => 
            obj.name == "SettingsCanvas" && obj.GetComponent<SettingsManager>() != null);
        
        if (settingsCanvas == null)
        {
            EditorUtility.DisplayDialog("Error", "SettingsCanvas with SettingsManager not found!", "OK");
            return;
        }
        
        Debug.Log($"Found SettingsCanvas: {settingsCanvas.name}");
        
        // Force Canvas to Screen Space Overlay
        Canvas canvas = settingsCanvas.GetComponent<Canvas>();
        if (canvas != null)
        {
            Undo.RecordObject(canvas, "Force Canvas 2D");
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // Top!
            Debug.Log("✓ Set Canvas to ScreenSpaceOverlay");
        }
        
        // Reset ALL transforms in the hierarchy to 2D defaults
        ResetTransformRecursive(settingsCanvas.transform);
        
        Debug.Log("\n✓ Forced all Settings panels to 2D!");
        EditorUtility.DisplayDialog("Success!", 
            "Settings forced to 2D!\n\nNow:\n1. Press Play\n2. Click Settings\n3. It should appear full screen in 2D!",
            "OK");
    }
    
    private static void ResetTransformRecursive(Transform t)
    {
        Undo.RecordObject(t, "Reset Transform");
        
        // Reset rotation and position (for ScreenSpaceOverlay, these don't matter, but let's be clean)
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        
        // If it's a RectTransform, ensure it's properly anchored
        RectTransform rt = t.GetComponent<RectTransform>();
        if (rt != null)
        {
            // Full screen by default
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }
        
        Debug.Log($"Reset: {t.name}");
        
        // Recurse to children
        foreach (Transform child in t)
        {
            ResetTransformRecursive(child);
        }
    }
}
