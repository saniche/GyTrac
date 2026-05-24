# GyTrac

GyTrac (GymTracker) is a bodybuilding-focused workout tracker built for athletes who want full control over their training. It lets users plan structured programs, log every set in real time, and review progress over time across weight, volume, and muscle groups.

The system is built around three surfaces that share a single backend API:

- A REST API (ASP.NET Core) that owns all business logic and data persistence
- A mobile app (.NET MAUI) for on-the-floor use during training sessions on iOS, Android, and Windows
- A web app (Angular) for program planning, history review, and analytics on larger screens

All three surfaces consume the same typed contracts. The mobile and web clients talk exclusively through the API — there is no direct database access from any client.

## Solution Structure

### Backend (`src/`)

| Project | Role |
| --- | --- |
| GymTracker.Common | `IDispatcher`, `IValidator`, cross-cutting plumbing |
| GymTracker.Domain | Entities, value objects (`Weight`), enums — no auth |
| GymTracker.Application | CQRS handlers, validators, feature vertical slices |
| GymTracker.Identity | Standalone auth bounded context, own DB, JWT generation |
| GymTracker.Infrastructure | EF Core `DbContext`, SQL Server, seed data |
| GymTracker.Api | ASP.NET Core host, JWT middleware, controllers, Serilog |
| GymTracker.Shared.Contracts | DTOs and request/response records — published as NuGet |
| GymTracker.Shared.HttpClient | Typed `IGymTrackerClient` — published as NuGet |

### Mobile (`mobile/`)

| Project | Role |
| --- | --- |
| GymTracker.MobileApp | .NET MAUI app — Shell navigation, MVVM, CommunityToolkit.Mvvm |

### Web (`web/`)

| Project | Role |
| --- | --- |
| GymTracker.Web | Angular 18+ SPA — standalone components, Signals, Angular Material |

### Test projects

| Folder | Project | Scope |
| --- | --- | --- |
| `src/tests/` | `Application.Tests` | Handler unit tests, in-memory EF Core |
| `src/tests/` | `Identity.Tests` | Auth service, token generation, password hashing |
| `src/tests/` | `Api.Integration.Tests` | End-to-end API route tests with real DB |
| `mobile/tests/` | `MobileApp.Tests` | ViewModel unit tests, mocked `IGymTrackerClient` |
| `web/tests/` | `Web.Tests` | Component specs, service stubs, Angular TestBed (Karma/Jest) |

## Repository Layout

```text
GymTracker/ (monorepo root)
├── src/ Backend solution
│   ├── GymTracker.Common/
│   ├── GymTracker.Domain/
│   ├── GymTracker.Application/
│   ├── GymTracker.Identity/
│   ├── GymTracker.Infrastructure/
│   ├── GymTracker.Api/
│   ├── GymTracker.Shared.Contracts/ ← published to NuGet
│   ├── GymTracker.Shared.HttpClient/ ← published to NuGet
│   ├── tests/
│   │   ├── GymTracker.Application.Tests/
│   │   ├── GymTracker.Identity.Tests/
│   │   └── GymTracker.Api.Integration.Tests/
│   └── GymTracker.Backend.sln
├── mobile/ MAUI solution
│   ├── GymTracker.MobileApp/
│   ├── tests/
│   │   └── GymTracker.MobileApp.Tests/
│   └── GymTracker.Mobile.sln
├── web/ Angular workspace
│   ├── src/
│   │   ├── app/
│   │   ├── contracts/ TS types (from OpenAPI gen)
│   │   └── api/ typed HttpClient services
│   ├── tests/ Karma / Jest specs
│   └── package.json
└── .github/
    └── workflows/
        ├── backend.yml paths: src/**
        ├── mobile.yml paths: mobile/**
        └── web.yml paths: web/**
```