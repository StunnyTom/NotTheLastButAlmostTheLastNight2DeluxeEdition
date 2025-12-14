using UnityEngine;
using SurvivorSystem;

public class SpawnerPlateInteract : MonoBehaviour
{
    public ObjectSpawnerPlate plate;

    private SurvivorController player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.GetComponent<SurvivorController>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = null;
    }

    private void Update()
    {
        if (player == null) return;

        Inventory inv = Inventory.Instance;
        if (inv == null) return;

        // Interaction automatique ou touche ?
        if (!plate.IsAvailable) return;

        bool success = plate.TryGiveToPlayer(inv);
        if (success)
        {
            Debug.Log("Objet récupéré depuis la plaque");
        }
    }
}
