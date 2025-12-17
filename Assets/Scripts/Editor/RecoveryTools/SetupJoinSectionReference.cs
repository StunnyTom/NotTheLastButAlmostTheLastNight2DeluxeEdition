using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class SetupJoinSectionReference : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/12. Setup Join Section Reference")]
    public static void Setup()
    {
        var controller = Object.FindFirstObjectByType<LobbyUIController>();
        if (controller == null)
        {
            Debug.LogError("LobbyUIController not found!");
            return;
        }

        if (controller.joinSection != null)
        {
            Debug.Log("Join Section already assigned.");
        }
        else
        {
            // Try to find a logical parent
            // The user said "toute la ligne". Often elements are scattered.
            // Best strategy: creating a new Parent and moving them inside if not already grouped.
            // Or identifying "Join Session By Code" (InputField) + Button + Label.

            var joinBtn = controller.joinSessionBtn ? controller.joinSessionBtn.gameObject : GameObject.Find("Join Session By Code");
            var joinInput = controller.joinCodeInput ? controller.joinCodeInput.gameObject : GameObject.Find("Join Session By Code"); // Input field often named same as button in messy setups? Or "Join Code Input"?

            // "Client Enter Join Code" label?
            // This is getting risky to guess. 
            // Better strategy: Find "Client Group" or "Join Group".
            
            var joinGroup = GameObject.Find("Client Group"); 
            if (joinGroup == null) joinGroup = GameObject.Find("Join Group");

            if (joinGroup != null)
            {
                Undo.RecordObject(controller, "Assign Join Section");
                controller.joinSection = joinGroup;
                Debug.Log($"Assigned '{joinGroup.name}' as Join Section.");
            }
            else
            {
                // If we can't find a group, let's warn the user or create one wrapping the button?
                // Given the screenshot, there is "[Client] Enter Join Code: ______"
                // It's likely separate objects.
                
                Debug.LogWarning("Could not auto-detect a 'Join Group'. Please assign 'JoinSection' manually in Inspector or group your objects.");
                
                // Fallback: If we have the button, maybe its parent is the group?
                if (controller.joinSessionBtn != null)
                {
                    Transform parent = controller.joinSessionBtn.transform.parent;
                    if (parent != null && parent.name.Contains("Canvas") == false) // Don't hide the whole canvas
                    {
                         // Potential candidate
                    }
                }
            }
        }
    }
}
