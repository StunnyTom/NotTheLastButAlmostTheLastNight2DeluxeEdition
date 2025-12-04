using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject titlePanel;   // Le panneau "Appuyer sur une touche"
    public GameObject menuPanel;    // Le panneau avec les boutons Host/Join/Settings
    public GameObject settingsPanel; // Le panneau des paramètres
    public GameObject hostPanel;     // Le panneau pour héberger
    public GameObject joinPanel;     // Le panneau pour rejoindre

    private bool isAtTitle = true;

    void Start()
    {
        // Au démarrage, on s'assure que seul le titre est visible
        ShowTitle();
    }

    void Update()
    {
        // Si on est sur l'écran titre et qu'on appuie sur une touche
        if (isAtTitle && Input.anyKeyDown)
        {
            ShowMenu();
        }
    }

    // Affiche l'écran titre
    public void ShowTitle()
    {
        isAtTitle = true;
        titlePanel.SetActive(true);
        if(menuPanel != null) menuPanel.SetActive(false);
        if(settingsPanel != null) settingsPanel.SetActive(false);
        if(hostPanel != null) hostPanel.SetActive(false);
        if(joinPanel != null) joinPanel.SetActive(false);
    }

    // Affiche le menu principal (Host/Join/Settings)
    public void ShowMenu()
    {
        isAtTitle = false;
        titlePanel.SetActive(false);
        if(menuPanel != null) menuPanel.SetActive(true);
        if(settingsPanel != null) settingsPanel.SetActive(false);
        if(hostPanel != null) hostPanel.SetActive(false);
        if(joinPanel != null) joinPanel.SetActive(false);
    }

    // Affiche les paramètres
    public void ShowSettings()
    {
        if(menuPanel != null) menuPanel.SetActive(false);
        if(settingsPanel != null) settingsPanel.SetActive(true);
        // On cache les autres au cas où
        if(hostPanel != null) hostPanel.SetActive(false);
        if(joinPanel != null) joinPanel.SetActive(false);
    }

    // Retour au menu depuis les paramètres
    public void BackToMenu()
    {
        ShowMenu();
    }

    // Méthode pour le bouton "Héberger" (Host)
    public void OnHostClicked()
    {
        if(menuPanel != null) menuPanel.SetActive(false);
        if(hostPanel != null) hostPanel.SetActive(true);
    }

    // Méthode pour le bouton "Rejoindre" (Join)
    public void OnJoinClicked()
    {
        if(menuPanel != null) menuPanel.SetActive(false);
        if(joinPanel != null) joinPanel.SetActive(true);
    }

    // Méthode pour le bouton "Quitter"
    public void OnQuitClicked()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();
    }

    // Méthode pour le bouton "Play" (Lance le jeu directement)
    public void OnPlayClicked()
    {
        // Assurez-vous que la scène est ajoutée dans File > Build Settings
        SceneManager.LoadScene("The_Viking_Village");
    }
}
