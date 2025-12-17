using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.IO;

public class SetupNetworkCharacters : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/Setup Network Characters")]
    public static void Setup()
    {
        Debug.Log("--- Setup Network Characters ---");

        // 1. Find the Survivor Prefab
        string originalPath = "Assets/characters/Survivor/Prefabs/FemaleSurvivor.prefab";
        GameObject originalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(originalPath);
        
        if (originalPrefab == null)
        {
            Debug.LogError($"Could not find prefab at: {originalPath}");
            return;
        }

        // 2. Create a Network Variant (Copy)
        string netDirectory = "Assets/Prefabs/Network";
        if (!Directory.Exists(netDirectory)) Directory.CreateDirectory(netDirectory);
        
        string netPath = $"{netDirectory}/NetworkSurvivor.prefab";
        
        // Check if exists, if not create copy
        GameObject netPrefab;
        if (!File.Exists(netPath))
        {
            AssetDatabase.CopyAsset(originalPath, netPath);
            AssetDatabase.Refresh();
        }
        netPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(netPath);
        
            // 3. Add Components to Prefab
        using (var editScope = new PrefabUtility.EditPrefabContentsScope(netPath))
        {
            GameObject root = editScope.prefabContentsRoot;
            
            // NetworkObject
            if (root.GetComponent<NetworkObject>() == null)
                root.AddComponent<NetworkObject>();
                
            // CLIENT NETWORK TRANSFORM (Crucial for smooth Client Movement)
            // Remove standard Server-Auth Transform if present
            var serverTrans = root.GetComponent<NetworkTransform>();
            if (serverTrans != null && !(serverTrans is Unity.Netcode.Samples.ClientNetworkTransform))
            {
                Object.DestroyImmediate(serverTrans, true);
            }

            if (root.GetComponent<Unity.Netcode.Samples.ClientNetworkTransform>() == null)
                root.AddComponent<Unity.Netcode.Samples.ClientNetworkTransform>();
                
            // Player Color
            if (root.GetComponent<NetworkPlayerColor>() == null)
                root.AddComponent<NetworkPlayerColor>();
                
            // FORCE VISIBILITY SCRIPT
            if (root.GetComponent<ForcePlayerVis>() == null)
                root.AddComponent<ForcePlayerVis>();
                
            // SPAWN OFFSET SCRIPT (Safe placement logic)
            if (root.GetComponent<NetworkSpawnOffset>() == null)
                root.AddComponent<NetworkSpawnOffset>();
            
            // REMOVED: SimpleNetworkMovement (conflicts with ThirdPersonController)
            var oldMove = root.GetComponent<SimpleNetworkMovement>();
            if (oldMove) Object.DestroyImmediate(oldMove, true);

            // REMOVED: LobbySafePlayer (SurvivorController handles itself now)
            var oldSafe = root.GetComponent<LobbySafePlayer>();
            if (oldSafe) Object.DestroyImmediate(oldSafe, true);

            // DEBUGGER: WhoDisabledMe
            if (root.GetComponent<WhoDisabledMe>() == null)
                root.AddComponent<WhoDisabledMe>();
                
            // VISUAL SAFEGUARD: Only warn if missing
            if (root.GetComponentInChildren<Renderer>() == null)
            {
                Debug.LogWarning("Prefab has no Renderer! Please verify character model.");
            }
            
            // Reset Scale (Just in case)
            root.transform.localScale = Vector3.one;

            // FORCE ACTIVE (Crucial Fix)
            root.SetActive(true);
                
            Debug.Log("Added Network Components to 'NetworkSurvivor.prefab'.");
        }

        // 4. Assign to NetworkManager in SCENE
        var nm = Object.FindFirstObjectByType<NetworkManager>();
        if (nm == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "NetworkManager not found in CURRENT scene!\n" +
                "Please open 'LobbyMenu' scene and run this again.", "OK");
            return;
        }
        
        // Register Prefab
        Undo.RecordObject(nm, "Setup Player Prefab");
        nm.NetworkConfig.PlayerPrefab = netPrefab;
        
        // Add to NetworkPrefabs list if not present
        bool found = false;
        foreach(var p in nm.NetworkConfig.Prefabs.Prefabs) // Access the internal list
        {
            if (p.Prefab == netPrefab) { found = true; break; }
        }
        
        if (!found)
        {
            nm.NetworkConfig.Prefabs.Add(new NetworkPrefab() { Prefab = netPrefab }); // Helper method usually exists
        }

        // Force Save
        EditorUtility.SetDirty(nm);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(nm.gameObject.scene);

        Debug.Log("NetworkManager Configured with Player Prefab");
        EditorUtility.DisplayDialog("Success", 
            "Network Character Setup Complete!\n\n" +
            "1. 'NetworkSurvivor' prefab created.\n" +
            "2. Assigned as Player Prefab in NetworkManager.\n\n" +
            "Next: Build & Run to test multiplayer!", "OK");
    }
}
