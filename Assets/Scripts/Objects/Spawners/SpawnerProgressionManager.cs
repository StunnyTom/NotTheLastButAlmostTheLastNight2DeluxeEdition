using UnityEngine;

public class SpawnerProgressionManager : MonoBehaviour
{
    public ObjectSpawnerPlate[] plates;

    public void UnlockPlate(int index)
    {
        if (index < 0 || index >= plates.Length) return;
        plates[index].Unlock();
    }

    public void ReduceAllCooldowns(float amount)
    {
        foreach (var plate in plates)
            plate.ReduceCooldown(amount);
    }

    // Exemple
    public void OnSecondaryObjectiveCompleted()
    {
        ReduceAllCooldowns(5f);
    }
}
