using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics
{
    public class CharacterSkillManager : MonoBehaviour
    {
        [Serializable]
        public class SkillEntry
        {
            public SkillData skillData;
            public KeyCode key;
            public UnityEvent onExecute;
            
            [HideInInspector] public float lastUsedTime = -999f;
            
            // Runtime helper
            public bool IsOnCooldown => Time.time < lastUsedTime + (skillData != null ? skillData.defaultCooldown : 0f);
            public float RemainingCooldown => Mathf.Max(0f, (lastUsedTime + (skillData != null ? skillData.defaultCooldown : 0f)) - Time.time);
        }

        [Header("Skills Configuration")]
        public List<SkillEntry> skills = new List<SkillEntry>();

        public event Action<int> OnSkillExecuted; // Event for UI updates if needed specifically

        /// <summary>
        /// Attempts to execute the skill at the given index.
        /// Returns true if execution was successful (started cooldown).
        /// </summary>
        public bool TryExecuteSkill(int index)
        {
            if (index < 0 || index >= skills.Count) return false;

            var entry = skills[index];

            if (entry.skillData == null)
            {
                Debug.LogWarning($"[CharacterSkillManager] Skill at index {index} has no SkillData assigned.");
                return false;
            }

            if (entry.IsOnCooldown)
            {
                // Debug.Log($"Skill {entry.skillData.skillName} is on cooldown. {entry.RemainingCooldown:F1}s remaining.");
                return false;
            }

            // Virtual check for custom logic (can be overridden by subclasses or handled here)
            if (!CanUseSkill(index))
            {
                return false;
            }

            // Execute
            entry.onExecute?.Invoke();
            entry.lastUsedTime = Time.time;
            
            Debug.Log($"[CharacterSkillManager] Executed skill: {entry.skillData.skillName}");
            OnSkillExecuted?.Invoke(index);
            
            return true;
        }
        
        /// <summary>
        /// Attempts to execute the skill linked to the given key.
        /// </summary>
        public bool TryExecuteSkillByKey(KeyCode key)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].key == key)
                {
                    return TryExecuteSkill(i);
                }
            }
            return false;
        }

        /// <summary>
        /// Overrideable method to check custom conditions (e.g. Rage, Mana, Stunned state).
        /// </summary>
        public virtual bool CanUseSkill(int index)
        {
            // Default: Always true if cooldown passed
            return true;
        }

        // Helper to get skill info for UI
        public SkillEntry GetSkill(int index)
        {
            if (index < 0 || index >= skills.Count) return null;
            return skills[index];
        }
        
        public int GetSkillCount() => skills.Count;
    }
}
