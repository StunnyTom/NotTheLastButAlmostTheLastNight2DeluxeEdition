using UnityEngine;
using UnityEditor;

public class AttachDebugUI : Editor
{
    [MenuItem("Tools/Recovery/7. Attach Debug UI (Lobby)")]
    public static void Attach()
    {
        // Create an object for the debugger
        GameObject debugObj = GameObject.Find("LobbyDebugger");
        if (debugObj == null)
        {
            debugObj = new GameObject("LobbyDebugger");
            Undo.RegisterCreatedObjectUndo(debugObj, "Create Debugger");
        }
        
        if (debugObj.GetComponent<LobbyDebugHelp>() == null)
        {
            Undo.AddComponent<LobbyDebugHelp>(debugObj);
            Debug.Log("Attached LobbyDebugHelp.");
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(debugObj.scene);
        EditorUtility.DisplayDialog("Debugger Attached", "Debug Overlay attached to Lobby.\nBuild and Run to see errors on screen.", "OK");
    }
}
