using UnityEngine;

public class ObjectSpawnerPlate : MonoBehaviour
{
    [Header("Item")]
    public UsableItem itemPrefab;

    [Header("Cooldown")]
    [SerializeField] private float baseCooldown = 30f;
    private float currentCooldown;
    private float nextAvailableTime;

    [Header("Progression")]
    [SerializeField] private bool isUnlocked = false;

    // ─────────────────────────────────────────
    // PROPERTIES (UTILISÉES PAR LES AUTRES SCRIPTS)
    // ─────────────────────────────────────────
    public bool IsUnlocked => isUnlocked;
    public bool IsReady => isUnlocked && Time.time >= nextAvailableTime;
    public bool IsAvailable => IsReady;


    private void Awake()
    {
        currentCooldown = baseCooldown;
        nextAvailableTime = 0f;
    }

    // ─────────────────────────────────────────
    // PROGRESSION
    // ─────────────────────────────────────────
    public void Unlock()
    {
        isUnlocked = true;
        nextAvailableTime = Time.time;
    }

    public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(1f, currentCooldown - seconds);
    }

    public void MultiplyCooldown(float factor)
    {
        currentCooldown = Mathf.Max(0.1f, currentCooldown * factor);
    }


    // ─────────────────────────────────────────
    // INTERACTION JOUEUR
    // ─────────────────────────────────────────
    public bool TryGiveToPlayer(Inventory inventory)
    {
        if (!IsReady) return false;
        if (itemPrefab == null) return false;
        if (!inventory.HasFreeSlot()) return false;

        UsableItem instance = Instantiate(itemPrefab);

        bool taken = inventory.TryAddFromSpawner(instance);
        if (!taken)
        {
            Destroy(instance.gameObject);
            return false;
        }

        nextAvailableTime = Time.time + currentCooldown;
        return true;
    }

    // ─────────────────────────────────────────
    // DEBUG / UTILITAIRE
    // ─────────────────────────────────────────
    public float GetCooldownProgress01()
    {
        if (!isUnlocked) return 0f;
        if (IsReady) return 1f;

        float elapsed = currentCooldown - (nextAvailableTime - Time.time);
        return Mathf.Clamp01(elapsed / currentCooldown);
    }
}
