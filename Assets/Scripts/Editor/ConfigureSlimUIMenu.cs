using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SlimUI.ModernMenu;

public class ConfigureSlimUIMenu : EditorWindow
{
    [MenuItem("Tools/Configure SlimUI Menu")]
    public static void ConfigureMenu()
    {
        Debug.Log("--- Configuring SlimUI Menu ---");

        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find UIMenuManager in the scene.", "OK");
            return;
        }

        // Find all buttons in the first menu
        GameObject firstMenu = manager.firstMenu;
        if (firstMenu == null)
        {
            EditorUtility.DisplayDialog("Error", "firstMenu is not assigned in UIMenuManager!", "OK");
            return;
        }

        // Find buttons by name
        Button settingsButton = null;
        Button extrasButton = null;

        Button[] buttons = firstMenu.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            string btnName = btn.name.ToLower();
            if (btnName.Contains("settings") || btnName.Contains("option"))
            {
                settingsButton = btn;
            }
            else if (btnName.Contains("extras") || btnName.Contains("extra"))
            {
                extrasButton = btn;
            }
        }

        // Hide Extras button
        if (extrasButton != null)
        {
            Undo.RecordObject(extrasButton.gameObject, "Hide Extras Button");
            extrasButton.gameObject.SetActive(false);
            Debug.Log("Hidden Extras button: " + extrasButton.name);
        }
        else
        {
            Debug.LogWarning("Could not find Extras button.");
        }

        // Configure Settings button
        if (settingsButton != null)
        {
            // Remove existing onClick listeners
            Undo.RecordObject(settingsButton, "Configure Settings Button");
            settingsButton.onClick.RemoveAllListeners();

            // Add the correct listener to open settings
            settingsButton.onClick.AddListener(() => {
                manager.Position2(); // Move camera to settings position
                manager.GamePanel(); // Show the Game settings panel by default
            });

            Debug.Log("Configured Settings button: " + settingsButton.name);
        }
        else
        {
            Debug.LogWarning("Could not find Settings button.");
        }

        EditorUtility.SetDirty(manager);
        if (settingsButton != null) EditorUtility.SetDirty(settingsButton);

        EditorUtility.DisplayDialog("Success", 
            "Menu configured!\n\n" +
            (extrasButton != null ? "✓ Extras button hidden\n" : "✗ Extras button not found\n") +
            (settingsButton != null ? "✓ Settings button configured" : "✗ Settings button not found"),
            "OK");
    }
}
