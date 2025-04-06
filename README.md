# MediatekDocuments
Les fonctionnalités de l'application d'origine sont détaillées dans le readme du dépôt d'origine.<br>
Vous pouvez consulter ce dépot à l'adresse : https://github.com/CNED-SLAM/MediaTekDocuments.git<br>
Vous trouverez ci-dessous les détails des éléments ajoutés à cette application.
## Nouvelles fonctionnalités
La nouvelle application permet maintenant de gérer la commande de documents à travers 3 nouveaux onglets
## Les nouveaux onglets
### Onglet 5 : Commandes livres
Dans cet onglet, la recherche se fait avec le numéro du livre.<br>
On retrouve alors l'ensemble des informations du livre, comme dans l'onglet 1.<br>
![comlivres](https://github.com/user-attachments/assets/b7320711-2284-4a0d-8589-dffe83df63b3)
#### Commandes en cours
Le tableau indique toutes les commandes passées avec possibilité de tri, croissant ou décroissant sur chaque colonne.<br>
#### Nouvelle commande
Il suffit de renseigner le montant de la commande ainsi que le nombre d'exemplaires pour créer une nouvelle commande.
La date de commande est automatiquement la date du jour de saisie de la commande et le numéro de commande est généré à partir des numéros de commandes présents dans la base de données.
#### Modification d'un suivi
Pour modifier le suivi d'une commande, il suffit de :
- la sélectionner,
- choisir son nouveau suivi dans la liste déroulante,
- cliquer sur <strong>Modifier le suivi</strong>.

Une commande est toujours <strong>en cours</strong> lorsqu'elle est créée et l'application empêche certaines modifications.
Les règles applicables à la modification du suivi et à la suppression d'une commande sont rappelées au survol de l'icône d'aide.
#### Suppression d'une commande
Pour supprimer une commande, il suffit de :
- la sélectionner,
- cliquer sur <strong>Supprimer la commande</strong>.

Le bouton n'est actif que si la suppression est possible pour cette commande.
### Onglet 6 : Commandes DVD
Cette fois, on entre le numéro du DVD.<br>
![comdvd](https://github.com/user-attachments/assets/51e9f0a6-9423-4d98-a90f-24dddeac01e3)
<br>Le fonctionnement est identique à l'onglet de commande des livres.<br>
La seule différence réside dans certaines informations détaillées, spécifiques aux DVD : durée (à la place de ISBN), réalisateur (à la place de l'auteur), synopsis (à la place de collection).
### Onglet 7 : Commandes revues
Cete fois, on entre le numéro de la revue.<br>
![comrevues](https://github.com/user-attachments/assets/81474691-e35a-45fd-bf9d-5e8303e366b5)
<br>Le fonctionnement est légérement différent.
#### Nouvelle commande
Une nouvelle commande correspond à un nouvel abonnement.<br>
Il faut préciser :
- le montant de l'abonnement,
- la date de fin d'abonnement.

Encore une fois la date de commande et le numéro de la commande sont générés automatiquement.
#### Suppression d'une commande
Ici, il n'y a pas de suivi de commande à modifier, on ne peut que supprimer une commande (ou abonnement).
Si des exemplaires restent rattachés à un abonnement, la suppression sera impossible et un message l'indiquera. 
#### Suivi des abonnements
Lorsqu'un responsable de la gestion des commandes se connecte à l'application, il reçoit une alerte sur les abonnement qui arrivent à échéance :<br>
![alerteabonnements](https://github.com/user-attachments/assets/ae8aed05-0d00-463f-91f2-3c5cf25774fa)
<br>Il peut ainsi commander un nouvel abonnement, si le réseau des médiathèques désire continuer à recevoir ces revues. 
## Utilisation de l'application
Il suffit de télécharger l'installeur Setup.msi qui se trouve à la racine de ce dépôt et de le lancer.<br>
Les identifiants nécessaires pour se connnecter à l'application sont fournis aux enseignants qui doivent l'évaluer.
