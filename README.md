# 🛡️ SentinelaAPI

> Audit log, anomaly detection and CVE monitoring system for .NET APIs — built with ASP.NET Core 8, Clean Architecture and Security-by-Design principles.

## 🌐 Live Demo

**API:** https://sentinelaapi-production.up.railway.app

> Swagger UI available at the root URL — all endpoints documented and testable.

## ✨ Features

- **Automatic Audit Logging** — every HTTP request is intercepted by a middleware and persisted automatically, with no controller code required
- **Anomaly Detection** — real-time detection of brute force attacks, port scanners and suspicious activity using time-window analysis
- **CVE Monitor** — scans NuGet packages against the NVD (National Vulnerability Database) and generates vulnerability reports
- **Clean Architecture** — strict layer separation (Domain, Application, Infrastructure, Api) following SOLID principles

## 🏗️ Architecture

```
SentinelaAPI/
├── SentinelaAPI.Api            # Controllers, Middleware, DTOs
├── SentinelaAPI.Application    # Services, Interfaces, Use Cases
├── SentinelaAPI.Domain         # Entities, Enums, Contracts
├── SentinelaAPI.Infrastructure # EF Core, Repositories, Migrations
└── SentinelaAPI.Tests          # xUnit, Moq, FluentAssertions
```

## 🔍 Anomaly Detection Rules

| Rule | Condition | Window |
|------|-----------|--------|
| Brute Force | 5+ failed logins from same IP | 5 minutes |
| Scanner | 50+ requests from same IP | 1 minute |
| Suspicious Activity | 10+ 401/403 errors from same user | 10 minutes |

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Running locally

```bash
# Clone the repository
git clone https://github.com/carlosemagalhaes/SentinelaAPI.git
cd SentinelaAPI/SentinelaAPI

# Update the connection string in appsettings.json
# "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SentinelaAPI;Trusted_Connection=True;"

# Apply migrations
dotnet ef database update --project SentinelaAPI.Infrastructure --startup-project SentinelaAPI.Api

# Run the API
dotnet run --project SentinelaAPI.Api
```

Open `https://localhost:7012/swagger` to explore the API.

### Running tests

```bash
dotnet test
```

## 📡 API Endpoints

### Audit Log
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/AuditLog` | Get all audit logs |
| GET | `/api/AuditLog/anomalies` | Get logs flagged as anomalies |
| GET | `/api/AuditLog/user/{userId}` | Get logs by user |
| POST | `/api/AuditLog` | Create audit log manually |

### Anomaly Alerts
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/AnomalyAlert` | Get all anomaly alerts |
| GET | `/api/AnomalyAlert/unresolved` | Get unresolved alerts |
| PATCH | `/api/AnomalyAlert/{id}/resolve` | Resolve an alert |

### CVE Monitor
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/CveMonitor/scan-package` | Scan a single NuGet package |
| GET | `/api/CveMonitor/scan-project` | Scan all packages in a .csproj |
| GET | `/api/CveMonitor/summary` | Get vulnerability summary |

## 🛠️ Tech Stack

- **Runtime** — .NET 8 / ASP.NET Core
- **ORM** — Entity Framework Core 8 with SQL Server (local) / PostgreSQL (production)
- **Documentation** — Swagger / OpenAPI
- **Testing** — xUnit · Moq · FluentAssertions
- **Deploy** — Railway + PostgreSQL
- **Security** — OWASP principles · RBAC · Security-by-Design
- **External API** — NVD (National Vulnerability Database) by NIST

## 📋 Security Concepts Applied

- Security-by-Design architecture
- Audit trail for LGPD compliance
- Real-time threat detection (SIEM patterns)
- CVE/CVSS vulnerability scoring
- IP extraction with X-Forwarded-For support
- Scoped vs Singleton lifetime management

## 🧪 Tests

13 automated tests covering:

- `AuditLog` entity — creation, encapsulation, MarkAsAnomaly, all action types
- `AnomalyDetectionService` — all 3 detection rules, resolve flow and edge cases

```bash
dotnet test
# 13 passed, 0 failed
```

## 👤 Author

**Carlos E. Magalhães**
- LinkedIn: [linkedin.com/in/carlosemagalhaes](https://linkedin.com/in/carlosemagalhaes)
- GitHub: [github.com/carlosemagalhaes](https://github.com/carlosemagalhaes)

---

> Built as a portfolio project to demonstrate C# back-end development with real-world security patterns.
````