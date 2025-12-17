using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace Antigravity.Recovery
{
    [InitializeOnLoad]
    public class AutoCreateLobbyPrefab
    {
        static AutoCreateLobbyPrefab()
        {
            // Delay slightly to ensure asset database is ready
            EditorApplication.delayCall += CheckAndCreate;
        }

        private static void CheckAndCreate()
        {
            string folderPath = "Assets/Resources";
            string filePath = "Assets/Resources/LobbyPlayerEntry.prefab";

            if (!Directory.Exists(folderPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            // Check if prefab exists
            if (AssetDatabase.LoadAssetAtPath<GameObject>(filePath) == null)
            {
                Debug.LogWarning("[AutoRecovery] 'LobbyPlayerEntry.prefab' missing in Resources. Creating default...");

                // Create the GameObject
                GameObject go = new GameObject("LobbyPlayerEntry", typeof(RectTransform));
                
                // Add Image Background
                var img = go.AddComponent<Image>();
                img.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark background
                var rect = go.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(0, 50); // Height 50

                // Add Text
                GameObject textObj = new GameObject("PlayerName", typeof(RectTransform));
                textObj.transform.SetParent(go.transform);
                var textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.offsetMin = new Vector2(20, 0); // Padding left
                textRect.offsetMax = new Vector2(-20, 0); // Padding right

                var txt = textObj.AddComponent<TextMeshProUGUI>();
                txt.text = "Unknown Player";
                txt.fontSize = 28;
                txt.alignment = TextAlignmentOptions.MidlineLeft;
                txt.color = Color.white;

                // Save as Prefab
                PrefabUtility.SaveAsPrefabAsset(go, filePath);
                Object.DestroyImmediate(go);

                Debug.Log($"[AutoRecovery] Created default prefab at: {filePath}");
                AssetDatabase.Refresh();
            }
        }
    }
}
