using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeDisplay; // To show the code when hosting
    public Button createSessionBtn;
    public Button joinSessionBtn;
    public Button copyCodeBtn;
    public Button leaveSessionBtn; // Optional
    
    [Header("Panels")]
    public GameObject lobbyPanel; 
    public GameObject joinSection; // New reference for the Join "Row"

    private void Start()
    {
        // Auto-assign references if logic allows, but better to force assignment via Editor Tool
        if (createSessionBtn) createSessionBtn.onClick.AddListener(OnCreateSessionClicked);
        if (joinSessionBtn) joinSessionBtn.onClick.AddListener(OnJoinSessionClicked);
        if (copyCodeBtn) copyCodeBtn.onClick.AddListener(OnCopyCodeClicked);
        if (leaveSessionBtn) leaveSessionBtn.onClick.AddListener(OnLeaveSessionClicked);
    }

    public async void OnCreateSessionClicked()
    {
        Debug.Log("UI: Create Session Clicked");
        if (RelayManager.Instance == null) { Debug.LogError("RelayManager missing!"); return; }

        createSessionBtn.interactable = false;
        
        // HIDE JOIN INTERFACE
        if (joinSection) joinSection.SetActive(false);
        
        string code = await RelayManager.Instance.CreateRelay();
        
        if (!string.IsNullOrEmpty(code))
        {
            if (joinCodeDisplay) joinCodeDisplay.text = code;
            if (joinCodeInput) joinCodeInput.text = code;
            
            // UX UPDATE: Do NOT load immediately. Wait for Host to click Start.
            Debug.Log("Relay Created. Waiting for Host to Start Game.");
            
            // Transform "Create Session" button into "Start Game"
            createSessionBtn.interactable = true;
            var btnText = createSessionBtn.GetComponentInChildren<TMP_Text>();
            if (btnText) btnText.text = "START GAME";
            
            // UX UPDATE: Disable Join Button and Input to prevent cross-actions
            if (joinSessionBtn) joinSessionBtn.interactable = false;
            if (joinCodeInput) joinCodeInput.interactable = false;

            createSessionBtn.onClick.RemoveAllListeners();
            createSessionBtn.onClick.AddListener(LoadGameScene);

            // FORCE REFRESH FOR HOST
            RefreshPlayerList();
        }
        else
        {
            createSessionBtn.interactable = true;
        }
    }

    public async void OnJoinSessionClicked()
    {
        Debug.Log("UI: Join Session Clicked");
        if (RelayManager.Instance == null) { Debug.LogError("RelayManager missing!"); return; }

        string code = joinCodeInput.text;
        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Please enter a Join Code");
            return;
        }

        joinSessionBtn.interactable = false;
        bool success = await RelayManager.Instance.JoinRelay(code);
        if (!success)
        {
            joinSessionBtn.interactable = true;
            Debug.LogError("Failed to join relay.");
        }
    }

    private void OnCopyCodeClicked()
    {
        if (joinCodeDisplay)
        {
            GUIUtility.systemCopyBuffer = joinCodeDisplay.text;
            Debug.Log("Code copied to clipboard: " + joinCodeDisplay.text);
        }
    }
    
    private void OnLeaveSessionClicked()
    {
        // Simple disconnect
        if (Unity.Netcode.NetworkManager.Singleton)
        {
             Unity.Netcode.NetworkManager.Singleton.Shutdown();
             // Reload Menu?
             UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    [Header("Player List Configuration")]
    public Transform playerListContent;  // Assign 'Content' from ScrollView
    public GameObject playerListEntryPrefab; // Assign the prefab

    private void OnEnable()
    {
        if (Unity.Netcode.NetworkManager.Singleton)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDisable()
    {
        if (Unity.Netcode.NetworkManager.Singleton)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[LobbyUI] OnClientConnected: {clientId}");
        RefreshPlayerList();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[LobbyUI] OnClientDisconnected: {clientId}");
        RefreshPlayerList();
    }

    public void RefreshPlayerList()
    {
        if (!playerListContent) 
        {
             Debug.LogError("ERROR: 'Player List Content' is NOT assigned in Inspector!");
             return;
        }
        if (!playerListEntryPrefab) 
        {
             Debug.LogError("ERROR: 'Player List Entry Prefab' is NOT assigned in Inspector!");
             return;
        }

        // Clear existing
        foreach(Transform child in playerListContent) Destroy(child.gameObject);

        var nm = Unity.Netcode.NetworkManager.Singleton;
        if (nm)
        {
            Debug.Log($"[LobbyUI] Refreshing List. ConnectedClients: {nm.ConnectedClientsList.Count}");
            foreach(var client in nm.ConnectedClientsList)
            {
                var entry = Instantiate(playerListEntryPrefab, playerListContent);
                var txt = entry.GetComponentInChildren<TMP_Text>();
                if (txt)
                {
                    // FIX: ServerClientId is a static const
                    string role = (client.ClientId == Unity.Netcode.NetworkManager.ServerClientId) ? "[HOST]" : "[PLAYER]";
                    txt.text = $"{role} Player {client.ClientId}";
                }
            }
        }
    }

    // ... (rest of the file)

    private void LoadGameScene()
    {
        var nm = Unity.Netcode.NetworkManager.Singleton;
        if (nm && nm.IsHost)
        {
            nm.SceneManager.LoadScene("The_Viking_Village", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
