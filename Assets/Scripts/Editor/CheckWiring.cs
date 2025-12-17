using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CheckWiring : Editor
{
    [MenuItem("Tools/Debug/Check Button Wiring")]
    public static void Check()
    {
        var btns = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach(var b in btns)
        {
            Debug.Log($"Button '{b.name}': {b.onClick.GetPersistentEventCount()} listeners.");
            for(int i=0; i<b.onClick.GetPersistentEventCount(); i++)
            {
                var target = b.onClick.GetPersistentTarget(i);
                var method = b.onClick.GetPersistentMethodName(i);
                Debug.Log($"   -> Target: {target?.name}, Method: {method}");
            }
        }
    }
}
