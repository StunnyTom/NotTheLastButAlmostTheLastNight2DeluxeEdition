using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class InspectButtonStructure : Editor
{
    [MenuItem("Tools/Debugging/Inspect 'Play' Button Structure")]
    public static void Inspect()
    {
        Debug.Log("--- INSPECTING BUTTON STRUCTURE ---");

        var btn = GameObject.Find("Btn_PlayCampaign");
        if (btn == null)
        {
            Debug.LogError("Could not find 'Btn_PlayCampaign'!");
            return;
        }

        Debug.Log($"Button Root: {btn.name} | Layer: {LayerMask.LayerToName(btn.layer)} | Active: {btn.activeInHierarchy}");
        
        // Check Image
        var img = btn.GetComponent<Image>();
        if (img) Debug.Log($"[ROOT IMAGE] Color: {img.color} | Material: {img.material.name}");

        // Iterate Children
        int index = 0;
        foreach (Transform child in btn.transform)
        {
            Debug.Log($"   [{index}] Child: {child.name} | Active: {child.gameObject.activeSelf} | Z: {child.localPosition.z}");
            
            var t = child.GetComponent<Text>();
            if (t) Debug.Log($"      -> COMPONENT: Legacy Text | Color: {t.color} | Font: {t.font.name} | Mat: {t.material.name}");

            var tmp = child.GetComponent<TextMeshProUGUI>();
            if (tmp) Debug.Log($"      -> COMPONENT: TMP Text | Color: {tmp.color} | Font: {tmp.font.name} | Mat: {tmp.fontMaterial.name} | Shader: {tmp.fontMaterial.shader.name}");
            
            index++;
        }
    }
}
