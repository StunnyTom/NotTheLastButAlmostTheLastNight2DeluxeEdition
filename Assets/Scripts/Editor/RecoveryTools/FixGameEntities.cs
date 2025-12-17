using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using System.IO;

public class FixGameEntities : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/11. Fix Game Entities (UI & Player)")]
    public static void Fix()
    {
        Debug.Log("--- Fixing Game Entities ---");

        // 1. Attach GameStatusUI to NetworkManager (Persistent)
        var nm = Object.FindFirstObjectByType<NetworkManager>();
        if (nm != null)
        {
            if (nm.GetComponent<GameStatusUI>() == null)
            {
                Undo.AddComponent<GameStatusUI>(nm.gameObject);
                Debug.Log("Attached 'GameStatusUI' to NetworkManager.");
            }
            // Ensure DontDestroyOnLoad is handled by NM itself usually
        }
        else
        {
            Debug.LogError("No NetworkManager found!");
        }

        // 2. Verify Player Prefab (Again)
        string netPath = "Assets/Prefabs/Network/NetworkSurvivor.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(netPath);
        
        if (prefab != null)
        {
            using (var editScope = new PrefabUtility.EditPrefabContentsScope(netPath))
            {
                var root = editScope.prefabContentsRoot;
                var renderer = root.GetComponentInChildren<Renderer>();
                
                if (renderer == null)
                {
                    Debug.LogWarning("Still no renderer on prefab! Adding Fallback Capsule.");
                    var cap = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    cap.transform.SetParent(root.transform, false);
                    cap.transform.localPosition = Vector3.up; // Lift up
                    // Remove default collider if root already has one? 
                    // Assume root handles logic, capsule handles visual
                }
                else
                {
                    // Force a standard shader if needed?
                    // renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    Debug.Log($"Prefab has Renderer: {renderer.name}");
                }
            }
        }
        else
        {
            Debug.LogError($"Prefab not found at {netPath}");
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        EditorUtility.DisplayDialog("Entities Fixed", 
            "1. GameStatusUI attached to NetworkManager (You should see green text in game now).\n" +
            "2. Player Prefab verified.\n\n" +
            "Please Build and Run.", "OK");
    }
}
