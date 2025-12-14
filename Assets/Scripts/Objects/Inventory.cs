using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Inventory")]
    public int maxItems = 4;

    private List<UsableItem> items = new List<UsableItem>();
    private int selectedIndex = 0;

    private bool maintainSelection = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool HasFreeSlot()
    {
        return items.Count < maxItems;
    }

    /// <summary>
    /// Utilisé par les spawners : tente d'ajouter sans casser l'état visuel
    /// </summary>
    public bool TryAddFromSpawner(UsableItem item)
    {
        if (!HasFreeSlot())
            return false;

        // On empêche toute modification de sélection
        maintainSelection = true;

        items.Add(item);

        // L'objet est rangé, pas équipé
        item.gameObject.SetActive(false);
        item.transform.SetParent(null);

        return true;
    }

    public int AddItem(UsableItem item)
    {
        if (items.Count >= maxItems)
            return -1;

        items.Add(item);
        int index = items.Count - 1;

        if (!maintainSelection)
            selectedIndex = index;

        maintainSelection = false;
        return index;
    }

    public UsableItem GetSelectedItem()
    {
        if (items.Count == 0) return null;
        return items[selectedIndex];
    }

    public UsableItem RemoveSelectedItem()
    {
        if (items.Count == 0)
            return null;

        UsableItem removed = items[selectedIndex];
        items.RemoveAt(selectedIndex);

        selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, items.Count - 1));
        return removed;
    }

    public UsableItem NextItem()
    {
        if (items.Count == 0) return null;
        selectedIndex = (selectedIndex + 1) % items.Count;
        return items[selectedIndex];
    }

    public UsableItem PreviousItem()
    {
        if (items.Count == 0) return null;
        selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;
        return items[selectedIndex];
    }

    public void ForceKeepCurrentSelection()
    {
        maintainSelection = true;
    }
}
