using UnityEngine;
using Unity.Netcode;
using TMPro;

public class LobbySync : NetworkBehaviour
{
    // The Shared List of Players (Synced automatically to all clients)
    public NetworkList<ulong> ConnectedPlayers;

    public static LobbySync Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        // Initialize the list (required for NetworkList)
        ConnectedPlayers = new NetworkList<ulong>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // SERVER SIDE: Manage the list
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            // Add existing (Host)
            foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!ConnectedPlayers.Contains(client))
                    ConnectedPlayers.Add(client);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!ConnectedPlayers.Contains(clientId))
        {
            ConnectedPlayers.Add(clientId);
            Debug.Log($"[LobbySync] Added Player {clientId}");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (ConnectedPlayers.Contains(clientId))
        {
            ConnectedPlayers.Remove(clientId);
            Debug.Log($"[LobbySync] Removed Player {clientId}");
        }
    }
}
