using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using SlimUI.ModernMenu;

public class FixMenuText : EditorWindow
{
    [MenuItem("Tools/Menu Support/Fix Text Visibility")]
    public static void FixVisibility()
    {
        var manager = Object.FindFirstObjectByType<UIMenuManager>();
        if (manager == null) return;

        GameObject menuRoot = manager.mainCanvas != null ? manager.mainCanvas : manager.gameObject;

        var texts = menuRoot.GetComponentsInChildren<Text>(true);
        var tmpTexts = menuRoot.GetComponentsInChildren<TMP_Text>(true);

        int count = 0;
        foreach (var t in texts)
        {
            Undo.RecordObject(t, "Fix Text");
            t.color = Color.white; // Force White
            if (t.gameObject.activeSelf == false) t.gameObject.SetActive(true);
            
            // Fix Z-Fighting (Move text slightly forward)
            var rect = t.GetComponent<RectTransform>();
            if (rect)
            {
                Vector3 pos = rect.localPosition;
                pos.z = -0.05f; // Pull slightly forward (not -5 which clips!)
                rect.localPosition = pos;
            }
            count++;
        }
        foreach (var t in tmpTexts)
        {
             Undo.RecordObject(t, "Fix Text");
             t.color = Color.white; // Force White
             t.alpha = 1f;
             if (t.gameObject.activeSelf == false) t.gameObject.SetActive(true);

            // Fix Z-Fighting (Move text slightly forward)
            var rect = t.GetComponent<RectTransform>();
            if (rect)
            {
                Vector3 pos = rect.localPosition;
                pos.z = -0.05f; // Pull slightly forward
                rect.localPosition = pos;
            }
             count++;
        }

        Debug.Log($"Fixed visibility for {count} text elements.");
        EditorUtility.DisplayDialog("Success", $"Fixed visibility for {count} text elements.", "OK");
    }
}
