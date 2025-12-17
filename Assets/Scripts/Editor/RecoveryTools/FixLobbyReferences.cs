using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FixLobbyReferences : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/16. REWIRE Lobby References")]
    public static void RunFix()
    {
        string scenePath = "Assets/Scenes/LobbyMenu.unity";
        EditorSceneManager.OpenScene(scenePath);

        var controller = Object.FindFirstObjectByType<LobbyUIController>();
        if (controller == null)
        {
             Debug.LogError("Could not find LobbyUIController!");
             return;
        }

        Undo.RecordObject(controller, "Fix Lobby References");

        // 1. Find Content (Robust Search)
        if (controller.playerListContent == null)
        {
             // Try specific path first (Standard Unity ScrollView structure)
             Transform contentHelpers = null;
             
             // Strategy A: Full Path search
             var obj = GameObject.Find("LobbyPanel/PlayerList/Viewport/Content");
             if (obj) contentHelpers = obj.transform;

             // Strategy B: Search inside the controller's hierarchy (if controller is on LobbyPanel)
             if (contentHelpers == null) contentHelpers = controller.transform.Find("PlayerList/Viewport/Content");

             // Strategy C: Search for unique parent name
             if (contentHelpers == null)
             {
                 var playerList = GameObject.Find("PlayerList");
                 if (playerList) contentHelpers = playerList.transform.Find("Viewport/Content");
             }

             if (contentHelpers != null)
             {
                 controller.playerListContent = contentHelpers;
                 Debug.Log($"[FixLobbyReferences] Success! Assigned 'playerListContent' to {contentHelpers.name} (Path: {GetPath(contentHelpers)}).");
             }
             else
             {
                 Debug.LogError("[FixLobbyReferences] FAILED: Could not find 'PlayerList/Viewport/Content'. Please assign manually!");
             }
        }

        // 2. Find Prefab (LobbyPlayerEntry)
        if (controller.playerListEntryPrefab == null)
        {
            // A. Try loading from Resources (Best for runtime)
            var resourcePrefab = Resources.Load<GameObject>("LobbyPlayerEntry");
            if (resourcePrefab != null)
            {
                controller.playerListEntryPrefab = resourcePrefab;
                 Debug.Log($"Found and Assigned 'playerListEntryPrefab' from Resources.");
            }
            // B. Try finding by GUID (Legacy)
            else
            {
                string[] guids = AssetDatabase.FindAssets("LobbyPlayerEntry t:Prefab");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    controller.playerListEntryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    Debug.Log($"Found and Assigned 'playerListEntryPrefab' from {path}.");
                }
                else
                {
                     Debug.LogWarning("Could not find 'LobbyPlayerEntry' prefab. Creating default in Resources.");
                     
                     // Create Placeholder in Resources
                     var go = new GameObject("LobbyPlayerEntry", typeof(RectTransform));
                     var img = go.AddComponent<Image>();
                     img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                     
                     var textObj = new GameObject("PlayerName", typeof(RectTransform));
                     textObj.transform.SetParent(go.transform);
                     var rt = textObj.GetComponent<RectTransform>();
                     rt.anchorMin = Vector2.zero;
                     rt.anchorMax = Vector2.one;
                     rt.offsetMin = new Vector2(10, 0);
                     rt.offsetMax = new Vector2(-10, 0);

                     var txt = textObj.AddComponent<TextMeshProUGUI>();
                     txt.text = "Player Name";
                     txt.fontSize = 24;
                     txt.alignment = TextAlignmentOptions.MidlineLeft;
                     txt.color = Color.white;

                     if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                     {
                         AssetDatabase.CreateFolder("Assets", "Resources");
                     }

                     string newPath = "Assets/Resources/LobbyPlayerEntry.prefab";
                     var prefab = PrefabUtility.SaveAsPrefabAsset(go, newPath);
                     GameObject.DestroyImmediate(go);
                     
                     controller.playerListEntryPrefab = prefab;
                     Debug.Log($"Created and Assigned 'playerListEntryPrefab' at {newPath}.");
                }
            }
        }

        // 3. Fix Join Section ref if missing
        if (controller.joinSection == null)
        {
             var joinRow = GameObject.Find("Row_Join");
             if (joinRow) controller.joinSection = joinRow;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        
        EditorUtility.DisplayDialog("Rewiring Complete", 
            "I have attempted to reconnect the missing references in LobbyUIController.\n\n" +
            "Please check the inspector to confirm.", "OK");
    }
    private static string GetPath(Transform t)
    {
        if (t.parent == null) return t.name;
        return GetPath(t.parent) + "/" + t.name;
    }
}
