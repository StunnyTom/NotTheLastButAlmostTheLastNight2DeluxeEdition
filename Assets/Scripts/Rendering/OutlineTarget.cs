using UnityEngine;

public class OutlineTarget : MonoBehaviour
{
    public bool isOutlined = false;

    public void SetOutlined(bool state)
    {
        gameObject.layer = state ? LayerMask.NameToLayer("Usable") 
                                 : LayerMask.NameToLayer("Default");
        isOutlined = state;
        //Debug.Log("OutlineTarget: " + gameObject.name + " outlined state set to " + state);
    }
}
