# Guide Complet : Création du Menu et des Scènes

C'est super que tu aies installé **TMP Essentials** (TextMeshPro). C'est l'outil standard d'Unity pour avoir de beaux textes bien nets. C'était nécessaire pour que le menu s'affiche correctement.

Comme tu pars de zéro, voici la marche à suivre complète, étape par étape.

## Étape 1 : Créer les Scènes du Jeu
Un jeu Unity est découpé en "Scènes" (comme les niveaux ou les écrans d'un jeu). Il nous en faut au moins 3 pour commencer.

1.  Va dans le dossier **Assets** -> **Scenes** (dans la fenêtre "Project" en bas).
2.  Fais un clic droit dans le vide -> **Create** -> **Scene**.
3.  Nomme-la **`MainMenu`** (c'est celle où on est actuellement).
4.  Refais pareil pour créer une scène nommée **`Lobby`** (là où les joueurs s'attendront avant de lancer).
5.  Refais pareil pour créer une scène nommée **`Game`** (le jeu lui-même).

👉 **Double-clique sur la scène `MainMenu` pour l'ouvrir et être sûr de travailler dedans.**

## Étape 2 : Générer le Menu (Automatique)
**IMPORTANT : Si tu as déjà un objet "MainMenu_QuantumTek" qui ne marche pas, supprime-le d'abord (Clic droit -> Delete).**

Maintenant que tu es dans la bonne scène (`MainMenu`) et qu'elle est vide :

1.  Regarde la barre de menu tout en haut de Unity.
2.  Clique sur **Tools** -> **Setup Main Menu**.
3.  Le menu va apparaître !

**Ce que l'outil va faire automatiquement :**
*   Mettre ton image de fond sur l'objet "Simple Background".
*   Créer l'effet de brouillard.
*   Configurer le script `MainMenuController` en reliant les fenêtres "Main Window" et en créant des copies pour "Host" et "Join".

## Étape 3 : Vérifier le Script
L'outil a dû ajouter un script `MainMenuController` sur l'objet principal du menu. On va vérifier qu'il est bien configuré.

1.  Dans la **Hierarchy**, clique sur l'objet **`MainMenu_QuantumTek`** (ou `Simple Main Menu`).
2.  Regarde dans l'**Inspector** (à droite).
3.  Cherche le composant **Main Menu Controller (Script)**.
4.  Vérifie que les cases (Title Panel, Menu Panel, etc.) ne sont pas vides.
    *   *Si elles sont vides ou si tu veux ajuster les liens :*
        *   Déploie la flèche à côté de l'objet `MainMenu_QuantumTek` dans la hiérarchie.
        *   Cherche l'objet `Windows`.
        *   Glisse les fenêtres correspondantes dans les cases du script (ex: `Window - Main` dans `Menu Panel`).

## Étape 4 : Tester
1.  Appuie sur le bouton **Play** (le triangle ▶️ en haut au centre).
2.  Le jeu se lance. Tu devrais voir ton écran titre.
3.  Appuie sur une touche -> Le menu apparaît.
4.  Teste les boutons (ils ne feront rien de spécial pour l'instant à part afficher des messages dans la console "Console" en bas à gauche, sauf si on a configuré les panneaux).

## Prochaine étape : Le Multijoueur
Une fois que ton menu est là, on s'attaquera à la connexion entre les joueurs (Héberger / Rejoindre) dans la scène `Lobby`.
