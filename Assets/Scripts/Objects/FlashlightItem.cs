using UnityEngine;

public class FlashlightItem : UsableItem
{
    [Header("Light Settings")]
    public Light flashlightLight;   // Lumière à attacher dans l’inspector
    public bool startTurnedOff = true;

    private bool isOn = false;

    private void Start()
    {
        if (flashlightLight == null)
        {
            Debug.LogWarning("FlashlightItem: aucune light n’est assignée !");
            return;
        }

        // L'état initial
        flashlightLight.enabled = !startTurnedOff;
        isOn = flashlightLight.enabled;
    }

    public override void Use()
    {
        if (flashlightLight == null)
        {
            Debug.LogWarning("FlashlightItem: Light non assignée !");
            return;
        }

        // Toggle
        isOn = !isOn;
        flashlightLight.enabled = isOn;

        Debug.Log(itemName + " turned " + (isOn ? "ON" : "OFF"));
    }
}
