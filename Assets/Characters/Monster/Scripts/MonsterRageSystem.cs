using Mechanics;
using UnityEngine;
using UI;

namespace MonsterSystem
{
    public class MonsterRageSystem : MonoBehaviour
    {
        [Header("Settings")]
        public float maxRage = 100f;
        
        [Header("Runtime State")]
        [SerializeField] private float currentRage = 0f;

        [Header("References")]
        public RageBarUI rageBarUI;

        // Called by colleagues code
        public void AddRage(float amount)
        {
            currentRage = Mathf.Clamp(currentRage + amount, 0f, maxRage);
            UpdateUI();
        }

        public float CurrentRage => currentRage;
        public bool IsFull => currentRage >= maxRage;

        private void UpdateUI()
        {
            if (rageBarUI != null)
            {
                rageBarUI.UpdateBar(currentRage, maxRage);
            }
        }
        
        // This is the function to link to the Ultimate Skill in the Inspector
        public void TryUseUltimate()
        {
            // Note: Validation is done in CanUseSkill inside MonsterController.
            // This method simply executes the EFFECT.
            
            Debug.Log("<b><color=red>MONSTER ULTIMATE UNLEASHED!</color></b>");
            
            // Reset Rage
            currentRage = 0f;
            UpdateUI();
        }
    }
}
