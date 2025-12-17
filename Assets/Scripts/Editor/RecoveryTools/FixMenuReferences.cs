using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace EditorTools
{
    public class FixMenuReferences : EditorWindow
    {
        [MenuItem("Tools/UI/Fix Menu References")]
        public static void FixReferences()
        {
            // 1. Find or Create Controller
            MainMenuController mmc = FindFirstObjectByType<MainMenuController>();
            if (mmc == null)
            {
                Debug.LogWarning("[FixMenu] MainMenuController not found. Searching for candidates...");
                
                // Try to find a likely host
                GameObject hostObj = GameObject.Find("MainMenu");
                if (hostObj == null) hostObj = GameObject.Find("Canvas");
                if (hostObj == null) hostObj = GameObject.Find("LobbyInterface"); // SlimUI name often
                
                if (hostObj != null)
                {
                    Debug.Log($"[FixMenu] Found '{hostObj.name}', adding MainMenuController...");
                    mmc = Undo.AddComponent<MainMenuController>(hostObj);
                }
                else
                {
                    Debug.Log("[FixMenu] No candidate found. Creating new 'MainMenuManager' object.");
                    hostObj = new GameObject("MainMenuManager");
                    Undo.RegisterCreatedObjectUndo(hostObj, "Create MainMenuManager");
                    mmc = Undo.AddComponent<MainMenuController>(hostObj);
                }
            }
            Undo.RecordObject(mmc, "Fix References");

            // 2. Panels (Heuristic Search)
            GuiFindAndAssign(ref mmc.menuPanel, "MenuPanel", "Menu", "TitleMenu", "Main Menu");
            GuiFindAndAssign(ref mmc.settingsPanel, "SettingsPanel", "Settings", "Options", "OptionsMenu");
            GuiFindAndAssign(ref mmc.hostPanel, "HostPanel", "CreateSession", "Host", "PlayPanel"); // SlimUI structure?
            GuiFindAndAssign(ref mmc.joinPanel, "JoinPanel", "JoinSession", "Join");
            GuiFindAndAssign(ref mmc.titlePanel, "TitlePanel", "Title", "StartScreen", "PressAnyKey");

            Debug.Log("[FixMenu] Panels Wired.");
            EditorUtility.SetDirty(mmc);

            // 3. Ensure LobbyUIController exists (for Relay logic)
            LobbyUIController lobbyUI = mmc.GetComponent<LobbyUIController>();
            if (lobbyUI == null)
            {
                lobbyUI = Undo.AddComponent<LobbyUIController>(mmc.gameObject);
            }
            Undo.RecordObject(lobbyUI, "Fix LobbyUI Refs");
            if (mmc.menuPanel != null) lobbyUI.lobbyPanel = mmc.menuPanel;
            if (mmc.joinPanel != null) lobbyUI.joinSection = mmc.joinPanel;

            // 4. Update Buttons - Logic for MULTI, SOLO, CUSTOM
            Button[] buttons = mmc.GetComponentsInChildren<Button>(true);
            // Fallback scope: search scene if not attached to controller hierarchy
            if (buttons.Length == 0) buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

            Button btnMulti = null;
            Button btnSolo = null;
            Button btnCustom = null;

            foreach (var btn in buttons)
            {
                string txt = btn.GetComponentInChildren<TMP_Text>()?.text.ToLower() ?? "";
                string n = btn.name.ToLower();

                // MULTI (Old Host)
                if (txt.Contains("host") || n.Contains("host") || txt.Contains("multi"))
                {
                    btnMulti = btn;
                }
                // SOLO (Old Join)
                else if (txt.Contains("join") || n.Contains("join") || txt.Contains("solo"))
                {
                    btnSolo = btn;
                }
                // CUSTOM (Any remaining main button?)
                else if ((txt.Contains("custom") || n.Contains("custom")))
                {
                    btnCustom = btn;
                }
            }

            // Apply Actions
            if (btnMulti) UpdateButton(btnMulti, "MULTI", mmc, "OnClickMulti"); // Loads Lobby
            if (btnSolo) UpdateButton(btnSolo, "SOLO (SOON)", mmc, "OnClickSolo");
            
            // Create Custom if missing but Solo exists
            if (btnCustom)
            {
                UpdateButton(btnCustom, "CUSTOM (SOON)", mmc, "OnClickCustom");
            }
            else if (btnSolo != null)
            {
                // Clone Solo to make Custom
                GameObject clone = Instantiate(btnSolo.gameObject, btnSolo.transform.parent);
                clone.name = "CustomButton";
                Undo.RegisterCreatedObjectUndo(clone, "Create Custom Button");
                clone.transform.SetSiblingIndex(btnSolo.transform.GetSiblingIndex() + 1);
                UpdateButton(clone.GetComponent<Button>(), "CUSTOM (SOON)", mmc, "OnClickCustom");
            }

            // 5. FIX VISUALS (Title & Buttons specific override)
            TMP_Text[] allTexts = mmc.GetComponentsInChildren<TMP_Text>(true);
            foreach(var t in allTexts)
            {
                // Fix Title
                if (t.name.ToLower().Contains("title") || t.fontSize > 40)
                {
                    if (t.transform.parent.name.Contains("Panel") == false) // Avoid modifying button text by mistake if they are large
                    {
                         t.text = "NOT THE LAST NIGHT\n<size=60%>Deluxe Edition</size>";
                         t.textWrappingMode = TextWrappingModes.NoWrap;
                         if (t.fontSize > 100) t.fontSize = 80; // Reasonable max
                         Debug.Log("[FixMenu] Fixed Title Text");
                    }
                }
            }

            // Force Button Labels again (in case Reset failed)
            if (btnMulti) { UpdateButton(btnMulti, "MULTI", mmc, "OnClickMulti"); }
            if (btnSolo) { UpdateButton(btnSolo, "SOLO (SOON)", mmc, "OnClickSolo"); }
            if (btnCustom) { UpdateButton(btnCustom, "CUSTOM (SOON)", mmc, "OnClickCustom"); }

            EditorUtility.SetDirty(mmc);
            Debug.Log($"[FixMenu] COMPLETE. Visuals Polished.");
        }


        static void GuiFindAndAssign(ref GameObject slot, params string[] names)
        {
            if (slot != null) return; 
            foreach (var name in names)
            {
                // Better Finder: Include inactive
                GameObject found = GameObject.Find(name); 
                if (found == null)
                {
                    Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>(); // Finds prefabs too, careful
                    foreach(var c in canvases)
                    {
                        if (EditorUtility.IsPersistent(c.gameObject)) continue; // Skip assets
                        foreach(Transform t in c.GetComponentsInChildren<Transform>(true))
                        {
                            if (t.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                            {
                                found = t.gameObject;
                                break;
                            }
                        }
                        if (found != null) break;
                    }
                }
                if (found != null) { slot = found; return; }
            }
        }

        static void UpdateButton(Button btn, string newLabel, Object target, string methodName)
        {
            Undo.RecordObject(btn.gameObject, "Update Button Text");
            btn.name = newLabel.Replace(" ", "").Replace("(", "").Replace(")", "") + "Button";
            
            TMP_Text t = btn.GetComponentInChildren<TMP_Text>();
            if (t) t.text = newLabel;

            // Safely remove existing listeners
            while (btn.onClick.GetPersistentEventCount() > 0)
            {
                UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 0);
            } 
            
            var targetComponent = target as Component;
            if (targetComponent)
            {
                var method = target.GetType().GetMethod(methodName);
                if (method != null)
                {
                    var action = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), target, method) as UnityEngine.Events.UnityAction;
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, action);
                }
                else
                {
                    Debug.LogWarning($"[FixMenu] Method '{methodName}' not found on {target.name}");
                }
            }
        }
    }
}
