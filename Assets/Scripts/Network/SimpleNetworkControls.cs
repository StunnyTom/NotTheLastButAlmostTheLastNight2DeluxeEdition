using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network.Controls
{
    [RequireComponent(typeof(NetworkManager))]
    public class SimpleNetworkControls : MonoBehaviour
    {
        private NetworkManager m_NetworkManager;
        private UnityTransport m_Transport;

        private void Awake()
        {
            m_NetworkManager = GetComponent<NetworkManager>();
            m_Transport = GetComponent<UnityTransport>();

            // FORCE ASSIGN TRANSPORT if missing
            if (m_NetworkManager.NetworkConfig == null)
                m_NetworkManager.NetworkConfig = new NetworkConfig();

            if (m_NetworkManager.NetworkConfig.NetworkTransport == null && m_Transport != null)
            {
                m_NetworkManager.NetworkConfig.NetworkTransport = m_Transport;
                Debug.Log("SimpleNetworkControls: Auto-assigned UnityTransport to NetworkManager.");
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        private void StartButtons()
        {
            if (GUILayout.Button("Host")) m_NetworkManager.StartHost();
            if (GUILayout.Button("Client")) m_NetworkManager.StartClient();
            if (GUILayout.Button("Server")) m_NetworkManager.StartServer();
        }

        private void StatusLabels()
        {
            var mode = m_NetworkManager.IsHost ? "Host" : m_NetworkManager.IsServer ? "Server" : "Client";
            
            GUILayout.Label("Transport: " + (m_Transport != null ? "UnityTransport" : "Unknown"));
            GUILayout.Label("Mode: " + mode);

            if (GUILayout.Button("Shutdown"))
            {
                m_NetworkManager.Shutdown();
            }
        }
    }
}
