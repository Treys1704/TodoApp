# TodoApp Backend

API REST construite avec **.NET 8 Web API**, **Entity Framework Core** et **SQLite**.

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- `dotnet-ef` CLI (pour gérer les migrations manuellement si besoin)

```bash
dotnet tool install --global dotnet-ef --version "8.0.*"
```

## Structure du projet

```
Backend/
├── TodoApp.Api/
│   ├── Controllers/        # Endpoints REST
│   ├── Models/             # Entité TodoItem + DTOs
│   ├── Data/               # DbContext (EF Core)
│   ├── Services/           # Logique métier (ITodoService / TodoService)
│   ├── Migrations/         # Migrations EF Core
│   ├── Program.cs          # Point d'entrée + configuration
│   └── appsettings.json    # Connection string SQLite
│
└── TodoApp.Api.Tests/
    ├── Controllers/        # Tests unitaires du controller (Moq)
    ├── Services/           # Tests unitaires du service (EF InMemory)
    └── Helpers/            # Factory DbContext pour les tests
```

## Lancer le projet

```bash
cd Backend/TodoApp.Api
dotnet run
```

L'API démarre sur **http://localhost:5272** et les migrations sont appliquées automatiquement au démarrage.

Swagger UI est disponible en mode Development : **http://localhost:5272/swagger**

## Lancer les tests

```bash
cd Backend/TodoApp.Api.Tests
dotnet test
```

33 tests unitaires couvrent le service et le controller.

## Stack technique

| Composant       | Technologie                               |
| --------------- | ----------------------------------------- |
| Framework       | .NET 8 Web API                            |
| ORM             | Entity Framework Core 8                   |
| Base de données | SQLite                                    |
| Documentation   | Swagger                                   |
| Tests           | xUnit, Moq, FluentAssertions, EF InMemory |

---

## API Endpoints

Base URL : `http://localhost:5272/api/todo`

---

### 1. Lister toutes les tâches

**GET** `/api/todo`

#### Réponse `200 OK`

```json
[
  {
    "id": 1,
    "title": "Ma tâche",
    "isCompleted": false,
    "createdAt": "2026-03-26T05:00:00Z"
  }
]
```

Les tâches sont triées par date de création décroissante (les plus récentes en premier).

---

### 2. Récupérer une tâche par ID

**GET** `/api/todo/{id}`

#### Réponse `200 OK`

```json
{
  "id": 1,
  "title": "Ma tâche",
  "isCompleted": false,
  "createdAt": "2026-03-26T05:00:00Z"
}
```

#### Réponse `404 Not Found`

Retourné si l'ID n'existe pas.

---

### 3. Créer une tâche

**POST** `/api/todo`

#### Body (JSON)

```json
{
  "title": "Nouvelle tâche"
}
```

| Champ   | Type   | Requis | Contraintes   |
| ------- | ------ | ------ | ------------- |
| `title` | string | oui    | max 200 chars |

#### Réponse `201 Created`

```json
{
  "id": 3,
  "title": "Nouvelle tâche",
  "isCompleted": false,
  "createdAt": "2026-03-26T05:10:00Z"
}
```

Header `Location` contient l'URL de la ressource créée.

#### Réponse `400 Bad Request`

Retourné si le body est invalide (titre manquant ou trop long).

---

### 4. Modifier une tâche

**PUT** `/api/todo/{id}`

#### Body (JSON)

```json
{
  "title": "Titre modifié"
}
```

| Champ   | Type   | Requis | Contraintes   |
| ------- | ------ | ------ | ------------- |
| `title` | string | oui    | max 200 chars |

#### Réponse `200 OK`

```json
{
  "id": 1,
  "title": "Titre modifié",
  "isCompleted": false,
  "createdAt": "2026-03-26T05:00:00Z"
}
```

#### Réponse `404 Not Found`

Retourné si l'ID n'existe pas.

#### Réponse `400 Bad Request`

Retourné si le body est invalide.

---

### 5. Compléter une tâche

**PUT** `/api/todo/{id}/complete`

Aucun body requis.

#### Réponse `200 OK`

```json
{
  "id": 1,
  "title": "Ma tâche",
  "isCompleted": true,
  "createdAt": "2026-03-26T05:00:00Z"
}
```

#### Réponse `404 Not Found`

Retourné si l'ID n'existe pas.

---

### 6. Supprimer une tâche

**DELETE** `/api/todo/{id}`

#### Réponse `204 No Content`

La tâche a été supprimée.

#### Réponse `404 Not Found`

Retourné si l'ID n'existe pas.

---

## Modèle de données

### TodoItem (entité)

| Champ         | Type     | Description               |
| ------------- | -------- | ------------------------- |
| `Id`          | int      | Clé primaire auto-générée |
| `Title`       | string   | Titre (requis, max 200)   |
| `IsCompleted` | bool     | Statut de complétion      |
| `CreatedAt`   | DateTime | Date de création (UTC)    |

### Codes de réponse HTTP

| Code | Signification                         |
| ---- | ------------------------------------- |
| 200  | Succès                                |
| 201  | Ressource créée                       |
| 204  | Suppression réussie (pas de contenu)  |
| 400  | Requête invalide (validation échouée) |
| 404  | Ressource non trouvée                 |
