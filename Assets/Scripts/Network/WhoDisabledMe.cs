using UnityEngine;
using System;

public class WhoDisabledMe : MonoBehaviour
{
    private void OnDisable()
    {
        string msg = $"[{DateTime.Now:HH:mm:ss}] [WhoDisabledMe] '{name}' was DISABLED! Stack Trace:\n{Environment.StackTrace}\n\n";
        
        // Write to Console (still useful)
        Debug.LogError(msg);
        
        // Write to File
        try
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "who_disabled_me.txt");
            System.IO.File.AppendAllText(path, msg);
        }
        catch { /* Best effort */ }
    }
}
