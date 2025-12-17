using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Antigravity.Recovery
{
    public class FixPlayerVisuals
    {
        [MenuItem("Tools/Antigravity Kit/Visuals/19. HIDE Player Debug Cylinder")]
        public static void HideCylinder()
        {
            // List of potential prefabs to checking
            string[] paths = new string[]
            {
                "Assets/Prefabs/Network/NetworkSurvivor.prefab",
                "Assets/Characters/Survivor/Prefabs/FemaleSurvivor.prefab",
                "Assets/Characters/Survivor/Prefabs/MaleSurvivor .prefab" // noted space
            };

            int fixedCount = 0;

            foreach (string path in paths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                // Instantiate in edit mode
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                bool modified = false;

                try
                {
                    // Search for "Capsule" or "Debug" objects with MeshRenderers
                    var renderers = instance.GetComponentsInChildren<MeshRenderer>(true);
                    foreach (var mr in renderers)
                    {
                        // Logic: If it looks like a debug capsule (primitive name) and is active
                        if (mr.gameObject.name.Contains("Capsule") || mr.gameObject.name.Contains("Cylinder") || mr.gameObject.name.Contains("Debug"))
                        {
                            // Don't disable the MAIN mesh if it's named oddly, but usually main mesh is SkinnedMeshRenderer
                            // Simple primitive capsules often have MeshFilter + MeshRenderer
                            if (mr.gameObject.GetComponent<MeshFilter>() != null)
                            {
                                Undo.RecordObject(mr.gameObject, "Hide Debug Cylinder");
                                mr.enabled = false; // Just disable the renderer, keep collider if needed
                                modified = true;
                                Debug.Log($"[FixPlayerVisuals] Disabled Renderer on '{mr.gameObject.name}' in {path}");
                            }
                        }
                    }

                    if (modified)
                    {
                        PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
                        fixedCount++;
                    }
                }
                finally
                {
                    GameObject.DestroyImmediate(instance);
                }
            }

            if (fixedCount > 0)
            {
                Debug.Log($"Successfully patched {fixedCount} prefabs. The cylinder should be gone.");
            }
            else
            {
                Debug.LogWarning("Found no 'Capsule' or 'Cylinder' MeshRenderers to disable. Please check the prefab manually.");
            }
        }
    }
}
