using UnityEngine;

public class AmmoSpawnerPlateVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AmmoSpawnerPlate plate;
    [SerializeField] private Renderer plateRenderer;
    [SerializeField] private SpriteRenderer ammoSprite;

    [Header("Sprite")]
    [SerializeField] private Sprite ammoIcon;

    [Header("Colors")]
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color cooldownColor = Color.red;

    private void Start()
    {
        if (ammoSprite != null && ammoIcon != null)
            ammoSprite.sprite = ammoIcon;

        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (plate == null || plateRenderer == null)
            return;

        if (!plate.IsUnlocked)
        {
            plateRenderer.material.color = lockedColor;
            SetSprite(false);
            return;
        }

        if (plate.IsReady)
        {
            plateRenderer.material.color = readyColor;
            SetSprite(true);
        }
        else
        {
            plateRenderer.material.color = cooldownColor;
            SetSprite(true);
        }
    }

    private void SetSprite(bool visible)
    {
        if (ammoSprite == null) return;
        ammoSprite.enabled = visible;
    }

}
