# Guide : Gestion des Personnages Multijoueur

Ce guide explique comment le système charge le personnage du joueur (Prefab) et comment le modifier ou en ajouter de nouveaux.

## 1. Comment ça marche ? (NetworkManager)

Dans un jeu multijoueur Unity (Netcode for GameObjects), c'est le **NetworkManager** qui décide quel Prefab faire apparaître pour chaque joueur lorsqu'il se connecte.

### Le "Player Prefab" par défaut
1. Ouvrez la scène (n'importe laquelle contenant le NetworkManager, souvent `MainMenu` ou `LobbyMenu`).
2. Sélectionnez l'objet **NetworkManager**.
3. Dans l'inspecteur, cherchez le champ **Player Prefab**.
   - C'est cet objet qui est instancié automatiquement (si "Auto Create Player" est coché).
   - Actuellement, cela pointe probablement vers `NetworkSurvivor`.

## 2. Liste des Prefabs (NetworkPrefabs)
Pour qu'un objet puisse être "spawn" (apparaître) via le réseau, il **DOIT** être enregistré dans la liste du NetworkManager.

1. Sélectionnez **NetworkManager**.
2. Ouvrez la liste **NetworkPrefabs**.
3. Vous y verrez tous les objets qui peuvent apparaître en jeu (Projectiles, Items, Monstres, et **Personnages**).

## 3. Structure du Personnage (`NetworkSurvivor`)

Le Prefab utilisé (`NetworkSurvivor`) est souvent une "coquille" vide ou un conteneur qui possède :
- Un component `NetworkObject` (Obligatoire).
- Un script de contrôle (`SurvivorController`).
- Un modèle 3D visuel (Enfant).

### Changer le visuel
Si vous voulez changer l'apparence (ex: passer de "Capsule" à "Viking") :
1. Ouvrez le Prefab `Assets/Prefabs/Network/NetworkSurvivor.prefab`.
2. Désactivez ou supprimez l'ancien modèle (ex: `DebugCapsule`).
3. Glissez votre nouveau modèle 3D à l'intérieur.
4. Assurez-vous qu'il y a un `Animator` si vous voulez des animations.
5. Sauvegardez le Prefab.

## 4. Gérer plusieurs personnages (Skins)

Si vous voulez que le Joueur 1 soit un "Guerrier" et le Joueur 2 un "Mage", vous ne pouvez pas utiliser la case "Player Prefab" par défaut (car elle est unique).

**Solution : Spawning Manuel**
1. Décochez "Auto Create Player" dans le NetworkManager.
2. Écrivez un script qui se branche sur l'événement `OnClientConnected`.
3. Dans ce script, instanciez manuellement le bon Prefab selon le choix du joueur.
4. Faites `objet.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);`.

Exemple :
```csharp
void OnClientConnected(ulong clientId) {
    GameObject prefab = (clientId == 0) ? warriorPrefab : magePrefab;
    GameObject instance = Instantiate(prefab);
    instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
}
```

## Résumé pour votre projet actuel
- Le fichier utilisé est : `Assets/Prefabs/Network/NetworkSurvivor.prefab`.
- Pour retirer le cylindre gris : Ouvrez ce fichier et désactivez le MeshRenderer du cylindre ("DebugCapsule").
- Un outil automatique a été créé pour le faire : **Tools > Antigravity Kit > Visuals > 19. HIDE Player Debug Cylinder**.
