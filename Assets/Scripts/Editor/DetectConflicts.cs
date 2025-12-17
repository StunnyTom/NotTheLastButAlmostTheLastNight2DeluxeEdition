using UnityEngine;
using UnityEditor;

public class DetectConflicts : Editor
{
    [MenuItem("Tools/Recovery/9. Detect Multiplayer Widgets Conflicts")]
    public static void Detect()
    {
        Debug.Log("--- Scanning for Multiplayer Widgets Conflicts ---");
        
        // Types known to cause issues (Reflection to avoid hard dependency errors if pkg missing)
        string[] typesToFind = new string[] 
        { 
            "Unity.Multiplayer.Widgets.SessionManager", 
            "Unity.Multiplayer.Widgets.LobbyList",
            "Unity.Services.Multiplayer.SessionManager" 
        };

        bool found = false;
        
        // 1. MonoBehaviours scan
        var allObjects = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            string typeName = obj.GetType().FullName;
            foreach (var target in typesToFind)
            {
                if (typeName != null && typeName.Contains("Multiplayer.Widgets"))
                {
                    Debug.LogError($"CONFLICT FOUND: '{obj.name}' has component '{typeName}'!");
                    found = true;
                    
                    if (EditorUtility.DisplayDialog("Conflict Found", 
                        $"Found '{typeName}' on '{obj.name}'.\nThis is likely causing the 'Already member of lobby' error.\n\nDisable it?", "Yes, Disable", "No"))
                    {
                        obj.enabled = false;
                        obj.gameObject.SetActive(false); // Nuke the GO if can
                        Undo.RecordObject(obj.gameObject, "Disable Widget");
                        Debug.Log($"DISABLED conflicting object: {obj.name}");
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(obj.gameObject.scene);
                    }
                }
            }
        }

        if (!found)
        {
            Debug.Log("No obvious 'Multiplayer Widgets' components found active in the scene.");
            Debug.Log("Check if 'NetworkManager' has any other scripts attached.");
        }
    }
}
