using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SlimUI.ModernMenu;
using TMPro;

public class MenuSetupSlimUI : EditorWindow
{
    [MenuItem("Tools/Setup SlimUI Menu")]
    public static void SetupMenu()
    {
        // 1. Load the Prefab
        string prefabPath = "Assets/SlimUI/Modern Menu 1/Prefabs/Canvas Templates/Canvas_DefaultTemplate1.prefab";
        GameObject menuPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (menuPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find SlimUI prefab at:\n" + prefabPath, "OK");
            return;
        }

        // 2. Instantiate
        GameObject menuInstance = (GameObject)PrefabUtility.InstantiatePrefab(menuPrefab);
        menuInstance.name = "MainMenu_SlimUI";
        Undo.RegisterCreatedObjectUndo(menuInstance, "Create SlimUI Menu");

        // 3. Setup Background
        SetupBackground(menuInstance);

        // 4. Configure Controller & Buttons
        SetupController(menuInstance);

        Selection.activeGameObject = menuInstance;
        EditorUtility.DisplayDialog("Success", "SlimUI Menu has been created!\n\nDon't forget to disable/delete the old menu.", "OK");
    }

    private static void SetupBackground(GameObject root)
    {
        string bgPath = "Assets/Ressources/bg_menu_principal.png";
        
        if (!System.IO.File.Exists(bgPath))
        {
            string altPath = "Assets/Ressources/g_menu_principal.bg"; 
            if (System.IO.File.Exists(altPath)) bgPath = altPath;
        }

        // 1. Force High Quality Sprite Settings
        TextureImporter importer = AssetImporter.GetAtPath(bgPath) as TextureImporter;
        if (importer != null)
        {
            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; changed = true; }
            if (importer.maxTextureSize < 4096) { importer.maxTextureSize = 4096; changed = true; }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; changed = true; }
            
            if (changed) importer.SaveAndReimport();
        }

        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath);

        // Fallback
        if (bgSprite == null)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(bgPath);
            if (tex != null)
            {
                bgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }

        if (bgSprite == null)
        {
            Debug.LogError("CRITICAL: Could not load background at: " + bgPath);
            return;
        }

        // 2. Clean up existing backgrounds that might block view
        // Disable any Image component at the root or immediate children that looks like a background panel
        // (except our CustomBackground)
        foreach (Transform child in root.transform)
        {
            if (child.name == "CustomBackground") continue;
            
            // If it's a panel covering the screen, it might be blocking
            Image img = child.GetComponent<Image>();
            if (img != null && !child.name.Contains("Button") && !child.name.Contains("Text"))
            {
                // Heuristic: if it's big and opaque, disable it or make it transparent
                if (img.color.a > 0.9f)
                {
                    // Check if it's the main canvas container, we don't want to disable that if it holds buttons
                    // But usually the main container is invisible.
                    // Let's just try to find the specific "Background" object SlimUI uses.
                    if (child.name.ToLower().Contains("background") || child.name.ToLower().Contains("bg"))
                    {
                        child.gameObject.SetActive(false);
                        Debug.Log("Disabled potential blocking background: " + child.name);
                    }
                }
            }
        }

        // 3. Create/Update Background
        // Parent to Root is safest for Overlay
        Transform bgParent = root.transform; 
        Transform customBgTrans = bgParent.Find("CustomBackground");
        GameObject customBg;

        if (customBgTrans != null)
        {
            customBg = customBgTrans.gameObject;
        }
        else
        {
            customBg = new GameObject("CustomBackground");
            customBg.transform.SetParent(bgParent, false);
        }

        customBg.transform.SetAsFirstSibling(); // Render BEHIND everything
        customBg.SetActive(true); // Ensure enabled

        Image newImg = customBg.GetComponent<Image>();
        if (newImg == null) newImg = customBg.AddComponent<Image>();
        
        newImg.sprite = bgSprite;
        newImg.type = Image.Type.Simple;
        newImg.preserveAspect = false;
        newImg.color = Color.white;
        
        RectTransform rt = customBg.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero; // Full stretch
        rt.anchoredPosition = Vector2.zero;
        
        Debug.Log("Success: Background set on " + customBg.name);
    }

    private static void SetupController(GameObject root)
    {
        // 1. Fix Canvas Scaling (Force Overlay)
        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                // INCREASED RESOLUTION TO 4K (3840x2160) to make UI elements smaller
                scaler.referenceResolution = new Vector2(3840, 2160); 
                scaler.matchWidthOrHeight = 0.5f;
            }
            Debug.Log("Fixed Canvas Scaling: Screen Space Overlay, 4K Reference Resolution");
        }

        // 2. Configure Buttons
        UIMenuManager manager = root.GetComponentInChildren<UIMenuManager>(true);
        if (manager == null)
        {
            Debug.LogError("UIMenuManager component missing on prefab hierarchy!");
            return;
        }

        // Configure Play Menu (Host / Join)
        if (manager.playMenu != null)
        {
            // Find buttons in the Play Menu
            Button[] playButtons = manager.playMenu.GetComponentsInChildren<Button>(true);
            
            // We expect at least 2 buttons (New Game, Load Game, etc.)
            if (playButtons.Length >= 2)
            {
                // Button 1: HOST
                Button hostBtn = playButtons[0];
                SetupButton(hostBtn, "HOST", () => {
                    // We can't use lambdas for persistent listeners in Editor, 
                    // so we wire it to LoadScene with the string arg.
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(hostBtn.onClick, 0);
                    UnityEditor.Events.UnityEventTools.AddStringPersistentListener(
                        hostBtn.onClick, 
                        new UnityEngine.Events.UnityAction<string>(manager.LoadScene), 
                        "The_Viking_Village"
                    );
                });
                
                // Button 2: JOIN
                Button joinBtn = playButtons[1];
                SetupButton(joinBtn, "JOIN", null); // Placeholder for now
                
                // Hide others if any
                for (int i = 2; i < playButtons.Length; i++)
                {
                    playButtons[i].gameObject.SetActive(false);
                }
                
                Debug.Log("Configured Play Menu: HOST and JOIN buttons set.");
            }
            else
            {
                Debug.LogWarning("Not enough buttons in Play Menu to configure Host/Join automatically.");
            }
        }
    }

    private static void SetupButton(Button btn, string labelText, System.Action onClickAction)
    {
        btn.gameObject.name = labelText + "Button";
        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.text = labelText;
        
        if (onClickAction != null)
        {
            // Note: This helper is for immediate editor actions, but for runtime clicks 
            // we need PersistentListeners as done above. 
            // This block is just for visual setup if we were adding non-persistent logic, 
            // but here we handled the click logic in the caller for the Host button.
        }
    }
}
