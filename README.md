# AI Meme Generator

An AI-powered meme generation platform built with an **ASP.NET Core 8 Web API** backend and an **Angular 19** frontend. The system takes arbitrary text inputs (such as diary entries, codebase logs, or paragraphs), identifies key themes using **Google Gemini 1.5 Flash**, selects relevant meme templates from a **PostgreSQL** database, generates funny captions, and overlays them using both server-side processing (**SkiaSharp**) and client-side web rendering (**HTML5 Canvas**).

---

## Features

- **Text-to-Meme Intelligence:** Submit any text snippet (articles, complaints, logs) to automatically generate high-context, context-appropriate memes.
- **Dynamic Image Processing:** Overlay text dynamically with custom font rendering, line-wrapping, and semi-transparent caption bars.
- **Google Sign-In Integration:** Securely authenticate users via Google Identity Services.
- **Credit System:** Built-in credit-based limits for authenticated users (defaulting to 5 starting credits).
- **Interactive Dashboard:** Input area with loading indicators, a grid view of generated memes, and a bulk-download option.
- **Swagger Documentation:** Auto-generated interactive API documentation for testing backend endpoints.

---

## Tech Stack

### Backend
- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Database Access:** Entity Framework Core 8 + Npgsql Provider (Code-First Migrations)
- **Image Processing:** SkiaSharp (for native image manipulations)
- **CDN / Storage:** Cloudinary SDK (for hosting and serving template images)
- **AI Engine:** Google Gemini AI API (Gemini 1.5 Flash)

### Frontend
- **Framework:** Angular 19 (Standalone Components, Signals, Router)
- **Styling:** TailwindCSS + PrimeNG (Aura theme)
- **Icons:** PrimeIcons

### Infrastructure
- **Containerization:** Docker & Docker Compose
- **Database:** PostgreSQL 15

---

## Directory Structure

- [be/](file:///Users/sumrendersingh/Desktop/meme/be) — ASP.NET Core 8 Web API application
  - [Controllers/](file:///Users/sumrendersingh/Desktop/meme/be/Controllers) — Endpoint routing and request validation
  - [Services/](file:///Users/sumrendersingh/Desktop/meme/be/Services) — Business logic handlers (Meme, Gemini, Image, User)
  - [Models/](file:///Users/sumrendersingh/Desktop/meme/be/Models) — Database entities (User, MemeTemplate)
  - [Dtos/](file:///Users/sumrendersingh/Desktop/meme/be/Dtos) — Data Transfer Objects for API contracts
  - [Data/](file:///Users/sumrendersingh/Desktop/meme/be/Data) — EF Core DbContext, Configurations, Repositories, Unit of Work, Seed Data
- [fe/](file:///Users/sumrendersingh/Desktop/meme/fe) — Angular 19 frontend Single Page Application (SPA)
  - [src/app/components/](file:///Users/sumrendersingh/Desktop/meme/fe/src/app/components) — Dashboard, Meme, Sidebar, Textarea, and Auth views
  - [src/app/services/](file:///Users/sumrendersingh/Desktop/meme/fe/src/app/services) — HTTP client integrations (Google Auth & Meme generation APIs)

---

## Database Setup

The application uses **Entity Framework Core** with code-first migrations. The database schema and seed data are managed entirely through EF Core.

### Apply Migrations
After starting PostgreSQL (see [Run PostgreSQL](#1-run-postgresql) below), apply the migrations:

```bash
cd be

# Install EF Core tools (one-time)
dotnet tool install --global dotnet-ef

# Apply migrations (creates tables + seeds 11 meme templates)
dotnet ef database update
```

This creates the `users` and `meme_templates` tables with all 11 default meme templates pre-seeded.

> **Note:** If you have an existing database with the old Dapper schema, drop it first:
> ```sql
> DROP TABLE IF EXISTS meme_templates;
> DROP TABLE IF EXISTS users;
> DROP TABLE IF EXISTS __EFMigrationsHistory;
> ```

---

## Local Development Setup

Follow these steps to run both the backend and frontend services locally.

### Prerequisites
- Install **.NET 8 SDK**
- Install **Node.js (v18 or higher)** and npm
- Install **Docker Desktop** (or a local PostgreSQL instance)

### 1. Run PostgreSQL
You can easily spin up the database service using the provided docker-compose configuration.
```bash
# Navigate to the backend directory
cd be

# Start the PostgreSQL service
docker compose up -d postgres
```
This launches a PostgreSQL container exposed on `localhost:5432` with username `postgres`, password `postgres`, and database name `meme_gen_db`. Then apply the EF Core migrations to set up the schema and seed data.

### 2. Apply Database Migrations
```bash
# Apply migrations to create tables and seed meme templates
dotnet ef database update
```

### 2. Configure Backend Secrets
Create or update [appsettings.Development.json](file:///Users/sumrendersingh/Desktop/meme/be/appsettings.Development.json) inside the `be` directory:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=meme_gen_db;Port=5432;Username=postgres;Password=postgres"
  },
  "GEMINI_API_KEY": "YOUR_GEMINI_API_KEY",
  "CLOUDINARY_CLOUD_NAME": "dplzxrvqy",
  "CLOUDINARY_API_KEY": "315875178361159",
  "CLOUDINARY_API_SECRET": "YOUR_CLOUDINARY_API_SECRET"
}
```

### 3. Run Backend API
```bash
# Verify you are in the be directory
cd be

# Run the .NET application
dotnet run
```
The server will start and listen on `http://localhost:8080`.
- **Swagger Documentation UI:** Open `http://localhost:8080/swagger` in your browser to view and interact with the endpoints.

### 4. Configure Frontend Environment
Ensure the frontend is pointing to the local API endpoint.
Modify [constants.ts](file:///Users/sumrendersingh/Desktop/meme/fe/src/app/models/constants.ts):
```typescript
export const MEME_USER = 'MEME_USER';

// Point to local .NET backend URL
export const BASE_URL = 'http://localhost:8080';
```

### 5. Install Dependencies & Run Frontend
```bash
# Navigate to the frontend directory
cd ../fe

# Install npm packages
npm install

# Start the development server
npm run start
```
Open `http://localhost:4200` in your web browser. The frontend is set up with TailwindCSS styling and will interact directly with your local backend API.

---

## Configuration Parameter Reference

The following environment variables and settings are used by the system:

| Config Key | Location / Usage | Purpose |
|------------|------------------|---------|
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | PostgreSQL DB connection parameters |
| `GEMINI_API_KEY` | `appsettings.json` | Credentials to authenticate with Google Gemini API |
| `CLOUDINARY_API_SECRET` | `appsettings.json` | Secret key to upload generated assets to Cloudinary |
| `BASE_URL` | `constants.ts` | Frontend base endpoint URL for backend API queries |
| `client_id` | `auth.service.ts` | Google Client ID credential for Google Sign-In prompt |
