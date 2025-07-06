# filepath: c:\Users\trice\Desktop\intership test\MentalHealthCheckIn\README.md
# Mental Health Check-In Platform

A comprehensive mental health monitoring system built with .NET 8 and Next.js.

## Architecture

- **Backend**: .NET 8 Web API with Entity Framework Core
- **Frontend**: Next.js 14 with TypeScript and Tailwind CSS
- **Database**: SQLite for development
- **Testing**: xUnit for backend, Jest for frontend

## Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- npm or yarn

### Backend Setup
```bash
cd Backend/MentalHealthAPI
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5006`
Swagger documentation: `http://localhost:5006/swagger`

### Frontend Setup
```bash
cd Frontend/mental-health-frontend
npm install
npm run dev
```

The web application will be available at `http://localhost:3000`

## Running Tests

### Backend Tests
```bash
cd Backend/MentalHealthAPI.Tests
dotnet test
```

### Frontend Tests
```bash
cd Frontend/mental-health-frontend
npm test
```

## Features

- Create daily mental health check-ins
- Rate mood from 1-5 scale
- Add optional notes
- Filter by user, date range, or check-in ID
- View detailed check-in information
- Edit and delete check-ins
- Responsive design
- Full test coverage

## API Endpoints

- `GET /api/checkins` - Get all check-ins
- `POST /api/checkins` - Create new check-in
- `GET /api/checkins/{id}` - Get check-in by ID
- `PUT /api/checkins/{id}` - Update check-in
- `DELETE /api/checkins/{id}` - Delete check-in
- `GET /api/checkins/user/{userId}` - Get check-ins by user

## Technology Stack

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- xUnit Testing
- Swagger/OpenAPI

### Frontend
- Next.js 14
- TypeScript
- Tailwind CSS
- SWR for data fetching
- Jest Testing

## Development

The project uses clean architecture principles with:
- Repository pattern
- Dependency injection
- DTO pattern
- Comprehensive error handling
- Input validation
- Automated testing

## License

This project is for educational purposes.