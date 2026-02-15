# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET Aspire-based firmware/software updater system for ESP32 devices. The application uses a distributed architecture with a web frontend, API backend, and PostgreSQL database for managing software versions and device updates.

## Project Structure

- **Updater.AppHost**: .NET Aspire orchestrator that manages service dependencies and local development
- **Updater.ApiService**: Backend API service providing update endpoints, file uploads, and device management
- **Updater.Web**: Blazor Server frontend with Google OAuth authentication and Redis caching
- **Updater.ServiceDefaults**: Shared service configuration and Aspire defaults
- **Updater.Tests**: xUnit tests using Aspire.Hosting.Testing

## Common Commands

### Build and Run
```bash
# Build entire solution
dotnet build Updater.sln

# Run the Aspire AppHost (starts all services)
dotnet run --project Updater.AppHost

# Run API service standalone
dotnet run --project Updater.ApiService

# Run web frontend standalone
dotnet run --project Updater.Web
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add <MigrationName> --project Updater.ApiService

# Apply migrations
dotnet ef database update --project Updater.ApiService

# Revert migration
dotnet ef database update <PreviousMigrationName> --project Updater.ApiService
```

### Local Database Setup (Docker)
```bash
# Start PostgreSQL container
docker run --name updater-postgres \
  -e POSTGRES_DB=UpdaterDB \
  -e POSTGRES_USER=updater_admin \
  -e POSTGRES_PASSWORD=your_password \
  -p 5432:5432 -d postgres:16

# Configure user secrets for local development
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Database=UpdaterDB;Username=updater_admin;Password=your_password;Port=5432" \
  --project Updater.ApiService

# Apply migrations to local database
dotnet ef database update --project Updater.ApiService

# Verify tables created
docker exec updater-postgres psql -U updater_admin -d UpdaterDB -c "\dt"
```

## Architecture Overview

### Service Communication
- **AppHost** orchestrates services using .NET Aspire
- **Web** communicates with **ApiService** via HttpClient (configured at Program.cs:24)
- **Redis** provides output caching for the web frontend
- **PostgreSQL** is the backend database (configured via user secrets)

### Key Components

**ApiService** (Updater.ApiService/Program.cs):
- Minimal API endpoints for firmware management
- Token-based authentication system
- Binary storage in PostgreSQL database
- In-memory caching (UserCache, SoftwareCache) for performance

**Database Models** (Updater.ApiService/Database/Models):
- `User`: User accounts with tokens
- `Software`: Versioned software packages (Name, Platform, VerMajor)
- `Chip`: Registered devices with current software versions
- `Binary`: Raw binary data for software packages

**Services** (Updater.ApiService/Services):
- `SoftwareService`: Core update logic, version management, cleanup
- `DeviceService`: Device/chip registration and tracking
- `UserService`: User authentication and token management

### Update Flow
1. Device checks for updates: `GET check/{token}/{swName}/{chipId}/{platform}`
2. If available, device downloads: `GET /{token}/{swName}/{chipId}/{platform}`
3. After successful update, device confirms: `GET /ud/{token}/{swName}/{chipId}/{platform}`
4. System tracks device state and prevents redundant downloads via caching

### Uploading New Software Versions

**Via Web UI (Blazor):**
1. Navigate to `/upload` page (requires Google OAuth authentication)
2. Select firmware file (.bin, .hex, .elf - max 10MB)
3. Choose target platform (ESP32, ESP32-C3, ESP32-S3, ESP32-C6, ESP32-P4)
4. Click "Upload firmware" button

**Via API Endpoint:**
```bash
POST /upload
Content-Type: multipart/form-data

Parameters:
- file: [binary file]
- nid: [user nameidentifier] or token: [user token]
- filename: [software name]
- platform: [target ESP32 platform]
```

**Upload Process (SoftwareService.cs:11-51):**
1. Authenticates user via `nid` (Google NameIdentifier) or `token`
2. Finds existing software with same Name and Platform
3. Increments version: `newVersion = existingSoftware.VerMajor + 1`
4. Creates new Software record with incremented VerMajor
5. Saves binary data to Binarys table
6. Clears cache for this software
7. Returns confirmation: `"{swName}-{platform} v{nextVersion}"`

**Important:**
- Each upload creates a NEW version (never overwrites)
- Filename becomes the software Name (used for device matching)
- Old versions remain in database for rollback capability
- All devices with this software will see update on next check

### Web Frontend
- Blazor Server application with Razor Pages
- Google OAuth authentication (configured via user secrets)
- Pages: Index, Upload, Devices, Docs, GetStarted, Manual, Token, Terms, Privacy
- Components: ButtonComponent, CardComponent, PanelComponent, StatCardComponent, ProductBanner

## Configuration

### User Secrets
Both ApiService and Web use user secrets for sensitive configuration:
- **ApiService**: Database connection string (`DefaultConnection`), `MasterToken`
- **Web**: Google OAuth credentials (`Google:ClientId`, `Google:ClientSecret`)

Use `dotnet user-secrets` command to manage secrets for each project.

**Configuration Priority (Development mode):**
1. User Secrets (highest priority - use for local development)
2. Environment variables
3. appsettings.Development.json
4. appsettings.json (lowest priority)

When running in debug/development, user secrets override appsettings.json values.

### Aspire Dashboard
When running via AppHost, access the Aspire dashboard to monitor services, logs, and dependencies.

## Important Implementation Details

### Version Management
Software versions use a simple integer (`VerMajor`) that increments with each upload. The system maintains:
- Latest version for new device registrations
- Current installed version per chip
- Cleanup logic (SoftwareService.cs:126-168) removes old versions not in use

### Caching Strategy
Two-level caching:
1. In-memory caches (UserCache, SoftwareCache) prevent redundant database queries
2. Redis output caching for web responses

### Supported Platforms
The system supports ESP32 variants: ESP32-C3, ESP32, ESP32-S3, ESP32-C6, ESP32-P4 (SoftwareService.cs:83-86)

### Cleanup Process
The `/clear/{token}` endpoint (requires MasterToken) removes:
- Versions that are not the newest
- Versions not installed on any device
- Versions not "one version below" an installed version (rollback safety)
- Orphaned binaries without corresponding software entries

## Development Notes

- Platform uses .NET 8.0 target framework
- Entity Framework Core with PostgreSQL provider (Npgsql)
- Aspire 9.0.0 for orchestration
- xUnit for testing with Aspire.Hosting.Testing integration
