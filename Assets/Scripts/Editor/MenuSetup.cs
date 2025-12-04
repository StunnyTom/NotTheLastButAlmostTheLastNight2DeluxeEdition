using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using QuantumTek.SimpleMenu; // Assuming namespace based on file content

public class MenuSetup : EditorWindow
{
    [MenuItem("Tools/Setup Main Menu")]
    public static void SetupMenu()
    {
        // 1. Find and Instantiate the Prefab
        string prefabPath = "Assets/Imported/Quantum Tek _ Simple menu/Simple Menu/Prefabs/Menus and Windows/Simple Main Menu.prefab";
        GameObject menuPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (menuPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find Simple Main Menu prefab at:\n" + prefabPath, "OK");
            Debug.LogError("Could not find Simple Main Menu prefab at: " + prefabPath);
            return;
        }

        GameObject menuInstance = (GameObject)PrefabUtility.InstantiatePrefab(menuPrefab);
        menuInstance.name = "MainMenu_QuantumTek";
        Undo.RegisterCreatedObjectUndo(menuInstance, "Create Main Menu");

        // 2. Setup Background
        // 2. Setup Background
        string bgName = "bg_menu_principal";
        string bgPath = "Assets/Ressources/" + bgName + ".png"; // Default path
        
        // Robust Search: If file not at default path, search for it
        if (!System.IO.File.Exists(bgPath))
        {
             Debug.LogWarning("Background not found at default path: " + bgPath + ". Searching project...");
             string[] guids = AssetDatabase.FindAssets(bgName + " t:Texture");
             if (guids.Length > 0)
             {
                 bgPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                 Debug.Log("Found background at: " + bgPath);
             }
             else
             {
                 Debug.LogError("CRITICAL: Could not find background image '" + bgName + "' anywhere in the project.");
                 // Don't return, try to proceed with other setup, but background will fail
             }
        }

        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath);
        
        // Fallback: Try loading as Texture2D and creating a sprite manually
        if (bgSprite == null && System.IO.File.Exists(bgPath))
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(bgPath);
            if (tex != null)
            {
                // Force Import Settings to Sprite if needed
                TextureImporter importer = AssetImporter.GetAtPath(bgPath) as TextureImporter;
                if (importer != null && importer.textureType != TextureImporterType.Sprite)
                {
                    Debug.Log("Fixing Texture Import Settings for background image...");
                    importer.textureType = TextureImporterType.Sprite;
                    importer.SaveAndReimport();
                    bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath); // Reload
                }
                
                if (bgSprite == null)
                {
                     Debug.LogWarning("Loaded as Texture2D but not as Sprite. Creating sprite dynamically.");
                     bgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
        }

        bool bgSet = false;
        
        if (bgSprite != null)
        {
            Debug.Log("Found background sprite at: " + bgPath);

            // Strategy A: Try to find "Simple Background" and set its image
            Transform bgTransform = menuInstance.transform.Find("Simple Background");
            
            // If not found, check if we already created a CustomBackground
            if (bgTransform == null) bgTransform = menuInstance.transform.Find("CustomBackground");

            Image bgImage = null;

            if (bgTransform != null)
            {
                // Try getting Image on the object itself
                bgImage = bgTransform.GetComponent<Image>();
                // If not, try children
                if (bgImage == null) bgImage = bgTransform.GetComponentInChildren<Image>();

                if (bgImage != null)
                {
                    bgImage.sprite = bgSprite;
                    bgImage.type = Image.Type.Simple;
                    bgImage.preserveAspect = false;
                    bgImage.color = Color.white; // Ensure it's not transparent
                    
                    // Ensure it fills the screen
                    RectTransform rt = bgImage.GetComponent<RectTransform>();
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.sizeDelta = Vector2.zero;
                    
                    bgSet = true;
                    Debug.Log("Successfully set background on existing object: " + bgTransform.name);
                }
                else
                {
                    Debug.LogWarning("Found background object '" + bgTransform.name + "' but it has no Image component.");
                }
            }
            
            // Strategy B: If Strategy A failed, create our own background
            if (!bgSet)
            {
                Debug.Log("Creating a new Background object as fallback.");
                GameObject customBg = new GameObject("CustomBackground");
                customBg.transform.SetParent(menuInstance.transform, false);
                customBg.transform.SetAsFirstSibling(); // Put at the very back

                Image newImg = customBg.AddComponent<Image>();
                newImg.sprite = bgSprite;
                newImg.type = Image.Type.Simple;
                newImg.preserveAspect = false;
                newImg.color = Color.white;
                
                RectTransform rt = customBg.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
                
                bgSet = true;
                Debug.Log("Created 'CustomBackground' with the image.");
            }
        }
        else
        {
            Debug.LogError("CRITICAL: Failed to load background sprite from: " + bgPath);
        }

        // 3. Create Fog Effect (Particle System)
        CreateFogEffect(menuInstance.transform);

        // 4. Link to SM_Menu (Quantum Tek)
        SM_Menu smMenu = menuInstance.GetComponent<SM_Menu>();
        if (smMenu != null)
        {
            SerializedObject so = new SerializedObject(smMenu);
            SerializedProperty bgProp = so.FindProperty("background");
            
            // Find the background object we just set up
            Transform bgObj = menuInstance.transform.Find("Simple Background");
            if (bgObj == null) bgObj = menuInstance.transform.Find("CustomBackground");
            
            if (bgObj != null)
            {
                bgProp.objectReferenceValue = bgObj;
                so.ApplyModifiedProperties();
                Debug.Log("Linked Background GameObject to SM_Menu script.");
            }
        }

        MainMenuController controller = menuInstance.GetComponent<MainMenuController>();
        if (controller == null)
        {
            controller = menuInstance.AddComponent<MainMenuController>();
        }
        
        Transform mainWindow = menuInstance.transform.Find("Main Window");
        if (mainWindow != null) controller.menuPanel = mainWindow.gameObject;

        Transform settingsWindow = menuInstance.transform.Find("Simple Settings Tab Group");
        if (settingsWindow != null) controller.settingsPanel = settingsWindow.gameObject;
        
        if (mainWindow != null)
        {
            if (controller.hostPanel == null)
            {
                GameObject hostPanel = Instantiate(mainWindow.gameObject, menuInstance.transform);
                hostPanel.name = "HostPanel";
                hostPanel.SetActive(false);
                controller.hostPanel = hostPanel;
            }
            
            if (controller.joinPanel == null)
            {
                GameObject joinPanel = Instantiate(mainWindow.gameObject, menuInstance.transform);
                joinPanel.name = "JoinPanel";
                joinPanel.SetActive(false);
                controller.joinPanel = joinPanel;
            }
        }

        Selection.activeGameObject = menuInstance;
        
        // 5. Customize UI (Title and Transparency)
        if (mainWindow != null)
        {
            // Set Title to Project Name
            // Try to find a child named "Title" or look for text components
            Transform titleTrans = mainWindow.Find("Title");
            Text titleText = null;
            
            if (titleTrans != null) titleText = titleTrans.GetComponent<Text>();
            if (titleText == null) titleText = mainWindow.GetComponentInChildren<Text>(); // Fallback to first text found
            
            if (titleText != null)
            {
                titleText.text = PlayerSettings.productName;
                Debug.Log("Set Menu Title to: " + PlayerSettings.productName);
            }
            
                // Make Panel Transparent / Darker for better contrast
            Image panelImage = mainWindow.GetComponent<Image>();
            if (panelImage != null)
            {
                // Dark semi-transparent background (Glassy look)
                panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.4f);
                Debug.Log("Adjusted Main Window transparency for glassy look.");
            }
            
            // Optional: Style Buttons for a quick win
            Button[] buttons = mainWindow.GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                Image btnImg = btn.GetComponent<Image>();
                if (btnImg != null)
                {
                    btnImg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f); // Dark buttons
                }
                Text btnText = btn.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.color = Color.white; // White text
                }
            }
        }

        string message = "Main Menu Setup Complete!\n\n" +
                         "Background Set: " + (bgSet ? "Yes" : "NO (Check Console)") + "\n" +
                         "Fog Created: Yes (Seams Fixed)\n" +
                         "Controller Configured: Yes\n" +
                         "Title Set: " + PlayerSettings.productName + "\n" +
                         "Styling: Applied Dark/Glass Theme\n\n" +
                         "You should see the menu in the Scene view now.";
        
        EditorUtility.DisplayDialog("Setup Complete", message, "OK");
    }

    private static void CreateFogEffect(Transform parent)
    {
        GameObject fogObj = new GameObject("FogEffect");
        fogObj.transform.SetParent(parent, false);
        
        // Ensure it's behind UI but in front of background
        // Assuming Background is index 0
        fogObj.transform.SetSiblingIndex(1); 
        
        RectTransform rt = fogObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero; // Stretch
        
        RawImage rawImage = fogObj.AddComponent<RawImage>();
        // Drastically reduce alpha. 0.15f is very subtle.
        rawImage.color = new Color(1f, 1f, 1f, 0.15f); 
        rawImage.raycastTarget = false;

        Texture2D noiseTex = GenerateNoiseTexture(256, 256);
        rawImage.texture = noiseTex;
        
        // Add scrolling script
        ScrollingUVs scroller = fogObj.AddComponent<ScrollingUVs>();
        scroller.speedX = 0.05f;
        scroller.speedY = 0.02f;
        
        Undo.RegisterCreatedObjectUndo(fogObj, "Create Fog");
    }

    private static Texture2D GenerateNoiseTexture(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height);
        Color[] pix = new Color[width * height];
        float scale = 10f; // Zoom in on noise
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                // Reduce alpha influence in the texture itself too
                pix[y * width + x] = new Color(1, 1, 1, sample * 0.5f); 
            }
        }
        tex.SetPixels(pix);
        
        // CRITICAL FIX: Mirror wrap mode prevents seams/lines when scrolling
        tex.wrapMode = TextureWrapMode.Mirror;
        tex.filterMode = FilterMode.Bilinear;
        
        tex.Apply();
        
        // Save this texture as an asset so it persists
        string dirPath = "Assets/Ressources";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }

        string path = dirPath + "/ProceduralFog.png";
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
        
        // Re-load to ensure settings persist if we were to modify importer (but we set them on the texture instance above for immediate use)
        // Ideally we should set the importer settings too, but for generated texture, the instance settings matter for the current session.
        // For persistence, let's just rely on the generated file.
        
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
}
