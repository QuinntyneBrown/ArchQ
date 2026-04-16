# 2. Running ArchQ Locally

By the end of this chapter you will have the **login page** open in your browser and be ready to register your first account.

There are two supported modes: a **fast dev loop** (for active development) and **all-in-one Docker** (for a quick demo).

---

## 2.1 Mode A — Fast dev loop (recommended for contributors)

Three processes run in parallel: Couchbase (in Docker), the API (locally), and the Angular dev server (locally).

### Step 1: Start Couchbase

```bash
$ docker compose up -d couchbase
```

This boots a single-node Couchbase cluster exposing:

- **8091** — Admin console (<http://localhost:8091>)
- **11210** — Data service

First boot downloads ~1 GB. After that it starts in ~15 seconds. Wait until the health check turns green:

```bash
$ docker compose ps
# couchbase should show "healthy"
```

### Step 2: One-time Couchbase bootstrap

The `docker/couchbase/init-cluster.sh` script creates the `archq` bucket, primary index, and admin user. It runs automatically on first start. If you ever need to re-run it manually:

```bash
$ docker compose exec couchbase bash /opt/couchbase/init-cluster.sh
```

Open <http://localhost:8091> and sign in with `Administrator` / `password123` to confirm the `archq` bucket exists.

### Step 3: Run the API

In a new terminal:

```bash
$ dotnet run --project src/ArchQ.Api
```

Wait for:

```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

Smoke test:

```bash
$ curl http://localhost:5000/health
# expect: {"status":"Healthy"}
```

### Step 4: Run the Angular dev server

In a third terminal:

```bash
$ cd src/ArchQ.Web
$ npx ng serve
```

Wait for:

```
** Angular Live Development Server is listening on localhost:4200 **
```

Open <http://localhost:4200>. You should see the sign-in screen:

![ArchQ login screen](../designs/exports/RWz27.png)

---

## 2.2 Mode B — All-in-one Docker

One command, three containers, no local .NET or Node dependencies at runtime (you still need Docker).

```bash
$ docker compose up --build
```

This builds and runs:

- `couchbase` — database on ports 8091–8097
- `api` — API on port 5000
- `web` — Nginx-hosted Angular build on port 4200

Open <http://localhost:4200>.

> **When to use this mode:** first-time demos, CI pipelines, or to confirm the production Docker images build cleanly. Hot reload is **not** available in this mode — rebuild after every change.

---

## 2.3 Default ports

| Service | Port | URL |
|---------|------|-----|
| Angular SPA | 4200 | <http://localhost:4200> |
| API | 5000 | <http://localhost:5000> |
| Swagger / OpenAPI | 5000 | <http://localhost:5000/swagger> |
| Couchbase Web Console | 8091 | <http://localhost:8091> |
| Couchbase Data | 11210 | (binary protocol) |

---

## 2.4 Stopping and cleaning up

```bash
# Stop everything but keep data volumes
$ docker compose down

# Nuke data volumes too (wipes all ADRs, users, tenants)
$ docker compose down -v
```

To stop an individual service: `docker compose stop couchbase`.

---

## 2.5 Environment configuration

Local defaults live in `src/ArchQ.Api/appsettings.Development.json`. You rarely need to change them for local dev, but these are the knobs:

| Setting | Default | What it does |
|---------|---------|--------------|
| `Couchbase:ConnectionString` | `couchbase://localhost` | Couchbase endpoint |
| `Couchbase:Username` | `Administrator` | Admin username |
| `Couchbase:Password` | `password123` | Admin password (dev only!) |
| `Jwt:Key` | dev-only key | HS256 signing key. **Must be 32+ chars in production.** |
| `Jwt:AccessTokenMinutes` | `15` | Access token lifetime |
| `Jwt:RefreshTokenDays` | `14` | Refresh token lifetime |
| `Cors:AllowedOrigins` | `http://localhost:4200` | SPA origin |

Production-safe overrides go in environment variables (see [5. Deploying to Azure](05-deploying-to-azure.md)).

---

## 2.6 Running the tests

```bash
# Unit tests
$ dotnet test tests/ArchQ.UnitTests

# Integration tests (requires Couchbase running)
$ dotnet test tests/ArchQ.IntegrationTests

# E2E (requires Angular + API running)
$ cd tests/ArchQ.E2E
$ npx playwright test
$ npx playwright show-report        # open the HTML report
```

---

## 2.7 What's next

You have ArchQ running. Head to the next chapter to register an account, create your tenant, and write your first ADR.

**Next:** [3. Using ArchQ →](03-using-archq.md)
