# TodoApp Frontend

Interface utilisateur construite avec **React 19**, **Vite**, **Tailwind CSS v4** et animée avec **Framer Motion**.

## Prérequis

- [Node.js](https://nodejs.org/) >= 18

## Lancer le projet

```bash
cd Frontend/TodoApp-frontend
npm install
npm run dev
```

L'application démarre sur **http://localhost:5173**.

> Le backend doit être démarré sur `http://localhost:5272` pour que les appels API fonctionnent.

## Scripts disponibles

| Commande          | Description                    |
| ----------------- | ------------------------------ |
| `npm run dev`     | Serveur de développement (HMR) |
| `npm run build`   | Build de production            |
| `npm run preview` | Prévisualisation du build      |
| `npm run lint`    | Vérification ESLint            |

## Stack technique

| Composant        | Technologie          | Rôle                                        |
| ---------------- | -------------------- | ------------------------------------------- |
| Framework UI     | React 19             | Composants et rendu                         |
| Bundler          | Vite 8               | Dev server avec HMR                         |
| Styling          | Tailwind CSS v4      | Classes utilitaires                         |
| Animations       | Framer Motion        | Transitions et micro-interactions           |
| State management | Zustand              | État UI local (filtre actif, mode édition)  |
| Data fetching    | TanStack React Query | Cache, mutations et synchronisation serveur |
| HTTP client      | Axios                | Appels vers l'API REST backend              |
| Icônes           | Lucide React         | Icônes SVG légères                          |

## Structure du projet

```
src/
├── api/
│   └── todoApi.js            # Couche Axios — appels vers toutes les APIs backend
│
├── hooks/
│   └── useTodos.js           # Hooks TanStack Query (queries + mutations)
│
├── store/
│   └── todoStore.js          # Store Zustand (filtre, état d'édition)
│
├── components/
│   ├── Header.jsx            # Titre + barre de progression
│   ├── TodoForm.jsx          # Formulaire d'ajout de tâche
│   ├── FilterBar.jsx         # Filtres (Toutes / Actives / Terminées)
│   ├── TodoList.jsx          # Liste des tâches avec loading/erreur/vide
│   ├── TodoItem.jsx          # Item individuel (complétion, édition, suppression)
│   └── EmptyState.jsx        # Message affiché quand la liste est vide
│
├── App.jsx                   # Layout principal
├── main.jsx                  # Point d'entrée + QueryClientProvider
└── index.css                 # Configuration Tailwind CSS v4
```

## Architecture

### Flux de données

```
API Backend (REST)
       ↕ axios
   todoApi.js
       ↕
   useTodos.js (TanStack Query)
       ↕ cache + invalidation automatique
   Composants React
       ↕
   todoStore.js (Zustand) — état UI uniquement
```

- **TanStack Query** gère tout le cycle de vie des données serveur : fetch, cache, refetch après mutation, états loading/error.
- **Zustand** ne gère que l'état purement UI (filtre actif, quel item est en mode édition).
- **Axios** est isolé dans `todoApi.js` — un seul point de configuration pour la base URL.

### Animations

Les animations sont gérées par **Framer Motion** avec :

- **Stagger** sur la liste : les items apparaissent en cascade fluide lors du changement de filtre
- **AnimatePresence** : animation de sortie progressive (réduction en hauteur + fade out) lors de la suppression
- **Spring transitions** sur la pill du filtre actif et le check de complétion
- **Barre de progression** animée dans le header

## Fonctionnalités

| Action         | Détails                                                     |
| -------------- | ----------------------------------------------------------- |
| Lister         | Chargement automatique, spinner pendant le loading          |
| Créer          | Formulaire avec validation, désactivé si vide               |
| Compléter      | Clic sur le cercle, animation check, texte barré            |
| Éditer         | Bouton crayon au survol, édition inline, Enter pour valider |
| Supprimer      | Bouton poubelle au survol, animation de sortie fluide       |
| Filtrer        | 3 filtres avec pill animée (Toutes / Actives / Terminées)   |
| Gestion erreur | Message affiché si le backend n'est pas joignable           |
