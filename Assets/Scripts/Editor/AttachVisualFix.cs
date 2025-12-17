using UnityEngine;
using UnityEditor;
using SlimUI.ModernMenu;

public class AttachVisualFix : Editor
{
    [MenuItem("Tools/Recovery/6. Attach Runtime Visual Fix")]
    public static void Attach()
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager != null)
        {
            if (manager.GetComponent<ForceStylesRuntime>() == null)
            {
                Undo.AddComponent<ForceStylesRuntime>(manager.gameObject);
                Debug.Log($"Attached 'ForceStylesRuntime' to {manager.name}");
                EditorUtility.DisplayDialog("Success", "Runtime Visual Fix attached!\nIt will force the screen scale when the game starts.\n\nNow SAVE and BUILD.", "OK");
            }
            else
            {
                Debug.Log("Fix already attached.");
            }
            
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        }
        else
        {
             Debug.LogError("Could not find UIMenuManager to attach script to.");
        }
    }
}
