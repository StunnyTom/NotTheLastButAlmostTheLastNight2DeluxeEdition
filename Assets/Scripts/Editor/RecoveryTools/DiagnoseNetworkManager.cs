using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class DiagnoseNetworkManager : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/8. Diagnose Network Manager")]
    public static void Diagnose()
    {
        var nm = Object.FindFirstObjectByType<NetworkManager>();
        if (nm == null)
        {
            Debug.LogError("NO NetworkManager found in the scene (LobbyMenu)!");
            return;
        }
        
        Debug.Log($"NetworkManager Found on '{nm.name}'.");
        
        var transport = nm.GetComponent<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("CRITICAL: NetworkManager is missing 'UnityTransport' component!");
            Debug.Log("Relay requires UnityTransport to work.");
             
             // Auto-fix proposal
             if (EditorUtility.DisplayDialog("Fix Missing Transport?", 
                 "NetworkManager needs 'UnityTransport' for Relay.\nAdd it now?", "Yes", "No"))
             {
                 Undo.AddComponent<UnityTransport>(nm.gameObject);
                 nm.NetworkConfig.NetworkTransport = nm.GetComponent<UnityTransport>();
                 EditorUtility.SetDirty(nm);
                 Debug.Log("Fixed: Added UnityTransport.");
             }
        }
        else
        {
            Debug.Log("OK: UnityTransport is present.");
            Debug.Log($" - Protocol: {transport.Protocol}");
            Debug.Log($" - Max Connect Attempts: {transport.MaxConnectAttempts}");
        }
    }
}
