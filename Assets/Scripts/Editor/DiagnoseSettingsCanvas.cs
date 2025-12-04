using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class DiagnoseSettingsCanvas : EditorWindow
{
    [MenuItem("Tools/Fix Settings/Diagnose Settings Canvas")]
    public static void Diagnose()
    {
        Debug.Log("=== DIAGNOSING SETTINGS CANVAS ===");
        
        // Search in active scene
        Scene activeScene = SceneManager.GetActiveScene();
        Debug.Log($"Active Scene: {activeScene.name}");
        
        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        Debug.Log($"Root GameObjects in scene: {rootObjects.Length}");
        
        bool found = false;
        foreach (var obj in rootObjects)
        {
            Debug.Log($"  - {obj.name}");
            if (obj.name == "SettingsCanvas")
            {
                found = true;
                Debug.Log($"    ✓ FOUND SettingsCanvas! Active: {obj.activeSelf}");
                
                // Check children
                Debug.Log($"    Children count: {obj.transform.childCount}");
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Debug.Log($"      - {obj.transform.GetChild(i).name}");
                }
            }
        }
        
        if (!found)
        {
            Debug.LogWarning("❌ SettingsCanvas NOT FOUND in scene root objects!");
            Debug.LogWarning("Try running 'Tools -> Fix Settings -> Create 2D Settings Canvas' again.");
        }
        
        Debug.Log("=== DIAGNOSIS COMPLETE ===");
    }
}
