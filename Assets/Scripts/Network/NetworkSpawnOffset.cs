using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSpawnOffset : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(SpawnMonitorRoutine());
        }
    }

    private System.Collections.IEnumerator SpawnMonitorRoutine()
    {
        // 1. Wait until we are in the Game Scene
        while (!IsGameScene())
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 2. Wait a tiny bit for geometry to load
        yield return new WaitForSeconds(0.5f);

        // 3. Initial Spawn
        AttemptSpawn();

        // 4. Safety Monitor (Keep checking if we fall into void)
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            
            if (transform.position.y < -30f) // Fallen into void
            {
                Debug.LogWarning($"[SpawnMonitor] Player {OwnerClientId} fell into void (Y={transform.position.y}). RESPAWNING.");
                AttemptSpawn();
            }
        }
    }

    private void AttemptSpawn()
    {
        // Find Anchor
        Vector3 anchorPos = Vector3.zero;
        var spawnPoint = GameObject.Find("SpawnPoint");
        if (spawnPoint) anchorPos = spawnPoint.transform.position;
        else anchorPos = new Vector3(-31.5f, 5.5f, 7.2f); // Fallback

        // Offset
        float offset = (float)OwnerClientId * 1.5f;
        Vector3 targetPos = anchorPos + (Vector3.right * offset);
        
        // Ground Check
        if (Physics.Raycast(targetPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
        {
            targetPos = hit.point + Vector3.up * 0.1f;
        }

        // Teleport
        var cc = GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
        
        transform.position = targetPos;
        Debug.Log($"[SpawnMonitor] Teleported Player {OwnerClientId} to {targetPos}");

        if (cc) StartCoroutine(ReEnableCC(cc));
    }

    private System.Collections.IEnumerator ReEnableCC(CharacterController cc)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.1f);
        if (cc) cc.enabled = true;
    }

    private bool IsGameScene()
    {
        var activeScene = SceneManager.GetActiveScene().name;
        // Check if NOT Lobby/Menu. Adjust strings if your scene is named differently!
        return !activeScene.Contains("Lobby") && !activeScene.Contains("Menu");
    }
}
