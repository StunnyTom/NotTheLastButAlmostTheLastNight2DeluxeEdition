using UnityEngine;
using UnityEditor;
using SlimUI.ModernMenu;

public class DiagnoseSlimUISettings : EditorWindow
{
    [MenuItem("Tools/Diagnose Settings Issue")]
    public static void DiagnoseSettings()
    {
        Debug.Log("=== DIAGNOSING SLIMUI SETTINGS ===");
        
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null)
        {
            Debug.LogError("UIMenuManager not found!");
            return;
        }

        // Check 1: Camera Animator
        Debug.Log("\n--- Camera Animator Check ---");
        Animator camAnim = manager.GetComponent<Animator>();
        if (camAnim == null)
        {
            Debug.LogError("❌ NO ANIMATOR on UIMenuManager! The Position2() method needs this to work!");
        }
        else
        {
            Debug.Log("✓ Animator found: " + camAnim.name);
            if (camAnim.runtimeAnimatorController == null)
            {
                Debug.LogError("❌ Animator has NO CONTROLLER! It won't animate anything!");
            }
            else
            {
                Debug.Log("✓ Animator Controller: " + camAnim.runtimeAnimatorController.name);
            }
        }

        // Check 2: Settings Panels
        Debug.Log("\n--- Settings Panels Check ---");
        CheckPanel("PanelGame", manager.PanelGame);
        CheckPanel("PanelVideo", manager.PanelVideo);
        CheckPanel("PanelControls", manager.PanelControls);
        CheckPanel("PanelKeyBindings", manager.PanelKeyBindings);

        // Check 3: Main Canvas
        Debug.Log("\n--- Canvas Check ---");
        if (manager.mainCanvas == null)
        {
            Debug.LogError("❌ mainCanvas is NULL!");
        }
        else
        {
            Debug.Log("✓ Main Canvas: " + manager.mainCanvas.name);
            // Check if panels are children of this canvas
            if (manager.PanelGame != null && !IsChildOf(manager.PanelGame.transform, manager.mainCanvas.transform))
            {
                Debug.LogWarning("⚠ PanelGame is NOT a child of mainCanvas! This might cause issues.");
            }
        }

        // Check 4: Camera Object
        Debug.Log("\n--- Camera Object Check ---");
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("❌ No Main Camera found!");
        }
        else
        {
            Debug.Log("✓ Main Camera: " + mainCam.name);
            Debug.Log("  Position: " + mainCam.transform.position);
            Debug.Log("  Rotation: " + mainCam.transform.eulerAngles);
        }

        Debug.Log("\n=== DIAGNOSIS COMPLETE ===");
        Debug.Log("Check the Console for details above.");
    }

    private static void CheckPanel(string name, GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogError($"❌ {name} is NULL!");
        }
        else
        {
            Debug.Log($"✓ {name}: {panel.name} (Active: {panel.activeSelf})");
        }
    }

    private static bool IsChildOf(Transform child, Transform parent)
    {
        Transform current = child;
        while (current != null)
        {
            if (current == parent) return true;
            current = current.parent;
        }
        return false;
    }
}
