using UnityEngine;
using UnityEditor;
using SlimUI.ModernMenu;

public class FixMenuScale : EditorWindow
{
    [MenuItem("Tools/Menu Support/Adjust Scale/Reset to Default")]
    public static void ResetScale()
    {
        ApplyScale(0.0015f, 3.0f);
    }

    [MenuItem("Tools/Menu Support/Adjust Scale/Make Bigger")]
    public static void MakeBigger()
    {
        ScaleMenu(1.2f); // Increase by 20%
    }

    [MenuItem("Tools/Menu Support/Adjust Scale/Make Smaller")]
    public static void MakeSmaller()
    {
        ScaleMenu(0.8f); // Decrease by 20%
    }

    private static void ApplyScale(float scaleVal, float distance)
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null) return;

        Canvas canvas = manager.mainCanvas != null ? manager.mainCanvas.GetComponent<Canvas>() : manager.GetComponentInChildren<Canvas>();
        if (canvas == null) return;

        Camera cam = Camera.main;
        if (cam == null) cam = Object.FindFirstObjectByType<Camera>();
        if (cam == null) return;

        Undo.RecordObject(canvas.transform, "Fix Menu Scale");

        canvas.transform.localScale = Vector3.one * scaleVal;
        canvas.transform.position = cam.transform.position + cam.transform.forward * distance;
        canvas.transform.rotation = cam.transform.rotation;

        Debug.Log($"Reset Menu: Scale {scaleVal}, Distance {distance}m");
    }

    private static void ScaleMenu(float factor)
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null) return;

        Canvas canvas = manager.mainCanvas != null ? manager.mainCanvas.GetComponent<Canvas>() : manager.GetComponentInChildren<Canvas>();
        if (canvas == null) return;

        Undo.RecordObject(canvas.transform, "Scale Menu");
        canvas.transform.localScale *= factor;
        Debug.Log($"Scaled Menu by {factor}. New Scale: {canvas.transform.localScale.x}");
    }
}
