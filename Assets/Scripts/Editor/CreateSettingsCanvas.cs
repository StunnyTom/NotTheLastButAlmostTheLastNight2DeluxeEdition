using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SlimUI.ModernMenu;

public class CreateSettingsCanvas : EditorWindow
{
    [MenuItem("Tools/Fix Settings/Create 2D Settings Canvas")]
    public static void CreateSettings2DCanvas()
    {
        Debug.Log("=== Creating 2D Settings Canvas ===");
        
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager not found!", "OK");
            return;
        }

        // Find or create Settings Canvas (Screen Space Overlay)
        // Search in the active scene, not just with GameObject.Find
        GameObject settingsCanvasObj = null;
        
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj.name == "SettingsCanvas")
            {
                settingsCanvasObj = obj;
                break;
            }
        }
        
        Canvas settingsCanvas;
        
        if (settingsCanvasObj == null)
        {
            settingsCanvasObj = new GameObject("SettingsCanvas");
            settingsCanvas = settingsCanvasObj.AddComponent<Canvas>();
            settingsCanvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            settingsCanvasObj.AddComponent<GraphicRaycaster>();
        }
        else
        {
            settingsCanvas = settingsCanvasObj.GetComponent<Canvas>();
        }

        Undo.RecordObject(settingsCanvas, "Configure Settings Canvas");
        
        // Configure as Screen Space Overlay (always on top, 2D)
        settingsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        settingsCanvas.sortingOrder = 100; // On top of everything
        
        CanvasScaler scaler = settingsCanvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Move all settings panels to this canvas
        GameObject[] panels = new GameObject[]
        {
            manager.PanelGame,
            manager.PanelVideo,
            manager.PanelControls,
            manager.PanelKeyBindings,
            manager.lineGame,
            manager.lineVideo,
            manager.lineControls,
            manager.lineKeyBindings
        };

        int moved = 0;
        foreach (var panel in panels)
        {
            if (panel == null) continue;
            
            Undo.SetTransformParent(panel.transform, settingsCanvasObj.transform, "Move to Settings Canvas");
            
            // Reset transform
            RectTransform rt = panel.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero; // Full screen
            }
            
            // Start disabled
            panel.SetActive(false);
            moved++;
        }

        // Start with settings canvas hidden
        settingsCanvasObj.SetActive(false);

        Debug.Log($"✓ Created SettingsCanvas and moved {moved} panels");
        EditorUtility.DisplayDialog("Success", 
            $"Created 2D Settings Canvas!\n\nMoved {moved} panels.\n\nNow update the Settings button to show/hide this canvas.",
            "OK");
    }

    [MenuItem("Tools/Fix Settings/Wire Settings Button to 2D Canvas")]
    public static void WireSettingsButton()
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null || manager.firstMenu == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager or firstMenu not found!", "OK");
            return;
        }

        // Find SettingsCanvas in the scene
        GameObject settingsCanvasObj = null;
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj.name == "SettingsCanvas")
            {
                settingsCanvasObj = obj;
                break;
            }
        }
        
        if (settingsCanvasObj == null)
        {
            EditorUtility.DisplayDialog("Error", "SettingsCanvas not found! Run 'Create 2D Settings Canvas' first.", "OK");
            return;
        }

        // Add SettingsManager component if not already present
        SettingsManager settingsManager = settingsCanvasObj.GetComponent<SettingsManager>();
        if (settingsManager == null)
        {
            settingsManager = settingsCanvasObj.AddComponent<SettingsManager>();
            Debug.Log("Added SettingsManager component");
        }

        // Find Settings button
        Button settingsButton = null;
        Button[] buttons = manager.firstMenu.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            if (btn.name.ToLower().Contains("settings") || btn.name.ToLower().Contains("option"))
            {
                settingsButton = btn;
                break;
            }
        }

        if (settingsButton == null)
        {
            EditorUtility.DisplayDialog("Error", "Settings button not found!", "OK");
            return;
        }

        Undo.RecordObject(settingsButton, "Wire Settings Button");
        settingsButton.onClick.RemoveAllListeners();
        
        // Use UnityEvent to persist the connection
        UnityEngine.Events.UnityAction action = () => settingsManager.ToggleSettings();
        settingsButton.onClick.AddListener(action);
        
        EditorUtility.SetDirty(settingsButton);

        Debug.Log("✓ Settings button wired to SettingsManager.ToggleSettings()");
        EditorUtility.DisplayDialog("Success", 
            "Settings button configured!\n\nIt will now call SettingsManager.ToggleSettings().\n\nCheck the Console for debug logs when you click Settings in Play mode!",
            "OK");
    }
}
