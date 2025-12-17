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
                    // 1. SPECIFIC TARGET: DEBUG_VISUAL_CAPSULE
                    // Based on logs, this is the exact name of the object causing issues.
                    var specificCapsule = instance.transform.Find("DEBUG_VISUAL_CAPSULE");
                    if (specificCapsule != null && specificCapsule.gameObject.activeSelf)
                    {
                        Undo.RecordObject(specificCapsule.gameObject, "Hide Debug Cylinder");
                        specificCapsule.gameObject.SetActive(false);
                        modified = true;
                        Debug.Log($"[FixPlayerVisuals] SUCCESS: Found and DISABLED 'DEBUG_VISUAL_CAPSULE' in {path}");
                    }
                    else
                    {
                        // 2. FALLBACK: Search for "Capsule" or "Debug" objects with MeshRenderers recursively
                        var renderers = instance.GetComponentsInChildren<MeshRenderer>(true);
                        foreach (var mr in renderers)
                        {
                            if (mr.gameObject.name.Contains("Capsule") || mr.gameObject.name.Contains("Cylinder") || mr.gameObject.name.Contains("Debug"))
                            {
                                // Don't disable the MAIN mesh if it's named oddly
                                if (mr.gameObject.GetComponent<MeshFilter>() != null)
                                {
                                    Undo.RecordObject(mr.gameObject, "Hide Debug Cylinder");
                                    mr.enabled = false; 
                                    modified = true;
                                    Debug.Log($"[FixPlayerVisuals] Fallback: Disabled Renderer on '{mr.gameObject.name}' in {path}");
                                }
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
                Debug.LogWarning("Found no 'DEBUG_VISUAL_CAPSULE' or known debug objects to disable. Please check the prefab manually.");
            }
        }
    }
}
