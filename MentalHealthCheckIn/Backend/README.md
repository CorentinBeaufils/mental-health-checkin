# Mental Health Check-in Backend

Solution .NET Core pour l'application de suivi du bien-être mental.

## 🏗️ Structure de la Solution

```
MentalHealthCheckIn.sln
├── MentalHealthAPI/              # Projet principal de l'API
│   ├── Controllers/              # Contrôleurs REST API
│   ├── Models/                   # Modèles de données (CheckIn)
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Services/                 # Logique métier
│   ├── Data/                     # Contexte Entity Framework
│   └── Program.cs                # Point d'entrée de l'application
└── MentalHealthAPI.Tests/        # Tests unitaires et d'intégration
    ├── Controllers/              # Tests des contrôleurs
    └── Services/                 # Tests des services
```

## 🚀 Commandes Utiles

### Compiler toute la solution
```bash
dotnet build
```

### Lancer l'API
```bash
dotnet run --project MentalHealthAPI
```

### Exécuter tous les tests
```bash
dotnet test
```

### Restaurer les packages NuGet
```bash
dotnet restore
```

## 📦 Dépendances Principales

- **Microsoft.AspNetCore.App** - Framework ASP.NET Core
- **Microsoft.EntityFrameworkCore.Sqlite** - Entity Framework avec SQLite
- **Microsoft.EntityFrameworkCore.Design** - Outils de migration EF
- **Swashbuckle.AspNetCore** - Documentation Swagger/OpenAPI

## 🗄️ Base de Données

- **Type** : SQLite
- **Approche** : Code-First avec Entity Framework Core
- **Fichier** : `checkins.db` (généré automatiquement)

## 🔗 API Endpoints

- `GET /api/checkins` - Tous les check-ins
- `GET /api/checkins/user/{userId}` - Check-ins par utilisateur
- `GET /api/checkins/{id}` - Check-in spécifique
- `POST /api/checkins` - Créer un check-in
- `PUT /api/checkins/{id}` - Modifier un check-in
- `DELETE /api/checkins/{id}` - Supprimer un check-in

## 📚 Documentation API

Swagger UI disponible à : `https://localhost:5006/swagger`

## 🧪 Tests

- **21 tests** au total (service + contrôleur)
- **Framework** : xUnit + FluentAssertions
- **Couverture** : Tests unitaires et d'intégration