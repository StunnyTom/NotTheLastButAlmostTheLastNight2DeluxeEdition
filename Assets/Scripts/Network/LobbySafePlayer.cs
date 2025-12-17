using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbySafePlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckLobbyState();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckLobbyState();
    }

    private void CheckLobbyState()
    {
        // If we are in the Lobby or Main Menu, DISABLE controls and UNLOCK mouse
        string sceneName = SceneManager.GetActiveScene().name;
        bool inLobby = sceneName.Contains("Lobby") || sceneName.Contains("Menu");

        // 1. Cursor State
        if (inLobby)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // GAMEPLAY: Lock Cursor (Modify if your game needs a visible cursor)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 2. Components (Camera, AudioListener, Controllers)
        
        // AudioListener
        var listener = GetComponentInChildren<AudioListener>(true); // Include inactive to find them
        if (listener) listener.enabled = !inLobby;

        // Cameras
        var cams = GetComponentsInChildren<Camera>(true);
        foreach (var c in cams) c.enabled = !inLobby;

        // Scripts (Controllers, Input)
        var scripts = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var script in scripts)
        {
            if (script == this) continue;
            if (script is NetworkBehaviour || script is NetworkObject) continue;
            
            // Re-enable or Disable based on scene AND Ownership
            if (script.GetType().Name.Contains("Input") || 
                script.GetType().Name.Contains("Controller") ||
                script.GetType().Name.Contains("Character") ||
                script.GetType().Name.Contains("Movement") ||
                script.GetType().Name.Contains("Motor"))
            {
                if (inLobby)
                {
                    script.enabled = false; // Always disabled in Lobby
                }
                else
                {
                    // Game Scene: Only enable if we OWN this object
                    if (IsOwner)
                    {
                         script.enabled = true;
                    }
                    else
                    {
                         script.enabled = false;
                         Debug.Log($"[LobbySafePlayer] Disabled '{script.GetType().Name}' on REMOTE player {OwnerClientId}");
                    }
                }
            }
        }
        
        Debug.Log($"[LobbySafePlayer] Scene '{sceneName}' (Lobby={inLobby}). Controls Active: {(!inLobby && IsOwner)} (IsOwner: {IsOwner})");
    }
    
    // Force Unlock every frame in Lobby just to be sure (some controllers fight back)
    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Lobby") || sceneName.Contains("Menu"))
        {
            if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
