using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class DiagnoseScene : Editor
{
    [MenuItem("Tools/Debug/DIAGNOSE SCENE (Run Me)")]
    public static void RunDiag()
    {
        Debug.Log("--- STARTING DIAGNOSIS ---");

        // 1. Check Controller
        var controller = FindFirstObjectByType<MainMenuController>();
        if (controller == null)
        {
            Debug.LogError("FAIL: 'MainMenuController' script is MISSING from the scene!");
        }
        else
        {
            Debug.Log($"PASS: Found 'MainMenuController' on object '{controller.name}'. Active: {controller.isActiveAndEnabled}");
        }

        // 2. Check Connector (Ghost script)
        var connectors = FindObjectsByType<MainMenuConnector>(FindObjectsSortMode.None);
        foreach(var c in connectors)
        {
            Debug.LogWarning($"INFO: Found 'MainMenuConnector' on '{c.name}'. This might be an old script.");
        }

        // 3. Check Buttons
        var buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach(var b in buttons)
        {
            var txt = b.GetComponentInChildren<TMPro.TMP_Text>()?.text.ToLower() ?? "no text";
            Debug.Log($"Button Found: '{b.name}' [Text: '{txt}']");
        }

        // 4. FIX IF MISSING
        if (controller == null)
        {
            var can = FindFirstObjectByType<Canvas>();
            if (can)
            {
                Debug.Log($"ATTEMPTING FIX: Adding MainMenuController to '{can.name}'...");
                Undo.AddComponent<MainMenuController>(can.gameObject);
            }
        }
        
        Debug.Log("--- END DIAGNOSIS ---");
    }
}
