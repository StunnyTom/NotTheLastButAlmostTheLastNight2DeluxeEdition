using UnityEngine;
using UnityEditor;
using SlimUI.ModernMenu;

public class FixSettingsPanels : EditorWindow
{
    [MenuItem("Tools/Fix Settings Panels Location")]
    public static void FixPanels()
    {
        Debug.Log("=== FIXING SETTINGS PANELS ===");
        
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager not found!", "OK");
            return;
        }

        if (manager.mainCanvas == null)
        {
            EditorUtility.DisplayDialog("Error", "mainCanvas is not assigned in UIMenuManager!", "OK");
            return;
        }

        Transform mainCanvasTransform = manager.mainCanvas.transform;
        int movedCount = 0;

        // Move all settings panels to be children of mainCanvas
        GameObject[] panels = new GameObject[]
        {
            manager.PanelGame,
            manager.PanelVideo,
            manager.PanelControls,
            manager.PanelKeyBindings
        };

        string[] panelNames = new string[] { "PanelGame", "PanelVideo", "PanelControls", "PanelKeyBindings" };

        for (int i = 0; i < panels.Length; i++)
        {
            GameObject panel = panels[i];
            string panelName = panelNames[i];

            if (panel == null)
            {
                Debug.LogWarning($"⚠ {panelName} is NULL, skipping...");
                continue;
            }

            // Check if already a child of mainCanvas
            if (IsChildOf(panel.transform, mainCanvasTransform))
            {
                Debug.Log($"✓ {panelName} is already under mainCanvas");
                continue;
            }

            // Move it!
            Debug.Log($"Moving {panelName} to mainCanvas...");
            Undo.SetTransformParent(panel.transform, mainCanvasTransform, "Move Settings Panel");
            
            // Reset transform to avoid weird positioning
            RectTransform rt = panel.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
            }

            movedCount++;
        }

        Debug.Log($"=== DONE: Moved {movedCount} panels ===");
        EditorUtility.DisplayDialog("Success", 
            $"Moved {movedCount} settings panels to mainCanvas.\n\nTry clicking Settings now!",
            "OK");
    }

    private static bool IsChildOf(Transform child, Transform parent)
    {
        Transform current = child;
        while (current != null)
        {
            if (current == parent) return true;
            current = current.parent;
        }
        return false;
    }
}
