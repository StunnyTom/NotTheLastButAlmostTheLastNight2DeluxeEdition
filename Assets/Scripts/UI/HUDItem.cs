using System;
using Mechanics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUDItem : MonoBehaviour
    {
        [Header("UI Components")]
        public Image iconImage;
        public Image cooldownOverlay;
        public TextMeshProUGUI cooldownText;
        public TextMeshProUGUI keyText; // To display 'A', 'Z', etc.
        public Image backgroundImage;
        
        [Header("State")]
        private CharacterSkillManager.SkillEntry skillEntry;
        private CharacterSkillManager manager;
        private int skillIndex;

        public void Setup(CharacterSkillManager manager, int index)
        {
            this.manager = manager;
            this.skillIndex = index;
            this.skillEntry = manager.GetSkill(index);

            if (skillEntry != null && skillEntry.skillData != null)
            {
                if (iconImage) iconImage.sprite = skillEntry.skillData.icon;
                if (keyText) keyText.text = skillEntry.key.ToString().Replace("Alpha", ""); // Simple clean up
            }

            // Initial visual state
            UpdateVisuals();
        }

        void Update()
        {
            if (manager == null || skillEntry == null) return;

            // Handle Input (Optional: Input can be handled centrally by Manager or HUDManager, 
            // but user asked for "HUD slot linked to key", so local check is valid too).
            // However, to avoid duplicate inputs if Manager also checks, we should coordinate.
            // Plan: HUDItem just reflects state. Manager handles Input. OR HUDManager calls TryExecute.
            
            // Actually, let's keep Input centralized in PlayerController or CharacterSkillManager to avoid UI dependency for logic.
            // But for this "HUD simple" request, having the Controller handle input and just call Manager is best.
            // So HUDItem purely visualizes Cooldown.
            
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (skillEntry == null) return;

            bool onCooldown = skillEntry.IsOnCooldown;
            
            if (cooldownOverlay) cooldownOverlay.gameObject.SetActive(onCooldown);
            if (cooldownText)
            {
                cooldownText.gameObject.SetActive(onCooldown);
                if (onCooldown)
                {
                    cooldownText.text = Mathf.CeilToInt(skillEntry.RemainingCooldown).ToString();
                }
            }
            
            // Optional: Grey out if cant use (e.g. Rage not full)
            // This would require a "CanUse" check that is cheap to call every frame.
            if (!onCooldown && manager != null)
            {
                 bool usable = manager.CanUseSkill(skillIndex);
                 if (iconImage) iconImage.color = usable ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            }
        }
    }
}
