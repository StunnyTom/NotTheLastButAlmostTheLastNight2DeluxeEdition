using UnityEngine;
using Unity.Netcode;

public class ForcePlayerVis : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log($"ForcePlayerVis: Spawning visual for Player {OwnerClientId}");

        // 1. Force Scale Reset
        transform.localScale = Vector3.one;

        // 2. ALWAYS Create Instant Primitive (Debug Mode) to ensure visibility
        //    (Previously we skipped if a renderer existed, but maybe that renderer is invisible/broken)
        GameObject debugVis = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        debugVis.transform.SetParent(transform, false);
        debugVis.transform.localPosition = Vector3.up * 1.1f; // Slightly higher to see feet
        debugVis.name = "DEBUG_VISUAL_CAPSULE";
        
        // 3. Force Layer to Default
        SetLayerRecursively(gameObject, 0); // 0 = Default layer

        // 4. [REMOVED] Color on Debug Capsule
        // var rend = debugVis.GetComponent<Renderer>(); ...
        
        // 5. Check original renderer too
        var existingRend = GetComponentInChildren<SkinnedMeshRenderer>(); // Specifically look for character mesh
        if (existingRend)
        {
            existingRend.enabled = true; // Force enable
            Debug.Log($"Found existing SkinnedMeshRenderer: {existingRend.name}");
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private Color GetColorID(ulong id)
    {
        Color[] colors = new Color[] { Color.white, Color.red, Color.blue, Color.green, Color.yellow, Color.cyan };
        return colors[id % (ulong)colors.Length];
    }
}
