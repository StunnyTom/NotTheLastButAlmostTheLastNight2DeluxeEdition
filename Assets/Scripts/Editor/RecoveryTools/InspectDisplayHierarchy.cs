using UnityEngine;
using UnityEditor;
using TMPro;

public class InspectDisplayHierarchy : Editor
{
    [MenuItem("Tools/Antigravity Kit/Recovery/Inspect 'Show Session Code'")]
    public static void Inspect()
    {
        Debug.Log("--- Inspecting Display Hierarchy ---");
        // Find by name "Show Session Code"
        var obj = GameObject.Find("Show Session Code");
        if (obj == null)
        {
            // Try partial
             var textObjs = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
             foreach(var t in textObjs)
             {
                 if (t.name.Contains("Show Session"))
                 {
                     obj = t.gameObject;
                     break;
                 }
             }
        }

        if (obj == null) 
        {
            Debug.LogError("Could not find 'Show Session Code'.");
            return;
        }

        Debug.Log($"Found Parent: {obj.name}");
        foreach(Transform child in obj.transform)
        {
            Debug.Log($" - Child: {child.name}");
            var text = child.GetComponent<TMP_Text>();
            var input = child.GetComponent<TMP_InputField>();
            
            if (text) Debug.Log($"   -> Has TMP_Text (Text: '{text.text}')");
            if (input) Debug.Log($"   -> Has TMP_InputField");
        }
    }
}
