using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.Netcode;

public class SetupLobbySync : EditorWindow
{
    [MenuItem("Tools/Antigravity Kit/Recovery/15. SETUP Lobby Sync")]
    public static void Setup()
    {
        string scenePath = "Assets/Scenes/LobbyMenu.unity";
        EditorSceneManager.OpenScene(scenePath);

        // 1. Find or Create LobbyManager
        GameObject manager = GameObject.Find("LobbyManager");
        if (manager == null)
        {
            manager = new GameObject("LobbyManager");
            Undo.RegisterCreatedObjectUndo(manager, "Create LobbyManager");
        }

        // 2. Add NetworkObject (Required for Sync)
        var netObj = manager.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            netObj = Undo.AddComponent<NetworkObject>(manager);
        }

        // 3. Add LobbySync
        var sync = manager.GetComponent<LobbySync>();
        if (sync == null)
        {
             sync = Undo.AddComponent<LobbySync>(manager);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        
        Debug.Log("LobbySync has been added to 'LobbyManager'.");
        EditorUtility.DisplayDialog("Lobby Sync Added", 
            "Created object 'LobbyManager' with LobbySync + NetworkObject.\n\n" +
            "This will now synchronize the player list automatically.", "OK");
    }
}
