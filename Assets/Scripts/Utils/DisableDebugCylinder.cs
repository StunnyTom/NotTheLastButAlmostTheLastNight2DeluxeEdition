using UnityEngine;

public class DisableDebugCylinder : MonoBehaviour
{
    void Start()
    {
        // Often default NetworkManager player is a capsule/cylinder
        // Or sometimes it's a child mesh.
        
        // 1. Check self
        var mesh = GetComponent<MeshRenderer>();
        if (mesh && (mesh.name.Contains("Cylinder") || mesh.name.Contains("Capsule")))
        {
            mesh.enabled = false;
        }

        // 2. Check children
        foreach(Transform child in transform)
        {
            if (child.name.Contains("Cylinder") || child.name.Contains("Capsule") || child.name.Contains("Body"))
            {
                 // Usually we want to hide the primitive cylinder but keep the character "Body" if it's the real model.
                 // If the user says "Cylinder surrounding the character", it implies an EXTRA mesh.
                 
                 // Heuristic: If it has a MeshFilter with "Cylinder" mesh
                 var filter = child.GetComponent<MeshFilter>();
                 if (filter && filter.sharedMesh && filter.sharedMesh.name.Contains("Cylinder"))
                 {
                     child.gameObject.SetActive(false);
                 }
            }
        }
    }
}
