using UnityEngine;

namespace Antigravity.Recovery
{
    public class RemoveDebugVisuals : MonoBehaviour
    {
        private void Start()
        {
            // 1. Target specific object
            var debugCapsule = transform.Find("DEBUG_VISUAL_CAPSULE");
            if (debugCapsule != null)
            {
                debugCapsule.gameObject.SetActive(false);
                // Optional: Destroy(debugCapsule.gameObject);
            }

            // 2. Fallback: Find any Capsule/Cylinder mesh renderer
            var renderers = GetComponentsInChildren<MeshRenderer>(true);
            foreach (var mr in renderers)
            {
                if (mr.name.Contains("Capsule") || mr.name.Contains("Cylinder") || mr.name.Contains("Debug"))
                {
                    // Avoid disabling the main character mesh if it happens to be named oddly
                    // Usually main meshes are SkinnedMeshRenderers, so this is safe for primitive capsules
                    mr.enabled = false;
                }
            }
        }
    }
}
