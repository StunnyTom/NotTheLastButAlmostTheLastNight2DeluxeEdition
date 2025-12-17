using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class DiagnoseMainMenuVisuals : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/Diagnose Visuals (Menu)")]
    public static void Diagnose()
    {
        Debug.Log("--- Inspecting Main Menu Visuals ---");
        
        // 1. Check Canvas Scaler
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                Debug.Log($"Canvas Scaler Mode: {scaler.uiScaleMode}");
                if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    Debug.Log($" - Ref Res: {scaler.referenceResolution}");
                    Debug.Log($" - Match Mode: {scaler.screenMatchMode}");
                    Debug.Log($" - Match Val: {scaler.matchWidthOrHeight}");
                }
            }
            else
            {
                Debug.LogError("Canvas has NO CanvasScaler!");
            }
        }
        else
        {
            Debug.LogError("No Canvas found!");
        }

        // 2. Check Background
        // Strategy: Look for "Background" object or known SlimUI paths
        var bgObj = GameObject.Find("Background");
        if (bgObj == null) bgObj = GameObject.Find("Menu Background");
        
        if (bgObj != null)
        {
            Debug.Log($"Found Background Object: '{bgObj.name}' (Active: {bgObj.activeSelf})");
            var img = bgObj.GetComponent<Image>();
            if (img != null) 
            {
                Debug.Log($" - Has Image Component. Sprite: {(img.sprite != null ? img.sprite.name : "NULL")}");
                Debug.Log($" - Color: {img.color}");
            }
            else
            {
                 Debug.LogWarning(" - NO Image component found.");
            }
        }
        else
        {
            Debug.LogError("Could not find object named 'Background' or 'Menu Background'.");
        }
    }
}
