using UnityEngine;
using UnityEditor;

namespace Antigravity.Recovery
{
    public class FixNetworkSurvivorTag
    {
        [MenuItem("Tools/Antigravity Kit/Network/21. FIX NetworkSurvivor Tag")]
        public static void FixTag()
        {
            string path = "Assets/Prefabs/Network/NetworkSurvivor.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) 
            {
                Debug.LogError($"Prefab not found at {path}");
                return;
            }

            // Instantiate to edit
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            bool modified = false;

            try 
            {
                if (!instance.CompareTag("Player"))
                {
                    Undo.RecordObject(instance, "Set Tag to Player");
                    instance.tag = "Player";
                    modified = true;
                    Debug.Log($"Updated Tag for {instance.name} to 'Player'.");
                }
                else
                {
                    Debug.Log($"{instance.name} is already tagged as 'Player'.");
                }

                if (modified)
                {
                    PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
                }
            }
            finally
            {
                GameObject.DestroyImmediate(instance);
            }
        }
    }
}
