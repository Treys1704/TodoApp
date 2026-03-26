# TodoApp

Application de gestion de tâches complète avec un backend API REST et une interface utilisateur moderne.

## Aperçu

TodoApp permet de **créer**, **lister**, **compléter**, **éditer** et **supprimer** des tâches via une interface épurée et réactive, connectée à une API .NET.

## Stack technique

| Couche   | Technologies                                                              |
| -------- | ------------------------------------------------------------------------- |
| Backend  | .NET 8 Web API, Entity Framework Core 8, SQLite                           |
| Frontend | React 19, Vite 8, Tailwind CSS v4, Framer Motion, Zustand, TanStack Query |
| Tests    | xUnit, Moq, FluentAssertions, EF Core InMemory                            |

## Structure du projet

```
TodoApp/
├── Backend/
│   ├── TodoApp.Api/              # API REST (.NET 8)
│   │   ├── Controllers/          # Endpoints REST
│   │   ├── Services/             # Logique métier (interface + implémentation)
│   │   ├── Models/               # Entité TodoItem + DTOs
│   │   ├── Data/                 # DbContext EF Core
│   │   ├── Migrations/           # Migrations EF Core (SQLite)
│   │   └── Program.cs            # Point d'entrée + configuration
│   │
│   ├── TodoApp.Api.Tests/        # Tests unitaires (33 tests)
│   │   ├── Services/             # Tests du service (EF InMemory)
│   │   ├── Controllers/          # Tests du controller (Moq)
│   │   └── Helpers/              # Factory DbContext pour les tests
│   │
│   └── README.md                 # Documentation backend + API
│
├── Frontend/
│   └── TodoApp-frontend/         # Application React
│       ├── src/
│       │   ├── api/              # Couche Axios (appels HTTP)
│       │   ├── hooks/            # Hooks TanStack Query
│       │   ├── store/            # Store Zustand (état UI)
│       │   └── components/       # Composants React
│       │
│       └── README.md             # Documentation frontend
│
└── README.md                     # Ce fichier
```

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) >= 18

## Démarrage rapide

### 1. Lancer le backend

```bash
cd Backend/TodoApp.Api
dotnet run
```

L'API démarre sur **http://localhost:5272**. La base de données SQLite est créée et migrée automatiquement au premier lancement.

Swagger UI : **http://localhost:5272/swagger**

### 2. Lancer le frontend

```bash
cd Frontend/TodoApp-frontend
npm install
npm run dev
```

L'interface est accessible sur **http://localhost:5173**.

### 3. Lancer les tests

```bash
cd Backend/TodoApp.Api.Tests
dotnet test
```

## Fonctionnalités

| Fonctionnalité | Description                                     |
| -------------- | ----------------------------------------------- |
| Lister         | Affichage de toutes les tâches, triées par date |
| Créer          | Ajout rapide via formulaire avec validation     |
| Compléter      | Marquage d'une tâche comme terminée             |
| Éditer         | Modification du titre en mode inline            |
| Supprimer      | Suppression individuelle d'une tâche            |
| Filtrer        | Vue par statut (Toutes / Actives / Terminées)   |

## API Endpoints

| Méthode  | Route                     | Description              |
| -------- | ------------------------- | ------------------------ |
| `GET`    | `/api/todo`               | Lister toutes les tâches |
| `GET`    | `/api/todo/{id}`          | Récupérer une tâche      |
| `POST`   | `/api/todo`               | Créer une tâche          |
| `PUT`    | `/api/todo/{id}`          | Modifier le titre        |
| `PUT`    | `/api/todo/{id}/complete` | Compléter une tâche      |
| `DELETE` | `/api/todo/{id}`          | Supprimer une tâche      |

> Consulter `Backend/README.md` pour la documentation complète de chaque endpoint (body, réponses, codes HTTP).

---

## Futures évolutions

L'application actuelle couvre les opérations CRUD essentielles sur une liste de tâches plate. Plusieurs axes d'évolution permettraient d'enrichir significativement ses fonctionnalités :

### Tâches hiérarchiques (récursivité)

Introduire la notion de **sous-tâches** en ajoutant une relation parent-enfant sur le modèle `TodoItem`. Chaque tâche pourrait contenir des sous-tâches, elles-mêmes pouvant en contenir d'autres, formant ainsi une **structure en arbre (tree)** de profondeur illimitée. Côté frontend, cela se traduirait par un affichage imbriqué avec indentation, possibilité de replier/déplier les niveaux, et navigation dans l'arborescence.

### Cascade de complétion

Lorsqu'une tâche parente est marquée comme complétée, **toutes ses sous-tâches sont automatiquement complétées** de manière récursive, quel que soit le niveau de profondeur. Cela garantit la cohérence de l'état de l'arbre : une tâche ne peut pas être considérée comme terminée si des sous-tâches restent actives.

### Cascade de suppression

La suppression d'une tâche parente entraînerait la **suppression automatique de toutes ses sous-tâches** et de leurs descendants. Cette suppression en cascade serait gérée au niveau de la base de données (via la configuration EF Core `OnDelete(DeleteBehavior.Cascade)`) pour garantir l'intégrité référentielle.

### Autres pistes

- **Authentification** : gestion multi-utilisateurs avec des listes de tâches privées
- **Drag & drop** : réorganisation manuelle de l'ordre des tâches
- **Dates d'échéance** : ajout d'une deadline avec indicateur visuel de retard
- **Catégories / tags** : organisation des tâches par thématique
