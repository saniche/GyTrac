**GymTracker**

Full-Stack Architecture Document

Backend · Mobile (MAUI) · Web (Angular)

**1. Project Vision**

GymTracker is a bodybuilding-focused workout tracker built for athletes
who want full control over their training. It lets users plan structured
programs, log every set in real time, and review progress over time
across weight, volume, and muscle groups.

The system is built around three surfaces that share a single backend
API:

- A REST API (ASP.NET Core) that owns all business logic and data
  persistence

- A mobile app (.NET MAUI) for on-the-floor use during training sessions
  on iOS, Android, and Windows

- A web app (Angular) for program planning, history review, and
  analytics on larger screens

All three surfaces consume the same typed contracts. The mobile and web
clients talk exclusively through the API --- there is no direct database
access from any client.

**2. Solution Structure**

**2.1 Backend (src/)**

  --------------------------------------------------------------------------
  **Project**                    **Role**
  ------------------------------ -------------------------------------------
  GymTracker.Common              IDispatcher, IValidator, cross-cutting
                                 plumbing

  GymTracker.Domain              Entities, value objects (Weight), enums ---
                                 no auth

  GymTracker.Application         CQRS handlers, validators, feature vertical
                                 slices

  GymTracker.Identity            Standalone auth bounded context, own DB,
                                 JWT generation

  GymTracker.Infrastructure      EF Core DbContext, SQL Server, seed data

  GymTracker.Api                 ASP.NET Core host, JWT middleware,
                                 controllers, Serilog

  GymTracker.Shared.Contracts    DTOs and request/response records ---
                                 published as NuGet

  GymTracker.Shared.HttpClient   Typed IGymTrackerClient --- published as
                                 NuGet
  --------------------------------------------------------------------------

**2.2 Mobile (mobile/)**

  ------------------------------------------------------------------
  **Project**            **Role**
  ---------------------- -------------------------------------------
  GymTracker.MobileApp   .NET MAUI app --- Shell navigation, MVVM,
                         CommunityToolkit.Mvvm

  ------------------------------------------------------------------

**2.3 Web (web/)**

  ------------------------------------------------------------------
  **Project**            **Role**
  ---------------------- -------------------------------------------
  GymTracker.Web         Angular 18+ SPA --- standalone components,
                         Signals, Angular Material

  ------------------------------------------------------------------

**2.4 Test projects**

  ----------------------------------------------------------------------------
  **Folder**      **Project**             **Scope**
  --------------- ----------------------- ------------------------------------
  src/tests/      Application.Tests       Handler unit tests, in-memory EF
                                          Core

  src/tests/      Identity.Tests          Auth service, token generation,
                                          password hashing

  src/tests/      Api.Integration.Tests   End-to-end API route tests with real
                                          DB

  mobile/tests/   MobileApp.Tests         ViewModel unit tests, mocked
                                          IGymTrackerClient

  web/tests/      Web.Tests               Component specs, service stubs,
                                          Angular TestBed (Karma/Jest)
  ----------------------------------------------------------------------------

**3. Repository Layout**

One monorepo, three solution areas, each self-contained with its own
tests folder.

> GymTracker/ (monorepo root)
>
> ├── src/ Backend solution
>
> │ ├── GymTracker.Common/
>
> │ ├── GymTracker.Domain/
>
> │ ├── GymTracker.Application/
>
> │ ├── GymTracker.Identity/
>
> │ ├── GymTracker.Infrastructure/
>
> │ ├── GymTracker.Api/
>
> │ ├── GymTracker.Shared.Contracts/ ← published to NuGet
>
> │ ├── GymTracker.Shared.HttpClient/ ← published to NuGet
>
> │ ├── tests/
>
> │ │ ├── GymTracker.Application.Tests/
>
> │ │ ├── GymTracker.Identity.Tests/
>
> │ │ └── GymTracker.Api.Integration.Tests/
>
> │ └── GymTracker.Backend.sln
>
> ├── mobile/ MAUI solution
>
> │ ├── GymTracker.MobileApp/
>
> │ ├── tests/
>
> │ │ └── GymTracker.MobileApp.Tests/
>
> │ └── GymTracker.Mobile.sln
>
> ├── web/ Angular workspace
>
> │ ├── src/
>
> │ │ ├── app/
>
> │ │ ├── contracts/ TS types (from OpenAPI gen)
>
> │ │ └── api/ typed HttpClient services
>
> │ ├── tests/ Karma / Jest specs
>
> │ └── package.json
>
> └── .github/
>
> └── workflows/
>
> ├── backend.yml paths: src/\*\*
>
> ├── mobile.yml paths: mobile/\*\*
>
> └── web.yml paths: web/\*\*

**4. Shared Contracts and HTTP Client**

**4.1 Distribution model**

Both shared projects live in src/ and are part of
GymTracker.Backend.sln. They are the only projects in the solution that
are published externally as NuGet packages (to GitHub Packages or a
private feed). No other project outside src/ uses project references
into src/.

  -----------------------------------------------------------------------------
  **Package**                    **Consumers**       **How consumed**
  ------------------------------ ------------------- --------------------------
  GymTracker.Shared.Contracts    Api, MobileApp,     NuGet reference in mobile;
                                 (Angular via        project ref within src/
                                 OpenAPI gen)        

  GymTracker.Shared.HttpClient   MobileApp           NuGet reference in mobile/
  -----------------------------------------------------------------------------

**4.2 GymTracker.Shared.Contracts**

Plain C# records targeting netstandard2.1. No EF Core, no domain logic,
no framework dependencies. Contains all DTO types used across the API
surface.

- Requests: CreateSessionRequest, LogSetRequest, RegisterRequest,
  LoginRequest, ...

- Responses: SessionDto, ExerciseLogDto, SetDto, AuthResponse,
  ExerciseDto, ...

- Pagination wrapper: PagedResult\<T\>

The Angular web app does not consume the NuGet package directly.
Instead, contracts are generated as TypeScript interfaces from the
OpenAPI spec emitted by the API (using openapi-generator-cli or
similar). This keeps the Angular workspace free of .NET toolchain
dependencies.

**4.3 GymTracker.Shared.HttpClient**

Typed wrapper around System.Net.Http.HttpClient. Handles base URL
configuration, JWT Bearer header injection, JSON serialization, and
error mapping to a typed ApiException. The Angular app implements its
own equivalent services using Angular\'s HttpClient directly.

- Interface: IGymTrackerClient

- Sub-interfaces: IAuthClient, ISessionClient, IExerciseClient,
  IRoutineClient, IProgramClient

- Registered in MobileApp via AddGymTrackerClient(baseUrl) extension
  method

**4.4 Versioning and release**

Shared packages follow semantic versioning. A breaking change to a DTO
(renaming a field, changing a type) requires a major version bump. The
mobile CI workflow declares a minimum acceptable version range and will
fail the build if an incompatible version is published. The API project
always references these packages via project reference internally so
that the API and the packages are always in sync within the same build.

**5. Project References**

**5.1 Backend dependency graph**

> GymTracker.Common
>
> └── (no external project deps)
>
> GymTracker.Domain
>
> └── Common
>
> GymTracker.Application
>
> ├── Domain
>
> └── Common
>
> GymTracker.Identity
>
> └── (standalone --- no other GymTracker refs)
>
> GymTracker.Infrastructure
>
> ├── Application
>
> ├── Domain
>
> └── Common
>
> GymTracker.Api
>
> ├── Infrastructure
>
> ├── Application
>
> ├── Identity
>
> ├── Shared.Contracts (project ref --- same solution)
>
> └── Common
>
> GymTracker.Shared.HttpClient
>
> └── Shared.Contracts (project ref --- same solution)

**5.2 Mobile dependency graph**

> GymTracker.MobileApp
>
> ├── GymTracker.Shared.Contracts (NuGet)
>
> └── GymTracker.Shared.HttpClient (NuGet)
>
> GymTracker.MobileApp.Tests
>
> ├── GymTracker.MobileApp
>
> └── GymTracker.Shared.Contracts (NuGet, transitive)

**5.3 Web dependency graph**

> GymTracker.Web (Angular --- no .csproj refs)
>
> └── contracts/ (TypeScript interfaces, OpenAPI-generated)
>
> No NuGet, no project refs --- pure TypeScript

**6. Domain Model**

**6.1 Core entities and relationships**

The domain models what a bodybuilder actually does: follow a structured
program, execute sessions, and log every set with weight and reps.

  ------------------------------------------------------------------
  **Entity**          **Description**
  ------------------- ----------------------------------------------
  Exercise            Reference data (Bench Press, Squat, ...).
                      Seeded, not user-created.

  Routine             Reusable session template, e.g. "Chest &
                      Triceps". Contains ordered exercises.

  Program             Ordered collection of routines forming a
                      training split (Push/Pull/Legs).

  WorkoutSession      An actual training session. Optionally created
                      from a Routine template.

  ExerciseLog         A concrete exercise within a session, with
                      order.

  Set                 One set: weight, reps, warmup flag, optional
                      notes. Owns the Weight value object.
  ------------------------------------------------------------------

**6.2 Value object: Weight**

Weight is not stored as a plain decimal. It enforces unit correctness
and prevents nonsensical values at the domain level.

- Allowed units: kg, lbs

- No negative values

- Stored as owned entity in EF Core: columns WeightValue and WeightUnit

**6.3 Enums**

- MuscleGroup: Chest, Back, Shoulders, Biceps, Triceps, Legs, Core,
  Calves

- ExerciseType: Compound, Isolation, Machine, Bodyweight, Cardio

- All enums stored as strings in the database (Fluent API configuration)

**6.4 No User entity in Domain**

The domain only uses a Guid UserId. User lifecycle, authentication, and
credential management are entirely within the Identity module. The two
bounded contexts share no entity types.

**7. Backend Architecture**

**7.1 Custom CQRS dispatcher (Common)**

There is no MediatR. The custom IDispatcher locates the correct handler
and optional validator via IServiceProvider, runs validation before the
handler, and logs the operation name and execution time.

- ICommandHandler\<TCommand\> and ICommandHandler\<TCommand, TResult\>
  for writes

- IQueryHandler\<TQuery, TResult\> for reads

- IValidator\<T\> runs before the handler; throws ValidationException on
  failure

- ValidationException is caught by global middleware and returned as
  HTTP 400

- services.AddHandlersFromAssembly() scans and registers all handlers
  and validators

**7.2 Application layer --- vertical slices**

Features are grouped by concern, not by layer. Each feature folder
contains its command or query record, its validator, and its handler
together.

> Application/Features/Workouts/CreateSession/
>
> ├── CreateSessionCommand.cs
>
> ├── CreateSessionValidator.cs
>
> └── CreateSessionHandler.cs

**7.3 Identity bounded context**

Identity is a completely standalone project with no reference to Domain,
Application, or Common. It has its own IdentityDbContext, its own
IdentityUser entity, and uses a plain service pattern (no CQRS). JWT
tokens it issues are validated by the API layer using shared Jwt
configuration settings.

  ------------------------------------------------------------------------------
  **Method**   **Route**            **Body**               **Response**
  ------------ -------------------- ---------------------- ---------------------
  POST         /api/auth/register   { email, password,     AuthResponse { Token,
                                    confirmPassword }      UserId }

  POST         /api/auth/login      { email, password }    AuthResponse { Token,
                                                           UserId }
  ------------------------------------------------------------------------------

**7.4 Infrastructure layer**

ApplicationDbContext implements IApplicationDbContext and applies all
entity configurations from the assembly. Seed data for exercises uses
fixed GUIDs so migrations are stable. All enums are stored as strings.
Cascade delete applies for compositions (Session to ExerciseLog to Set);
Restrict applies for reference data (Exercise).

**8. Mobile App (GymTracker.MobileApp)**

**8.1 Architecture pattern**

MVVM using CommunityToolkit.Mvvm source generators. Shell-based
navigation. The app is read-cached offline via SQLite but all writes go
through the API. JWT is stored in SecureStorage.

**8.2 Folder structure**

> mobile/GymTracker.MobileApp/
>
> ├── MauiProgram.cs
>
> ├── AppShell.xaml
>
> ├── Features/
>
> │ ├── Auth/ LoginPage, RegisterPage, LoginViewModel
>
> │ ├── Dashboard/ DashboardPage, DashboardViewModel
>
> │ ├── Workout/ ActiveSessionPage, LogSetPage + ViewModels
>
> │ ├── History/ HistoryPage, SessionDetailPage + ViewModels
>
> │ ├── Routines/
>
> │ └── Programs/
>
> ├── Services/
>
> │ ├── IAuthService.cs Wraps IAuthClient + SecureStorage
>
> │ └── IOfflineCacheService.cs SQLite read cache
>
> ├── Controls/ Reusable ContentViews
>
> └── Resources/ Colors.xaml, Styles.xaml, fonts

**8.3 NuGet packages**

  -----------------------------------------------------------------------
  **Package**                    **Purpose**
  ------------------------------ ----------------------------------------
  GymTracker.Shared.Contracts    DTOs shared with API

  GymTracker.Shared.HttpClient   Typed API client (IGymTrackerClient)

  CommunityToolkit.Mvvm          MVVM source generators

  CommunityToolkit.Maui          Popup, Toast utilities

  sqlite-net-pcl                 Local read cache

  Microsoft.Extensions.Http      HttpClient factory
  -----------------------------------------------------------------------

**9. Web App (GymTracker.Web)**

**9.1 Architecture pattern**

Feature-based standalone components. Signals for local reactive state;
RxJS stays inside API services only. Angular Material for UI components.
TypeScript interfaces in contracts/ are generated from the API\'s
OpenAPI spec and mirror Shared.Contracts exactly.

**9.2 Folder structure**

> web/src/app/
>
> ├── app.config.ts provideRouter, provideHttpClient
>
> ├── app.routes.ts lazy-loaded feature routes
>
> ├── core/
>
> │ ├── interceptors/
>
> │ │ ├── auth.interceptor.ts Bearer token injection
>
> │ │ └── error.interceptor.ts Global error toasts
>
> │ ├── guards/auth.guard.ts
>
> │ └── services/auth.service.ts
>
> ├── api/ Typed services (mirror IGymTrackerClient)
>
> ├── contracts/ TS interfaces (OpenAPI-generated)
>
> └── features/
>
> ├── auth/
>
> ├── dashboard/
>
> ├── workout/
>
> ├── history/
>
> ├── routines/
>
> └── programs/

**9.3 Key decisions**

  ------------------------------------------------------------------
  **Decision**           **Rationale**
  ---------------------- -------------------------------------------
  Standalone components, Angular 18+ standard; better tree-shaking
  no NgModule            

  Signals for component  Fine-grained reactivity without RxJS in
  state                  templates

  RxJS in API services   HttpClient returns Observables; converted
  only                   to signals at the component boundary

  Lazy-loaded feature    Fast initial load; each feature bundled
  routes                 separately

  OpenAPI-generated      TypeScript interfaces stay in sync with the
  contracts              C# DTOs without manual effort

  Functional             Auth and error handling as pure functions,
  interceptors           no class boilerplate
  ------------------------------------------------------------------

**10. API Surface**

**10.1 Workout endpoints**

  -------------------------------------------------------------------------------------------
  **Method**   **Route**                              **Notes**
  ------------ -------------------------------------- ---------------------------------------
  POST         /api/sessions                          Create session, optionally from a
                                                      routine template

  PATCH        /api/sessions/{id}/end                 Mark session as complete

  GET          /api/sessions                          Paginated session history for the
                                                      authenticated user

  GET          /api/sessions/{id}                     Session detail with all logs and sets

  POST         /api/sessions/{id}/logs                Add an exercise log to an active
                                                      session

  POST         /api/sessions/{id}/logs/{logId}/sets   Log a set (weight, reps, warmup flag)

  GET          /api/exercises                         Full seeded exercise reference list

  POST         /api/routines                          Create a routine template

  GET          /api/routines/{id}                     Routine detail with ordered exercises

  POST         /api/routines/{id}/exercises           Add an exercise to a routine

  POST         /api/programs                          Create a training program

  POST         /api/programs/{id}/routines            Add a routine to a program with an
                                                      order index

  GET          /api/progress/{exerciseId}             Weight and reps over time for one
                                                      exercise
  -------------------------------------------------------------------------------------------

**11. CI Workflows**

Each workflow is path-filtered so only the affected area builds on a
given push. The shared packages are a special case: a change to
src/GymTracker.Shared.Contracts or src/GymTracker.Shared.HttpClient must
also trigger the mobile workflow, since those packages are consumed
there.

  -------------------------------------------------------------------------------
  **Workflow**   **Trigger paths**               **Steps**
  -------------- ------------------------------- --------------------------------
  backend.yml    src/\*\*                        dotnet restore → build → test →
                                                 pack Shared.\* → publish to feed

  mobile.yml     mobile/\*\*,                    dotnet restore (NuGet feed) →
                 src/GymTracker.Shared.\*/\*\*   build → test

  web.yml        web/\*\*                        npm ci → openapi-generate
                                                 contracts → ng build → ng test
  -------------------------------------------------------------------------------

The backend workflow packs and publishes the shared NuGet packages as
part of its pipeline. The mobile workflow pulls from the same feed, so a
backend CI run that changes contracts automatically makes the updated
package available for the next mobile build.

**12. Implementation Roadmap**

**Phase 1 --- Backend skeleton**

- Solution scaffold, all project references

- Common: IDispatcher, IValidator, ValidationException

- Identity: IdentityDbContext, register/login, JWT

- appsettings.json with IdentityConnection + DefaultConnection

- Verify: register → login → receive JWT

**Phase 2 --- Shared packages**

- Shared.Contracts: all DTO records, PagedResult\<T\>

- Shared.HttpClient: IGymTrackerClient + implementation +
  AddGymTrackerClient()

- Pack and publish to feed (GitHub Packages or local)

- Unit-test the client against a mock HttpMessageHandler

**Phase 3 --- Workout core (backend)**

- Domain entities, EF configurations, ApplicationDbContext, exercise
  seed data

- CreateSession, AddExerciseLog, LogSet features

- GetSessionHistory, GetSessionDetail queries

- Integration tests for all endpoints

**Phase 4 --- MAUI app**

- MauiProgram.cs: DI, Shell, AddGymTrackerClient() from NuGet

- Auth flow: login/register pages, SecureStorage JWT

- Dashboard, active session, set logging

- History: paginated list + session detail

- Routines and Programs: browse and create

- SQLite offline read cache for exercises and recent sessions

**Phase 5 --- Angular web app**

- ng new with standalone routing, SCSS

- OpenAPI contract generation from the running API

- Auth pages, interceptors, guard

- Dashboard, active workout stepper, history table

- Progress charts (Chart.js)

- Routines and Programs full CRUD

**Phase 6 --- Analytics**

- GetExerciseProgress: weight and reps over time

- GetVolumeByMuscleGroup: weekly volume breakdown

**Phase 7 --- Polish**

- Serilog structured logging, correlation IDs

- Global exception middleware

- OpenAPI / Swagger with JWT auth in UI

- CORS policy for Angular dev server

- MAUI dark mode and accessibility

- Angular PWA manifest

- Docker Compose for API + SQL Server local dev

**13. Package Summary**

**13.1 NuGet packages by project**

  ----------------------------------------------------------------------------
  **Project**         **Key packages**
  ------------------- --------------------------------------------------------
  Common              Microsoft.Extensions.DependencyInjection.Abstractions,
                      Logging.Abstractions

  Domain              (none)

  Application         Microsoft.EntityFrameworkCore (interfaces only)

  Identity            EF Core SQL Server, BCrypt.Net-Next,
                      System.IdentityModel.Tokens.Jwt, JwtBearer

  Infrastructure      EF Core SQL Server, Microsoft.Extensions.Configuration

  Api                 JwtBearer, Serilog.AspNetCore, Swashbuckle.AspNetCore

  Shared.Contracts    (none --- netstandard2.1 plain records)

  Shared.HttpClient   Microsoft.Extensions.Http

  MobileApp           Shared.Contracts (NuGet), Shared.HttpClient (NuGet),
                      CommunityToolkit.Mvvm, CommunityToolkit.Maui,
                      sqlite-net-pcl
  ----------------------------------------------------------------------------

**13.2 NPM packages (Angular)**

  -------------------------------------------------------------------------------
  **Package**                            **Purpose**
  -------------------------------------- ----------------------------------------
  \@angular/material + \@angular/cdk     UI component library

  rxjs                                   Reactive streams for HttpClient

  chart.js + ng2-charts                  Progress and volume charts

  \@openapitools/openapi-generator-cli   Generate contracts/ TS interfaces from
                                         API spec
  -------------------------------------------------------------------------------
