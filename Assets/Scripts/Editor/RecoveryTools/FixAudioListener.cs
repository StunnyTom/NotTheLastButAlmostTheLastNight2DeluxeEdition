using UnityEngine;
using UnityEditor;
using System.Linq;

namespace EditorTools
{
    public class FixAudioListener : EditorWindow
    {
        [MenuItem("Tools/Antigravity Kit/Recovery/Fix Audio Listeners")]
        public static void FixListeners()
        {
            var listeners = Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            if (listeners.Length <= 1)
            {
                Debug.Log($"AudioListeners OK: Found {listeners.Length}.");
                return;
            }

            Debug.LogWarning($"Found {listeners.Length} AudioListeners. Cleaning up...");
            
            // Prefer keeping the one on "MainCamera"
            AudioListener keeper = null;
            foreach(var l in listeners)
            {
                if (l.CompareTag("MainCamera") || l.name.Equals("Main Camera") || l.name.Equals("Camera"))
                {
                    keeper = l;
                    break;
                }
            }

            // Fallback: Keep the first one
            if (keeper == null) keeper = listeners[0];

            int count = 0;
            foreach(var l in listeners)
            {
                if (l != keeper)
                {
                    Undo.DestroyObjectImmediate(l);
                    count++;
                }
            }
            
            Debug.Log($"Removed {count} extra AudioListeners. Kept on '{keeper.name}'.");
        }
    }
}
