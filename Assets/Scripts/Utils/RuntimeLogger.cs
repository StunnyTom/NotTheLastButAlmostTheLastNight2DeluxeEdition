using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class RuntimeLogger : MonoBehaviour
{
    private string logPath;

    void Awake()
    {
        string persistentPath = Application.persistentDataPath;
        logPath = Path.Combine(persistentPath, "game_debug_log.txt");

        // Try to clear/init the log file
        try 
        { 
            File.WriteAllText(logPath, $"--- GAME LOG START {System.DateTime.Now} ---\n"); 
        } 
        catch (System.Exception e)
        {
            // If we can't write, we can't log.
            Debug.LogError($"RuntimeLogger Init Failed: {e.Message}");
        }

        Application.logMessageReceived += HandleLog;
        
        // Log explicitly to the file that we started
        HandleLog($"LOG FILE PATH: {logPath}", "", LogType.Warning);
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (string.IsNullOrEmpty(logPath)) return;

        try
        {
            string header = $"[{System.DateTime.Now:HH:mm:ss}] [{type}] ";
            string fileLine = header + logString;
            
            // Append to file
            File.AppendAllText(logPath, fileLine + "\n");
             
            // If error, append stack trace too
            if (type == LogType.Error || type == LogType.Exception)
            {
               File.AppendAllText(logPath, stackTrace + "\n");
            }
        }
        catch 
        {
            // Fail silently to avoid infinite error loops
        }
    }
}
