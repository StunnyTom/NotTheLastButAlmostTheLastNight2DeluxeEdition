using UnityEditor;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Network.Controls;
using UnityEditor.SceneManagement;

public class CreateDebugScene
{
    [MenuItem("Tools/Recovery/Create Debug Scene")]
    public static void CreateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // Create NetworkManager
        GameObject netManagerObj = new GameObject("NetworkManager");
        var netManager = netManagerObj.AddComponent<NetworkManager>();
        var transport = netManagerObj.AddComponent<UnityTransport>();
        
        // Assign Transport
        netManager.NetworkConfig = new NetworkConfig();
        // netManager.NetworkConfig.NetworkTransport = transport; // Is mostly auto-assigned in inspector, but let's double check properties
        
        // Add SimpleControls
        netManagerObj.AddComponent<SimpleNetworkControls>();
        
        // Add Settings Test
        GameObject settingsObj = new GameObject("SettingsDebugger");
        settingsObj.AddComponent<DebugTests.SimpleSettingsTest>();

        // Save
        string path = "Assets/Scenes/_DEBUG_Recovery.unity";
        EditorSceneManager.SaveScene(scene, path);
        Debug.Log($"Created Debug Scene at {path}");
    }
}
