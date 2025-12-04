using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SimpleSettingsCheck : EditorWindow
{
    [MenuItem("Tools/SIMPLE Settings Check")]
    public static void Check()
    {
        Debug.Log("=== SIMPLE SETTINGS CHECK ===");
        
        // 1. Check scene objects
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] all = scene.GetRootGameObjects();
        
        Debug.Log($"\nAll root objects in scene ({all.Length}):");
        foreach (var obj in all)
        {
            Debug.Log($"  - {obj.name} (Active: {obj.activeSelf})");
            
            // Look for Canvas components
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"    → Canvas! RenderMode: {canvas.renderMode}, SortingOrder: {canvas.sortingOrder}");
            }
            
            // Look for SettingsManager
            var settingsMgr = obj.GetComponent<SettingsManager>();
            if (settingsMgr != null)
            {
                Debug.Log($"    → Has SettingsManager component!");
            }
        }
        
        // 2. Test: Manually create and show a simple settings panel
        Debug.Log("\n=== CREATING TEST SETTINGS PANEL ===");
        
        GameObject testCanvas = new GameObject("TEST_SettingsCanvas");
        Canvas canvas2 = testCanvas.AddComponent<Canvas>();
        canvas2.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas2.sortingOrder = 999; // TOP!
        
        testCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        testCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Add a visible panel
        GameObject panel = new GameObject("TestPanel");
        panel.transform.SetParent(testCanvas.transform, false);
        
        var img = panel.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.red; // BRIGHT RED so we can see it!
        
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.2f, 0.2f);
        rt.anchorMax = new Vector2(0.8f, 0.8f);
        rt.sizeDelta = Vector2.zero;
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(panel.transform, false);
        var text = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = "TEST SETTINGS - IF YOU SEE THIS, RENDERING WORKS!";
        text.fontSize = 48;
        text.color = Color.white;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        
        Debug.Log("✓ Created TEST_SettingsCanvas with bright red panel");
        Debug.Log("\n>>> PRESS PLAY and look for the RED PANEL <<<");
        Debug.Log("If you DON'T see it, something is fundamentally broken with UI rendering.");
        
        Selection.activeGameObject = testCanvas;
    }
}
