# Manuel de Récupération du Projet

Ce document recense tous les scripts critiques modifiés ou créés pour faire fonctionner le multijoueur, le spawn et l'interface.

## 1. Ordre des Opérations (Setup from Scratch)

Si vous devez tout refaire, suivez cet ordre précis :

### Étape 1 : Préparation des Personnages
1.  Ouvrez Unity.
2.  Allez dans le menu du haut : `Tools > Recovery > Setup Network Characters`.
    *   **Ce que ça fait** : Modifie le Prefab du joueur (`Survivor`).
    *   Ajoute `ClientNetworkTransform` (pour bouger).
    *   Ajoute `NetworkSpawnOffset` (pour apparaître au bon endroit).
    *   Ajoute `SurvivorController` (si manquant) et configure les vitesses.
    *   Nettoie les anciens scripts (`SimpleNetworkMovement`, `LobbySafePlayer`).

### Étape 2 : Correction de l'Interface (Menus)
1.  Ouvrez la scène `MainMenu` (ou Lobby).
2.  Allez dans le menu du haut : `Tools > UI > Fix Canvas Scale`.
    *   **Ce que ça fait** : Force tous les écrans à s'adapter à la résolution (fini les boutons coupés).
3.  **Sauvegardez la scène** (`Ctrl+S`).

### Étape 3 : Placement du Spawn
1.  Ouvrez la scène de jeu (`The_Viking_Village`).
2.  Créez un **Empty GameObject** nommé exactement : `SpawnPoint`.
3.  Placez-le près du bateau (ou là où les joueurs doivent commencer).
4.  **Sauvegardez la scène**.

### Étape 4 : Test
1.  Revenez à la scène `MainMenu`.
2.  Faites **File > Build and Run**.

---

## 2. Inventaire des Scripts Critiques

Voici les fichiers clés qui font tourner la logique actuelle.

### A. Réseau & Spawn (Dossier `Scripts/Network`)

| Script                          | Rôle                                                                                                                                                                                                                                                      |
| :------------------------------ | :-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **`NetworkSpawnOffset.cs`**     | **CRITIQUE**. Gère l'apparition du joueur. <br>1. Attend que la scène de Jeu charge.<br>2. Trouve l'objet `SpawnPoint`.<br>3. Téléporte le joueur.<br>4. **Moniteur de Sécurité** : Si le joueur tombe dans le vide (bug), il le respawn automatiquement. |
| **`ClientNetworkTransform.cs`** | Permet au joueur de bouger de manière fluide (Client Authoritative). Sans lui, le joueur est paralysé ou saccadé.                                                                                                                                         |
| **`DiagnoseSpawning.cs`**       | "Le Nettoyeur". Vérifie en permanence s'il y a des "Zombies" (joueurs dupliqués non contrôlés) et les détruit. Évite l'effet miroir fantôme.                                                                                                              |

### B. Contrôle Joueur (Dossier `Characters/Survivor/Scripts`)

| Script                      | Rôle                                                                                                                                                                                                                                                                       |
| :-------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **`SurvivorController.cs`** | Cerveau du joueur. <br>1. Gère les mouvements (Marche/Course/Saut).<br>2. **Caméra** : S'auto-initialise en TPS/FPS au démarrage (plus besoin d'appuyer sur C).<br>3. **Anti-Conflit Menu** : Désactive ses caméras quand il est dans le Lobby pour ne pas cacher le menu. |

### C. Outils Éditeur (Dossier `Scripts/Editor`)

Ces scripts ne se lancent pas en jeu, mais servent à configurer le projet via la barre de menu `Tools`.

| Script                          | Menu Unity                    | Rôle                                                                               |
| :------------------------------ | :---------------------------- | :--------------------------------------------------------------------------------- |
| **`SetupNetworkCharacters.cs`** | `Tools > Recovery > Setup...` | Configure automatiquement le Prefab du joueur avec les bons composants réseau.     |
| **`ForceUIFix.cs`**             | `Tools > UI > Fix Canvas...`  | Répare l'échelle des menus (Canvas Scaler) pour qu'ls soient lisibles en 1080p/4K. |

---

## 3. Dépannage Rapide

*   **Le joueur est invisible ?** -> Vérifiez qu'il y a bien un `SpawnPoint` dans la scène de jeu. Regardez les logs (`game_debug_log.txt`) pour voir si le `SpawnMonitor` crie.
*   **Le menu est tout petit ou énorme ?** -> Lancez `Tools > UI > Fix Canvas Scale` dans la scène du menu et sauvegardez.
*   **Je ne peux pas bouger ?** -> Relancez `Tools > Recovery > Setup Network Characters`, le `ClientNetworkTransform` a peut-être sauté.
