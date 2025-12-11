using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;

    public void LoadScene()
    {
        // Charge la nouvelle scène et décharge l'actuelle
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        // Pas besoin de décharger explicitement, LoadSceneMode.Single le fait automatiquement
    }
}