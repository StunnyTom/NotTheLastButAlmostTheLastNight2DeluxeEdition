# Architecture & Stratégie des Compétences

Ce document explique les concepts avancés d'assignation des compétences pour les Designers et les Développeurs.

## Deux Approches : Héritage vs Composition

Nous utilisons deux méthodes différentes selon le type de personnage (Monstre vs Survivant) pour maximiser la flexibilité.

### 1. Le Monstre (Approche "Intégrée")
Le Monstre est unique et possède des mécaniques très spécifiques (Rage).
*   **Architecture** : Le script `MonsterController` **EST** aussi le Gestionnaire de Compétences (`MonsterController : CharacterSkillManager`).
*   **Avantage** : Permet de coder des règles strictes surchargerables (ex: "Je ne peux utiliser l'Ultime que si ma Rage est à 100").
*   **Comment l'utiliser** : Sur le Prefab du Monstre, les compétences sont directement dans le composant `MonsterController`.

### 2. Les Survivants (Approche "Modulaire")
Les survivants sont multiples et peuvent avoir des "Classes" (Medic, Soldat, Eclaireur) sans qu'on ait besoin de recréer un script de contrôleur à chaque fois.
*   **Architecture** : Le `SurvivorController` **A** une référence vers un `CharacterSkillManager`. Ce sont deux composants séparés côte à côte.
*   **Avantage** : Vous pouvez créer plusieurs Prefabs de survivants (ex: `Survivor_Medic`, `Survivor_Soldier`) qui ont tous le même `SurvivorController`, mais une liste de compétences différente dans leur composant `CharacterSkillManager`.
*   **Comment l'utiliser** :
    1.  Ajoutez le composant `CharacterSkillManager` sur votre Prefab de Survivant.
    2.  Remplissez la liste `Skills` avec les compétences de cette classe (ex: Soin, Sprint).
    3.  Le `SurvivorController` va automatiquement détecter ce composant et l'utiliser.

---

## Créer des Classes (ex: Medic, Scout)

Grace à l'approche modulaire, créer une nouvelle "Classe" de joueur est très simple :

1.  **Dupliquez le Prefab** du Survivant de base.
2.  Renommez-le (ex: `Survivor_Medic`).
3.  Allez sur son composant **Character Skill Manager**.
4.  Changez les compétences dans la liste :
    *   Remplacez "Sprint" par "Soin de Zone" (SkillData).
    *   Changez l'icone.
    *   Changez la fonction appelée dans **On Execute** (ex: faites glisser un script `HealManager` et sélectionnez `HealAround`).

Vous avez maintenant un nouveau personnage jouable avec ses propres sorts, sans aucune ligne de code !

---

## Résumé pour les Designers

| Situation                                 | Méthode          | Où modifier les compétences ?                                      |
| :---------------------------------------- | :--------------- | :----------------------------------------------------------------- |
| **Je veux modifier les sorts du Monstre** | Intégrée         | Directement sur le `MonsterController`.                            |
| **Je veux créer un Survivant "Medic"**    | Modulaire        | Sur le composant `CharacterSkillManager` à côté du contrôleur.     |
| **Je veux que le sort "Rage" change**     | ScriptableObject | Modifiez le fichier `SkillData` du sort (dans le dossier Project). |

---

## Intégration HUD

Peu importe la méthode (Intégrée ou Modulaire), le système de HUD (`HUDManager`) fonctionne exactement de la même manière : il demande au personnage "Donne-moi ta liste de compétences" et affiche les icônes correspondantes. Vous n'avez rien à faire de spécial pour le HUD.
