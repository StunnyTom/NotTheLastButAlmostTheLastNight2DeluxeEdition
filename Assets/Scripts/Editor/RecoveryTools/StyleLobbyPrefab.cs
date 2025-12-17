using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace Antigravity.Recovery
{
    public class StyleLobbyPrefab
    {
        [MenuItem("Tools/Antigravity Kit/Recovery/18. STYLE Lobby Prefab (Make it Pretty)")]
        public static void StylePrefab()
        {
            string path = "Assets/Resources/LobbyPlayerEntry.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogError($"Prefab not found at {path}. Please run the game or 'REWIRE Lobby References' to generate it first.");
                return;
            }

            // Instantiate to edit
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            
            try 
            {
                Undo.RecordObject(instance, "Style Lobby Prefab");
                
                // 1. Setup Layout Element (for Vertical Layout Group compatibility)
                var le = instance.GetComponent<LayoutElement>();
                if (le == null) le = instance.AddComponent<LayoutElement>();
                le.minHeight = 60;
                le.preferredHeight = 60;
                le.flexibleWidth = 1;

                // 2. Setup Background Image
                var img = instance.GetComponent<Image>();
                if (img == null) img = instance.AddComponent<Image>();
                img.color = new Color(0.15f, 0.15f, 0.2f, 0.9f); // Dark tint
                
                // 3. Find/Create Text
                TextMeshProUGUI txt = instance.GetComponentInChildren<TextMeshProUGUI>();
                if (txt)
                {
                    // Center the text properly
                    var rt = txt.GetComponent<RectTransform>();
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = new Vector2(25, 0); // Left padding
                    rt.offsetMax = new Vector2(-25, 0); // Right padding
                    
                    txt.alignment = TextAlignmentOptions.MidlineLeft;
                    txt.fontSize = 28;
                    txt.color = new Color(0.9f, 0.9f, 0.9f, 1f);
                    txt.fontStyle = FontStyles.Bold;
                    
                    // Add Shadow or Outline if needed? (Skip for now to keep it simple)
                }

                // Apply changes back to Prefab
                PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
                Debug.Log("Styled 'LobbyPlayerEntry.prefab' successfully!");
            }
            finally
            {
                GameObject.DestroyImmediate(instance);
            }
        }
    }
}
