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
Il suffit de renseigner le montant de la commande ainsi que le nombre d'exemplaires pour créer une nouvelle commande.<br>
La date de commande est automatiquement la date du jour de saisie de la commande et le numéro de commande est généré à partir des numéros de commandes présents dans la base de données. 
#### Modification d'un suivi
Pour modifier le suivi d'une commande, il suffit de :<br>
- la sélectionner,<br>
- choisir son nouveau suivi dans la liste déroulante,<br>
- cliquer sur "Modifier le suivi".<br>
<br>
Une commande est toujours "en cours" lorsqu'elle est créée et l'application empêche certaines modifications.<br>
Les règles applicables à la modification du suivi et à la suppression d'une commande sont rappelées au survol de l'icône d'aide.<br>
#### Suppression d'une commande
Pour supprimer une commande, il suffit de :<br>
- la sélectionner,<br>
- cliquer sur "Supprimer la commande".<br>
<br>
Le bouton n'est actif que si la suppression est possible pour cette commande.<br>
### Onglet 6 : Commandes DVD
Cette fois, on entre le numéro du DVD.<br>
![comdvd](https://github.com/user-attachments/assets/7fc91d80-f8c9-4c29-84dd-56f05fa84e76)
Le fonctionnement est identique à l'onglet de commande des livres.<br>
La seule différence réside dans certaines informations détaillées, spécifiques aux DVD : durée (à la place de ISBN), réalisateur (à la place de l'auteur), synopsis (à la place de collection).
### Onglet 7 : Commandes revues
Cete fois, on entre le numéro de la revue.<br>
![comrevues](https://github.com/user-attachments/assets/2c56ff92-d2a9-4152-9ae5-77010392c646)
Le fonctionnement est légérement différent.<br>
#### Nouvelle commande
Une nouvelle commande correspond à un nouvel abonnement.<br>
Il faut préciser :<br>
- le montant de l'abonnement,<br>
- la date de fin d'abonnement.<br>
<br>
Encore une fois la date de commande et le numéro de la commande sont générés automatiquement.<br>
### Suppression d'une commande
Ici, il n'y a pas de suivi de commande à modifier, on ne peut que supprimer une commande (ou abonnement).<br>
Si des exemplaires restent rattachés à un abonnement, la suppression sera impossible et un message l'indiquera. 
### Onglet 4 : Parutions des revues
Cet onglet permet d'enregistrer la réception de nouvelles parutions d'une revue.<br>
Il se décompose en 2 parties (groupbox).
## La base de données
La base de données 'mediatek86 ' est au format MySQL.<br>
Voici sa structure :<br>
![img4](https://github.com/CNED-SLAM/MediaTekDocuments/assets/100127886/4314f083-ec8b-4d27-9746-fecd1387d77b)
<br>On distingue les documents "génériques" (ce sont les entités Document, Revue, Livres-DVD, Livre et DVD) des documents "physiques" qui sont les exemplaires de livres ou de DVD, ou bien les numéros d’une revue ou d’un journal.<br>
Chaque exemplaire est numéroté à l’intérieur du document correspondant, et a donc un identifiant relatif. Cet identifiant est réel : ce n'est pas un numéro automatique. <br>
Un exemplaire est caractérisé par :<br>
. un état d’usure, les différents états étant mémorisés dans la table Etat ;<br>
. sa date d’achat ou de parution dans le cas d’une revue ;<br>
. un lien vers le fichier contenant sa photo de couverture de l'exemplaire, renseigné uniquement pour les exemplaires des revues, donc les parutions (chemin complet) ;
<br>
Un document a un titre (titre de livre, titre de DVD ou titre de la revue), concerne une catégorie de public, possède un genre et est entreposé dans un rayon défini. Les genres, les catégories de public et les rayons sont gérés dans la base de données. Un document possède aussi une image dont le chemin complet est mémorisé. Même les revues peuvent avoir une image générique, en plus des photos liées à chaque exemplaire (parution).<br>
Une revue est un document, d’où le lien de spécialisation entre les 2 entités. Une revue est donc identifiée par son numéro de document. Elle a une périodicité (quotidien, hebdomadaire, etc.) et un délai de mise à disposition (temps pendant lequel chaque exemplaire est laissé en consultation). Chaque parution (exemplaire) d'une revue n'est disponible qu'en un seul "exemplaire".<br>
Un livre a aussi pour identifiant son numéro de document, possède un code ISBN, un auteur et peut faire partie d’une collection. Les auteurs et les collections ne sont pas gérés dans des tables séparées (ce sont de simples champs textes dans la table Livre).<br>
De même, un DVD est aussi identifié par son numéro de document, et possède un synopsis, un réalisateur et une durée. Les réalisateurs ne sont pas gérés dans une table séparée (c’est un simple champ texte dans la table DVD).
Enfin, 3 tables permettent de mémoriser les données concernant les commandes de livres ou DVD et les abonnements. Une commande est effectuée à une date pour un certain montant. Un abonnement est une commande qui a pour propriété complémentaire la date de fin de l’abonnement : il concerne une revue.  Une commande de livre ou DVD a comme caractéristique le nombre d’exemplaires commandé et concerne donc un livre ou un DVD.<br>
<br>
La base de données est remplie de quelques exemples pour pouvoir tester son application. Dans les champs image (de Document) et photo (de Exemplaire) doit normalement se trouver le chemin complet vers l'image correspondante. Pour les tests, vous devrez créer un dossier, le remplir de quelques images et mettre directement les chemins dans certains tuples de la base de données qui, pour le moment, ne contient aucune image.<br>
Lorsque l'application sera opérationnelle, c'est le personnel de la médiathèque qui sera en charge de saisir les informations des documents.
## L'API REST
L'accès à la BDD se fait à travers une API REST protégée par une authentification basique.<br>
Le code de l'API se trouve ici :<br>
https://github.com/CNED-SLAM/rest_mediatekdocuments<br>
avec toutes les explications pour l'utiliser (dans le readme).
## Installation de l'application
Ce mode opératoire permet d'installer l'application pour pouvoir travailler dessus.<br>
- Installer Visual Studio 2019 entreprise et les extension Specflow et newtonsoft.json (pour ce dernier, voir l'article "Accéder à une API REST à partir d'une application C#" dans le wiki de ce dépôt : consulter juste le début pour la configuration, car la suite permet de comprendre le code existant).<br>
- Télécharger le code et le dézipper puis renommer le dossier en "mediatekdocuments".<br>
- Récupérer et installer l'API REST nécessaire (https://github.com/CNED-SLAM/rest_mediatekdocuments) ainsi que la base de données (les explications sont données dans le readme correspondant).
