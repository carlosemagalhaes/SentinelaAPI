# 🛡️ SentinelaAPI

> **Security observability infrastructure for .NET APIs** — transparent audit logging, real-time anomaly detection and CVE monitoring, built with ASP.NET Core 8, Clean Architecture and Security-by-Design principles.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4)](https://learn.microsoft.com/aspnet/core)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4)](https://learn.microsoft.com/ef/core)
[![Tests](https://img.shields.io/badge/tests-13_passed-brightgreen)](https://github.com/carlosemagalhaes/SentinelaAPI/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

🌐 **Live:** [sentinelaapi-production.up.railway.app](https://sentinelaapi-production.up.railway.app) · Swagger UI available at root URL.

---

## The Problem it Solves

Most APIs have no visibility into *what* is happening, *who* did it, and *when* something suspicious started. SentinelaAPI plugs directly into the ASP.NET Core pipeline and provides three layers of security observability — **without requiring any code changes in your controllers.**

---

## Core Features

### 🔎 Automatic Audit Logging
A custom `AuditLogMiddleware` intercepts **every HTTP request** at the pipeline level before it reaches any controller. Each request is persisted with full context — IP address, user identity, endpoint, status code and timestamp — enabling complete traceability for compliance (LGPD/GDPR) and forensic analysis.

```
Request → AuditLogMiddleware → Controller → Response
              ↓
         AuditLog persisted automatically
         (zero controller code required)
```

### 🚨 Real-Time Anomaly Detection
A time-window analysis engine runs on every incoming request and classifies threats in real time:

| Rule | Trigger | Window | Threat Type |
|------|---------|--------|-------------|
| Brute Force | 5+ failed logins from same IP | 5 min | Credential attack |
| Port Scanner | 50+ requests from same IP | 1 min | Reconnaissance |
| Suspicious Activity | 10+ 401/403 from same user | 10 min | Privilege probing |

When a rule fires, an `AnomalyAlert` is created and the originating log is flagged — giving security teams a full audit trail from detection back to the raw requests.

### 🔬 CVE Monitor
Scans `.csproj` NuGet packages against the **NVD (National Vulnerability Database)** by NIST and returns CVSS-scored vulnerability reports per dependency. Useful for CI/CD pipelines and dependency audits.

---

## Architecture

Clean Architecture with strict layer boundaries — dependencies always flow inward toward the Domain.

```
SentinelaAPI/
├── SentinelaAPI.Api             # HTTP layer: Controllers, Middleware, DTOs
├── SentinelaAPI.Application     # Business logic: Services, Interfaces, Use Cases
├── SentinelaAPI.Domain          # Core: Entities, Enums, Domain contracts
├── SentinelaAPI.Infrastructure  # Data: EF Core, Repositories, Migrations
└── SentinelaAPI.Tests           # xUnit · Moq · FluentAssertions (13 tests)
```

**Key design decisions:**
- `AuditLogMiddleware` lives in the **Api** layer — it's an infrastructure concern, not business logic
- `AnomalyDetectionService` lives in **Application** — it's a use case with no framework dependencies, making it fully testable
- Repositories are defined as interfaces in **Application** and implemented in **Infrastructure** — the business layer never touches EF Core directly

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 8 / ASP.NET Core 8 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server (local) · PostgreSQL (production) |
| Testing | xUnit · Moq · FluentAssertions |
| Documentation | Swagger / OpenAPI |
| Deploy | Railway |
| External API | NVD / NIST |

---

## API Reference

<details>
<summary><strong>Audit Log</strong></summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/AuditLog` | All audit logs |
| `GET` | `/api/AuditLog/anomalies` | Logs flagged as anomalies |
| `GET` | `/api/AuditLog/user/{userId}` | Logs filtered by user |
| `POST` | `/api/AuditLog` | Manually create an audit log |
</details>

<details>
<summary><strong>Anomaly Alerts</strong></summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/AnomalyAlert` | All anomaly alerts |
| `GET` | `/api/AnomalyAlert/unresolved` | Active unresolved alerts |
| `PATCH` | `/api/AnomalyAlert/{id}/resolve` | Mark alert as resolved |
</details>

<details>
<summary><strong>CVE Monitor</strong></summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/CveMonitor/scan-package` | Scan a single NuGet package |
| `GET` | `/api/CveMonitor/scan-project` | Scan all packages in a `.csproj` |
| `GET` | `/api/CveMonitor/summary` | Vulnerability summary report |
</details>

---

## Getting Started

**Prerequisites:** .NET 8 SDK · SQL Server (LocalDB or full instance)

```bash
# 1. Clone
git clone https://github.com/carlosemagalhaes/SentinelaAPI.git
cd SentinelaAPI/SentinelaAPI

# 2. Configure connection string in appsettings.json
# "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SentinelaAPI;Trusted_Connection=True;"

# 3. Apply migrations
dotnet ef database update --project SentinelaAPI.Infrastructure --startup-project SentinelaAPI.Api

# 4. Run
dotnet run --project SentinelaAPI.Api
```

Open [`https://localhost:7012/swagger`](https://localhost:7012/swagger) — all endpoints are documented and testable.

```bash
# Run tests
dotnet test
# 13 passed, 0 failed
```

---

## Security Concepts Applied

- **Security-by-Design** — security concerns are first-class architectural decisions, not afterthoughts
- **Audit trail for LGPD/GDPR compliance** — full request history with user identity and IP
- **SIEM patterns** — real-time threat correlation across multiple events and time windows
- **CVE/CVSS scoring** — automated vulnerability assessment against the NVD database
- **IP extraction with X-Forwarded-For support** — proxy-aware client identification
- **Scoped vs Singleton lifetime management** — correct DI lifetimes prevent data leaks between requests
- **OWASP principles** — input validation, proper error handling, no sensitive data in logs

---

## Author

**Carlos E. Magalhães** — Backend developer with focus on application security and Security-by-Design practices.

[![LinkedIn](https://img.shields.io/badge/LinkedIn-carlosemagalhaes-0A66C2?logo=linkedin)](https://linkedin.com/in/carlosemagalhaes)
[![GitHub](https://img.shields.io/badge/GitHub-carlosemagalhaes-181717?logo=github)](https://github.com/carlosemagalhaes)
