using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace EditorTools
{
    public class FixVisualsAndWiring : EditorWindow
    {
        [MenuItem("Tools/UI/Fix Visuals And Wiring (Final)")]
        public static void Fix()
        {
            MainMenuController mmc = FindFirstObjectByType<MainMenuController>();
            if (!mmc)
            {
                Debug.LogError("No MainMenuController found.");
                return;
            }
            Undo.RecordObject(mmc, "Fix Menu");

            // 1. SMART FIND PANELS
            // Reset to ensure we don't keep bad refs
            mmc.menuPanel = null;
            mmc.settingsPanel = null;
            mmc.hostPanel = null;

            var allPanels = mmc.GetComponentsInChildren<RectTransform>(true);
            foreach(var r in allPanels)
            {
                // Heuristic: Settings Panel has Sliders
                if (r.GetComponentInChildren<Slider>(true) != null)
                {
                    if (mmc.settingsPanel == null)
                    {
                        mmc.settingsPanel = r.gameObject;
                        r.gameObject.name = "SettingsPanel";
                        Debug.Log("Found Settings Panel (has slider)");
                        continue;
                    }
                }

                // Heuristic: Root Menu has "Exit" or "Quit" button
                var btnTexts = r.GetComponentsInChildren<TMP_Text>(true);
                foreach(var t in btnTexts)
                {
                    if (t.text.ToLower().Contains("exit") || t.text.ToLower().Contains("quit"))
                    {
                        if (mmc.menuPanel == null && r.transform.parent == mmc.transform) // Usually direct child or close
                        {
                            // Verify it's not the popup
                            mmc.menuPanel = r.gameObject;
                            r.gameObject.name = "MenuPanel_Root";
                            Debug.Log("Found Root Menu Panel (has Exit button)");
                            break;
                        }
                    }
                }
            }

            // Find Play/Host Panel (The one that isn't root or settings)
            foreach(var r in allPanels)
            {
                if (r.gameObject == mmc.menuPanel) continue;
                if (r.gameObject == mmc.settingsPanel) continue;
                if (r.GetComponent<Button>() == null) continue; // Must have buttons
                if (r.name.Contains("Title")) continue; 

                // If it has >= 2 buttons and isn't the others, it's likely the Play Submenu
                var btns = r.GetComponentsInChildren<Button>(true);
                if (btns.Length >= 2)
                {
                    if (mmc.hostPanel == null)
                    {
                        mmc.hostPanel = r.gameObject;
                        r.gameObject.name = "HostPanel_PlaySelection";
                        Debug.Log($"Found Play Selection Panel: {r.name}");
                        break;
                    }
                }
            }
            
            // 2. WIRE & LABEL ROOT BUTTONS (JOUER, OPTIONS, QUITTER)
            if (mmc.menuPanel)
            {
                var rootBtns = mmc.menuPanel.GetComponentsInChildren<Button>(true);
                // We expect 3. If 2, create one.
                if (rootBtns.Length == 2)
                {
                    // Create Options button
                    GameObject opt = Instantiate(rootBtns[0].gameObject, rootBtns[0].transform.parent);
                    opt.transform.SetSiblingIndex(1); // Middle
                    rootBtns = mmc.menuPanel.GetComponentsInChildren<Button>(true); // Refresh
                }

                if (rootBtns.Length >= 3)
                {
                    SetupButton(rootBtns[0], "JOUER", mmc, "OnHostClicked"); // Shows HostPanel
                    SetupButton(rootBtns[1], "OPTIONS", mmc, "ShowSettings");
                    SetupButton(rootBtns[2], "QUITTER", mmc, "OnQuitClicked");
                }
                else if (rootBtns.Length >= 1)
                {
                    // Fallback
                   SetupButton(rootBtns[0], "JOUER", mmc, "OnHostClicked");
                }
            }

            // 3. WIRE & LABEL PLAY SUB-MENU BUTTONS (MULTI, SOLO, CUSTOM)
            if (mmc.hostPanel)
            {
                 var subBtns = mmc.hostPanel.GetComponentsInChildren<Button>(true);
                 // Need 3
                 if (subBtns.Length < 3 && subBtns.Length > 0)
                 {
                     GameObject clone = Instantiate(subBtns[0].gameObject, subBtns[0].transform.parent);
                     subBtns = mmc.hostPanel.GetComponentsInChildren<Button>(true);
                 }
                 if (subBtns.Length < 3 && subBtns.Length > 0)
                 {
                     GameObject clone = Instantiate(subBtns[0].gameObject, subBtns[0].transform.parent);
                     subBtns = mmc.hostPanel.GetComponentsInChildren<Button>(true);
                 }

                 if (subBtns.Length >= 3)
                 {
                     SetupButton(subBtns[0], "MULTI", mmc, "OnClickMulti");
                     SetupButton(subBtns[1], "SOLO (SOON)", mmc, "OnClickSolo");
                     SetupButton(subBtns[2], "CUSTOM (SOON)", mmc, "OnClickCustom");
                 }
            }

            EditorUtility.SetDirty(mmc);
            Debug.Log("FINAL WIRING COMPLETE. Please Save.");
        }

        static void SetupButton(Button btn, string label, Object target, string method)
        {
            btn.name = "Btn_" + label.Replace(" ", "");
            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt) txt.text = label;

            while(btn.onClick.GetPersistentEventCount() > 0)
                 UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 0);

            var targetComponent = target as Component;
            var mInfo = target.GetType().GetMethod(method);
            if (mInfo != null)
            {
                var action = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), target, mInfo) as UnityEngine.Events.UnityAction;
                UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, action);
            }
        }
    }
}
