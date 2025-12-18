using UnityEngine;

public class SpawnerProgressionManager : MonoBehaviour
{
    [Header("Spawner Plates")]
    public ObjectSpawnerPlate lampPlate;
    public ObjectSpawnerPlate pressurePlate;
    public AmmoSpawnerPlate ammoPlate;

    private int completedObjectives = 0;

    private void Start()
    {
        // Lampes disponibles dès le début
        lampPlate.Unlock();
        ammoPlate.Unlock();
    }

    /// <summary>
    /// À appeler à chaque objectif secondaire validé
    /// </summary>
    public void OnSecondaryObjectiveCompleted()
    {
        completedObjectives++;

        switch (completedObjectives)
        {
            case 1:
                UnlockPressurePlate();
                break;

            case 2:
                ReduceAllCooldownsByHalf();
                break;
        }
    }

    private void UnlockPressurePlate()
    {
        pressurePlate.Unlock();
        Debug.Log("Plaque de pression débloquée");
    }

    private void ReduceAllCooldownsByHalf()
    {
        lampPlate.MultiplyCooldown(0.5f);
        pressurePlate.MultiplyCooldown(0.5f);

        Debug.Log("Cooldowns divisés par 2");
    }
}
