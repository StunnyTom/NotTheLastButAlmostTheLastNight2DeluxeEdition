# Guide du Système de Compétences

## Introduction
Ce module permet d'ajouter très facilement des compétences à n'importe quel personnage (Monstre ou Survivant) et de lier ces compétences à l'interface (HUD) automatiquement.

Il repose sur 3 éléments :
1.  **SkillData (ScriptableObject)** : La fiche d'identité de la compétence (Nom, Icône, description, cooldown par défaut).
2.  **CharacterSkillManager** : Le composant à placer sur le personnage qui contient la liste des compétences actives.
3.  **L'Inspector Unity** : Où vous ferez les liens (Drag & Drop).

---

## 1. Créer une Nouvelle Compétence

Pour définir qu'une compétence "exist", vous devez créer un fichier de données pour elle.

1.  Allez dans la fenêtre **Project**.
2.  Faites **Clic Droit** -> **Create** -> **Game** -> **Skill Data**.
3.  Donnez un nom au fichier (ex: `Skill_Roar`).
4.  Sélectionnez le fichier et configurez l'Inspector :
    *   **Skill Name** : Le nom affiché (ex: "Rugissement").
    *   **Icon** : L'image qui apparaîtra dans le HUD.
    *   **Default Cooldown** : Le temps d'attente en secondes.
    *   **Description** : (Optionnel) Pour info.

---

## 2. Assigner une Compétence à un Personnage

Pour qu'un personnage puisse utiliser cette compétence :

1.  Sélectionnez votre personnage dans la scène (ex: `Monster`).
2.  Assurez-vous qu'il possède le composant `MonsterController` (qui hérite maintenant de `CharacterSkillManager`).
3.  Cherchez la section **Skills Configuration** (Liste `Skills`).
4.  Cliquez sur **+** pour ajouter une entrée.
5.  **Remplissez les champs** :
    *   **Skill Data** : Glissez le fichier `Skill_Roar` créé à l'étape 1.
    *   **Key** : Choisissez la touche du clavier (ex: `A` pour Azerty).
    *   **On Execute ()** : C'est ici que la magie opère. Cliquez sur **+**.
        *   Glissez l'objet qui contient le script de logique (ex: Le `MonsterController` lui-même).
        *   Choisissez la fonction à appeler (ex: `MonsterController.TriggerAttack` ou `MonsterRageSystem.AddRage`).

---

## 3. Le Cas Spécial : La Rage du Monstre

Le système de Rage nécessite une logique spécifique (la barre doit être pleine).
C'est déjà codé dans `MonsterController` !

Pour configurer l'Ultime :
1.  Ajoutez une nouvelle compétence dans la liste du Monstre.
2.  Mettez la touche **R**.
3.  Dans **Skill Data**, créez/assignez `Skill_Ultimate`.
4.  Dans **On Execute ()**, glissez l'objet `Monster` (ou celui qui a le script `MonsterRageSystem`).
5.  Choisissez la fonction `MonsterRageSystem.TryUseUltimate`.

> **Note** : Le code vérifie automatiquement si la Rage est pleine avant de lancer l'action. Si elle n'est pas pleine, rien ne se passe (et le cooldown ne se lance pas).

---

## 4. Setup du HUD (Interface)

Pour que les cases s'affichent :
1.  Il doit y avoir un objet avec le script `HUDManager` dans la scène (attaché à un Canvas).
2.  Assignez le Prefab `HUDItem` dans le champ **Hud Item Prefab** du Manager.
3.  Assignez un conteneur (ex: un Panel avec `HorizontalLayoutGroup`) dans **Items Container**.
4.  Au lancement du jeu (`Start`), le Monstre va automatiquement dire au HUD "Hé, voici mes compétences, affiche-les !".

## En Résumé

*   **Graphistes / GD** : Créez des `SkillData` et dessinez les icônes.
*   **Intégrateurs** : Glissez les `SkillData` sur le Monstre et liez les touches.
*   **Programmeurs** : Si vous voulez une nouvelle mécanique (ex: "Voler"), créez juste la fonction `public void StartFlying() {...}` n'importe où, et liez-la dans l'Inspector via l'event **On Execute**. Pas besoin de toucher au système de HUD !
