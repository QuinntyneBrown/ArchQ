# ArchQ User Guide

A complete, end-to-end guide for installing, running, using, and deploying **ArchQ** — the multi-tenant Architecture Decision Record management platform.

![ArchQ dashboard](../designs/exports/Ae0g2.png)

---

## Who this guide is for

| Audience | Recommended chapters |
|----------|----------------------|
| **Developers** setting up ArchQ on a laptop | [1. Installation](01-installation.md) -> [2. Running Locally](02-running-locally.md) |
| **Authors & Reviewers** using the product day-to-day | [3. Using ArchQ](03-using-archq.md) -> [4. ADR Lifecycle](04-adr-lifecycle.md) |
| **Operators** shipping ArchQ to the cloud | [5. Deploying to Azure (Cheapest)](05-deploying-to-azure.md) |
| **Tenant Admins** managing users, roles, and templates | [6. Administration](06-administration.md) |
| **Anyone hitting a wall** | [7. Troubleshooting & FAQ](07-troubleshooting.md) |

---

## Table of contents

1. [Installation](01-installation.md) — prerequisites, cloning, dependencies.
2. [Running Locally](02-running-locally.md) — Couchbase via Docker, API, Angular dev server.
3. [Using ArchQ](03-using-archq.md) — sign-up, first tenant, creating your first ADR.
4. [ADR Lifecycle: Draft -> Approved](04-adr-lifecycle.md) — the full workflow with every state transition.
5. [Deploying to Azure (Cheapest Plan)](05-deploying-to-azure.md) — a step-by-step, cost-minimised deploy.
6. [Administration](06-administration.md) — roles, approval thresholds, templates, tags.
7. [Troubleshooting & FAQ](07-troubleshooting.md) — common errors and how to fix them.

---

## What ArchQ gives you at a glance

| Capability | Where to read more |
|------------|-------------------|
| Markdown ADR editor with live preview | [Using ArchQ: Writing an ADR](03-using-archq.md#5-write-your-first-adr) |
| Structured approval workflow (Draft -> In Review -> Approved) | [ADR Lifecycle](04-adr-lifecycle.md) |
| Multi-tenant isolation with Couchbase scopes | [Administration: Tenancy](06-administration.md#tenancy-model) |
| Role-based access (Admin, Author, Reviewer, Viewer) | [Administration: Roles](06-administration.md#roles-and-permissions) |
| Meeting notes, comments, document uploads | [Using ArchQ: Collaborating](03-using-archq.md#7-collaborate-on-an-adr) |
| Full-text search, tags, version history | [Using ArchQ: Browsing](03-using-archq.md#6-browsing-and-searching) |

---

## Conventions used in this guide

- Commands prefixed with `$` are run from the **repository root** unless stated otherwise.
- Commands prefixed with `>` are run from **Azure Cloud Shell** or a local `az` CLI.
- Shell examples use **bash**; adapt paths for PowerShell where needed.
- Screenshots reflect the current dark theme. Labels and microcopy may differ slightly as the product evolves.

---

## Getting help

- Bugs and feature requests: open an issue at <https://github.com/QuinntyneBrown/ArchQ/issues>.
- Security vulnerabilities: see [SECURITY.md](../../SECURITY.md) in the repository root.
- Contributing: see [CONTRIBUTING.md](../../CONTRIBUTING.md).
