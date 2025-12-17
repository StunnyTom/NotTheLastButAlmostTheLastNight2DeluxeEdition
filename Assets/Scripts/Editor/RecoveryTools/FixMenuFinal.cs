using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixMenuFinal : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/5. FORCE FIX Menu Visuals")]
    public static void ForceFix()
    {
        Debug.Log("--- Force Fixing Visuals ---");

        // 1. Fix ALL Canvases (Scaler)
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            Undo.RecordObject(c.gameObject, "Fix Canvas Scaler");
            var scaler = c.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = c.gameObject.AddComponent<CanvasScaler>();
            
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            // Ensure configured to render
            if (c.renderMode == RenderMode.ScreenSpaceCamera && c.worldCamera == null)
            {
                c.renderMode = RenderMode.ScreenSpaceOverlay; // Fallback to Overlay if no Cam
            }
            
            Debug.Log($"Fixed Scaler on Canvas: {c.name}");
        }

        // 2. Fix Camera (No Skybox)
        var cam = Camera.main;
        if (cam != null)
        {
            Undo.RecordObject(cam, "Fix Camera BG");
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            Debug.Log($"Camera '{cam.name}' set to Solid Black (No Skybox).");
        }

        // 3. Fix Background Image Layering
        var bgObj = GameObject.Find("Background");
        if (bgObj == null) bgObj = GameObject.Find("Menu Background");
        
        if (bgObj != null)
        {
            var img = bgObj.GetComponent<Image>();
            if (img != null)
            {
                // Ensure it's active
                bgObj.SetActive(true);
                
                // Ensure it's on a Canvas
                Canvas parentCanvas = bgObj.GetComponentInParent<Canvas>();
                if (parentCanvas == null && canvases.Length > 0)
                {
                    // Reparent to first found canvas
                    Undo.SetTransformParent(bgObj.transform, canvases[0].transform, "Reparent BG");
                    Debug.Log("Moved Background inside Canvas.");
                }

                // Force Stretch
                var rect = bgObj.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                // Send to Back
                bgObj.transform.SetAsFirstSibling();
                Debug.Log("Background sent to back and stretched.");
            }
        }
        else
        {
            Debug.LogWarning("Still no 'Background' object found!");
        }

        // 4. Force Save EVERYTHING
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        
        EditorUtility.DisplayDialog("Final Fix Applied", 
            "1. All Canvases -> Scale With Screen Size.\n" +
            "2. Camera -> Black Background (No Skybox).\n" +
            "3. Background Image -> Stretched & Sent to Back.\n\n" +
            "PLEASE BUILD AND TEST.", "OK");
    }
}
