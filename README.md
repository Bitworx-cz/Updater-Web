## Updater – Firmware update platform for microcontrollers (MVP)

Updater is a **minimal yet complete platform for managing firmware updates for fleets of microcontroller-based devices** (e.g. ESP32).  
It consists of:
- **`Updater.ApiService`** – a minimal REST API that distributes firmware binaries, tracks versions, and serves devices.
- **`Updater.Web`** – a Blazor-based web UI for developers to upload firmware and see their devices.
- **`Updater.AppHost`** – a .NET Aspire host that wires the services together.
- **`Updater.ServiceDefaults`** – shared observability, health checks, and resilience configuration.

The MVP focuses on making it **trivial for a single developer or small team to ship OTA updates** to their devices with just a web dashboard and a few HTTP calls.

### 🚀 Try it now

**Updater is already hosted and available for free at [updater.bitworx.cz](https://updater.bitworx.cz)**  
Sign in with your Google account and start managing firmware updates for your microcontroller fleet in minutes.

---

## Core problem the MVP solves

Microcontroller projects often end up with:
- **Manual flashing** (cables on the bench, one board at a time).
- **Ad‑hoc binaries** stored on local disks or CI artifacts with no clear "what runs where".
- **No simple way** to roll out a new firmware to many devices in the field.

The Updater MVP solves this by providing:
- A **single backend** that stores firmware binaries and versions.
- A **simple device protocol** (three endpoints) for checking, downloading, and confirming updates.
- A **small web UI** where developers can see their devices and upload new firmware.

---

## MVP domain model & concepts

At MVP level, the main concepts are:

- **User**
  - Identified by an external `nid` (e.g. from Google login).
  - Gets a **long‑lived token** from the API (`/{nid}/token`) that authorizes use of their projects and devices.

- **Project**
  - Logical container for a set of devices and firmware.
  - Backed by API endpoints (e.g. `/projects/{token}`) so a user can **list, create, and delete** their projects.

- **Group**
  - Sub‑division of a project, typically representing a **coherent fleet segment** (product variant, environment, customer, etc.).
  - Each group has a **Target Software** (the firmware version that devices in that group should run).
  - Managed through endpoints like `/projects/{projectId}/groups/{token}` and `/groups/{groupId}/target-software/{token}`.

- **Device**
  - A physical microcontroller (e.g. ESP32) identified by a `deviceId` and `MacAddress`.
  - Belongs to a single group.
  - Tracks:
    - `CurrentSoftwareId` – what is actually running.
    - `PendingSoftwareId` – what the device is in the process of updating to.
  - Device registration and group assignment are mediated through `DeviceService`, with user access checked via projects.

- **Software**
  - A **versioned firmware entry** (name + major version) scoped to a group.
  - Each upload automatically increments `VerMajor` within that group.
  - Linked to a `Binary` (the raw bytes of the firmware).

- **Binary**
  - The **actual firmware image** stored as bytes (`RawBinary`), keyed by `SoftwareId`.

- **Device activity**
  - The API logs lightweight **device activity entries** for update-related endpoints (check, download, update done) so that usage can be inspected later.

---

## Device update protocol (MVP)

The firmware update flow for a device is intentionally small and opinionated:

- **1. Check if an update is available**
  - Endpoint: `GET /device/check-update/{token}/{deviceId}/{groupId}`
  - Device sends:
    - `token` – user's token (authorizes the project).
    - `deviceId` – its own logical ID.
    - `groupId` – the group it belongs to.
  - API logic:
    - Looks up the device and its group.
    - Compares group's **Target Software** with device's `CurrentSoftwareId`.
    - Returns:
      - `200 OK` if an update is available.
      - `404 NotFound` if there is no newer version or on error.

- **2. Download the firmware**
  - Endpoint: `GET /device/download/{token}/{deviceId}/{groupId}`
  - On success:
    - Sets `PendingSoftwareId` on the device to the group's current Target Software.
    - Returns the binary firmware as `application/octet-stream` (default filename `firmware.bin`).
  - Devices are expected to:
    - Download the file.
    - Flash it using their own OTA mechanism.

- **3. Confirm update completion**
  - Endpoint: `POST /device/update-done/{token}/{deviceId}/{groupId}`
  - When the device has successfully flashed and rebooted:
    - API moves `PendingSoftwareId` → `CurrentSoftwareId`.
    - Clears `PendingSoftwareId`.
    - Updates timestamps and logs the activity.

These three endpoints form the **entire OTA contract** for the MVP: simple, stateless requests with minimal assumptions about the device firmware stack.

---

## Firmware lifecycle (MVP)

From a developer's perspective, pushing firmware is also kept minimal:

- **Upload new firmware**
  - Endpoint: `POST /upload`
  - Expected form fields:
    - `token` – user token (must have access to the project).
    - `filename` – human-friendly name for the firmware.
    - `projectId` – ID of the project.
    - `groupId` – ID of the target group.
    - `file` – binary file payload.
  - Behavior:
    - Validates user access to the project and group.
    - Computes the **next major version** (`VerMajor`) for that group.
    - Stores:
      - `Software` row (metadata, version, uploader, timestamps).
      - `Binary` row (raw firmware bytes).
    - Automatically sets the group's **Target Software** to this new version.

- **Clean up unused firmware**
  - Endpoint: `GET /clear/{token}`
  - Protected by a **master token** (`Configuration.MasterToken`).
  - Removes:
    - `Software` entries that are neither:
      - Target of any group nor
      - Currently running on any device.
    - Orphaned `Binary` rows that no longer have a `Software` owner.

This gives the MVP a **complete lifecycle**: upload → serve → confirm → garbage collect, without overwhelming features like staged percentages or environments.

---

## Web UI capabilities (MVP)

`Updater.Web` is a **server-side Blazor** app that provides a basic but polished front end:

- **Authentication**
  - Uses **Google login** with cookie-based auth.
  - After login, the UI resolves the user's `nid` and obtains their API token from `/{nid}/token`.

- **Devices overview**
  - Page: `/devices`
  - Calls `GET /devices/{token}` on the API.
  - Shows:
    - Device count and simple stats (number of device types, software types).
    - A table with:
      - `MAC address`
      - `Software` name
      - `Software version`
      - `Microcontroller` type.

- **Landing / marketing page**
  - Root page (`/`) explains:
    - What the updater does for microcontroller projects.
    - That it works both:
      - From **Arduino IDE / manual binaries**, and
      - From **CI pipelines** (e.g. GitHub Actions pushing to `/upload`).
  - Provides calls-to-action to:
    - Obtain a token.
    - Integrate firmware upload with the API.

The MVP UI intentionally keeps management simple: **see your devices, push a new firmware, and let the devices pick it up.**

---

## Architectural overview (MVP)

At a high level:

- **API service (`Updater.ApiService`)**
  - ASP.NET Core minimal API.
  - Uses EF Core with PostgreSQL (`UseNpgsql`) for persistence.
  - Contains services such as:
    - `UserService` – token management and user caching.
    - `ProjectService` / `GroupService` – ownership boundaries and group targeting.
    - `SoftwareService` – firmware upload/versioning, download, and cleanup.
    - `DeviceService` – registration, lookup, and group assignment.
  - Logs **device activity** and exposes minimal management endpoints for projects, groups, software versions, and devices.

- **Web frontend (`Updater.Web`)**
  - Razor Pages + server-side Blazor.
  - Auth via Google + cookies.
  - Uses an `HttpClient` named `"apiservice"` to talk to the API.

- **App host (`Updater.AppHost`)**
  - A .NET Aspire `DistributedApplication` that:
    - Declares the API and web projects.
    - Configures dependencies and startup order.

- **Service defaults (`Updater.ServiceDefaults`)**
  - Provides:
    - OpenTelemetry logging/metrics/tracing.
    - Health checks (e.g. `/health`, `/alive` in development).
    - Service discovery and HttpClient resilience defaults.

Taken together, these components deliver an **MVP-ready firmware update platform** that is:
- Focused on **ESP32-style microcontroller fleets**.
- Small enough to understand and host easily.
- Extensible for future features like staged rollouts, multiple environments, and richer device telemetry.
