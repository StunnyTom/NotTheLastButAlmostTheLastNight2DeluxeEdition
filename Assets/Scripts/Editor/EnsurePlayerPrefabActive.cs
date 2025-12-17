using UnityEngine;
using UnityEditor;
using System.IO;

public class EnsurePlayerPrefabActive : Editor
{
    [MenuItem("Tools/Recovery/15. Enable Player Prefab")]
    public static void Enable()
    {
        string netPath = "Assets/Prefabs/Network/NetworkSurvivor.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(netPath);
        
        if (prefab == null)
        {
            Debug.LogError("Player Prefab not found!");
            return;
        }

        using (var editScope = new PrefabUtility.EditPrefabContentsScope(netPath))
        {
            GameObject root = editScope.prefabContentsRoot;
            
            // 1. Force Active
            if (!root.activeSelf)
            {
                root.SetActive(true);
                Debug.Log("Fixed: Player Prefab was disabled. Enabled it.");
            }
            else
            {
                Debug.Log("Player Prefab root is already active.");
            }

            // 2. Check Renderer
            var renderers = root.GetComponentsInChildren<Renderer>(true); // Include inactive
            foreach(var r in renderers)
            {
                if (!r.enabled || !r.gameObject.activeSelf)
                {
                    r.enabled = true;
                    r.gameObject.SetActive(true);
                    Debug.Log($"Fixed: Enabled renderer '{r.name}'");
                }
            }
            
            // 3. Ensure no weird scale
            if (root.transform.localScale == Vector3.zero)
            {
                root.transform.localScale = Vector3.one;
                Debug.Log("Fixed: Player Prefab scale was zero.");
            }
        }
        
        Debug.Log("Player Prefab verification complete.");
        EditorUtility.DisplayDialog("Fixed", "Verified Player Prefab is Active and Visible.\nRebuild and Test.", "OK");
    }
}
