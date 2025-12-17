using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System.Net;
using System.Net.Sockets;

public class NetworkUIHelper : MonoBehaviour
{
    public TMP_Text hostIpDisplay;
    public TMP_InputField joinIpInput;

    void Start()
    {
        if (hostIpDisplay != null)
        {
            hostIpDisplay.text = "Host IP: " + GetLocalIPAddress();
        }
    }

    public void StartHostVoid()
    {
        if (NetworkManager.Singleton != null)
        {
            // Ensure we bind to the correct IP? Usually 0.0.0.0 is fine for hosting to accept all
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host Started via Helper");
            
            // Load the Game Scene
            // NOTE: The scene must be in Build Settings!
            if (NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("The_Viking_Village", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("NetworkSceneManager is null. Cannot load game scene.");
            }
        }
        else
        {
            Debug.LogError("NetworkManager Singleton is null!");
        }
    }

    public void StartClientVoid()
    {
        if (NetworkManager.Singleton != null)
        {
            // Set IP from Input Field
            if (joinIpInput != null && !string.IsNullOrEmpty(joinIpInput.text))
            {
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                if (transport != null)
                {
                    transport.SetConnectionData(joinIpInput.text, 7777); // Default port 7777
                    Debug.Log($"Setting Transport IP to: {joinIpInput.text}");
                }
            }
            
            NetworkManager.Singleton.StartClient();
             Debug.Log("Client Started via Helper");
        }
        else
        {
             Debug.LogError("NetworkManager Singleton is null!");
        }
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }
}
