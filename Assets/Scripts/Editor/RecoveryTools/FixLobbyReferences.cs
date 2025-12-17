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

        // 1. Find Content
        // Expected hierarchy: LobbyPanel -> Background -> ... -> PlayerList -> Viewport -> Content
        if (controller.playerListContent == null)
        {
             var content = GameObject.Find("Content"); 
             if (content != null)
             {
                 controller.playerListContent = content.transform;
                 Debug.Log("Found and Assigned 'playerListContent'.");
             }
             else
             {
                 Debug.LogWarning("Could not find object named 'Content' in scene.");
             }
        }

        // 2. Find Prefab (LobbyPlayerEntry)
        if (controller.playerListEntryPrefab == null)
        {
            // Try to find it in assets
            string[] guids = AssetDatabase.FindAssets("LobbyPlayerEntry t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                controller.playerListEntryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Debug.Log($"Found and Assigned 'playerListEntryPrefab' from {path}.");
            }
            else
            {
                 Debug.LogWarning("Could not find 'LobbyPlayerEntry' prefab in project. Creating a placeholder.");
                 
                 // Create Placeholder
                 var go = new GameObject("LobbyPlayerEntry", typeof(RectTransform));
                 var img = go.AddComponent<Image>();
                 img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                 
                 var textObj = new GameObject("NameText", typeof(RectTransform));
                 textObj.transform.SetParent(go.transform);
                 // Reset transform of text
                 var rt = textObj.GetComponent<RectTransform>();
                 rt.anchorMin = Vector2.zero;
                 rt.anchorMax = Vector2.one;
                 rt.offsetMin = new Vector2(10, 0);
                 rt.offsetMax = new Vector2(-10, 0);

                 var txt = textObj.AddComponent<TextMeshProUGUI>();
                 txt.text = "Player Name";
                 txt.color = Color.white;
                 txt.fontSize = 24;
                 txt.alignment = TextAlignmentOptions.MidlineLeft;

                 // Ensure Prefabs folder exists
                 if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                 {
                     AssetDatabase.CreateFolder("Assets", "Prefabs");
                 }

                 string newPath = "Assets/Prefabs/LobbyPlayerEntry.prefab";
                 var prefab = PrefabUtility.SaveAsPrefabAsset(go, newPath);
                 GameObject.DestroyImmediate(go);
                 
                 controller.playerListEntryPrefab = prefab;
                 Debug.Log($"Created and Assigned 'playerListEntryPrefab' at {newPath}.");
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
}
