using UnityEngine;
using SurvivorSystem;

public class AmmoSpawnerPlate : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int bulletsGiven = 1;

    [Header("Cooldown")]
    [SerializeField] private float baseCooldown = 30f;
    private float currentCooldown;
    private float nextAvailableTime;

    [Header("Progression")]
    [SerializeField] private bool isUnlocked = false;

    public bool IsUnlocked => isUnlocked;
    public bool IsReady => isUnlocked && Time.time >= nextAvailableTime;

    private void Awake()
    {
        currentCooldown = baseCooldown;
        nextAvailableTime = 0f;
    }

    public void Unlock()
    {
        isUnlocked = true;
        nextAvailableTime = Time.time;
    }

    public void MultiplyCooldown(float factor)
    {
        currentCooldown = Mathf.Max(0.1f, currentCooldown * factor);
    }

    public bool TryGiveAmmo(SurvivorController player)
    {
        if (!IsReady) return false;
        if (player == null) return false;

        player.AddBullet(bulletsGiven);

        nextAvailableTime = Time.time + currentCooldown;
        Debug.Log("[AmmoSpawner] Bullet given, cooldown started");

        return true;
    }
}
