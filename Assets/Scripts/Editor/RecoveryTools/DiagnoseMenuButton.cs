using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SlimUI.ModernMenu;

public class DiagnoseMenuButton : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/Diagnose Host Button")]
    public static void RunDiagnosis()
    {
        Debug.Log("--- START DEEP DIAGNOSIS ---");

        // 1. Check EventSystem and Input Module
        var es = Object.FindFirstObjectByType<EventSystem>();
        if (es == null) {
            Debug.LogError("CRITICAL: No EventSystem found in active scene!");
        } else {
            Debug.Log($"OK: EventSystem found ({es.name}).");
            
            // Check for Module
            var standAlone = es.GetComponent<StandaloneInputModule>();
            var inputSystem = es.GetComponent("InputSystemUIInputModule"); // Reflection-ish check by string if namespace triggers errors

            if (standAlone != null) Debug.LogWarning("WARNING: Found 'StandaloneInputModule' (Old System). If you are using New Input System, this button WON'T work.");
            if (inputSystem != null) Debug.Log("OK: Found 'InputSystemUIInputModule' (New System).");
            
            if (standAlone == null && inputSystem == null) Debug.LogError("CRITICAL: EventSystem has NO Input Module!");
        }

        // 2. Find Button via Manager (Handles Inactive)
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null)
        {
            Debug.LogError("CRITICAL: UIMenuManager not found.");
            return;
        }

        if (manager.playMenu == null)
        {
            Debug.LogError("CRITICAL: UIMenuManager.playMenu is null.");
            return;
        }

        // Search specifically for Btn_Host inside playMenu
        Button hostBtn = null;
        var buttons = manager.playMenu.GetComponentsInChildren<Button>(true);
        foreach(var b in buttons)
        {
            if (b.name == "Btn_Host") 
            {
                hostBtn = b;
                break;
            }
        }

        if (hostBtn == null)
        {
            Debug.LogError($"CRITICAL: Could not find 'Btn_Host' inside '{manager.playMenu.name}'. Did you run Setup?");
            // List what we did find
            string names = string.Join(", ", System.Array.ConvertAll(buttons, x => x.name));
            Debug.Log($"Found these buttons instead: {names}");
            return;
        }

        Debug.Log($"OK: Found 'Btn_Host' on '{hostBtn.gameObject.name}'. ActiveSelf: {hostBtn.gameObject.activeSelf}");

        // 3. Check Components
        var connector = hostBtn.GetComponent<MainMenuConnector>();
        if (connector == null)
            Debug.LogError("CRITICAL: Missing 'MainMenuConnector' script. The Setup tool did not attach it.");
        else
            Debug.Log("OK: 'MainMenuConnector' script is attached.");

        // 4. Check Listeners
        int count = hostBtn.onClick.GetPersistentEventCount();
        Debug.Log($"Listener Count (Persistent): {count}");
        
        bool hasLoadLobby = false;
        for(int i=0; i<count; i++)
        {
            string targetName = hostBtn.onClick.GetPersistentTarget(i)?.ToString() ?? "null";
            string method = hostBtn.onClick.GetPersistentMethodName(i);
            Debug.Log($"Listener {i}: Target={targetName}, Method={method}");
            if ( method == "LoadLobby") hasLoadLobby = true;
        }

        if (!hasLoadLobby)
            Debug.LogError("CRITICAL: Button has listeners, but NOT 'LoadLobby'. Setup did not wire it correctly.");
        else
            Debug.Log("SUCCESS: Button is wired to LoadLobby.");

        Debug.Log("--- END DIAGNOSIS ---");
    }
}
