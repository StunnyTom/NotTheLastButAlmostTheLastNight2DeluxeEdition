using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class RestoreOrigin : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/6. Restore to Camera Mode (Safe Fix)")]
    public static void Restore()
    {
        Debug.Log("--- RESTORING UI TO CAMERA MODE ---");

        // 1. Find Camera
        Camera mainCam = Camera.main;
        if (mainCam == null) mainCam = Object.FindFirstObjectByType<Camera>();

        if (mainCam == null)
        {
            Debug.LogError("No Camera found! Cannot apply Camera Mode fix.");
            return;
        }

        // 2. Fix Canvases
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            if (c.name.Contains("Main") || c.name.Contains("Menu"))
            {
                Undo.RecordObject(c, "Restore Camera Mode");
                
                // The Fix: Screen Space - Camera
                c.renderMode = RenderMode.ScreenSpaceCamera;
                c.worldCamera = mainCam;
                c.planeDistance = 5; // Safe distance
                
                // Sorting Layer
                c.sortingOrder = 100;
                
                // Scaler
                var scaler = c.GetComponent<CanvasScaler>();
                if (scaler)
                {
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.matchWidthOrHeight = 0.5f;
                }
                
                Debug.Log($"[Restore] Canvas '{c.name}' set to ScreenSpaceCamera (Cam: {mainCam.name})");
            }
            
            // Fix Background too
            if (c.name.Contains("Background"))
            {
                c.renderMode = RenderMode.ScreenSpaceCamera;
                c.worldCamera = mainCam;
                c.planeDistance = 10; // Further back
                c.sortingOrder = -100;
            }
        }
        
        EditorUtility.DisplayDialog("UI Restored", 
            "I have switched the Menu to 'Screen Space - Camera'.\n\n" +
            "This usually fixes the 'Invisible Buttons' issue while keeping the correct Resolution.\n\n" +
            "Please Build and Run.", "OK");
    }
}
