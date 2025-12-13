using UnityEngine;

// Simple pistol implementation using raycast
public class GunItem : UsableItem
{
    [Header("Gun Settings")]
    public float fireRate = 0.25f;
    public float range = 100f;
    [Tooltip("Layers pouvant être touchés par le tir")]
    public LayerMask shootableLayer;

    [Header("Ammo")]
    public int maxAmmo = 12;
    public int currentAmmo = 12;

    private float nextShootTime = 0f;

    private Camera GetAimCamera()
    {
        return Camera.main;
    }

    public override void Use()
    {
        if (Time.time < nextShootTime)
            return;

        if (currentAmmo <= 0)
            return;

        nextShootTime = Time.time + fireRate;
        currentAmmo--;

        if (gameObject.activeInHierarchy)
            StartCoroutine(RecoilAnimation());

        Camera cam = GetAimCamera();
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        Debug.Log("Gun fired");

        if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                range,
                shootableLayer,
                QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Gun hit: " + hit.collider.name);

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnShot();
            }
        }
    }

    private System.Collections.IEnumerator RecoilAnimation()
    {
        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;

        float recoilDistance = 0.07f;
        float recoilAngle = 25f;
        float kickDuration = 0.05f;
        float returnDuration = 0.28f;

        float elapsed = 0f;

        while (elapsed < kickDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / kickDuration;

            transform.localPosition = Vector3.Lerp(
                originalPosition,
                originalPosition + Vector3.back * recoilDistance,
                t);

            transform.localRotation = Quaternion.Lerp(
                originalRotation,
                originalRotation * Quaternion.Euler(-recoilAngle, 0, 0),
                t);

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;

            transform.localPosition = Vector3.Lerp(
                originalPosition + Vector3.back * recoilDistance,
                originalPosition,
                t);

            transform.localRotation = Quaternion.Lerp(
                originalRotation * Quaternion.Euler(-recoilAngle, 0, 0),
                originalRotation,
                t);

            yield return null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}
