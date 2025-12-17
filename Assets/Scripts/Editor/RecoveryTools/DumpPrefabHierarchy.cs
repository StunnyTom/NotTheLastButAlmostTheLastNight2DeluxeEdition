using UnityEngine;
using UnityEditor;

namespace Antigravity.Recovery
{
    public class DumpPrefabHierarchy
    {
        [MenuItem("Tools/Antigravity Kit/Visuals/Debug Prefab Hierarchy")]
        public static void Dump()
        {
            string path = "Assets/Prefabs/Network/NetworkSurvivor.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) 
            {
                Debug.LogError("Prefab not found!");
                return;
            }

            Debug.Log($"--- DUMPING ITERATIVE HIERARCHY FOR {prefab.name} ---");
            DumpRecursive(prefab.transform, "");
            Debug.Log("--- END DUMP ---");
        }

        private static void DumpRecursive(Transform t, string indent)
        {
            var mr = t.GetComponent<MeshRenderer>();
            var smr = t.GetComponent<SkinnedMeshRenderer>();
            string renderInfo = "";
            if (mr) renderInfo = $" [MeshRenderer: {mr.enabled}]";
            if (smr) renderInfo = $" [SkinnedMeshRenderer: {smr.enabled}]";

            Debug.Log($"{indent}{t.name}{renderInfo}");

            foreach (Transform child in t)
            {
                DumpRecursive(child, indent + "  ");
            }
        }
    }
}
