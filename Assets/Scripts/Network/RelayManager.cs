using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignedIn += () =>
                {
                    Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
                };
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            else
            {
                Debug.Log("Already signed in as " + AuthenticationService.Instance.PlayerId);
            }
        }
        catch (System.Exception e)
        {
            // Ignore "already signing in" race conditions
            if (e.Message.Contains("already signing in")) 
            {
                Debug.LogWarning("RelayManager: Sign-in already in progress (Ignored).");
            }
            else
            {
                Debug.LogError($"RelayManager Init Error: {e.Message}");
            }
        }
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            // Max params: 3 (Host + 3 players = 4 total)
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Relay Created! Join Code: {joinCode}");

            // FIX: Ensure UnityTransport exists
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport == null) 
            {
                Debug.LogWarning("[Fix] UnityTransport missing on NetworkManager. Adding it dynamically.");
                transport = NetworkManager.Singleton.gameObject.AddComponent<UnityTransport>();
            }

            // FIX: Ensure NetworkConfig has the transport assigned
            NetworkManager.Singleton.NetworkConfig.NetworkTransport = transport;

            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public async Task<bool> JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log($"Joining Relay with code: {joinCode}");
            
            // FIX: Ensure UnityTransport exists
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport == null) 
            {
                transport = NetworkManager.Singleton.gameObject.AddComponent<UnityTransport>();
            }
            NetworkManager.Singleton.NetworkConfig.NetworkTransport = transport;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            return true;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }
}
