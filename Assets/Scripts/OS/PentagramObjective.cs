using UnityEngine;

public class PentagramObjective : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PentagramCandleGroup candleGroup;
    [SerializeField] private float requiredTime = 30f; // Temps pour valider l'objectif
    [SerializeField] private SpawnerProgressionManager spawnerManager;

    private float timer = 0f;
    private bool playerInside = false;

    // Sauvegarde de la progression si le joueur sort de la zone
    private float savedProgress = 0f;

    private int totalCandles;
    
    private void Awake()
    {
        if (candleGroup == null)
            Debug.LogWarning("PentagramCandleGroup not assigned.");
        totalCandles = candleGroup.Count;

        // Éteindre toutes les bougies au départ
        for (int i = 0; i < totalCandles; i++)
        {
            candleGroup.SetLit(i, false);
        }
    }

    private void Update()
    {
        if (!playerInside) return;

        timer += Time.deltaTime;
        float progress = Mathf.Clamp(savedProgress + timer, 0f, requiredTime);

        // Déterminer combien de bougies doivent être allumées
        int candlesToLight = Mathf.FloorToInt(progress / requiredTime * totalCandles);

        // Light up progressively
        for (int i = 0; i < totalCandles; i++)
        {
            candleGroup.SetLit(i, i < candlesToLight);
        }

        // Objectif validé
        if (progress >= requiredTime)
        {
            Debug.Log("Pentagram objective completed!");
            playerInside = false;
            NotifySpawnerManager();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        timer = 0f; // reset timer pour la session actuelle
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        savedProgress += timer; // sauvegarde la progression
        timer = 0f;
    }

    private void NotifySpawnerManager(){
        if (spawnerManager != null)
        {
            spawnerManager.OnSecondaryObjectiveCompleted();
        }
    }
}
