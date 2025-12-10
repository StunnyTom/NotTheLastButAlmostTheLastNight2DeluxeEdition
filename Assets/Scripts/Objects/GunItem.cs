using UnityEngine;

// Simple pistol implementation using raycast
public class GunItem : UsableItem
{
    [Header("Gun Settings")]
    public int damage = 25;
    public float range = 50f;
    public float fireRate = 0.25f; // seconds between shots

    [Header("Ammo")]
    public int maxAmmo = 12;
    public int currentAmmo = 12;

    // Aim origin and animation use this object's transform

    private float nextShootTime = 0f;

    private Camera GetAimCamera()
    {
        // Prefer the player camera if available; fallback to main
        Camera cam = Camera.main;
        return cam;
    }

    public override void Use()
    {
        if (Time.time < nextShootTime)
            return;

        if (currentAmmo <= 0)
        {
            return;
        }

        nextShootTime = Time.time + fireRate;
        currentAmmo = Mathf.Max(0, currentAmmo - 1);

        // Play shoot animation (recoil-like)
        ShootAnimation();

        Camera cam = GetAimCamera();
        Vector3 origin = (cam != null) ? cam.transform.position : transform.position;
        Vector3 direction = (cam != null) ? cam.transform.forward : transform.forward;

        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            // Try to apply damage if target supports it
            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            else
            {
                // Optional: add rigidbody impact
                if (hit.rigidbody != null)
                    hit.rigidbody.AddForceAtPosition(direction * 5f, hit.point, ForceMode.Impulse);
            }
        }
    }

    private void ShootAnimation()
    {
        // Slight recoil animation on this object's transform
        StartCoroutine(RecoilAnimation());
    }

        private System.Collections.IEnumerator RecoilAnimation()
        {
        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;

        // Emphasize upward rotation, reduce positional lift
        float recoilDistance = 0.07f;   // minimal backward push
        float recoilAngle = 25f;        // stronger upward tilt
        float kickDuration = 0.05f;     // quick kick
        float returnDuration = 0.28f;   // slower return

        float elapsed = 0f;

        // Recoil back and up
        while (elapsed < kickDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / kickDuration;

            transform.localPosition = Vector3.Lerp(originalPosition, originalPosition + Vector3.back * recoilDistance, t);
            transform.localRotation = Quaternion.Lerp(originalRotation, originalRotation * Quaternion.Euler(-recoilAngle, 0, 0), t);

            yield return null;
        }

        elapsed = 0f;

        // Return to original position slowly
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;

            transform.localPosition = Vector3.Lerp(originalPosition + Vector3.back * recoilDistance, originalPosition, t);
            transform.localRotation = Quaternion.Lerp(originalRotation * Quaternion.Euler(-recoilAngle, 0, 0), originalRotation, t);

            yield return null;
        }

        // Ensure exact return to original
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}

// Define a simple damageable interface if none exists
public interface IDamageable
{
    void TakeDamage(int amount);
}
