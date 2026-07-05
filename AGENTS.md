# Agent Guidelines & Project Instructions

Welcome! This file contains essential instructions, conventions, and constraints for AI agents/chatbots working on the **AI Meme Generator** project. Always adhere to these guidelines when analyzing code, writing implementation plans, or executing changes.

---

## 1. Documentation Synchronization Rule

> [!IMPORTANT]
> **Keep Documentation in Sync:** If any code change introduces, modifies, or deletes system capabilities, databases, or API contracts, you **MUST** update the following files in the same workspace iteration:
> - **[ARCHITECTURE.md](file:///ARCHITECTURE.md):** Update the Mermaid diagrams, core component lists, detailed sequences/data flows, database schema tables, or design decision sections.
> - **[README.md](file:///README.md):** Update the features list, tech stack details, directory structure, setup guides, or configuration parameter tables.

---

## 2. Core Project Context

### Tech Stack Reference
- **Backend:** ASP.NET Core 8 REST API, Entity Framework Core 8 (Code-First Migrations, Repository + Unit of Work patterns), SkiaSharp (server-side image utilities), Cloudinary (CDN storage).
- **AI Integration:** Cloudflare REST API used for generating captions for memes.
- **Frontend:** Angular 19 SPA (Standalone Components, Signals for state management, Router, TailwindCSS, PrimeNG UI Component Library).
- **Database:** PostgreSQL 15.

### Database Conventions
- Always use **EF Core code-first migrations** to modify the database schema.
- Database configurations must be separated into the `be/Data/Configurations` directory using Fluent API (`IEntityTypeConfiguration<T>`).
- Database columns and tables follow `snake_case` naming conventions mapping via the Fluent API.

### Image Rendering & Processing
- The principal rendering mechanism is **client-side HTML5 Canvas overlay** (implemented in `MemeComponent`).
- Keep server-side SkiaSharp capabilities in sync for alternative processing or batch export utilities, but avoid unnecessary server-side rendering for standard dashboard flows.

### Credit System & Concurrency
- Credits are tracked inside a dedicated table (`user_credits`) and balanced using a transaction ledger (`credit_transactions`) rather than a simple counter on the `User` profile.
- Always perform credit checks and deduction operations within a transaction block in `CreditService` before executing LLM generations.
