using UnityEngine;
using UnityEditor;
using Antigravity.Recovery;

public class AttachVisualsFixer
{
    [MenuItem("Tools/Antigravity Kit/Visuals/20. INSTALL Runtime Visuals Fixer")]
    public static void Install()
    {
        string[] paths = new string[]
        {
            "Assets/Prefabs/Network/NetworkSurvivor.prefab",
            "Assets/Characters/Survivor/Prefabs/MaleSurvivor .prefab",
            "Assets/Characters/Survivor/Prefabs/FemaleSurvivor.prefab"
        };

        foreach (var path in paths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            
            if (instance.GetComponent<RemoveDebugVisuals>() == null)
            {
                instance.AddComponent<RemoveDebugVisuals>();
                PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
                Debug.Log($"Installed 'RemoveDebugVisuals' on {prefab.name}");
            }
            
            GameObject.DestroyImmediate(instance);
        }
        Debug.Log("Installation Complete.");
    }
}
