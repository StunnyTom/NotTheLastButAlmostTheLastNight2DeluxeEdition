using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

namespace Antigravity.Recovery
{
    public class CheckLobbyLayout
    {
        [MenuItem("Tools/Antigravity Kit/Recovery/Diagnose/Check Lobby Layout")]
        public static void Check()
        {
            string scenePath = "Assets/Scenes/LobbyMenu.unity";
            EditorSceneManager.OpenScene(scenePath);

            // Find Controller
            var controller = Object.FindFirstObjectByType<LobbyUIController>();
            if (!controller)
            {
                Debug.LogError("LobbyUIController not found!");
                return;
            }

            if (!controller.playerListContent)
            {
                Debug.LogError("playerListContent reference is missing on controller.");
                return;
            }

            var layout = controller.playerListContent.GetComponent<VerticalLayoutGroup>();
            if (!layout)
            {
                Debug.LogWarning("Missing 'VerticalLayoutGroup' on Content object! Adding one now...");
                layout = controller.playerListContent.gameObject.AddComponent<VerticalLayoutGroup>();
                layout.childControlHeight = false;
                layout.childControlWidth = true;
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = true;
                layout.spacing = 5;
                
                var csf = controller.playerListContent.GetComponent<ContentSizeFitter>();
                if (!csf) csf = controller.playerListContent.gameObject.AddComponent<ContentSizeFitter>();
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                Debug.Log("Fixed: Added VerticalLayoutGroup to Content.");
            }
            else
            {
                Debug.Log("Layout configuration looks correct (VerticalLayoutGroup found).");
            }
        }
    }
}
