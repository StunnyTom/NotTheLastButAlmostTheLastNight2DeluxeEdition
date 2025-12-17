using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class LobbyDebugHelp : MonoBehaviour
{
    private TextMeshProUGUI debugText;
    private string logBuffer = "";
    private string filePath;
    
    void Start()
    {
        // Setup Log File
        filePath = System.IO.Path.Combine(Application.persistentDataPath, "lobby_debug_logs.txt");
        try
        {
            System.IO.File.WriteAllText(filePath, $"--- START LOG {System.DateTime.Now} ---\n");
        }
        catch { Debug.LogError("Could not write to log file!"); }

        // Create a massive text overlay on top of everything
        GameObject canvasObj = new GameObject("DebugCanvas");
        Canvas c = canvasObj.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 9999;
        
        GameObject textObj = new GameObject("DebugText");
        textObj.transform.SetParent(canvasObj.transform, false);
        
        debugText = textObj.AddComponent<TextMeshProUGUI>();
        debugText.font = Resources.Load<TMP_FontAsset>("LiberationSans SDF");
        debugText.fontSize = 20;
        debugText.color = Color.black; // Changed to Black for visibility on white/grey BG? Or Red? Sticking to Red but with bg.
        debugText.color = Color.red;
        debugText.alignment = TextAlignmentOptions.TopLeft;
        debugText.raycastTarget = false; 
        
        // Add a background panel for readability
        GameObject bgPanel = new GameObject("DebugBG");
        bgPanel.transform.SetParent(canvasObj.transform, false);
        bgPanel.transform.SetAsFirstSibling();
        Image img = bgPanel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.5f);
        RectTransform rtBg = bgPanel.GetComponent<RectTransform>();
        rtBg.anchorMin = Vector2.zero;
        rtBg.anchorMax = Vector2.one;
        
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(20, 20);
        rt.offsetMax = new Vector2(-20, -20);
        
        Application.logMessageReceived += HandleLog;
        DontDestroyOnLoad(canvasObj);
        
        Debug.Log($"LOGS WRITTEN TO: {filePath}");
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // File Log
        try
        {
            string line = $"[{System.DateTime.Now:HH:mm:ss}] [{type}] {logString}\n";
            if (type == LogType.Exception || type == LogType.Error) line += stackTrace + "\n";
            System.IO.File.AppendAllText(filePath, line);
        }
        catch { }

        // Screen Log
        // Screen Log DISABLED
        /*
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
        {
            if (logBuffer.Length > 1500) logBuffer = logBuffer.Substring(0, 1500);
            logBuffer = $"[{type}] {logString}\n" + logBuffer;
            UpdateDisplay();
        }
        */
    }

    void Update()
    {
        // Refresh occasionally? No, event based is fine.
        // Update keys?
    }
    
    void UpdateDisplay()
    {
        if (debugText == null) return;
        
        string netState = "Not Running";
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost) netState = "HOST";
            else if (NetworkManager.Singleton.IsClient) netState = NetworkManager.Singleton.IsConnectedClient ? "CLIENT (Connected)" : "CLIENT (Connecting...)";
            else netState = "Offline";
        }
        
        debugText.text = $"LOG FILE: {filePath}\nNET: {netState}\n----------------\n{logBuffer}";
    }
}
