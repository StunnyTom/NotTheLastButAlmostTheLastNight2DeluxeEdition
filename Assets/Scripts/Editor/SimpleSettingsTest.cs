using UnityEngine;

namespace DebugTests
{
    public class SimpleSettingsTest : MonoBehaviour
    {
        private float testVolume = 0.5f;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(320, 10, 300, 300));
            GUILayout.Label("--- SETTINGS TEST ---");

            GUILayout.Label($"Current Music Vol: {PlayerPrefs.GetFloat("MusicVolume", -1)}");
            GUILayout.Label($"Current SFX Vol: {PlayerPrefs.GetFloat("SFXVolume", -1)}");

            GUILayout.Space(10);
            
            GUILayout.Label("Set Test Volume:");
            testVolume = GUILayout.HorizontalSlider(testVolume, 0f, 1f);
            
            if (GUILayout.Button("Save Volume to 1.0 (Max)"))
            {
                PlayerPrefs.SetFloat("MusicVolume", 1.0f);
                PlayerPrefs.SetFloat("SFXVolume", 1.0f);
                PlayerPrefs.Save();
                Debug.Log("Saved Volume to 1.0");
            }

            if (GUILayout.Button("Save Volume to 0.0 (Mute)"))
            {
                PlayerPrefs.SetFloat("MusicVolume", 0.0f);
                PlayerPrefs.SetFloat("SFXVolume", 0.0f);
                PlayerPrefs.Save();
                Debug.Log("Saved Volume to 0.0");
            }
            
            if (GUILayout.Button($"Save Slider ({testVolume:F2})"))
            {
                PlayerPrefs.SetFloat("MusicVolume", testVolume);
                PlayerPrefs.SetFloat("SFXVolume", testVolume);
                PlayerPrefs.Save();
            }

            if (GUILayout.Button("Delete All Prefs"))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("Deleted All PlayerPrefs");
            }

            GUILayout.EndArea();
        }
    }
}
