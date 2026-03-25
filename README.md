# CaseNexus — Case Investigation Management System

A web-based enterprise-inspired investigation 
and compliance platform built with ASP.NET Core MVC.

## Tech Stack
- ASP.NET Core MVC (.NET 8)
- PostgreSQL + Npgsql
- Repository Pattern + Dependency Injection
- SignalR (Real-time Notifications)
- Kendo Grid + jQuery/AJAX
- Chart.js Dashboard
- Bootstrap 5

## Features
- Role-based access (Admin/Investigator/User)
- Multi-stage case workflow
  (Open → InReview → Resolved → Closed)
- Complete audit trail
- SLA breach detection
- Real-time SignalR notifications
- Document evidence management
- Analytics dashboard (Chart.js)
- Investigation timeline

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- PostgreSQL
- Visual Studio / VS Code

### Database Setup
```sql
-- Run these in pgAdmin:
create table public.t_users(...)
create table public.t_cases(...)
create table public.t_comments(...)
create table public.t_documents(...)
create table public.t_audit_trail(...)
```

### Connection String
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;
     Port=5432;Database=YourDB;
     Username=postgres;Password=yourpassword"
  }
}
```

### Run Project
```bash
dotnet restore
dotnet run
```

## Default Users
| Role | Email | Password |
|------|-------|----------|
| Admin | admin@casenexus.com | Admin@123 |
| Investigator | inv@casenexus.com | Inv@123 |
| User | Register from UI | - |

## Docker (Local)
1. Create a local `.env` file by copying `.env.example` and replace `CHANGE_ME` with your PostgreSQL password.
2. Start everything:
   ```bash
   docker compose up --build
   ```
3. Open the app at: `http://localhost:8080`
4. PostgreSQL is available at: `localhost:5433` (inside Docker network it uses the host name `db`).

Note: this repo’s README doesn’t include migrations. Create the required database schema (tables) before the app will work.

## CI/CD (GitHub Actions)
This repo includes a pipeline that builds your Docker image, pushes it to Azure Container Registry (ACR), and deploys to an Azure Web App for Containers:
`.github/workflows/ci-cd-azure.yml`

Add these GitHub repository secrets:
- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_CLIENT_SECRET`
- `AZURE_ACR_NAME` (e.g., `myappacr`)
- `AZURE_RESOURCE_GROUP`
- `AZURE_WEBAPP_NAME`
- `AZURE_CONNECTIONSTRING` (set to `Server=db;Port=5432;Database=SmartEmployeeDB;Username=postgres;Password=...;Include Error Detail=true;`)

On merge/push to `main`, the workflow will deploy the image tagged with the Git commit SHA.

## Azure (Web App for Containers)
Example CLI flow (adjust names):
```bash
az login

RESOURCE_GROUP="caseinvestigation-rg"
LOCATION="eastus"
ACR_NAME="caseinvestigationacr"
WEBAPP_NAME="caseinvestigation-webapp"
PLAN_NAME="caseinvestigation-plan"

az group create --name "$RESOURCE_GROUP" --location "$LOCATION"
az acr create --resource-group "$RESOURCE_GROUP" --name "$ACR_NAME" --sku Basic

az appservice plan create \
  --name "$PLAN_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --sku B1 \
  --is-linux

# Create the Web App (pipeline will set the container image + app settings)
az webapp create \
  --name "$WEBAPP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --plan "$PLAN_NAME" \
  --deployment-container-image-name "mcr.microsoft.com/dotnet/aspnet:8.0"
```

After that, set the GitHub secrets listed in the CI/CD section and trigger the workflow.

To create the credentials for GitHub Actions, you can run:
```bash
SP_NAME="caseinvestigation-cicd-sp"
az ad sp create-for-rbac \
  --name "$SP_NAME" \
  --role Contributor \
  --scopes "$(az group show -n "$RESOURCE_GROUP" --query id -o tsv)" \
  --sdk-auth
```

Then paste these into GitHub secrets:
- `clientId` -> `AZURE_CLIENT_ID`
- `clientSecret` -> `AZURE_CLIENT_SECRET`
- `tenantId` -> `AZURE_TENANT_ID`
- `subscriptionId` -> `AZURE_SUBSCRIPTION_ID`

Note: you must provide a reachable PostgreSQL connection string in `AZURE_CONNECTIONSTRING` (this pipeline does not provision the database for you).