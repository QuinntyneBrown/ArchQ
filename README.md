# ArchQ - Architecture Decision Record Management

An open-source, multi-tenant platform for creating, reviewing, and managing Architecture Decision Records (ADRs) with collaborative workflows, Markdown editing, and full audit trails.

[Getting Started](#get-started) | [Documentation](docs/) | [Contributing](CONTRIBUTING.md) | [License](LICENSE)

---

## Why ArchQ

Architecture decisions are the most important technical artifacts a team produces, yet they are often lost in Confluence pages, Slack threads, or undocumented tribal knowledge. ArchQ provides a dedicated platform where teams can:

- **Write** ADRs using a split-pane Markdown editor with live preview and customizable templates
- **Review** decisions through a structured approval workflow with configurable thresholds
- **Discover** past decisions via full-text search, filtering, and tag-based categorization
- **Audit** every change with an immutable, append-only audit trail

```yaml
# Example: ADR lifecycle in ArchQ
ADR-007: Adopt Event-Driven Architecture
  Status:    Draft -> In Review -> Approved
  Author:    Alice Chen
  Approvers: Bob Kim (approved), Carol Mata (approved)
  Tags:      [architecture, events, messaging]
  Versions:  3
```

---

## Highlights

- **Multi-tenant isolation** -- Each organization gets a dedicated Couchbase scope with complete data separation at the database level. Zero cross-tenant data leakage by design.
- **Structured approval workflow** -- Finite state machine governing ADR lifecycle (Draft, In Review, Approved, Rejected, Superseded, Deprecated) with configurable approval thresholds and self-approval prevention.
- **Rich Markdown editor** -- Split-pane desktop layout with toolbar formatting, live preview via `marked` + DOMPurify sanitization, and tabbed mobile layout.
- **Role-based access control** -- Four roles (Admin, Author, Reviewer, Viewer) with a 21-permission matrix. Sensitive operations check live database roles, not just JWT claims.
- **Full-text search** -- Search across ADR titles, body content, and tags with relevance ranking and highlighted snippets.
- **Version history & diff** -- Every edit creates an immutable version snapshot. Compare any two versions with line-level diff highlighting.
- **Collaborative artifacts** -- Attach meeting notes, general notes, threaded comments, and architecture documents (PDF, PNG, SVG, DRAWIO) to any ADR.
- **Responsive design** -- Dark-themed UI with three breakpoints: desktop data table, tablet condensed view, and mobile card layout with bottom-sheet interactions.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | ASP.NET Core 9, Clean Architecture (Core / Application / Infrastructure / API) |
| **Frontend** | Angular 19, standalone components, SCSS |
| **Database** | Couchbase with scope-per-tenant isolation |
| **Auth** | JWT (HS256) with HttpOnly Secure SameSite=Strict cookies, bcrypt (cost 12) |
| **CQRS** | MediatR with FluentValidation pipeline behaviors |
| **E2E Tests** | Playwright with Page Object Model |
| **Containerization** | Docker & Docker Compose |

---

## Get Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 22 LTS](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Quick Start

```bash
# Clone the repository
git clone https://github.com/QuinntyneBrown/ArchQ.git
cd ArchQ

# Start Couchbase
docker compose up -d couchbase

# Build and run the API
dotnet build
dotnet run --project src/ArchQ.Api

# In another terminal, start the Angular dev server
cd src/ArchQ.Web
npm install
npx ng serve
```

Open [http://localhost:4200](http://localhost:4200) in your browser.

### Run E2E Tests

```bash
cd tests/ArchQ.E2E
npm install
npx playwright install chromium
npx playwright test
```

See the [Scaffolding Guide](docs/scaffolding-guide.md) for the complete project structure and setup instructions.

---

## Architecture

ArchQ follows **Clean Architecture** with four layers:

```
ArchQ.Core           -- Domain entities, interfaces, exceptions (no external dependencies)
ArchQ.Application    -- Use cases, CQRS commands/queries, validators, DTOs
ArchQ.Infrastructure -- Couchbase repositories, JWT service, email, file storage
ArchQ.Api            -- Controllers, middleware, DI wiring
ArchQ.Web            -- Angular 19 SPA (standalone components, dark theme)
```

### Key Architectural Decisions

| Decision | Rationale |
|----------|-----------|
| Couchbase scope-per-tenant | Database-level isolation prevents application bugs from leaking data across organizations |
| JWT in HttpOnly cookies | Prevents XSS-based token theft; SameSite=Strict mitigates CSRF |
| MediatR + FluentValidation | Decouples request handling from controllers; validation runs as pipeline behavior |
| Cursor-based pagination | Stable results when ADRs are created/updated between page loads |
| CAS optimistic concurrency | Prevents lost updates on concurrent ADR edits without database locks |

---

## Features

All 20 features are implemented and traced to requirements:

| # | Feature | Description |
|---|---------|-------------|
| 01 | Tenant Provisioning | Create organizations with Couchbase scope isolation |
| 02 | User Registration | Register with email/password, email verification, bcrypt hashing |
| 03 | User Login & Session | JWT access/refresh tokens, account lockout after 5 failures |
| 04 | Organization Switching | Switch active tenant without re-authentication |
| 05 | Role Management | Assign/revoke Admin, Author, Reviewer, Viewer roles |
| 06 | ADR Creation | Markdown editor with template pre-population |
| 07 | ADR Editing | Edit with version archiving and CAS concurrency |
| 08 | ADR List & Browse | Responsive table/cards with filters, sort, cursor pagination |
| 09 | ADR Search | Full-text search with relevance ranking and snippets |
| 10 | ADR Workflow | State machine: Draft, In Review, Approved, Rejected, Superseded, Deprecated |
| 11 | Approver Assignment | Assign reviewers with role and self-assignment validation |
| 12 | Approval Decisions | Record approve/reject with configurable threshold auto-transition |
| 13 | Meeting Notes | Structured meeting notes with date, attendees, Markdown body |
| 14 | General Notes | Free-form Markdown notes attached to ADRs |
| 15 | Threaded Comments | Nested comments with 15-minute edit window |
| 16 | Document Attachments | Upload PDF, PNG, SVG, DRAWIO (10 MB limit) |
| 17 | ADR Tagging | Autocomplete tags with usage counting |
| 18 | Version History & Diff | Browse versions and compare with line-level diff |
| 19 | Audit Trail | Immutable, append-only log of all actions |
| 20 | Security & Validation | Input sanitization, security headers, HTTPS, parameterized queries |

See [detailed designs](docs/detailed-designs/) for full specifications with C4 diagrams, sequence diagrams, and API contracts.

---

## Documentation

| Document | Description |
|----------|-------------|
| [Scaffolding Guide](docs/scaffolding-guide.md) | Project structure, setup instructions, conventions |
| [Detailed Designs](docs/detailed-designs/) | Feature specifications with diagrams and API contracts |
| [Specifications](docs/specs/) | L1 and L2 requirements with acceptance criteria |
| [ADRs](docs/adr/) | Architecture Decision Records for this project |
| [UI Designs](docs/designs/) | Exported design mockups |

---

## Community

ArchQ is created and maintained by [Quinntyne Brown](https://github.com/QuinntyneBrown).

- [Contributing Guide](CONTRIBUTING.md) -- How to contribute code, report bugs, and suggest features
- [Code of Conduct](CODE_OF_CONDUCT.md) -- Community standards and expectations
- [Security Policy](SECURITY.md) -- How to report security vulnerabilities
- [License](LICENSE) -- MIT License

---

## Trademarks

This project may contain references to trademarks or logos of third-party projects. Use of these trademarks must follow each project's trademark guidelines. Third-party trademarks or logos are subject to those third-party projects' policies.
