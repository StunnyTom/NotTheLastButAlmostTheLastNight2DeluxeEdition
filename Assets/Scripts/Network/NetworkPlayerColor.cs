using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerColor : NetworkBehaviour
{
    private readonly Color[] _colors = new Color[] 
    {
        Color.white, 
        Color.red, 
        Color.blue, 
        Color.green, 
        Color.yellow, 
        Color.cyan 
    };

    public override void OnNetworkSpawn()
    {
        // Simple logic: Change color based on ID
        ulong id = OwnerClientId;
        Color assignedColor = _colors[id % (ulong)_colors.Length];

        // Apply to all Renderers (MeshRenderer or SkinnedMeshRenderer)
        var renderers = GetComponentsInChildren<Renderer>();
        foreach(var r in renderers)
        {
            r.material.color = assignedColor;
        }
        
        Debug.Log($"Player {id} spawned with color {assignedColor}");
    }
}
