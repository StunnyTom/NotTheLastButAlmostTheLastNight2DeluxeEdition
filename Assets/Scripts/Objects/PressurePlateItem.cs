using UnityEngine;
using System.Collections;

public class PressurePlateItem : UsableItem
{
    [Header("Plate Parts")]
    public Transform plateMesh;
    public Light plateLight;

    [Header("Plate Settings")]
    public float pressDepth = 0.05f;
    public float pressSpeed = 0.15f;
    public float activeDuration = 1.0f;
    public float minPlateThicknessY = 0.02f;

    [Header("Light Settings")]
    public float minLightIntensity = 20f;
    public float maxLightIntensity = 150f;

    private bool isPressed = false;
    private bool isHeld = false;

    private Vector3 originalPlatePosition;
    private Vector3 originalPlateScale;
    private float originalLightIntensity;
    private bool onGround = true;

    private void Start()
    {
        if (plateMesh == null)
            plateMesh = transform;

        originalPlatePosition = plateMesh.localPosition;
        originalPlateScale = plateMesh.localScale;

        if (plateLight != null)
            originalLightIntensity = plateLight.intensity;
    }

    public override void Use()
    {
        // Rien ici : la plaque s'active via TryPress()
    }

    public bool IsPressed()
    {
        return isPressed;
    }

    public void OnPickedUpByPlayer()
    {
        isHeld = true;
        onGround = false; // Plus au sol
    }

    public void OnDroppedOrUsedByPlayer()
    {
        isHeld = false;
        onGround = true; // Maintenant posé
        RegisterRestingState();
    }

    public void TryPress()
    {
        if (!isPressed && !isHeld && onGround)
            StartCoroutine(ActivatePlate());
    }

    private IEnumerator ActivatePlate()
    {
        isPressed = true;

        yield return StartCoroutine(AnimatePlate(true));

        yield return new WaitForSeconds(activeDuration);

        yield return StartCoroutine(AnimatePlate(false));

        isPressed = false;
    }

    private IEnumerator AnimatePlate(bool press)
    {
        float targetY = press ? Mathf.Max(originalPlateScale.y - pressDepth, minPlateThicknessY)
                              : originalPlateScale.y;

        Vector3 targetScale = new Vector3(originalPlateScale.x, targetY, originalPlateScale.z);
        float targetIntensity = press ? maxLightIntensity : minLightIntensity;

        float elapsed = 0f;
        float duration = pressSpeed;

        Vector3 startScale = plateMesh.localScale;
        float startIntensity = plateLight != null ? plateLight.intensity : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            plateMesh.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (plateLight != null)
                plateLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null;
        }

        plateMesh.localScale = targetScale;

        if (plateLight != null)
            plateLight.intensity = targetIntensity;
    }

    public void RegisterRestingState()
    {
        originalPlatePosition = plateMesh.localPosition;
        originalPlateScale = plateMesh.localScale;
        if (plateLight != null)
            originalLightIntensity = plateLight.intensity;
    }
}
