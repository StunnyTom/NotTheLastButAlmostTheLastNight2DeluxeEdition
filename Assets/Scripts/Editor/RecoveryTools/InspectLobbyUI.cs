using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class InspectLobbyUI : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/Inspect Lobby UI Wiring")]
    public static void Inspect()
    {
        Debug.Log("--- INSPECTING LOBBY UI ---");
        var buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        if (buttons.Length == 0) Debug.Log("No buttons found.");
        
        foreach (var btn in buttons)
        {
            Debug.Log($"BUTTON: '{btn.name}' (Parent: {btn.transform.parent.name})");
            int count = btn.onClick.GetPersistentEventCount();
            if (count == 0) Debug.Log("  -> No Listeners.");
            
            for(int i=0; i<count; i++)
            {
                var target = btn.onClick.GetPersistentTarget(i);
                string targetName = target ? target.GetType().Name : "null/Missing";
                string method = btn.onClick.GetPersistentMethodName(i);
                
                Debug.Log($"  -> Listener {i}: Target={targetName}, Method={method}");
            }
        }
        Debug.Log("--- END INSPECTION ---");
    }
}
