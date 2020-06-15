# Lucca Dépense API

Cette API a été developpée avec .NET Core 3.1
<br>
Elle permet de :
<ul>
<li>Créer des dépenses</li>
<li>Lister des dépenses</li>
</ul>


## Installation de la Base de données

Afin de pouvoir initialiser la base de donnée, il faut construire le projet sql LuccaDatabase.
Dans le répertoire bin de ce projet sera généré un script sql *LuccaDatabase_Create.sql* permettant de créer la base de donnée et de l'initialiser dans une instance de Microsoft SQLServer.

Les 2 utilisateurs Stark Anthony et Romanova Natasha sont ajoutés grace à ce script de création...

## Configuration
Le fichier *appsettings.json* permet de configurer la chaine de connection à la base de donnée.
Si la base de donnée est située sur localhost dans l'instance de Microsoft SQLServer par défaut, il ne sera pas nécessaire de modifier cette ligne.


## Exécution
Pour lancer l'API, il suffit de démarrer Lucca.Expense

Au démarrage, votre navigateur par défaut devrait se lancer sur l'url https://localhost:44341/api/expenses

Accepter le certificat de IIS pour pouvoir utiliser l'API depuis votre navigateur.

Pour utiliser l'API avec Postman, il faut désactiver la vérification des certificats SSL dans les settings de Postman.


## Utilisation de l'API
### Ajouter une dépense
Pour ajouter une dépense, il est nécessaire de faire une requête POST sur l'url https://localhost:44341/api/expenses avec un body contenant le JSON de la dépense à ajouter:

```
{
    "UserId": "C95F9FEB-31A4-46AD-B3AC-6FEC5804863F",
    "PurchasedOn": "2020-06-12T00:07:00",
    "Comment": "Chez papa",
    "Category": "Restaurant",
    "Amount": 10.23,
    "Currency":"RUB"
}
```

En cas d'erreur, un middleware permet de catcher les exceptions de l'API.
Les erreurs de validation sont gérés dans une exception catchée par ce middleware puis transformée par ExpenseProblemDetailsFactory.

Voici par exemple une erreur de validation multiple:
```
{
    "type": "Validation error",
    "title": "Expense validation error",
    "status": 400,
    "detail": "Exception of type 'Lucca.Expense.Services.ExpenseValidationException' was thrown.",
    "DuplicatedExpense": "The expense purchased on 12/06/2020 12:07:00 AM with an amount of 10.23 seems already recorded.",
    "ExpenseCurrencyDifferentThanUserCurrency": "The expense currency EUR should be the same than the user currency RUB",
    "traceId": "|eea54bd2-48596aaddcb172c6."
}
```

### Lister les dépenses
Pour lister les dépenses, il suffit de faire le GET sur l'url https://localhost:44341/api/expenses

Toutes les dépenses de la base seront listées.

Pour lister les dépenses d'un utilisateur, il faut passer l'identifiant de l'utilisateur dans le paramètre *userId*.
<ul>
<li>Natasha Romanova a l'identifiant c95f9feb-31a4-46ad-b3ac-6fec5804863f</li>
<li>Anthony Stark a l'identifiant 32952B48-7D36-48C4-9413-F9E70B002C16</li>
</ul>

Pour trier les dépenses par montant ou par date, il faut préciser le paramètre *sort*.
*sort* peut prendre 2 valeurs:
<ul>
<li>0 pour le tri par montant croissant</li>
<li>1 pour le tri par date croissante</li>
</ul>

### Test unitaire
Les tests unitaires sont dans le répertoire UnitTests.
Seule la validation d'une dépense est testée.

### Choix techniques
Il est pratique de séparer le model exposé par les controleurs de l'API du model de EF d'où DTOModels et EFModels.
Je n'ai pas pris le temps d'utiliser un mapper (ex AutoMapper) pour mapper les 2 types Expenses...


