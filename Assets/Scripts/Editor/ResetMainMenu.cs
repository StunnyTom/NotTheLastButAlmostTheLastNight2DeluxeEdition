using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using SlimUI.ModernMenu; // Namespace for the asset if available

namespace EditorTools
{
    public class ResetMainMenu : EditorWindow
    {
        [MenuItem("Tools/Recovery/RESET MAIN MENU (Nuclear)")]
        public static void NuclearReset()
        {
            // 1. CLEAR SCENE
            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.name.Contains("Camera")) continue; // Keep Camera
                if (root.name.Contains("Directional")) continue; // Keep Light
                if (root.name.Contains("NetworkManager")) continue; // Keep NetManager if there
                
                Undo.DestroyObjectImmediate(root);
            }

            // 2. SPAWN PREFAB
            string prefabPath = "Assets/SlimUI/Modern Menu 1/Prefabs/Canvas Templates/Canvas_DefaultTemplate1.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (!prefab)
            {
                Debug.LogError("SlimUI Prefab not found! Check path.");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = "MainMenu_Clean";
            Undo.RegisterCreatedObjectUndo(instance, "Spawn Menu");

            // 3. SETUP CONTROLLER
            MainMenuController mmc = instance.GetComponent<MainMenuController>();
            if (!mmc) mmc = Undo.AddComponent<MainMenuController>(instance);
            
            // 4. FIND & RENAME BUTTONS
            // We know SlimUI structure: UI Manager -> Play Menu -> Buttons
            UIMenuManager uiMgr = instance.GetComponentInChildren<UIMenuManager>(true);
            
            if (uiMgr && uiMgr.playMenu)
            {
                // Assign to MainMenuController
                mmc.hostPanel = uiMgr.playMenu; 

                Button[] buttons = uiMgr.playMenu.GetComponentsInChildren<Button>(true);
                // Expected: 0=NewGame(Host), 1=LoadGame(Join), 2=Continue...
                
                if (buttons.Length >= 2)
                {
                    // MULTI
                    SetupButton(buttons[0], "MULTI", mmc, "OnClickMulti");
                    
                    // SOLO
                    SetupButton(buttons[1], "SOLO (SOON)", mmc, "OnClickSolo");

                    // CUSTOM (Clone Solo if only 2 buttons exist, or use 3rd)
                    if (buttons.Length > 2)
                    {
                        SetupButton(buttons[2], "CUSTOM (SOON)", mmc, "OnClickCustom");
                        buttons[2].gameObject.SetActive(true);
                    }
                    else
                    {
                        GameObject customBtn = Instantiate(buttons[1].gameObject, buttons[1].transform.parent);
                        customBtn.name = "CustomButton";
                        customBtn.transform.SetSiblingIndex(2);
                        SetupButton(customBtn.GetComponent<Button>(), "CUSTOM (SOON)", mmc, "OnClickCustom");
                    }
                }
            }

            // 5. FIX TITLE
            // Try to find the title text
            TMP_Text[] allTexts = instance.GetComponentsInChildren<TMP_Text>(true);
            foreach(var t in allTexts)
            {
                if (t.name.ToLower().Contains("title") || t.text.Contains("SlimUI"))
                {
                    t.text = "NOT THE LAST NIGHT";
                    t.fontSize = 64; // Force readable size
                }
            }
            
            // 6. FIX BACKGROUND
            EditorTools.ForceMenuBackground.FixBackground(); // Reuse existing tool logic
            
            // 7. FIX SCALE
            CanvasScaler scaler = instance.GetComponent<CanvasScaler>();
            if (scaler)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }

            Debug.Log("NUCLEAR RESET COMPLETE. SCENE IS CLEAN.");
        }

        static void SetupButton(Button btn, string label, Object target, string method)
        {
            btn.name = label.Replace(" ", "") + "Btn";
            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt) txt.text = label;

            UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 0);
            
            var targetComponent = target as Component;
            if (targetComponent)
            {
                var mInfo = target.GetType().GetMethod(method);
                if (mInfo != null)
                {
                    var action = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), target, mInfo) as UnityEngine.Events.UnityAction;
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, action);
                }
            }
        }
    }
}
