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