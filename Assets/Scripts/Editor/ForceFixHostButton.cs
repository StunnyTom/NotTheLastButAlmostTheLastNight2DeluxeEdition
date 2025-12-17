using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using SlimUI.ModernMenu;

public class ForceFixHostButton : Editor
{
    [MenuItem("Tools/Recovery/FORCE FIX Host Button")]
    public static void ForceFix()
    {
        Debug.Log("--- STARTING FORCE FIX ---");
        
        // 1. Find Button
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null || manager.playMenu == null)
        {
            Debug.LogError("Cannot find UIMenuManager or PlayMenu.");
            return;
        }

        Button hostBtn = null;
        var buttons = manager.playMenu.GetComponentsInChildren<Button>(true);
        foreach(var b in buttons)
        {
            if (b.name == "Btn_Host") 
            {
                hostBtn = b;
                break;
            }
        }

        if (hostBtn == null)
        {
            Debug.LogError("Could not find 'Btn_Host'. Run the Setup buttons tool first.");
            return;
        }

        // 2. Connector
        var connector = hostBtn.GetComponent<MainMenuConnector>();
        if (connector == null) connector = hostBtn.gameObject.AddComponent<MainMenuConnector>();

        // 3. Clear & Wire
        int count = hostBtn.onClick.GetPersistentEventCount();
        for (int i = count - 1; i >= 0; i--)
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(hostBtn.onClick, i);

        UnityEditor.Events.UnityEventTools.AddPersistentListener(hostBtn.onClick, connector.LoadLobby);

        // 4. Force Save
        EditorUtility.SetDirty(hostBtn);
        EditorSceneManager.MarkSceneDirty(hostBtn.gameObject.scene);
        EditorSceneManager.SaveScene(hostBtn.gameObject.scene);

        Debug.Log("SUCCESS: Host Button Wired and Scene SAVED.");
        EditorUtility.DisplayDialog("Fixed", "Button Wired and Scene Saved!\nTry Play Mode now.", "OK");
    }
}
