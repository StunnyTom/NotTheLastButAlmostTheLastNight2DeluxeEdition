using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Unity.Netcode;

public class DiagnoseLobby : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/Diagnose Lobby Logic")]
    public static void RunDiagnosis()
    {
        Debug.Log("--- START LOBBY DIAGNOSIS ---");

        // 1. NetworkManager
        var nm = Object.FindFirstObjectByType<NetworkManager>();
        if (nm == null) 
        {
            Debug.LogError("CRITICAL: NetworkManager NOT found in this scene. Hosting/Joining will fail.");
        }
        else
        {
            Debug.Log($"OK: NetworkManager found ({nm.name}).");
            if (nm.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>() == null)
            {
                Debug.LogError("CRITICAL: NetworkManager is missing 'UnityTransport' component!");
            }
            else
            {
                Debug.Log("OK: UnityTransport attached.");
            }
        }

        // 2. NetworkUIHelper
        var helper = Object.FindFirstObjectByType<NetworkUIHelper>();
        if (helper == null)
        {
            Debug.LogError("CRITICAL: NetworkUIHelper script NOT found in scene. buttons won't work.");
            return;
        }
        else
        {
            Debug.Log($"OK: NetworkUIHelper found on ({helper.name}).");
        }

        // 3. Check References
        if (helper.hostIpDisplay == null) Debug.LogWarning("WARNING: 'hostIpDisplay' is Not assigned. IP won't show.");
        else Debug.Log("OK: Host IP Display assigned.");

        if (helper.joinIpInput == null) Debug.LogWarning("WARNING: 'joinIpInput' is Not assigned. Client IP entry won't work.");
        else Debug.Log($"OK: Join IP Input assigned.");

        // 4. Check Buttons (Optional heuristic)
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        bool hasHost = false;
        bool hasJoin = false;
        foreach (var btn in buttons)
        {
            int count = btn.onClick.GetPersistentEventCount();
            for(int i=0; i<count; i++)
            {
                string method = btn.onClick.GetPersistentMethodName(i);
                if (method == "StartHostVoid") hasHost = true;
                if (method == "StartClientVoid") hasJoin = true;
            }
        }

        if (hasHost) Debug.Log("OK: Found a button wired to 'StartHostVoid'.");
        else Debug.LogError("CRITICAL: No button found wired to 'StartHostVoid'.");

        if (hasJoin) Debug.Log("OK: Found a button wired to 'StartClientVoid'.");
        else Debug.LogError("CRITICAL: No button found wired to 'StartClientVoid'.");

        Debug.Log("--- END LOBBY DIAGNOSIS ---");
    }
}
