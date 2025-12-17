using UnityEngine;
using UnityEngine.UI;

public class ForceStylesRuntime : MonoBehaviour
{
    void Start()
    {
        // DISABLED TO PREVENT BUILD ISSUES
        /*
        // 1. Force Canvas Scaler
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            var scaler = c.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = c.gameObject.AddComponent<CanvasScaler>();

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            // Force Update
            Canvas.ForceUpdateCanvases();
        }
        
        // 2. Force Camera Background
        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;
        }
        
        Debug.Log("ForceStylesRuntime: Applied Visual Fixes");
        */
    }
}
