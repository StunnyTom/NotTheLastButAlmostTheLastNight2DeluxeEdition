using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private List<UsableItem> items = new List<UsableItem>();
    private int selectedIndex = 0;
    public int maxItems = 4;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int AddItem(UsableItem item)
    {
        if (items.Count >= maxItems) return -1;
        items.Add(item);
        Debug.Log("Added item: " + item.itemName);
        // Ne pas modifier l'item sélectionné actuel
        return items.Count - 1;
    }

    public void RemoveSelectedItem()
    {
        if (items.Count == 0) return;
        items.RemoveAt(selectedIndex);
        selectedIndex = Mathf.Clamp(selectedIndex, 0, items.Count - 1);
    }

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
}
