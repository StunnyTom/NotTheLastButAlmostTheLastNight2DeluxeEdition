using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SlimUI.ModernMenu;

public class SetupHybridMenu : EditorWindow
{
    [MenuItem("Tools/Menu Setup (Essential)/2. Setup Hybrid Menu")]
    public static void SetupHybrid()
    {
        Debug.Log("--- Setup Hybrid Menu ---");

        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager not found!", "OK");
            return;
        }

        // 1. Create Background Canvas (Screen Space - Camera) if not exists
        GameObject bgCanvasObj = GameObject.Find("BackgroundCanvas");
        Canvas bgCanvas;
        
        if (bgCanvasObj == null)
        {
            bgCanvasObj = new GameObject("BackgroundCanvas");
            bgCanvas = bgCanvasObj.AddComponent<Canvas>();
            bgCanvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            bgCanvasObj.AddComponent<GraphicRaycaster>();
        }
        else
        {
            bgCanvas = bgCanvasObj.GetComponent<Canvas>();
        }

        Undo.RecordObject(bgCanvasObj, "Setup Hybrid Menu");

        // Configure Background Canvas
        bgCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        bgCanvas.worldCamera = Camera.main;
        bgCanvas.planeDistance = 100; // Far behind
        bgCanvas.sortingOrder = -100; // Behind everything

        CanvasScaler bgScaler = bgCanvasObj.GetComponent<CanvasScaler>();
        bgScaler.referenceResolution = new Vector2(1920, 1080);
        bgScaler.matchWidthOrHeight = 0.5f;

        // 2. Move Background Image to Background Canvas
        if (manager.mainCanvas != null)
        {
            Transform bgTransform = manager.mainCanvas.transform.Find("Background");
            if (bgTransform != null)
            {
                Undo.SetTransformParent(bgTransform, bgCanvasObj.transform, "Move Background");
                
                RectTransform rt = bgTransform.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
                
                Debug.Log("Moved Background to BackgroundCanvas");
            }
        }

        // 3. Set Main Menu to World Space
        Canvas mainCanvas = manager.mainCanvas.GetComponent<Canvas>();
        if (mainCanvas != null)
        {
            Undo.RecordObject(mainCanvas, "Set Menu World Space");
            mainCanvas.renderMode = RenderMode.WorldSpace;
            
            // Reset position if it looks wrong (optional, usually handled by scale fix)
            // But let's ensure it's not totally invisible
            if (mainCanvas.transform.localScale.x == 1) 
            {
                mainCanvas.transform.localScale = Vector3.one * 0.0015f;
                mainCanvas.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3.0f;
                mainCanvas.transform.rotation = Camera.main.transform.rotation;
            }
        }

        Debug.Log("Hybrid Menu Setup Complete!");
        EditorUtility.DisplayDialog("Success", "Hybrid Menu Setup Complete!\nBackground is now 2D, Menu is 3D.", "OK");
    }
}
