using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class DiagnoseLobbyLayout : Editor
{
    [MenuItem("Tools/Debugging/Diagnose Lobby Scene")]
    public static void RunDiagnosis()
    {
        // Open Scene
        string scenePath = "Assets/Scenes/LobbyMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);
        
        Debug.Log($"--- DIAGNOSING SCENE: {scene.name} ---");

        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            Debug.Log($"CANVAS: {c.name} | Mode: {c.renderMode} | SortOrder: {c.sortingOrder}");
            
            var scaler = c.GetComponent<CanvasScaler>();
            if (scaler)
            {
                Debug.Log($"   -> SCALER: {scaler.uiScaleMode} | RefRes: {scaler.referenceResolution} | Match: {scaler.matchWidthOrHeight}");
            }
            else
            {
                Debug.LogError("   -> SCALER MISSING!");
            }

            // Check first child (Access Panel?)
            if (c.transform.childCount > 0)
            {
                var child = c.transform.GetChild(0) as RectTransform;
                if (child)
                {
                     Debug.Log($"   -> FIRST CHILD ({child.name}): AnchorMin {child.anchorMin} | AnchorMax {child.anchorMax} | Pivot {child.pivot}");
                     Debug.Log($"      Pos: {child.anchoredPosition} | Size: {child.sizeDelta}");
                }
            }
        }
    }
}
