using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlateTrigger : MonoBehaviour
{
    [Tooltip("Référence vers le PressurePlateItem parent")]
    public PressurePlateItem parentPlate;

    private void Awake()
    {
        // sécurité : si pas assigné, on cherche un parent
        if (parentPlate == null)
            parentPlate = GetComponentInParent<PressurePlateItem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (parentPlate != null)
            parentPlate.TryPress();
    }

    // facultatif : on peut aussi appeler TryRelease si tu veux réaction à la sortie
    private void OnTriggerExit(Collider other)
    {
        // if (other.CompareTag("Player") && parentPlate != null)
        //     parentPlate.TryRelease();
    }
}
