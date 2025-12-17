using UnityEngine;
using UnityEditor;

public class AttachRuntimeLogger : Editor
{
    [MenuItem("Tools/Recovery/13. Attach Runtime Logger")]
    public static void Attach()
    {
        var nm = Object.FindFirstObjectByType<Unity.Netcode.NetworkManager>();
        if (nm != null)
        {
            if (nm.GetComponent<RuntimeLogger>() == null)
            {
                Undo.AddComponent<RuntimeLogger>(nm.gameObject);
                Debug.Log("Attached 'RuntimeLogger' to NetworkManager.");
            }
            else
            {
                Debug.Log("'RuntimeLogger' already attached.");
            }
        }
        else
        {
            // Fallback: Create standalone object
            GameObject go = new GameObject("GlobalRuntimeLogger");
            go.AddComponent<RuntimeLogger>();
            Debug.Log("Created standalone 'GlobalRuntimeLogger'.");
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }
}
