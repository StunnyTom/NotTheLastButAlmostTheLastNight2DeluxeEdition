using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class FinalSettingsFix : EditorWindow
{
    [MenuItem("Tools/FINAL Settings Fix")]
    public static void Fix()
    {
        Debug.Log("=== FINAL SETTINGS FIX ===");
        
        // 1. Find all SettingsCanvas
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] allObjects = scene.GetRootGameObjects();
        
        GameObject[] settingsCanvases = System.Array.FindAll(allObjects, obj => obj.name == "SettingsCanvas");
        
        Debug.Log($"Found {settingsCanvases.Length} SettingsCanvas objects");
        
        // 2. Keep only the one with SettingsManager, delete the rest
        GameObject keepCanvas = null;
        int deleted = 0;
        
        foreach (var canvas in settingsCanvases)
        {
            if (canvas.GetComponent<SettingsManager>() != null)
            {
                keepCanvas = canvas;
                Debug.Log($"Keeping: {canvas.name} (has SettingsManager)");
            }
            else
            {
                Debug.Log($"Destroying duplicate: {canvas.name}");
                Undo.DestroyObjectImmediate(canvas);
                deleted++;
            }
        }
        
        if (keepCanvas == null)
        {
            Debug.LogError("No SettingsCanvas with SettingsManager found!");
            return;
        }
        
        // 3. Make sure it starts disabled
        keepCanvas.SetActive(false);
        
        // 4. Check that it has content
        int childCount = keepCanvas.transform.childCount;
        Debug.Log($"SettingsCanvas has {childCount} children");
        
        if (childCount == 0)
        {
            Debug.LogWarning("⚠ SettingsCanvas has NO children! It will be empty when shown.");
        }
        
        Debug.Log($"\n✓ Deleted {deleted} duplicate SettingsCanvas");
        Debug.Log("✓ Settings should now work when you click the Settings button!");
        
        EditorUtility.DisplayDialog("Success!", 
            $"Cleaned up {deleted} duplicate SettingsCanvas!\n\nNow run 'Tools -> Fix Settings -> Wire Settings Button to 2D Canvas' one more time, then test!",
            "OK");
    }
}
