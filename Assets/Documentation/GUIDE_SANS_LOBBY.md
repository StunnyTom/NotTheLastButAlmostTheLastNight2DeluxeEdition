# Démarrer le Jeu SANS le MainMenu (Lobby)

Si vous supprimez ou n'utilisez plus la scène `MainLobby`, vous devez configurer la scène de jeu (`The_Viking_Village`) pour qu'elle puisse gérer le réseau toute seule.

Voici la procédure à suivre :

## 1. Ajouter le NetworkManager
Le `NetworkManager` est l'objet qui gère la connexion. Normalement, il venait du Lobby (`DontDestroyOnLoad`). Si le Lobby n'est plus là, il faut l'ajouter directement dans le jeu.

1.  Ouvrez la scène **`The_Viking_Village`**.
2.  Cherchez votre Prefab **`NetworkManager`** (dans le dossier `Assets/Prefabs/Network` ou similaire, sinon créez-en un).
3.  Glissez-le dans la scène.

## 2. Remplacer les Boutons (Interface de Connexion)
Sans le Lobby, vous n'avez plus les boutons "Host" et "Join". Vous avez deux options :

### Option A : Utiliser le HUD par défaut de Unity (Le plus rapide pour tester)
1.  Sélectionnez l'objet `NetworkManager` dans la scène.
2.  Cliquez sur **Add Component** dans l'Inspecteur.
3.  Cherchez et ajoutez **`NetworkManagerHUD`** (c'est un script intégré à Unity Netcode).
4.  **Lancez le jeu (Play)**.
    *   Vous verrez des petits boutons gris en haut à gauche ("Start Host", "Start Client").

### Option B : Script de Démarrage Automatique (Auto-Host)
Si vous voulez que le jeu se lance tout seul sans cliquer :
1.  Créez un script nommé `AutoHost.cs`.
2.  Collez ce code dedans :
    ```csharp
    using UnityEngine;
    using Unity.Netcode;

    public class AutoHost : MonoBehaviour
    {
        void Start()
        {
            if (Application.isEditor)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                // En Build, on peut choisir ou laisser le HUD
                NetworkManager.Singleton.StartClient(); 
            }
        }
    }
    ```
3.  Attachez ce script à votre `NetworkManager`.

## 3. Le SpawnPoint
Comme d'habitude, assurez-vous que l'objet vide **`SpawnPoint`** existe toujours dans la scène, à l'endroit où vous voulez apparaître.

---

## Résumé des Scripts Utiles en mode "Sans Lobby"

*   **`NetworkManager`** (Objet) : Obligatoire dans la scène.
*   **`NetworkManagerHUD`** (Composant) : Pour avoir les boutons de connexion temporaires.
*   **`NetworkSpawnOffset.cs`** (Sur le joueur) : Continue de fonctionner normalement pour vous placer au SpawnPoint.
