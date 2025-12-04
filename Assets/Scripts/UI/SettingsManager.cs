using UnityEngine;
using UnityEngine.UI;
using SlimUI.ModernMenu;

public class SettingsManager : MonoBehaviour
{
    private UIMenuManager menuManager;
    private GameObject settingsCanvas;

    void Start()
    {
        settingsCanvas = this.gameObject;
        menuManager = Object.FindFirstObjectByType<UIMenuManager>();
        
        // Start hidden
        settingsCanvas.SetActive(false);
        
        Debug.Log("SettingsManager initialized");
    }

    public void ShowSettings()
    {
        Debug.Log("ShowSettings called!");
        
        settingsCanvas.SetActive(true);
        
        if (menuManager != null)
        {
            // Show Game panel by default
            menuManager.GamePanel();
        }
        
        Debug.Log("Settings canvas activated");
    }

    public void HideSettings()
    {
        Debug.Log("HideSettings called!");
        settingsCanvas.SetActive(false);
    }

    public void ToggleSettings()
    {
        if (settingsCanvas.activeSelf)
        {
            HideSettings();
        }
        else
        {
            ShowSettings();
        }
    }
}
