using UnityEngine;
using UnityEngine.UI;

namespace UI.Fixes
{
    public class FixCanvasScale : MonoBehaviour
    {
        void Start()
        {
            // ApplyFix(); // Disabled to respect Editor settings
        }

        public void ApplyFix()
        {
            // Find ALL Canvases in the scene (even inactive ones if we use Resources.FindObjectsOfTypeAll, but arguably Start only runs on active)
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None); // Finds loaded active canvases

            foreach (var canvas in canvases)
            {
                // We only care about root canvases usually, or overlay/camera ones
                // Check if it has a scaler
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler == null)
                {
                    scaler = canvas.gameObject.AddComponent<CanvasScaler>();
                }

                // Force Scale With Screen Size
                if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    Debug.Log($"[FixCanvasScale] Fixing Canvas '{canvas.name}' from {scaler.uiScaleMode} to ScaleWithScreenSize");
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                }

                // Standard resolution (usually 1920x1080)
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f; // Balanced match
            }
        }
    }
}
