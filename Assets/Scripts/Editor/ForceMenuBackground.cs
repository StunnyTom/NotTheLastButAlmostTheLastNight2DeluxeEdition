using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace EditorTools
{
    public class ForceMenuBackground : EditorWindow
    {
        [MenuItem("Tools/UI/Force Menu Background")]
        public static void FixBackground()
        {
            // 1. Load Sprite
            string path = "Assets/Ressources/bg_menu_principal.png";
            Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (bgSprite == null)
            {
                // Try texture if sprite fails
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    // Create sprite from texture
                    bgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }

            if (bgSprite == null)
            {
                Debug.LogError($"[FixBG] Could not find image at {path}. Please check path!");
                return;
            }

            // 2. Setup Canvas
            GameObject bgObj = GameObject.Find("BackgroundCanvas");
            if (bgObj == null)
            {
                bgObj = new GameObject("BackgroundCanvas");
                Undo.RegisterCreatedObjectUndo(bgObj, "Create BG Canvas");
            }
            else
            {
                Undo.RecordObject(bgObj, "Update BG Canvas");
            }

            // Ensure Canvas Component
            Canvas c = bgObj.GetComponent<Canvas>();
            if (c == null) c = Undo.AddComponent<Canvas>(bgObj);
            
            // IMPORTANT: ScreenSpaceOverlay ensures it covers any camera skybox
            c.renderMode = RenderMode.ScreenSpaceOverlay; 
            c.sortingOrder = -100; // Put it BEHIND everything else (UI is usually 0)

            // Scaler
            CanvasScaler scaler = bgObj.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = Undo.AddComponent<CanvasScaler>(bgObj);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // 3. Setup Image
            Image img = bgObj.GetComponent<Image>();
            if (img == null) img = Undo.AddComponent<Image>(bgObj);
            
            img.sprite = bgSprite;
            img.color = Color.white;
            
            // Full Stretch
            RectTransform rt = img.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Debug.Log("[FixBG] Background Restored! Saved to 'BackgroundCanvas'.");
        }
    }
}
