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
        selectedIndex = items.Count - 1;
        Debug.Log("Added item: " + item.itemName);
        return selectedIndex;
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
}
