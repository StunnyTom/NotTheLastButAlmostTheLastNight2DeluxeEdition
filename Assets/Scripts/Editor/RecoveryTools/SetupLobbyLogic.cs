using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class SetupLobbyLogic : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/Setup Lobby Logic (Netcode)")]
    public static void Setup()
    {
        Debug.Log("--- Setup Lobby Logic ---");

        // 1. Ensure RelayManager
        var relay = Object.FindFirstObjectByType<RelayManager>();
        if (relay == null)
        {
            GameObject relayObj = new GameObject("RelayManager");
            relay = relayObj.AddComponent<RelayManager>();
            Undo.RegisterCreatedObjectUndo(relayObj, "Create RelayManager");
            Debug.Log("Created RelayManager");
        }

        // 2. Find/Setup UI Controller
        var controller = Object.FindFirstObjectByType<LobbyUIController>();
        if (controller == null)
        {
            // PRO TRY: Find "Lobby" or "Canvas" specifically
            GameObject targetObj = GameObject.Find("Lobby");
            if (targetObj == null) targetObj = GameObject.Find("Canvas");
            if (targetObj == null) targetObj = Object.FindFirstObjectByType<Canvas>()?.gameObject;
            
            if (targetObj != null)
            {
                controller = targetObj.AddComponent<LobbyUIController>();
                Undo.RegisterCreatedObjectUndo(controller, "Add Controller");
                Debug.Log($"Added LobbyUIController to '{targetObj.name}'");
            }
            else
            {
                // Fallback: Create dedicated object
                targetObj = new GameObject("LobbyUI_Controller");
                controller = targetObj.AddComponent<LobbyUIController>();
                Undo.RegisterCreatedObjectUndo(targetObj, "Create Controller Obj");
                Debug.Log("Created new object 'LobbyUI_Controller' for script.");
            }
        }
        else
        {
            Debug.Log($"Found existing controller on '{controller.gameObject.name}'");
        }

        // 3. Find Buttons & Inputs (Heuristic Search)
        // Names based on Inspection: "Button" (Create), "Button" (Join), "Copy Button", "Leave Session"
        // Parents: "Create Session", "Join Session By Code", "Show Session Code", "Row 2"
        
        Button[] allButtons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        TMP_InputField[] allInputs = Object.FindObjectsByType<TMP_InputField>(FindObjectsSortMode.None);
        
        foreach(var btn in allButtons)
        {
            string pName = btn.transform.parent.name;
            
            if (pName.Contains("Create Session")) controller.createSessionBtn = btn;
            if (pName.Contains("Join Session")) controller.joinSessionBtn = btn;
            if (pName.Contains("Show Session")) controller.copyCodeBtn = btn;
            if (btn.name.Contains("Leave")) controller.leaveSessionBtn = btn;
        }

        // Find Input Field (likely inside "Join Session By Code")
        foreach(var inp in allInputs)
        {
             if (inp.transform.parent.name.Contains("Join Session") || inp.name.Contains("Code"))
             {
                 controller.joinCodeInput = inp;
                 break;
             }
        }
        
        // Find Code Display Text (Specific match for 'Session Code Text')
        var allTmpTexts = Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        foreach(var txt in allTmpTexts)
        {
            // Look for "Session Code Text" OR a child of "Show Session Code"
            if (txt.name.Contains("Session Code") || txt.transform.parent.name.Contains("Show Session"))
            {
                // Verify it's not the button label
                if (!txt.name.Contains("Text (TMP)")) 
                {
                    controller.joinCodeDisplay = txt;
                    Debug.Log($"Found Display Text: {txt.name}");
                    break;
                }
            }
        }
        
        // 4. Force Save
        EditorUtility.SetDirty(controller);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(controller.gameObject.scene);

        Debug.Log("Lobby Logic Setup Complete!");
        EditorUtility.DisplayDialog("Lobby Setup", 
            $"Logic installed on object: '{controller.gameObject.name}'\n\n" +
            "Please check the Inspector for 'LobbyUIController' references.\n" +
            "If buttons are missing, assign them manually.", 
            "OK");
    }
}
