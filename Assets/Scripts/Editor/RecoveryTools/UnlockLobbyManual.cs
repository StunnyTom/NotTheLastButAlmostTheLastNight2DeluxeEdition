using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class UnlockLobbyManual : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/11. UNLOCK Lobby (Manual Edit Mode)")]
    public static void Unlock()
    {
        // 1. Open Scene
        string scenePath = "Assets/Scenes/LobbyMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);
        
        Debug.Log("--- UNLOCKING LOBBY FOR MANUAL EDIT ---");

        // 2. Unlock Canvas (World Space)
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            Undo.RecordObject(c.gameObject, "Unlock Canvas");
            
            // The Key to "Unlocking": World Space
            c.renderMode = RenderMode.WorldSpace;
            
            // Reset to somewhere visible
            var rect = c.GetComponent<RectTransform>();
            rect.anchoredPosition3D = new Vector3(0, 0, 100); // 100 units away
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one * 0.005f; // Reasonable scale for UI in World
            
            // Remove Scaler? Or keep it? keeping it might fight the user.
            // Let's set it to Constant Pixel Size so it doesn't fight.
            var scaler = c.GetComponent<CanvasScaler>();
            if (scaler)
            {
                 scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                 scaler.scaleFactor = 1;
            }

            Debug.Log($"[Unlock] Canvas '{c.name}' set to WorldSpace. You can now resize it manually.");
        }
        
        // 3. Save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Lobby Unlocked", 
            "The Lobby Canvas is now in 'World Space'.\n\n" +
            "You can now change Pos X, Y, Width, Height manually.\n" +
            "Note: It is located at Z=100.", "OK");
    }
}
