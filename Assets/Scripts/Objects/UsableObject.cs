using UnityEngine;

public class UsableItem : MonoBehaviour
{
    public string itemName;
    public Sprite icon;

    public virtual void Use()
    {
        Debug.Log("Used item: " + itemName);
    }
}
