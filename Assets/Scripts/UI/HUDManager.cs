using System.Collections.Generic;
using Mechanics;
using UnityEngine;

namespace UI
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Configuration")]
        public HUDItem hudItemPrefab;
        public Transform itemsContainer; // Grid or Horizontal Layout Group

        private List<HUDItem> activeItems = new List<HUDItem>();

        // Singleton for easy access (optional, but requested "simple HUD")
        public static HUDManager Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        public void InitializeHUD(CharacterSkillManager skillManager)
        {
            // Clear existing
            foreach (Transform child in itemsContainer)
            {
                Destroy(child.gameObject);
            }
            activeItems.Clear();

            // Create new
            for (int i = 0; i < skillManager.GetSkillCount(); i++)
            {
                var skill = skillManager.GetSkill(i);
                if (skill.skillData != null)
                {
                    HUDItem newItem = Instantiate(hudItemPrefab, itemsContainer);
                    newItem.gameObject.SetActive(true);
                    newItem.Setup(skillManager, i);
                    activeItems.Add(newItem);
                }
            }
        }
    }
}
