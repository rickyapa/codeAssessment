# CodeAssessment

This repository contains the .NET backend API and Angular frontend for the product inventory assessment.

## Projects

- `codeAssessmentAPI/` — ASP.NET Core REST API built with EF Core and SQL Server
- `codeAssessmentWeb/` — Angular frontend that consumes the API

## Backend setup

1. Install SQL Server locally or use a SQL Server-compatible instance.
2. Update the connection string in `codeAssessmentAPI/src/CodeAssessment.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=codeAssessment;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Start the API from the solution root:

```powershell
cd codeAssessmentAPI
dotnet restore
dotnet tool restore

dotnet run --project src/CodeAssessment.Api/CodeAssessment.Api.csproj
```

The API runs on `http://localhost:5070` by default via the launch profile in `src/CodeAssessment.Api/Properties/launchSettings.json`.

4. EF Core migrations are applied automatically when the app runs in Development mode.

## Frontend setup

1. Install Node.js dependencies:

```powershell
cd codeAssessmentWeb
npm install
```

2. Start the Angular app:

```powershell
npm start
```

The Angular app is configured to proxy `/api` calls to the API at `http://localhost:5070` through `proxy.conf.json`.

Open `http://localhost:4200` in the browser.

## Build and test commands

Backend:

```powershell
cd codeAssessmentAPI
dotnet test codeAssessmentAPI.slnx
```

Frontend:

```powershell
cd codeAssessmentWeb
npm run build
npm test -- --watch=false
```

## API endpoints

The backend implements the required product endpoints:

- `GET /api/products`
- `POST /api/products`
- `GET /api/products/{id}`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`
- `POST /api/products/{id}/decrement-stock/{quantity}`
- `POST /api/products/{id}/add-to-stock/{quantity}`
- `GET /api/products/search?name={name}`
- `GET /api/products/stock-level?min={min}&max={max}`

## Notes

- Product IDs are generated with an SQL Server sequence and constrained to a 6-digit range (`100000` to `999999`).
- The database is seeded with initial product data via EF Core migrations.
- The solution follows a layered architecture with domain, application, infrastructure, and API layers.
