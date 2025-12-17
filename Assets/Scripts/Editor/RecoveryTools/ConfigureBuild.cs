using UnityEngine;
using UnityEditor;

namespace Antigravity.Recovery
{
    public class ConfigureBuild
    {
        [MenuItem("Tools/Antigravity Kit/Build/23. CONFIGURE Fullscreen")]
        public static void SetFullscreen()
        {
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow; // Borderless Windowed (best compatibility)
            // PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen; // Alternative if they want exclusive
            
            PlayerSettings.defaultIsNativeResolution = true;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.allowFullscreenSwitch = true;
            PlayerSettings.runInBackground = true;
            
            // Force resolution (optional fallback)
            // PlayerSettings.defaultScreenWidth = 1920;
            // PlayerSettings.defaultScreenHeight = 1080;

            Debug.Log("Build Settings Updated: Fullscreen Enabled (Native Resolution).");
        }
    }
}
