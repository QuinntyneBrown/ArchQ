# 1. Installation

This chapter gets ArchQ onto your machine and builds it once. It does **not** start the app — that happens in the next chapter.

> **Heads-up:** if you only want to *try* ArchQ with the smallest possible setup, skim ahead to [2. Running Locally](02-running-locally.md) — everything below is a one-time setup.

---

## 1.1 Prerequisites

Install these before you clone the repository. All are free.

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | **9.0** or newer | <https://dotnet.microsoft.com/download/dotnet/9.0> |
| Node.js | **22 LTS** or newer | <https://nodejs.org/> |
| Docker Desktop | latest | <https://www.docker.com/products/docker-desktop/> |
| Git | any modern version | <https://git-scm.com/downloads> |

### Verify the install

```bash
$ dotnet --version          # expect 9.0.x or newer
$ node --version            # expect v22.x or newer
$ npm --version             # expect 10.x or newer
$ docker --version          # expect Docker version 24+
$ git --version             # any modern git
```

If any of these fail, revisit the installer for that tool.

### Platform notes

- **Windows 10/11** — Use Docker Desktop with the WSL2 backend. Ensure virtualization is enabled in BIOS.
- **macOS (Apple Silicon)** — All tools ship ARM64 builds. Couchbase 7.6 has a native ARM image.
- **Linux** — Install Docker Engine + Docker Compose plugin. On Ubuntu: `sudo apt install docker.io docker-compose-plugin`.

---

## 1.2 Clone the repository

```bash
$ git clone https://github.com/QuinntyneBrown/ArchQ.git
$ cd ArchQ
```

You should see this layout (abbreviated):

```
ArchQ/
├── docker/                 # Dockerfiles for api/web/couchbase
├── docs/                   # Documentation (you are here)
├── eng/                    # Build & scripts
├── src/
│   ├── ArchQ.Api/          # ASP.NET Core API host
│   ├── ArchQ.Application/  # CQRS commands, queries, validators
│   ├── ArchQ.Core/         # Domain entities
│   ├── ArchQ.Infrastructure/
│   └── ArchQ.Web/          # Angular 19 SPA
├── tests/
├── docker-compose.yml
└── ArchQ.slnx
```

---

## 1.3 Restore backend dependencies

```bash
$ dotnet restore
$ dotnet build
```

The first build pulls NuGet packages (MediatR, FluentValidation, Couchbase SDK, JwtBearer, BCrypt). Expect it to take 1–2 minutes on the first run and a few seconds thereafter.

> **If the build fails** with a missing SDK version, run `dotnet --list-sdks` and install the one referenced in `global.json` (if present) or the latest .NET 9 SDK.

---

## 1.4 Install the Angular frontend

```bash
$ cd src/ArchQ.Web
$ npm install
$ cd ../..
```

This installs Angular 19, `marked`, `DOMPurify`, `highlight.js`, and the Angular CDK. First run takes ~2 minutes.

---

## 1.5 Install Playwright (optional, only for E2E tests)

```bash
$ cd tests/ArchQ.E2E
$ npm install
$ npx playwright install chromium
$ cd ../..
```

Playwright downloads ~170 MB of browser binaries. Skip this step if you don't plan to run E2E tests locally.

---

## 1.6 Install the ArchQ CLI (optional)

ArchQ ships a companion CLI for administrative tasks (seeding tenants, generating templates, etc.).

```bash
# From the repository root
$ eng/scripts/install-cli.bat           # Windows
# or, on macOS/Linux:
$ dotnet tool install --global --add-source ./artifacts ArchQ.Cli
```

Verify:

```bash
$ archq --help
```

---

## 1.7 You're done

Everything needed to run ArchQ is on disk. The next chapter boots the stack and shows the login screen.

**Next:** [2. Running Locally →](02-running-locally.md)
