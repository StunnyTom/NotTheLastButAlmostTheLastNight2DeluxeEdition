using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Antigravity.Recovery
{
    public class CheckSceneContent
    {
        [MenuItem("Tools/Antigravity Kit/Diagnostics/22. CHECK Scene Content (Tom's Updates)")]
        public static void Check()
        {
            var rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
            bool foundLimits = false;

            Debug.Log($"--- Checking Scene: {SceneManager.GetActiveScene().name} ---");
            foreach (var go in rootObjs)
            {
                if (go.name == "Gameplay Limits" || go.name.Contains("Content"))
                {
                    Debug.Log($"[FOUND] {go.name} (Active: {go.activeInHierarchy})");
                    // Check children
                    foreach(Transform child in go.transform)
                    {
                         if (child.name.Contains("InvisibleWall") || child.name.Contains("Limit"))
                            Debug.Log($"   -> Child: {child.name}");
                    }
                    foundLimits = true;
                }
            }

            if (!foundLimits)
            {
                Debug.LogWarning("❌ 'Gameplay Limits' NOT found in root objects.");
                // Try searching strictly in "Content" if it exists
                var content = GameObject.Find("Content");
                if (content)
                {
                    var limits = content.transform.Find("Gameplay Limits");
                    if (limits)
                        Debug.Log($"✅ Found 'Gameplay Limits' nested inside 'Content'.");
                }
            }
        }
    }
}
