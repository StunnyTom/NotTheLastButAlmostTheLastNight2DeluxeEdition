using UnityEngine;
using UnityEditor;
using TMPro;
using SlimUI.ModernMenu;

public class MenuEnhancements : EditorWindow
{
    [MenuItem("Tools/Menu Enhancements/Add Game Title")]
    public static void AddGameTitle()
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null || manager.mainCanvas == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager or mainCanvas not found!", "OK");
            return;
        }

        // Check if title already exists
        Transform existingTitle = manager.mainCanvas.transform.Find("GameTitle");
        if (existingTitle != null)
        {
            Selection.activeGameObject = existingTitle.gameObject;
            EditorGUIUtility.PingObject(existingTitle.gameObject);
            EditorUtility.DisplayDialog("Info", "Game title already exists! Selected it for you to edit.", "OK");
            return;
        }

        // Create title GameObject
        GameObject titleObj = new GameObject("GameTitle");
        Undo.RegisterCreatedObjectUndo(titleObj, "Create Game Title");
        titleObj.transform.SetParent(manager.mainCanvas.transform, false);

        // Add TextMeshPro component
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "YOUR GAME TITLE";
        titleText.fontSize = 72;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;

        // Setup RectTransform
        RectTransform rt = titleObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f); // Top center
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -100); // 100 units from top
        rt.sizeDelta = new Vector2(800, 100);

        // Apply theme color if available
        if (manager.themeController != null)
        {
            titleText.color = manager.themeController.textColor;
        }

        Selection.activeGameObject = titleObj;
        EditorGUIUtility.PingObject(titleObj);

        Debug.Log("Game title created! Edit the text in the Inspector.");
        EditorUtility.DisplayDialog("Success", 
            "Game title added!\n\nIt's now selected in the Inspector - change 'YOUR GAME TITLE' to your actual game name.",
            "OK");
    }

    [MenuItem("Tools/Menu Enhancements/Force Show Settings Panels")]
    public static void ForceShowSettings()
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager not found!", "OK");
            return;
        }

        // Force enable all settings panels for testing
        if (manager.PanelGame != null) manager.PanelGame.SetActive(true);
        if (manager.PanelVideo != null) manager.PanelVideo.SetActive(true);
        if (manager.PanelControls != null) manager.PanelControls.SetActive(true);
        if (manager.PanelKeyBindings != null) manager.PanelKeyBindings.SetActive(true);

        Debug.Log("All settings panels forced to active. Check if you can see them in Game view!");
        EditorUtility.DisplayDialog("Test", 
            "All settings panels are now ACTIVE.\n\nLook at the Game view - do you see ANY settings panels? This will tell us if they're just positioned wrong or truly invisible.",
            "OK");
    }
}
