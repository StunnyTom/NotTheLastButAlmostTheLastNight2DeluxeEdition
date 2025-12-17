using UnityEngine;

namespace Mechanics
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Display Info")]
        public string skillName = "New Skill";
        public Sprite icon;
        [TextArea] public string description = "";

        [Header("Settings")]
        public float defaultCooldown = 20f;
    }
}
