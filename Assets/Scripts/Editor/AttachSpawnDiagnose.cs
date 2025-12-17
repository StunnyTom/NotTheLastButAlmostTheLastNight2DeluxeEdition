    using UnityEngine;
using UnityEditor;
using Unity.Netcode;

public class AttachSpawnDiagnose : Editor
{
    [MenuItem("Tools/Recovery/14. Attach Spawn Diagnostics")]
    public static void Attach()
    {
        var nm = Object.FindFirstObjectByType<NetworkManager>();
        if (nm != null)
        {
            if (nm.GetComponent<DiagnoseSpawning>() == null)
            {
                Undo.AddComponent<DiagnoseSpawning>(nm.gameObject);
                Debug.Log("Attached 'DiagnoseSpawning' to NetworkManager.");
            }
            else
            {
                Debug.Log("'DiagnoseSpawning' already attached.");
            }
             UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
        else
        {
            Debug.LogError("No NetworkManager found!");
        }
    }
}
