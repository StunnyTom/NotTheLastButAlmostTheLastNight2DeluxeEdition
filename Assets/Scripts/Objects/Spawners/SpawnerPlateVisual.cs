using UnityEngine;

public class SpawnerPlateVisual : MonoBehaviour
{
    [Header("References")]
    public ObjectSpawnerPlate plate;
    public Renderer plateRenderer;
    public SpriteRenderer itemSpriteRenderer;

    [Header("Colors")]
    public Color lockedColor = Color.gray;
    public Color readyColor = Color.green;
    public Color cooldownColor = Color.red;

    private void Start()
    {
        UpdateSprite();
        Refresh();
    }

    private void Update()
    {
        Refresh();
        UpdateSprite();
    }

    void Refresh()
    {
        if (!plate.IsUnlocked)
        {
            plateRenderer.material.color = lockedColor;
            itemSpriteRenderer.enabled = false;
            return;
        }

        // Débloqué
        itemSpriteRenderer.enabled = true;

        if (plate.IsReady)
        {
            plateRenderer.material.color = readyColor;
        }
        else
        {
            plateRenderer.material.color = cooldownColor;
        }
    }

    void UpdateSprite()
    {
        if (plate.itemPrefab == null) return;

        var icon = plate.itemPrefab.GetComponent<ItemIcon>();
        if (icon != null)
            itemSpriteRenderer.sprite = icon.GetIcon();
    }
}
