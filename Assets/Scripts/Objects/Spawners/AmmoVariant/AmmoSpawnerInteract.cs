using UnityEngine;
using SurvivorSystem;

public class AmmoSpawnerInteract : MonoBehaviour
{
    private AmmoSpawnerPlate plate;

    private void Awake()
    {
        plate = GetComponentInParent<AmmoSpawnerPlate>();
    }

    private void OnTriggerEnter(Collider other)
    {
        SurvivorController player = other.GetComponentInParent<SurvivorController>();
        if (player == null) return;

        plate.TryGiveAmmo(player);
    }
}
