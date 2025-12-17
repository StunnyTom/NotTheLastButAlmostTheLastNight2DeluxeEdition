using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class RestoreAndCleanUI : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/10. Restore and Clean Lobby UI")]
    public static void Restore()
    {
        Debug.Log("--- Restoring and Cleaning UI ---");

        // 1. Find ALL objects (including disabled ones)
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        var lobbyCanvas = GameObject.Find("LobbyCanvas"); // Try to narrow down if possible, but global is okay for recovery
        
        string[] targetNames = new string[] 
        { 
            "Create Session", 
            "Join Session By Code", 
            "Leave Session", 
            "Show Session Code",
            "Session Player List"
        };

        int restoredCount = 0;

        foreach (var obj in allObjects)
        {
            // Filter out assets/prefabs, only scene objects
            if (obj.hideFlags != HideFlags.None || EditorUtility.IsPersistent(obj)) continue;

            foreach (var targetName in targetNames)
            {
                if (obj.name == targetName && !obj.activeSelf)
                {
                    // 2. Re-enable Object
                    obj.SetActive(true);
                    Debug.Log($"Restored (Re-enabled): {obj.name}");
                    restoredCount++;

                    // 3. Strip Conflicting Components
                    var components = obj.GetComponents<MonoBehaviour>();
                    foreach (var comp in components)
                    {
                        if (comp == null) continue;
                        string typeName = comp.GetType().FullName;
                        if (typeName.Contains("Multiplayer.Widgets"))
                        {
                            Undo.DestroyObjectImmediate(comp);
                            Debug.Log($" - Removed Conflicting Component: {typeName}");
                        }
                    }
                }
            }
        }

        // 4. Ensure LobbyUIController is connected
        var controller = Object.FindFirstObjectByType<LobbyUIController>();
        if (controller != null)
        {
            // Auto-wire if null
            if (controller.createSessionBtn == null) 
                controller.createSessionBtn = FindButton("Create Session");
            
            if (controller.joinSessionBtn == null) 
                controller.joinSessionBtn = FindButton("Join Session By Code"); // Often inside this object
            
            if (controller.joinCodeInput == null)
                controller.joinCodeInput = FindInputField("Join Session By Code");

            if (controller.joinCodeDisplay == null)
            {
                var displayObj = GameObject.Find("Show Session Code");
                if (displayObj) controller.joinCodeDisplay = displayObj.GetComponentInChildren<TMP_Text>();
            }
            
            if (controller.leaveSessionBtn == null)
                 controller.leaveSessionBtn = FindButton("Leave Session");

            EditorUtility.SetDirty(controller);
            Debug.Log("Attempted to auto-wire LobbyUIController.");
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        EditorUtility.DisplayDialog("UI Restored", $"Restored {restoredCount} objects and stripped conflicting scripts.\nCheck LobbyUIController connections in Inspector if needed.", "OK");
    }

    private static Button FindButton(string goName)
    {
        var go = GameObject.Find(goName);
        if (go) return go.GetComponent<Button>() ?? go.GetComponentInChildren<Button>();
        return null;
    }

    private static TMP_InputField FindInputField(string goName)
    {
        var go = GameObject.Find(goName);
        if (go) return go.GetComponent<TMP_InputField>() ?? go.GetComponentInChildren<TMP_InputField>();
        return null;
    }
}
