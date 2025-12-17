using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using SlimUI.ModernMenu;

public class SetupHostJoinButtons : EditorWindow
{
    [MenuItem("Tools/Menu Setup (Essential)/7. Setup Game Modes")]
    public static void SetupButtons()
    {
        Debug.Log("--- Setup Game Modes ---");

        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null || manager.playMenu == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager or playMenu not found!", "OK");
            return;
        }

        GameObject playMenu = manager.playMenu;
        Button[] buttons = playMenu.GetComponentsInChildren<Button>(true);
        
        // Ensure we have at least 3 buttons for a nice list
        Undo.RecordObject(playMenu, "Setup Game Modes");

        if (buttons.Length < 1) { Debug.LogError("No buttons!"); return; }

        // BUTTON 1: LOBBY
        SetupButton(buttons[0], "MULTIPLAYER LOBBY", "Btn_Lobby", true);
        WireToLobby(buttons[0]);

        // BUTTON 2:DUMMY 1
        Button btn2 = GetOrCreateButton(playMenu, buttons, 1);
        SetupButton(btn2, "SOLO CAMPAIGN (Soon)", "Btn_DummySolo", false);

        // BUTTON 3: DUMMY 2
        Button btn3 = GetOrCreateButton(playMenu, buttons, 2);
        SetupButton(btn3, "CUSTOM GAME (Soon)", "Btn_DummyCustom", false);

        // Hide others if any
        buttons = playMenu.GetComponentsInChildren<Button>(true); // Refresh
        for (int i = 3; i < buttons.Length; i++) buttons[i].gameObject.SetActive(false);

        Debug.Log("Game Modes configured!");
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        
        EditorUtility.DisplayDialog("Success", "Game Modes Setup!\n\n1. Lobby (Active)\n2. Solo (Inactive)\n3. Custom (Inactive)", "OK");
    }

    private static Button GetOrCreateButton(GameObject parent, Button[] existing, int index)
    {
        if (index < existing.Length) 
        {
            existing[index].gameObject.SetActive(true);
            return existing[index];
        }
        
        // Clone the first one to keep style
        GameObject newObj = Instantiate(existing[0].gameObject, parent.transform);
        // Move to bottom?
        newObj.transform.SetSiblingIndex(index);
        Undo.RegisterCreatedObjectUndo(newObj, "Create Dummy Button");
        return newObj.GetComponent<Button>();
    }

    private static void WireToLobby(Button btn)
    {
        MainMenuConnector connector = btn.GetComponent<MainMenuConnector>();
        if (connector == null) connector = btn.gameObject.AddComponent<MainMenuConnector>();
        
        int count = btn.onClick.GetPersistentEventCount();
        for (int i = count - 1; i >= 0; i--) UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, i);

        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, connector.LoadLobby);
    }

    private static void SetupButton(Button btn, string text, string objName, bool interactable)
    {
        Undo.RecordObject(btn.gameObject, "Setup Button");
        btn.gameObject.name = objName;
        btn.interactable = interactable;
        
        TMP_Text tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp) { Undo.RecordObject(tmp, "Set Text"); tmp.text = text; }
        else {
            Text legacy = btn.GetComponentInChildren<Text>();
            if (legacy) { Undo.RecordObject(legacy, "Set Text"); legacy.text = text; }
        }
    }

    private static void SetupButton(Button btn, string text, string objName)
    {
        Undo.RecordObject(btn.gameObject, "Setup Button");
        btn.gameObject.name = objName;
        btn.gameObject.SetActive(true);

        // Try TMP first
        TMP_Text tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            Undo.RecordObject(tmp, "Set Button Text");
            tmp.text = text;
        }
        else
        {
            // Fallback to legacy Text
            Text legacyText = btn.GetComponentInChildren<Text>();
            if (legacyText != null)
            {
                Undo.RecordObject(legacyText, "Set Button Text");
                legacyText.text = text;
            }
        }
    }
}
