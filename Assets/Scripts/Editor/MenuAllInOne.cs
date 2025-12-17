using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;
using SlimUI.ModernMenu;
using System.Collections.Generic;

public class MenuAllInOne : EditorWindow
{
    private static Queue<System.Action> actionQueue = new Queue<System.Action>();
    private static double lastActionTime;
    private static float delayBetweenSteps = 0.5f; // Half second delay
    private static bool isRunning = false;

    [MenuItem("Tools/1-CLICK SETUP (All-in-One)")]
    public static void SetupEverything()
    {
        bool confirm = EditorUtility.DisplayDialog("Start 1-Click Setup?", 
            "This will run ALL setup scripts in order WITH DELAYS to ensure stability:\n\n" +
            "1. Essentials (Background, Hybrid, Settings, Buttons)\n" +
            "2. Scale Adjustments\n" +
            "3. Polish\n" +
            "4. Game Title\n\n" +
            "Are you sure?", "Yes, Do It!", "Cancel");

        if (!confirm) return;

        Debug.Log("🚀 STARTING 1-CLICK SETUP (Async Mode)...");
        
        actionQueue.Clear();
        
        // --- QUEUE ACTIONS ---
        
        // 1. Essentials
        EnqueueAction(() => SetSlimUIBackground.SetupBackground(), "Setup Background");
        EnqueueAction(() => SetupHybridMenu.SetupHybrid(), "Setup Hybrid Menu");
        // EnqueueAction(() => CreateSettingsCanvas.CreateSettings2DCanvas(), "Create Settings Canvas"); // OLD
        EnqueueAction(() => CreateSimpleSettings.CreateCustomMenu(), "Create Custom Settings"); // NEW
        // EnqueueAction(() => CreateSettingsCanvas.WireSettingsButton(), "Wire Settings"); // Handle inside CreateCustomMenu now
        // EnqueueAction(() => ForceSettings2D.Force2D(), "Force Settings 2D"); // Not needed
        EnqueueAction(() => ConfigureSlimUIMenu.ConfigureMenu(), "Configure Menu");
        EnqueueAction(() => SetupHostJoinButtons.SetupButtons(), "Setup Host/Join");
        EnqueueAction(() => NetworkSetup.SetupNetwork(), "Setup Network Manager");
        EnqueueAction(() => FixSceneSetup.FixIt(), "Fix Build Settings"); // NEW

        // 2. Adjust Scale
        //EnqueueAction(() => FixMenuScale.ResetScale(), "Reset Scale");
        EnqueueAction(() => FixMenuScale.MakeBigger(), "Scale Up 1");
        EnqueueAction(() => FixMenuScale.MakeBigger(), "Scale Up 2");

        // 3. Polish
        EnqueueAction(() => MenuPolisher.FixPixelation(), "Fix Pixelation");
        EnqueueAction(() => MenuPolisher.FixOverflow(), "Fix Overflow");
        EnqueueAction(() => MenuPolisher.IncreaseBorders(), "Increase Borders");
        EnqueueAction(() => MenuPolisher.IncreaseHeight(), "Increase Height");
        //EnqueueAction(() => FixMenuText.FixVisibility(), "Fix Text Visibility");

        // 4. Title
        EnqueueAction(() => AddGameTitleAuto(), "Add Game Title");

        // Start execution loop
        isRunning = true;
        lastActionTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += UpdateLoop;
    }

    private static void EnqueueAction(System.Action action, string name)
    {
        actionQueue.Enqueue(() => {
            Debug.Log($"... Running: {name} ...");
            action.Invoke();
        });
    }

    private static void UpdateLoop()
    {
        if (!isRunning)
        {
            EditorApplication.update -= UpdateLoop;
            return;
        }

        if (actionQueue.Count > 0)
        {
            if (EditorApplication.timeSinceStartup - lastActionTime > delayBetweenSteps)
            {
                var action = actionQueue.Dequeue();
                try
                {
                    action.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error in step: {e.Message}");
                    // Stop on error? Or continue? Let's continue but log.
                }
                lastActionTime = EditorApplication.timeSinceStartup;
            }
        }
        else
        {
            isRunning = false;
            EditorApplication.update -= UpdateLoop;
            Debug.Log("✨ ALL DONE! Setup Completed Successfully. ✨");
            EditorUtility.DisplayDialog("Finished!", "All setup steps completed!", "Awesome");
        }
    }

    private static void AddGameTitleAuto()
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null || manager.mainCanvas == null)
        {
            Debug.LogError("Cannot add title: Menu Manager or Canvas not found.");
            return;
        }

        string projectName = "NotTheLastButAlmostTheLastNight2DeluxeEdition"; // Hardcoded backup
        try
        {
            DirectoryInfo assetsDir = new DirectoryInfo(Application.dataPath);
            projectName = assetsDir.Parent.Name;
        }
        catch {}

        Transform existingTitle = manager.mainCanvas.transform.Find("GameTitle");
        if (existingTitle != null) Undo.DestroyObjectImmediate(existingTitle.gameObject);

        GameObject titleObj = new GameObject("GameTitle");
        Undo.RegisterCreatedObjectUndo(titleObj, "Create Game Title");
        titleObj.transform.SetParent(manager.mainCanvas.transform, false);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = projectName.Replace("DeluxeEdition", "\nDeluxe Edition"); // Small formatting check
        titleText.fontSize = 72;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        
        // Add shadow
        titleText.enableVertexGradient = true;
        titleText.colorGradient = new VertexGradient(Color.white, Color.white, new Color(0.8f, 0.8f, 0.8f), new Color(0.5f, 0.5f, 0.5f));

        RectTransform rt = titleObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -100);
        rt.sizeDelta = new Vector2(1200, 200);

        if (manager.themeController != null) titleText.color = manager.themeController.textColor;
        
        Debug.Log($"Game Title Added: {titleText.text}");
    }
}
