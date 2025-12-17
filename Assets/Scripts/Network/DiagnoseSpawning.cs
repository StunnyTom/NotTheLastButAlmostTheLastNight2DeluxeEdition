using UnityEngine;
using Unity.Netcode;

public class DiagnoseSpawning : NetworkBehaviour
{
    private void Start()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            // Start logging immediately, don't wait for OnNetworkSpawn (as NM isn't always a Spawned Object)
            StartCoroutine(LogLocalPlayerStatus());
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
        base.OnDestroy();
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.LogWarning($"[DIAGNOSE] Client Connected Event: {clientId}");
    }

    // Removing OnNetworkSpawn as it might not fire if NM is just a scene object without NetworkObject component (or not spawned)
    
    private System.Collections.IEnumerator LogLocalPlayerStatus()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            
            if (NetworkManager.Singleton && NetworkManager.Singleton.IsClient)
            {
                // --- ZOMBIE KILLER START ---
                var allSurvivors = FindObjectsByType<SurvivorSystem.SurvivorController>(FindObjectsSortMode.None);
                foreach (var sur in allSurvivors)
                {
                    var no = sur.GetComponent<NetworkObject>();
                    // Delete duplicates that are NOT spawned by Netcode (phantom objects)
                    if (no == null || !no.IsSpawned)
                    {
                        Debug.LogWarning($"[ZOMBIE KILLER] Removed intruder '{sur.name}' (Not Spawned).");
                        Destroy(sur.gameObject);
                    }
                }
                // --- ZOMBIE KILLER END ---

                var localObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                if (localObj != null)
                {
                    // RUNTIME FIX: If object is disabled, FORCE ENABLE IT
                    if (!localObj.gameObject.activeSelf)
                    {
                        Debug.LogWarning("[DIAGNOSE-CLIENT] Player is disabled! Forcing SetActive(true)...");
                        localObj.gameObject.SetActive(true);
                    }

                    string info = $"[DIAGNOSE-CLIENT] Local Player EXISTS. Pos: {localObj.transform.position}. Active: {localObj.gameObject.activeSelf}";
                    
                    var rend = localObj.GetComponentInChildren<Renderer>();
                    if (rend) info += $" | Renderer: {rend.name} (Enabled: {rend.enabled})";
                    else info += " | NO RENDERER FOUND";
                    
                    var debugCap = localObj.transform.Find("DEBUG_VISUAL_CAPSULE");
                    if (debugCap) info += " | DebugCapsule: FOUND";
                    else info += " | DebugCapsule: MISSING";

                    Debug.LogWarning(info);
                }
                else
                {
                    Debug.LogError($"[DIAGNOSE-CLIENT] Local Player Object is TRUE NULL! (Connected: {NetworkManager.Singleton.IsConnectedClient})");
                }
            }
        }
    }
}
