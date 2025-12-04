using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SlimUI.ModernMenu;

public class MenuPolisher : EditorWindow
{
    [MenuItem("Tools/Menu Polish/Fix Text Overflow (Widen Borders)")]
    public static void FixOverflow()
    {
        // Aggressively widen and heighten buttons to fit the large text
        ResizeButtons(1.5f, 1.5f); 
    }

    [MenuItem("Tools/Menu Polish/Increase Button Borders")]
    public static void IncreaseBorders()
    {
        ResizeButtons(1.1f, 1.0f); 
    }

    [MenuItem("Tools/Menu Polish/Increase Button Height")]
    public static void IncreaseHeight()
    {
        ResizeButtons(1.0f, 1.1f);
    }

    [MenuItem("Tools/Menu Polish/Fix Pixelation (High Quality)")]
    public static void FixPixelation()
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null) return;

        Canvas canvas = manager.mainCanvas != null ? manager.mainCanvas.GetComponent<Canvas>() : manager.GetComponentInChildren<Canvas>();
        if (canvas == null) return;
        
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            Undo.RecordObject(scaler, "Fix Pixelation");
            // High values for World Space to ensure crisp text
            scaler.dynamicPixelsPerUnit = 300; 
            scaler.referencePixelsPerUnit = 100;
            Debug.Log("Set Dynamic Pixels Per Unit to 300 (High Quality)");
        }
        
        EditorUtility.DisplayDialog("Success", "Resolution increased significantly!\nText should be very crisp now.", "OK");
    }

    private static void ResizeButtons(float widthFactor, float heightFactor)
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null) return;

        GameObject menuRoot = manager.mainCanvas != null ? manager.mainCanvas : manager.gameObject;
        
        // Find all Buttons
        var buttons = menuRoot.GetComponentsInChildren<Button>(true);
        int count = 0;

        foreach (var btn in buttons)
        {
            RectTransform rt = btn.GetComponent<RectTransform>();
            if (rt != null)
            {
                Undo.RecordObject(rt, "Resize Button");
                Vector2 size = rt.sizeDelta;
                size.x *= widthFactor;
                size.y *= heightFactor;
                rt.sizeDelta = size;
                count++;
            }
        }

        Debug.Log($"Resized {count} buttons.");
        EditorUtility.DisplayDialog("Success", $"Resized {count} buttons.", "OK");
    }
}
