using UnityEngine;

public class ElectricCabinetOS : MonoBehaviour
{
    [Header("OS Settings")]
    [SerializeField] private float requiredTime = 15f;

    [Header("Visuals")]
    [SerializeField] private Light detectionLight; // lumière rouge au-dessus de l'armoire
    [SerializeField] private GameObject screen1;
    [SerializeField] private GameObject screen2;

    private float currentTime;
    private bool completed;

    public bool IsCompleted => completed;
    public float Progress01 => Mathf.Clamp01(currentTime / requiredTime);

    public void Start()
    {
        detectionLight.enabled = false;
        screen1.SetActive(false);
        screen2.SetActive(false);
    }

    /// <summary>
    /// Appelée chaque frame quand le joueur garde le curseur sur le panneau
    /// </summary>
    public void Tick(float deltaTime)
    {
        if (completed) return;

        currentTime += deltaTime;

        // Allume la lumière tant que le remplissage est en cours
        if (detectionLight != null)
            detectionLight.enabled = true;

        if (currentTime >= requiredTime)
        {
            Complete();
        }
    }

    /// <summary>
    /// Réinitialise la progression si le joueur sort du panneau
    /// </summary>
    public void ResetProgress()
    {
        if (completed) return;

        currentTime = 0f;

        // Éteint la lumière si l'OS n'est pas encore complété
        if (detectionLight != null)
            detectionLight.enabled = false;
    }

    private void Complete()
    {
        completed = true;

        // Signale le manager des spawners
        SpawnerProgressionManager manager = FindObjectOfType<SpawnerProgressionManager>();
        manager?.OnSecondaryObjectiveCompleted();

        // Laisse la lumière allumée après complétion
        if (detectionLight != null)
            detectionLight.enabled = true;
        
        if (screen1 != null)
            screen1.SetActive(true);
        
        if (screen2 != null)
            screen2.SetActive(true);

        Debug.Log("OS Armoire électrique complété");
    }
}
