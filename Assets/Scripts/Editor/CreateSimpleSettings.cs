using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using SlimUI.ModernMenu;
using UnityEditor.Events;

public class CreateSimpleSettings : EditorWindow
{
    [MenuItem("Tools/Menu Setup (Essential)/3. Create Custom Settings Menu")]
    public static void CreateCustomMenu()
    {
        Debug.Log("=== Creating Custom Settings Menu ===");

        UIMenuManager manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager not found!", "OK");
            return;
        }

        if (manager.mainCanvas == null)
        {
            EditorUtility.DisplayDialog("Error", "Main Canvas not found in Manager!", "OK");
            return;
        }

        // 1. Create or Find the Menu Object
        GameObject settingsObj = null;
        Transform existing = manager.mainCanvas.transform.Find("CustomSettingsMenu");
        if (existing != null)
        {
            settingsObj = existing.gameObject;
            // Optional: Clear children to rebuild? Let's just clear for now to ensure clean state
            Undo.DestroyObjectImmediate(settingsObj);
            settingsObj = null;
        }

        settingsObj = new GameObject("CustomSettingsMenu");
        Undo.RegisterCreatedObjectUndo(settingsObj, "Create Custom Settings");
        settingsObj.transform.SetParent(manager.mainCanvas.transform, false);

        // Fullscreen Rect
        RectTransform rt = settingsObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        // Background Image (Dark)
        Image bg = settingsObj.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

        // 2. Add Content
        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(settingsObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "SETTINGS";
        titleText.fontSize = 60;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        titleObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);

        // Volume Slider
        CreateSlider(settingsObj, "Master Volume", 200);
        
        // Quality Dropdown (Simplified representation)
        // (Skipping complex dropdown setup for brevity, using a placeholder text/button or simple toggle for now to ensure robustness, 
        // actually let's just make it a simple list of dummy buttons or toggles to avoid complex TMP Dropdown setup issues from script)
        CreateToggle(settingsObj, "Fullscreen Mode", 100);
        CreateToggle(settingsObj, "High Quality", 0);

        // Back Button
        GameObject backBtnObj = new GameObject("BackButton");
        backBtnObj.transform.SetParent(settingsObj.transform, false);
        Image backImg = backBtnObj.AddComponent<Image>();
        backImg.color = new Color(0.8f, 0.2f, 0.2f, 1f); // Reddish
        Button backBtn = backBtnObj.AddComponent<Button>();
        
        RectTransform backRT = backBtnObj.GetComponent<RectTransform>();
        backRT.sizeDelta = new Vector2(200, 60);
        backRT.anchoredPosition = new Vector2(0, -400);

        GameObject backTextObj = new GameObject("Text");
        backTextObj.transform.SetParent(backBtnObj.transform, false);
        TextMeshProUGUI backText = backTextObj.AddComponent<TextMeshProUGUI>();
        backText.text = "BACK";
        backText.fontSize = 32;
        backText.alignment = TextAlignmentOptions.Center;
        backTextObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 60);

        // 3. Assign to Manager
        Undo.RecordObject(manager, "Assign Custom Settings");
        manager.customSettingsMenu = settingsObj;

        // 4. Wire Back Button
        // Wire to Manager.CloseCustomSettings
        UnityEventTools.AddPersistentListener(backBtn.onClick, manager.CloseCustomSettings);

        // 5. Wire Main Menu Settings Button
        WireMainMenuButton(manager);

        // Start Hidden
        settingsObj.SetActive(false);

        Debug.Log("✓ Custom Settings Menu Created & Wired!");
        EditorUtility.DisplayDialog("Success", "Custom Settings Menu Created!", "OK");
    }

    private static void CreateSlider(GameObject parent, string label, float yPos)
    {
        GameObject container = new GameObject(label);
        container.transform.SetParent(parent.transform, false);
        RectTransform rt = container.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, yPos);
        rt.sizeDelta = new Vector2(400, 50);

        // Limit complexity: Just visual blocks for now
        Image sliderBg = container.AddComponent<Image>();
        sliderBg.color = Color.gray;

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(container.transform, false);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        RectTransform hRT = handle.GetComponent<RectTransform>();
        hRT.sizeDelta = new Vector2(20, 60);

        GameObject txt = new GameObject("Label");
        txt.transform.SetParent(container.transform, false);
        TextMeshProUGUI t = txt.AddComponent<TextMeshProUGUI>();
        t.text = label;
        t.fontSize = 24;
        t.alignment = TextAlignmentOptions.Left;
        RectTransform tRT = txt.GetComponent<RectTransform>();
        tRT.anchoredPosition = new Vector2(-250, 0);
    }

    private static void CreateToggle(GameObject parent, string label, float yPos)
    {
         GameObject container = new GameObject(label);
        container.transform.SetParent(parent.transform, false);
        RectTransform rt = container.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, yPos);
        rt.sizeDelta = new Vector2(400, 50);

        // Visual
        Image bg = container.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f);

        GameObject txt = new GameObject("Label");
        txt.transform.SetParent(container.transform, false);
        TextMeshProUGUI t = txt.AddComponent<TextMeshProUGUI>();
        t.text = label;
        t.fontSize = 24;
        t.alignment = TextAlignmentOptions.Center;
    }

    public static void WireMainMenuButton(UIMenuManager manager)
    {
        if (manager.firstMenu == null) return;
        Button[] buttons = manager.firstMenu.GetComponentsInChildren<Button>(true);
        Button settingsBtn = null;
        foreach (var b in buttons)
        {
            if (b.name.ToLower().Contains("settings") || b.name.ToLower().Contains("option"))
            {
                settingsBtn = b;
                break;
            }
        }

        if (settingsBtn != null)
        {
            Undo.RecordObject(settingsBtn, "Wire Custom Settings");
            settingsBtn.onClick.RemoveAllListeners();
            UnityEventTools.AddPersistentListener(settingsBtn.onClick, manager.OpenCustomSettings);
            Debug.Log("Wired Main Menu Settings Button.");
        }
    }
}
