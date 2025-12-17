using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuConnector : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"MainMenuConnector initialized on {gameObject.name}");
    }

    public void LoadLobby()
    {
        Debug.Log("MainMenuConnector: CLICK RECEIVED! Attempting to load LobbyMenu scene...");
        SceneManager.LoadScene("LobbyMenu");
    }
}
