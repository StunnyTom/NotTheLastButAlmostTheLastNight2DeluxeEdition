using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace EditorTools
{
    public class ForceUIFix : EditorWindow
    {
        [MenuItem("Tools/UI/Fix Canvas Scale")]
        public static void FixCanvasScale()
        {
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            int count = 0;

            foreach (var canvas in canvases)
            {
                Undo.RecordObject(canvas.gameObject, "Fix Canvas Scale");

                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler == null)
                {
                    scaler = Undo.AddComponent<CanvasScaler>(canvas.gameObject);
                }
                else
                {
                    Undo.RecordObject(scaler, "Fix Canvas Scale Settings");
                }

                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                EditorUtility.SetDirty(canvas.gameObject);
                count++;
            }

            Debug.Log($"[UI Fix] Updated {count} Canvases to ScaleWithScreenSize (1920x1080). Don't forget to SAVE the scene!");
        }
    }
}
