# AI Meme Generator

An AI-powered meme generation platform built with an **ASP.NET Core 8 Web API** backend and an **Angular 19** frontend. The system takes arbitrary text inputs (such as diary entries, codebase logs, or paragraphs), identifies key themes using a **Cloudflare AI LLM (e.g. Llama 3.1)**, selects relevant meme templates from a **PostgreSQL** database, generates funny captions, and overlays them using both server-side processing (**SkiaSharp**) and client-side web rendering (**HTML5 Canvas**).

---

## Features

- **Text-to-Meme Intelligence:** Submit any text snippet (articles, complaints, logs) to automatically generate high-context, context-appropriate memes.
- **Dynamic Image Processing:** Overlay text dynamically with custom font rendering, line-wrapping, and semi-transparent caption bars.
- **Google Sign-In Integration:** Securely authenticate users via Google Identity Services.
- **Meme Albums & History:** Automatically saves generated memes under distinct albums. Users can view their generation history, inspect past albums, and retrieve saved memes.
- **Advanced Credit Ledger:** Multi-tiered credit system tracking user credits and transactions via a ledger to prevent double-spending and record a history of balance changes.
- **Interactive Dashboard:** Input area with loading indicators, a grid view of generated memes, and options to download.
- **Swagger Documentation:** Auto-generated interactive API documentation for testing backend endpoints.

---

## Tech Stack

### Backend
- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Database Access:** Entity Framework Core 8 + Npgsql Provider (Code-First Migrations)
- **Image Processing:** SkiaSharp (for native image manipulations)
- **CDN / Storage:** Cloudinary SDK (for hosting and serving template images)
- **AI Engine:** Cloudflare AI API (defaulting to `@cf/meta/llama-3.1-8b-instruct`)

### Frontend
- **Framework:** Angular 19 (Standalone Components, Signals, Router)
- **Styling:** TailwindCSS + PrimeNG (Aura theme)
- **Icons:** PrimeIcons

### Infrastructure
- **Containerization:** Docker & Docker Compose
- **Database:** PostgreSQL 15

---

## Directory Structure

- [be/](file:///be) — ASP.NET Core 8 Web API application
  - [Controllers/](file:///be/Controllers) — Endpoint routing and request validation (ApiController, AlbumsController, CreditsController)
  - [Services/](file:///be/Services) — Business logic handlers (Meme, CloudflareAiService, Image, User, Credit)
  - [Models/](file:///be/Models) — Database entities (User, MemeTemplate, Album, Meme, UserCredit, CreditTransaction)
  - [Dtos/](file:///be/Dtos) — Data Transfer Objects for API contracts
  - [Data/](file:///be/Data) — EF Core DbContext, Configurations, Repositories, Unit of Work, Seed Data
- [fe/](file:///fe) — Angular 19 frontend Single Page Application (SPA)
  - [src/app/components/](file:///fe/src/app/components) — Dashboard, Meme, Sidebar, Textarea, Auth, My Memes, and Album Detail views
  - [src/app/services/](file:///fe/src/app/services) — HTTP client integrations (Google Auth, Meme generation, and Credit APIs)

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

This creates all required tables (`users`, `meme_templates`, `albums`, `memes`, `user_credits`, `credit_transactions`) with default meme templates pre-seeded.

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

### 3. Configure Backend Environment
Create a `.env` file in the `be/` directory (you can copy `.env.example` as a starting point) and configure the environment variables:
```env
# Cloudflare AI
CloudflareSettings__ApiToken=YOUR_CLOUDFLARE_API_TOKEN
CloudflareSettings__BaseUrl=https://gateway.ai.cloudflare.com/v1/YOUR_ACCOUNT_ID/default/compat
CloudflareSettings__Model=@cf/meta/llama-3.1-8b-instruct

# Cloudinary
CLOUDINARY_CLOUD_NAME=YOUR_CLOUDINARY_CLOUD_NAME
CLOUDINARY_API_KEY=YOUR_CLOUDINARY_API_KEY
CLOUDINARY_API_SECRET=YOUR_CLOUDINARY_API_SECRET

# Database
ConnectionStrings__DefaultConnection=Server=localhost;Database=meme_gen_db;Port=5432;User Id=postgres;Password=postgres

# JWT Auth
JWT_SECRET=YOUR_JWT_SECRET
JWT_ISSUER=meme-api
JWT_AUDIENCE=meme-app

# Google Auth
GOOGLE_CLIENT_ID=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com
```

Alternatively, you can configure these settings inside `appsettings.Development.json` using the standard nested JSON structure.

### 4. Run Backend API
```bash
# Verify you are in the be directory
cd be

# Run the .NET application
dotnet run
```
The server will start and listen on `http://localhost:8080`.
- **Swagger Documentation UI:** Open `http://localhost:8080/swagger` in your browser to view and interact with the endpoints.

### 5. Configure Frontend Environment
Ensure the frontend is pointing to the local API endpoint.
Modify [constants.ts](file:///fe/src/app/models/constants.ts):
```typescript
export const MEME_USER = 'MEME_USER';

// Point to local .NET backend URL
export const BASE_URL = 'http://localhost:8080';
```

### 6. Install Dependencies & Run Frontend
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
| `ConnectionStrings:DefaultConnection` | `appsettings.json` / `.env` | PostgreSQL DB connection parameters |
| `CloudflareSettings:ApiToken` | `appsettings.json` / `.env` | Credentials to authenticate with Cloudflare AI API |
| `CloudflareSettings:BaseUrl` | `appsettings.json` / `.env` | Target URL prefix for Cloudflare AI API gateway |
| `CloudflareSettings:Model` | `appsettings.json` / `.env` | The specific LLM to target on Cloudflare (e.g. Llama-3.1) |
| `CLOUDINARY_API_SECRET` | `appsettings.json` / `.env` | Secret key to upload generated assets to Cloudinary |
| `BASE_URL` | `constants.ts` | Frontend base endpoint URL for backend API queries |
| `client_id` | `auth.service.ts` | Google Client ID credential for Google Sign-In prompt |
