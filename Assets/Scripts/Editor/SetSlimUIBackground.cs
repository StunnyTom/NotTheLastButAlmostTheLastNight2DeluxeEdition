using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SlimUI.ModernMenu;

public class SetSlimUIBackground : EditorWindow
{
    [MenuItem("Tools/Menu Setup (Essential)/1. Setup Background")]
    public static void SetupBackground()
    {
        Debug.Log("--- Setup SlimUI Background ---");

        // 1. Find the Menu Manager
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        
        // If missing, try to restore from prefab
        if (manager == null)
        {
            Debug.Log("UIMenuManager not found. Attempting to instantiate SlimUI Canvas Template...");
            
            string prefabPath = "Assets/SlimUI/Modern Menu 1/Prefabs/Canvas Templates/Canvas_DefaultTemplate1.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.name = "MainMenu_SlimUI";
                Undo.RegisterCreatedObjectUndo(instance, "Create SlimUI Menu");
                
                // Find manager again
                manager = instance.GetComponent<UIMenuManager>();
                if (manager == null) manager = instance.GetComponentInChildren<UIMenuManager>();
                
                Debug.Log("✓ Instantiated SlimUI Menu from prefab!");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", 
                    "Could not find UIMenuManager in scene.\n\nAlso failed to load prefab at:\n" + prefabPath + 
                    "\n\nPlease drag the SlimUI 'Canvas_DefaultTemplate1' prefab into the scene manually.", "OK");
                return;
            }
        }

        if (manager == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find UIMenuManager even after trying to create it.", "OK");
            return;
        }

        // 2. Find the Background Image
        string partialPath = "bg_menu_principal";
        Sprite bgSprite = null;

        string[] guids = AssetDatabase.FindAssets(partialPath + " t:Sprite");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            Debug.Log($"Found sprite at: {path}");
        }
        else
        {
            // Try texture if sprite not found
            guids = AssetDatabase.FindAssets(partialPath + " t:Texture2D");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    // Convert to sprite temporarily or warning
                    Debug.LogWarning($"Found texture at {path} but it is not set as Sprite in Import Settings.");
                    // Attempt to create a sprite from it
                    bgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    bgSprite.name = tex.name;
                }
            }
        }

        if (bgSprite == null)
        {
            EditorUtility.DisplayDialog("Error", $"Could not find sprite or texture with name '{partialPath}'", "OK");
            return;
        }

        // 3. Find or Create the Background GameObject
        // It should be under the main Canvas (manager.mainCanvas) to be visible initially
        if (manager.mainCanvas == null)
        {
            EditorUtility.DisplayDialog("Error", "UIMenuManager does not have mainCanvas assigned.", "OK");
            return;
        }

        Transform existingBg = manager.mainCanvas.transform.Find("Background");
        GameObject bgObj;
        Image bgImage;

        if (existingBg != null)
        {
            bgObj = existingBg.gameObject;
            bgImage = bgObj.GetComponent<Image>();
        }
        else
        {
            bgObj = new GameObject("Background");
            bgObj.transform.SetParent(manager.mainCanvas.transform, false);
            bgImage = bgObj.AddComponent<Image>();
        }

        Undo.RecordObject(bgObj.transform, "Setup Background");
        Undo.RecordObject(bgImage, "Setup Background Image");

        // 4. Configure the Image
        bgImage.sprite = bgSprite;
        bgImage.color = Color.white;
        
        // 5. Configure RectTransform (Fill Parent)
        RectTransform rt = bgObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;

        // 6. Send to Back (First Sibling)
        bgObj.transform.SetAsFirstSibling();

        Debug.Log("Background setup complete!");
        EditorUtility.DisplayDialog("Success", "Background image set successfully!", "OK");
    }
}
