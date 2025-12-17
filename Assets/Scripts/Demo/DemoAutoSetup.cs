using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Demo
{
    public class DemoAutoSetup : MonoBehaviour
    {
        // This script purely exists to auto-wire the demo so you don't have to do it manually.
        // It creates a temporary HUD and temporary Skills.

        private bool isSetup = false;

        void Update()
        {
            if (isSetup) return;

            // wait for a player to exist
            var player = FindObjectOfType<CharacterSkillManager>();
            if (player != null)
            {
                SetupDemo(player);
                isSetup = true;
            }
        }

        void SetupDemo(CharacterSkillManager player)
        {
            Debug.Log("--- SETTING UP SKILL DEMO ---");

            // 1. Ensure HUD System Exists
            if (HUDManager.Instance == null)
            {
                CreateHUDSystem();
            }

            // 2. Create Demo Skills (In Memory actions)
            if (player.skills.Count == 0)
            {
                AddDemoSkill(player, "Roar (A)", KeyCode.A, () => Debug.Log("<b><color=green>ACTION A: ROAR!</color></b>"));
                AddDemoSkill(player, "Sprint (Z)", KeyCode.Z, () => Debug.Log("<b><color=yellow>ACTION Z: SPRINT!</color></b>"));
                AddDemoSkill(player, "Heal (E)", KeyCode.E, () => Debug.Log("<b><color=cyan>ACTION E: HEAL!</color></b>"));
                AddDemoSkill(player, "Ultimate (R)", KeyCode.R, () => Debug.Log("<b><color=red>ACTION R: ULTIMATE!</color></b>"));
            }

            // 3. Force UI Refresh
            StartCoroutine(RefreshHUDNextFrame(player));
        }

        IEnumerator RefreshHUDNextFrame(CharacterSkillManager player)
        {
            yield return null;
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.InitializeHUD(player);
                Debug.Log("HUD Initialized with Demo Skills");
            }
        }

        void AddDemoSkill(CharacterSkillManager player, string name, KeyCode key, UnityEngine.Events.UnityAction action)
        {
            // Create Scriptable Object in memory
            var skillData = ScriptableObject.CreateInstance<SkillData>();
            skillData.skillName = name;
            skillData.defaultCooldown = 5f; // Short cooldown for testing
            
            // Add to player
            var entry = new CharacterSkillManager.SkillEntry();
            entry.skillData = skillData;
            entry.key = key;
            entry.onExecute = new UnityEngine.Events.UnityEvent();
            entry.onExecute.AddListener(action);

            player.skills.Add(entry);
        }

        void CreateHUDSystem()
        {
            // Canvas
            GameObject canvasObj = new GameObject("Demo_HUD_Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // Extreme value to be sure
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // DON'T DESTROY ON LOAD to persist between scene changes/reloads if needed
            DontDestroyOnLoad(canvasObj);

            // HUD Manager
            GameObject inputsContainer = new GameObject("Inputs_Container");
            inputsContainer.transform.SetParent(canvasObj.transform, false);
            
            // Position carefully: Bottom Center
            RectTransform containerRect = inputsContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0f);
            containerRect.anchorMax = new Vector2(0.5f, 0f); // Anchor to bottom center
            containerRect.pivot = new Vector2(0.5f, 0f);     // Pivot at bottom center
            containerRect.anchoredPosition = new Vector2(0, 50); // 50px up from bottom
            containerRect.sizeDelta = new Vector2(0, 100);  // Height 100

            var hLayout = inputsContainer.AddComponent<HorizontalLayoutGroup>();
            hLayout.childControlWidth = false;
            hLayout.childControlHeight = false;
            hLayout.spacing = 20;
            hLayout.childAlignment = TextAnchor.MiddleCenter;
            hLayout.childForceExpandWidth = false;
            hLayout.childForceExpandHeight = false;

            HUDManager hudManager = canvasObj.AddComponent<HUDManager>();
            hudManager.itemsContainer = inputsContainer.transform;
            
            // Create a pseudo-prefab for the items
            hudManager.hudItemPrefab = CreateItemPrefab(canvasObj.transform);
            
            Debug.Log($"[DemoAutoSetup] HUD Created. Canvas: {canvasObj.name}, Container: {inputsContainer.name}");
        }

        HUDItem CreateItemPrefab(Transform storage)
        {
            GameObject itemObj = new GameObject("HUDItem_Prefab");
            itemObj.transform.SetParent(storage);
            
            // Important: Prefab RectTransform
            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100); // Bigger size
            
            Image bg = itemObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Almost black

            // Text Key (Top Left)
            GameObject textObj = new GameObject("KeyText");
            textObj.transform.SetParent(itemObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = new Vector2(5, 0); // Padding left
            textRect.offsetMax = new Vector2(0, -5); // Padding top
            
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "A";
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.fontSize = 24;
            tmp.color = Color.yellow; // High contrast
            tmp.fontStyle = FontStyles.Bold;

            // Cooldown Overlay (Full Stretch)
            GameObject overlayObj = new GameObject("CooldownOverlay");
            overlayObj.transform.SetParent(itemObj.transform, false);
            RectTransform overlayRect = overlayObj.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.sizeDelta = Vector2.zero;
            
            Image overlayImg = overlayObj.AddComponent<Image>();
            overlayImg.color = new Color(0.8f, 0f, 0f, 0.6f); // Red transparent

            // Cooldown Text (Center)
            GameObject cdTextObj = new GameObject("CDText");
            cdTextObj.transform.SetParent(overlayObj.transform, false);
            RectTransform cdRect = cdTextObj.AddComponent<RectTransform>();
            cdRect.anchorMin = Vector2.zero;
            cdRect.anchorMax = Vector2.one;
            cdRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI cdTmp = cdTextObj.AddComponent<TextMeshProUGUI>();
            cdTmp.text = "5";
            cdTmp.alignment = TextAlignmentOptions.Center;
            cdTmp.fontSize = 40;
            cdTmp.color = Color.white;
            cdTmp.fontStyle = FontStyles.Bold;

            HUDItem item = itemObj.AddComponent<HUDItem>();
            item.backgroundImage = bg;
            item.keyText = tmp;
            item.cooldownOverlay = overlayImg;
            item.cooldownText = cdTmp;
            
            itemObj.SetActive(false); // Disable prefab
            return item;
        }

        void OnGUI()
        {
            // Fallback DEBUG UI
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.Label($"<size=20><color=white>Demo Status: {(isSetup ? "On" : "Waiting...")}</color></size>");
            
            var player = FindObjectOfType<CharacterSkillManager>();
            GUILayout.Label($"Player Found: {(player != null ? player.name : "None")}");
            
            if(player != null)
            {
                GUILayout.Label($"Skills: {player.skills.Count}");
                foreach(var s in player.skills)
                {
                    GUILayout.Label($"- {s.skillData?.skillName ?? "null"} ({s.key}) CD: {s.RemainingCooldown:F1}");
                }
            }
            GUILayout.EndArea();
        }
    }
}
