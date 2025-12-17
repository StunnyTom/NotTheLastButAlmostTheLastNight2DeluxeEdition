# REFAIRE LE MENU DE ZERO (Procédure Propre)

Si votre scène MainMenu est cassée, suivez ces étapes pour en créer une toute neuve et fonctionnelle.

## 1. Création de la Scène
1.  Dans Unity, allez dans **File > New Scene**.
2.  Choisissez "Basic (Built-in)" ou "Empty".
3.  Sauvegardez la scène tout de suite (`Ctrl+S`) sous le nom : `MainMenu_Reboot`.

## 2. Génération du Menu (Script Automatique)
J'ai déjà codé un outil qui fait tout le travail (interface, fond d'écran 4K, boutons).
1.  Allez dans le menu du haut : **Tools > Setup SlimUI Menu**.
2.  Une fenêtre va s'ouvrir pour dire "Success". Cliquez OK.

*À ce stade, vous devriez voir le menu et le fond d'écran `bg_menu_principal`.*

## 3. Ajout du NetworkManager
Le menu a besoin du gestionnaire réseau pour fonctionner.
1.  Cherchez votre Prefab **`NetworkManager`** dans le dossier `Assets`.
    *   *(Astuce : Tapez "NetworkManager" dans la barre de recherche du bas "Project").*
2.  Glissez ce Prefab dans la scène (dans la liste de gauche "Hierarchy").
3.  Assurez-vous qu'il est bien présent.

## 4. Nettoyage (Camera)
Le Prefab "SlimUI" et la scène de base ont peut-être chacun une caméra.
1.  Vérifiez s'il y a **deux** "Main Camera".
2.  S'il y en a deux, supprimez celle de base (gardez celle qui est propre au Menu ou celle qui vous semble la mieux placée, mais souvent pour un Menu Overlay, on n'a besoin que d'une seule caméra simple).
    *   *Note : Mon script force le Menu en "Overlay", donc il s'affiche par-dessus n'importe quelle caméra.*

## 5. Test
1.  Lancez le jeu (Play).
2.  Cliquez sur **PLAY > HOST**.
3.  Ça devrait charger la scène de jeu (`The_Viking_Village`).

---
**Si l'affichage est mauvais (trop gros/petit) :**
Mon script de génération force une résolution 4K (3840x2160). Si ça ne va pas :
1.  Sélectionnez l'objet racine du menu (`MainMenu_SlimUI` ou `Canvas`).
2.  Dans l'inspecteur, cherchez **Canvas Scaler**.
3.  Changez "Reference Resolution" à **1920 x 1080**.
