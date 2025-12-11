using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Inventory")]
    public int maxItems = 4;

    private List<UsableItem> items = new List<UsableItem>();
    private int selectedIndex = 0;

    // Permet d’empêcher AddItem de changer la sélection
    private bool maintainSelection = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //──────────────────────────────────────────────────────────────
    // ADD ITEM
    //──────────────────────────────────────────────────────────────
    public int AddItem(UsableItem item)
    {
        if (items.Count >= maxItems)
            return -1;

        items.Add(item);
        int index = items.Count - 1;

        // Ne change la sélection QUE si rien n’empêche
        if (!maintainSelection)
            selectedIndex = index;

        // Reset du flag
        maintainSelection = false;

        return index;
    }

    //──────────────────────────────────────────────────────────────
    // REMOVE SELECTED ITEM
    //──────────────────────────────────────────────────────────────
    public UsableItem RemoveSelectedItem()
    {
        if (items.Count == 0)
            return null;

        UsableItem removed = items[selectedIndex];
        items.RemoveAt(selectedIndex);

        selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, items.Count - 1));
        return removed;
    }

    //──────────────────────────────────────────────────────────────
    public UsableItem GetSelectedItem()
    {
        if (items.Count == 0) return null;
        return items[selectedIndex];
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

    //──────────────────────────────────────────────────────────────
    // Empêche la sélection de changer à l’ajout d’un item
    //──────────────────────────────────────────────────────────────
    public void ForceKeepCurrentSelection()
    {
        maintainSelection = true;
    }
}
