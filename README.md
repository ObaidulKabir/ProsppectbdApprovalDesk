# ProspectbdApprovalDesk

Enterprise web application for managing RAJUK approval projects and ECPS credentials.

## Backend (ASP.NET Core Web API, .NET 8)

- Solution: `ProspectbdApprovalDesk.sln`
- Projects:
  - `ProspectbdApprovalDesk.Domain`
  - `ProspectbdApprovalDesk.Application`
  - `ProspectbdApprovalDesk.Infrastructure`
  - `ProspectbdApprovalDesk.Api`

### Run (with PostgreSQL)

1. Start PostgreSQL (optional Docker):

```bash
docker compose up -d
```

2. Update connection string and keys:
   - `src/ProspectbdApprovalDesk.Api/appsettings.Development.json`
   - `ConnectionStrings:DefaultConnection`
   - `Jwt:SigningKey` (min 32 chars)
   - `Security:EncryptionKeyBase64` (base64 16/24/32 bytes)

3. Run API:

```bash
dotnet run --project src/ProspectbdApprovalDesk.Api
```

Swagger: `http://localhost:5096/swagger`

Seeded admin (development defaults):
- Email: `admin@prospectbd.local`
- Password: `Admin@12345`

## Frontend (React + TypeScript + Tailwind)

1. Configure API base URL:

```bash
copy frontend\\.env.example frontend\\.env
```

2. Run:

```bash
cd frontend
npm install
npm run dev
```

Frontend default: `http://localhost:5173`

