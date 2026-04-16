# ArchQ Solution Scaffolding Guide

This document defines the folder structure and step-by-step scaffolding instructions for the ArchQ solution — a multi-tenant Architecture Decision Record (ADR) management system.

**Tech Stack:**
- **Backend:** ASP.NET Core 9 Web API (Clean Architecture)
- **Frontend:** Angular 19 (standalone components)
- **Database:** Couchbase (Scopes & Collections per tenant)
- **E2E Testing:** Playwright with Page Object Model
- **Containerization:** Docker & Docker Compose

---

## Folder Structure

```
ArchQ/
├── docs/                                    # Existing documentation
│   ├── adr/                                 # Architecture Decision Records
│   ├── designs/                             # UI design exports
│   ├── detailed-designs/                    # Feature design documents
│   ├── specs/                               # L1 & L2 requirements
│   └── ui-design.pen                        # Pencil design file
│
├── src/
│   ├── ArchQ.Api/                           # ASP.NET Core Web API host
│   │   ├── Controllers/
│   │   │   ├── AdrsController.cs
│   │   │   ├── AuthController.cs
│   │   │   ├── TenantsController.cs
│   │   │   ├── UsersController.cs
│   │   │   ├── ApprovalsController.cs
│   │   │   ├── ArtifactsController.cs
│   │   │   ├── CommentsController.cs
│   │   │   └── TagsController.cs
│   │   ├── Middleware/
│   │   │   ├── TenantResolutionMiddleware.cs
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── AuditTrailMiddleware.cs
│   │   ├── Filters/
│   │   │   └── ValidateModelAttribute.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── ArchQ.Api.csproj
│   │
│   ├── ArchQ.Core/                          # Domain layer (no external dependencies)
│   │   ├── Entities/
│   │   │   ├── Adr.cs
│   │   │   ├── AdrVersion.cs
│   │   │   ├── Approval.cs
│   │   │   ├── Artifact.cs
│   │   │   ├── Comment.cs
│   │   │   ├── MeetingNote.cs
│   │   │   ├── GeneralNote.cs
│   │   │   ├── Tag.cs
│   │   │   ├── Tenant.cs
│   │   │   ├── User.cs
│   │   │   ├── UserRole.cs
│   │   │   └── AuditEntry.cs
│   │   ├── Enums/
│   │   │   ├── AdrStatus.cs
│   │   │   └── Role.cs
│   │   ├── Interfaces/
│   │   │   ├── IAdrRepository.cs
│   │   │   ├── IApprovalRepository.cs
│   │   │   ├── IArtifactRepository.cs
│   │   │   ├── ICommentRepository.cs
│   │   │   ├── ITagRepository.cs
│   │   │   ├── ITenantRepository.cs
│   │   │   ├── IUserRepository.cs
│   │   │   ├── IAuditRepository.cs
│   │   │   ├── IFileStorageService.cs
│   │   │   ├── IEmailService.cs
│   │   │   └── ITenantContext.cs
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs
│   │   │   ├── NotFoundException.cs
│   │   │   ├── ForbiddenException.cs
│   │   │   └── ConflictException.cs
│   │   └── ArchQ.Core.csproj
│   │
│   ├── ArchQ.Application/                   # Application layer (use cases, CQRS)
│   │   ├── Adrs/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateAdr/
│   │   │   │   │   ├── CreateAdrCommand.cs
│   │   │   │   │   ├── CreateAdrCommandHandler.cs
│   │   │   │   │   └── CreateAdrCommandValidator.cs
│   │   │   │   ├── UpdateAdr/
│   │   │   │   ├── SubmitForReview/
│   │   │   │   ├── ApproveAdr/
│   │   │   │   ├── RejectAdr/
│   │   │   │   └── SupersedeAdr/
│   │   │   └── Queries/
│   │   │       ├── GetAdrById/
│   │   │       ├── GetAdrList/
│   │   │       ├── SearchAdrs/
│   │   │       └── GetAdrVersionHistory/
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   │   ├── Register/
│   │   │   │   ├── Login/
│   │   │   │   └── RefreshToken/
│   │   │   └── Queries/
│   │   │       └── GetCurrentUser/
│   │   ├── Tenants/
│   │   │   ├── Commands/
│   │   │   │   └── CreateTenant/
│   │   │   └── Queries/
│   │   │       └── GetUserTenants/
│   │   ├── Approvals/
│   │   │   ├── Commands/
│   │   │   │   ├── AssignApprovers/
│   │   │   │   └── RecordDecision/
│   │   │   └── Queries/
│   │   │       └── GetApprovalStatus/
│   │   ├── Artifacts/
│   │   │   ├── Commands/
│   │   │   │   ├── AddMeetingNote/
│   │   │   │   ├── AddGeneralNote/
│   │   │   │   └── UploadDocument/
│   │   │   └── Queries/
│   │   │       └── GetArtifactsByAdr/
│   │   ├── Comments/
│   │   │   ├── Commands/
│   │   │   │   ├── AddComment/
│   │   │   │   └── EditComment/
│   │   │   └── Queries/
│   │   │       └── GetCommentThread/
│   │   ├── Tags/
│   │   │   ├── Commands/
│   │   │   │   └── AssignTags/
│   │   │   └── Queries/
│   │   │       └── SuggestTags/
│   │   ├── Common/
│   │   │   ├── Behaviours/
│   │   │   │   ├── ValidationBehaviour.cs
│   │   │   │   ├── LoggingBehaviour.cs
│   │   │   │   └── TenantScopeBehaviour.cs
│   │   │   └── Mappings/
│   │   │       └── MappingProfile.cs
│   │   └── ArchQ.Application.csproj
│   │
│   ├── ArchQ.Infrastructure/                # Infrastructure layer
│   │   ├── Persistence/
│   │   │   ├── CouchbaseContext.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── AdrRepository.cs
│   │   │   │   ├── ApprovalRepository.cs
│   │   │   │   ├── ArtifactRepository.cs
│   │   │   │   ├── CommentRepository.cs
│   │   │   │   ├── TagRepository.cs
│   │   │   │   ├── TenantRepository.cs
│   │   │   │   ├── UserRepository.cs
│   │   │   │   └── AuditRepository.cs
│   │   │   └── Configuration/
│   │   │       └── CouchbaseConfiguration.cs
│   │   ├── Identity/
│   │   │   ├── JwtTokenService.cs
│   │   │   ├── PasswordHasher.cs
│   │   │   └── TenantContext.cs
│   │   ├── FileStorage/
│   │   │   └── LocalFileStorageService.cs
│   │   ├── Email/
│   │   │   └── SmtpEmailService.cs
│   │   ├── DependencyInjection.cs
│   │   └── ArchQ.Infrastructure.csproj
│   │
│   └── ArchQ.Web/                           # Angular frontend
│       ├── src/
│       │   ├── app/
│       │   │   ├── core/
│       │   │   │   ├── auth/
│       │   │   │   │   ├── auth.service.ts
│       │   │   │   │   ├── auth.guard.ts
│       │   │   │   │   ├── auth.interceptor.ts
│       │   │   │   │   └── token.service.ts
│       │   │   │   ├── tenant/
│       │   │   │   │   ├── tenant.service.ts
│       │   │   │   │   └── tenant.interceptor.ts
│       │   │   │   ├── services/
│       │   │   │   │   ├── adr.service.ts
│       │   │   │   │   ├── approval.service.ts
│       │   │   │   │   ├── artifact.service.ts
│       │   │   │   │   ├── comment.service.ts
│       │   │   │   │   ├── tag.service.ts
│       │   │   │   │   ├── user.service.ts
│       │   │   │   │   └── notification.service.ts
│       │   │   │   └── models/
│       │   │   │       ├── adr.model.ts
│       │   │   │       ├── approval.model.ts
│       │   │   │       ├── artifact.model.ts
│       │   │   │       ├── comment.model.ts
│       │   │   │       ├── tenant.model.ts
│       │   │   │       ├── user.model.ts
│       │   │   │       └── tag.model.ts
│       │   │   ├── features/
│       │   │   │   ├── auth/
│       │   │   │   │   ├── login/
│       │   │   │   │   │   └── login.component.ts
│       │   │   │   │   └── register/
│       │   │   │   │       └── register.component.ts
│       │   │   │   ├── dashboard/
│       │   │   │   │   ├── adr-list/
│       │   │   │   │   │   └── adr-list.component.ts
│       │   │   │   │   └── adr-card/
│       │   │   │   │       └── adr-card.component.ts
│       │   │   │   ├── adr/
│       │   │   │   │   ├── adr-detail/
│       │   │   │   │   │   └── adr-detail.component.ts
│       │   │   │   │   ├── adr-editor/
│       │   │   │   │   │   └── adr-editor.component.ts
│       │   │   │   │   ├── adr-version-history/
│       │   │   │   │   │   └── adr-version-history.component.ts
│       │   │   │   │   └── adr-diff-viewer/
│       │   │   │   │       └── adr-diff-viewer.component.ts
│       │   │   │   ├── approval/
│       │   │   │   │   ├── approval-panel/
│       │   │   │   │   │   └── approval-panel.component.ts
│       │   │   │   │   └── approver-assignment/
│       │   │   │   │       └── approver-assignment.component.ts
│       │   │   │   ├── artifacts/
│       │   │   │   │   ├── meeting-note-form/
│       │   │   │   │   │   └── meeting-note-form.component.ts
│       │   │   │   │   ├── general-note-form/
│       │   │   │   │   │   └── general-note-form.component.ts
│       │   │   │   │   ├── document-upload/
│       │   │   │   │   │   └── document-upload.component.ts
│       │   │   │   │   └── artifact-list/
│       │   │   │   │       └── artifact-list.component.ts
│       │   │   │   ├── comments/
│       │   │   │   │   ├── comment-thread/
│       │   │   │   │   │   └── comment-thread.component.ts
│       │   │   │   │   └── comment-form/
│       │   │   │   │       └── comment-form.component.ts
│       │   │   │   ├── settings/
│       │   │   │   │   ├── tenant-settings/
│       │   │   │   │   │   └── tenant-settings.component.ts
│       │   │   │   │   ├── user-management/
│       │   │   │   │   │   └── user-management.component.ts
│       │   │   │   │   └── template-editor/
│       │   │   │   │       └── template-editor.component.ts
│       │   │   │   └── search/
│       │   │   │       └── search-results/
│       │   │   │           └── search-results.component.ts
│       │   │   ├── shared/
│       │   │   │   ├── components/
│       │   │   │   │   ├── sidebar/
│       │   │   │   │   │   └── sidebar.component.ts
│       │   │   │   │   ├── top-bar/
│       │   │   │   │   │   └── top-bar.component.ts
│       │   │   │   │   ├── org-switcher/
│       │   │   │   │   │   └── org-switcher.component.ts
│       │   │   │   │   ├── status-badge/
│       │   │   │   │   │   └── status-badge.component.ts
│       │   │   │   │   ├── tag-input/
│       │   │   │   │   │   └── tag-input.component.ts
│       │   │   │   │   ├── markdown-editor/
│       │   │   │   │   │   └── markdown-editor.component.ts
│       │   │   │   │   ├── markdown-preview/
│       │   │   │   │   │   └── markdown-preview.component.ts
│       │   │   │   │   ├── avatar/
│       │   │   │   │   │   └── avatar.component.ts
│       │   │   │   │   ├── confirm-dialog/
│       │   │   │   │   │   └── confirm-dialog.component.ts
│       │   │   │   │   └── toast/
│       │   │   │   │       └── toast.component.ts
│       │   │   │   ├── pipes/
│       │   │   │   │   ├── relative-time.pipe.ts
│       │   │   │   │   └── truncate.pipe.ts
│       │   │   │   └── directives/
│       │   │   │       └── role.directive.ts
│       │   │   ├── app.component.ts
│       │   │   ├── app.config.ts
│       │   │   └── app.routes.ts
│       │   ├── environments/
│       │   │   ├── environment.ts
│       │   │   └── environment.development.ts
│       │   ├── styles.scss
│       │   ├── index.html
│       │   └── main.ts
│       ├── angular.json
│       ├── package.json
│       └── tsconfig.json
│
├── tests/
│   ├── ArchQ.UnitTests/                     # Unit tests
│   │   ├── Core/
│   │   │   └── Entities/
│   │   ├── Application/
│   │   │   ├── Adrs/
│   │   │   ├── Auth/
│   │   │   └── Approvals/
│   │   └── ArchQ.UnitTests.csproj
│   │
│   ├── ArchQ.IntegrationTests/              # Integration tests
│   │   ├── Api/
│   │   │   ├── AdrsControllerTests.cs
│   │   │   ├── AuthControllerTests.cs
│   │   │   └── TenantsControllerTests.cs
│   │   ├── Fixtures/
│   │   │   ├── CouchbaseFixture.cs
│   │   │   └── ApiFactory.cs
│   │   └── ArchQ.IntegrationTests.csproj
│   │
│   └── ArchQ.E2E/                           # Playwright E2E tests
│       ├── pages/                           # Page Object Model classes
│       │   ├── base.page.ts
│       │   ├── login.page.ts
│       │   ├── register.page.ts
│       │   ├── dashboard.page.ts
│       │   ├── adr-detail.page.ts
│       │   ├── adr-editor.page.ts
│       │   ├── approval-panel.page.ts
│       │   ├── settings.page.ts
│       │   └── components/
│       │       ├── sidebar.component.ts
│       │       ├── top-bar.component.ts
│       │       ├── org-switcher.component.ts
│       │       ├── status-badge.component.ts
│       │       ├── comment-thread.component.ts
│       │       ├── markdown-editor.component.ts
│       │       └── toast.component.ts
│       ├── fixtures/
│       │   ├── auth.fixture.ts
│       │   ├── tenant.fixture.ts
│       │   └── test-data.ts
│       ├── tests/
│       │   ├── auth/
│       │   │   ├── login.spec.ts
│       │   │   ├── register.spec.ts
│       │   │   └── session.spec.ts
│       │   ├── adr/
│       │   │   ├── adr-create.spec.ts
│       │   │   ├── adr-edit.spec.ts
│       │   │   ├── adr-list.spec.ts
│       │   │   ├── adr-search.spec.ts
│       │   │   └── adr-version-history.spec.ts
│       │   ├── workflow/
│       │   │   ├── submit-for-review.spec.ts
│       │   │   ├── approve-adr.spec.ts
│       │   │   ├── reject-adr.spec.ts
│       │   │   └── supersede-adr.spec.ts
│       │   ├── artifacts/
│       │   │   ├── meeting-notes.spec.ts
│       │   │   ├── general-notes.spec.ts
│       │   │   ├── document-upload.spec.ts
│       │   │   └── comments.spec.ts
│       │   ├── tenant/
│       │   │   ├── org-switching.spec.ts
│       │   │   └── tenant-isolation.spec.ts
│       │   ├── admin/
│       │   │   ├── role-management.spec.ts
│       │   │   ├── approval-threshold.spec.ts
│       │   │   └── template-config.spec.ts
│       │   └── responsive/
│       │       ├── mobile-layout.spec.ts
│       │       └── tablet-layout.spec.ts
│       ├── helpers/
│       │   ├── api.helper.ts
│       │   └── db.helper.ts
│       ├── playwright.config.ts
│       ├── package.json
│       └── tsconfig.json
│
├── docker/
│   ├── api/
│   │   └── Dockerfile
│   ├── web/
│   │   └── Dockerfile
│   └── couchbase/
│       └── init-cluster.sh
│
├── ArchQ.sln
├── docker-compose.yml
├── docker-compose.override.yml
├── .gitignore
├── .editorconfig
└── CLAUDE.md
```

---

## Scaffolding Instructions

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 22 LTS](https://nodejs.org/)
- [Angular CLI 19](https://angular.dev/tools/cli) (`npm install -g @angular/cli`)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Playwright](https://playwright.dev/) (installed per project)

---

### Step 1: Create the Solution and Backend Projects

```bash
# From the repository root (C:\projects\ArchQ)

# Create the solution file
dotnet new sln -n ArchQ

# Create source projects
dotnet new classlib -n ArchQ.Core -o src/ArchQ.Core
dotnet new classlib -n ArchQ.Application -o src/ArchQ.Application
dotnet new classlib -n ArchQ.Infrastructure -o src/ArchQ.Infrastructure
dotnet new webapi -n ArchQ.Api -o src/ArchQ.Api --no-openapi

# Create test projects
dotnet new xunit -n ArchQ.UnitTests -o tests/ArchQ.UnitTests
dotnet new xunit -n ArchQ.IntegrationTests -o tests/ArchQ.IntegrationTests

# Add all projects to the solution
dotnet sln add src/ArchQ.Core/ArchQ.Core.csproj
dotnet sln add src/ArchQ.Application/ArchQ.Application.csproj
dotnet sln add src/ArchQ.Infrastructure/ArchQ.Infrastructure.csproj
dotnet sln add src/ArchQ.Api/ArchQ.Api.csproj
dotnet sln add tests/ArchQ.UnitTests/ArchQ.UnitTests.csproj
dotnet sln add tests/ArchQ.IntegrationTests/ArchQ.IntegrationTests.csproj
```

### Step 2: Set Up Project References (Clean Architecture)

```bash
# Core has no project references (domain layer)

# Application references Core
dotnet add src/ArchQ.Application reference src/ArchQ.Core

# Infrastructure references Core and Application
dotnet add src/ArchQ.Infrastructure reference src/ArchQ.Core
dotnet add src/ArchQ.Infrastructure reference src/ArchQ.Application

# Api references Application and Infrastructure
dotnet add src/ArchQ.Api reference src/ArchQ.Application
dotnet add src/ArchQ.Api reference src/ArchQ.Infrastructure

# Test projects reference what they test
dotnet add tests/ArchQ.UnitTests reference src/ArchQ.Core
dotnet add tests/ArchQ.UnitTests reference src/ArchQ.Application
dotnet add tests/ArchQ.IntegrationTests reference src/ArchQ.Api
```

### Step 3: Install Backend NuGet Packages

```bash
# Application layer - MediatR for CQRS, FluentValidation
dotnet add src/ArchQ.Application package MediatR
dotnet add src/ArchQ.Application package FluentValidation
dotnet add src/ArchQ.Application package FluentValidation.DependencyInjectionExtensions

# Infrastructure layer - Couchbase SDK, JWT, bcrypt
dotnet add src/ArchQ.Infrastructure package CouchbaseNetClient
dotnet add src/ArchQ.Infrastructure package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/ArchQ.Infrastructure package BCrypt.Net-Next

# Api layer
dotnet add src/ArchQ.Api package Serilog.AspNetCore

# Test projects
dotnet add tests/ArchQ.UnitTests package NSubstitute
dotnet add tests/ArchQ.UnitTests package FluentAssertions
dotnet add tests/ArchQ.IntegrationTests package Microsoft.AspNetCore.Mvc.Testing
dotnet add tests/ArchQ.IntegrationTests package FluentAssertions
```

### Step 4: Scaffold the Angular Frontend

```bash
# Create Angular project with standalone components and SCSS
ng new ArchQ.Web --directory src/ArchQ.Web --style scss --ssr false --skip-git

cd src/ArchQ.Web

# Install Markdown editor and syntax highlighting
npm install marked highlight.js

# Install Angular CDK for accessibility and overlays
ng add @angular/cdk

# Return to repo root
cd ../..
```

### Step 5: Create Angular Folder Structure

```bash
cd src/ArchQ.Web/src/app

# Core module structure
mkdir -p core/auth core/tenant core/services core/models

# Feature modules
mkdir -p features/auth/login features/auth/register
mkdir -p features/dashboard/adr-list features/dashboard/adr-card
mkdir -p features/adr/adr-detail features/adr/adr-editor
mkdir -p features/adr/adr-version-history features/adr/adr-diff-viewer
mkdir -p features/approval/approval-panel features/approval/approver-assignment
mkdir -p features/artifacts/meeting-note-form features/artifacts/general-note-form
mkdir -p features/artifacts/document-upload features/artifacts/artifact-list
mkdir -p features/comments/comment-thread features/comments/comment-form
mkdir -p features/settings/tenant-settings features/settings/user-management
mkdir -p features/settings/template-editor
mkdir -p features/search/search-results

# Shared components, pipes, directives
mkdir -p shared/components/sidebar shared/components/top-bar
mkdir -p shared/components/org-switcher shared/components/status-badge
mkdir -p shared/components/tag-input shared/components/markdown-editor
mkdir -p shared/components/markdown-preview shared/components/avatar
mkdir -p shared/components/confirm-dialog shared/components/toast
mkdir -p shared/pipes shared/directives

cd ../../../..
```

### Step 6: Set Up Playwright E2E Tests

```bash
# Create the E2E test project
mkdir -p tests/ArchQ.E2E
cd tests/ArchQ.E2E

# Initialize package.json
npm init -y

# Install Playwright
npm install -D @playwright/test

# Install browsers
npx playwright install

# Create directory structure
mkdir -p pages/components fixtures tests/auth tests/adr tests/workflow
mkdir -p tests/artifacts tests/tenant tests/admin tests/responsive helpers

cd ../..
```

### Step 7: Configure Playwright

Create `tests/ArchQ.E2E/playwright.config.ts`:

```typescript
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [
    ['html', { open: 'never' }],
    ['list']
  ],
  use: {
    baseURL: 'http://localhost:4200',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },
  projects: [
    // Desktop browsers
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    // Tablet viewport
    {
      name: 'tablet',
      use: { ...devices['iPad (gen 7)'] },
    },
    // Mobile viewport
    {
      name: 'mobile',
      use: { ...devices['iPhone 14'] },
    },
  ],
  webServer: {
    command: 'cd ../../src/ArchQ.Web && ng serve',
    url: 'http://localhost:4200',
    reuseExistingServer: !process.env.CI,
    timeout: 120000,
  },
});
```

### Step 8: Create Page Object Model Base Class

Create `tests/ArchQ.E2E/pages/base.page.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';

export abstract class BasePage {
  constructor(protected readonly page: Page) {}

  abstract readonly path: string;

  async navigate(): Promise<void> {
    await this.page.goto(this.path);
    await this.waitForReady();
  }

  async waitForReady(): Promise<void> {
    await this.page.waitForLoadState('networkidle');
  }

  get toastMessage(): Locator {
    return this.page.locator('[data-testid="toast-message"]');
  }

  async getToastText(): Promise<string> {
    await this.toastMessage.waitFor({ state: 'visible' });
    return await this.toastMessage.innerText();
  }
}
```

### Step 9: Create Page Objects

Create `tests/ArchQ.E2E/pages/login.page.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class LoginPage extends BasePage {
  readonly path = '/login';

  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly signInButton: Locator;
  readonly forgotPasswordLink: Locator;
  readonly signUpLink: Locator;
  readonly errorMessage: Locator;

  constructor(page: Page) {
    super(page);
    this.emailInput = page.locator('[data-testid="email-input"]');
    this.passwordInput = page.locator('[data-testid="password-input"]');
    this.signInButton = page.locator('[data-testid="sign-in-button"]');
    this.forgotPasswordLink = page.locator('[data-testid="forgot-password"]');
    this.signUpLink = page.locator('[data-testid="sign-up-link"]');
    this.errorMessage = page.locator('[data-testid="error-message"]');
  }

  async login(email: string, password: string): Promise<void> {
    await this.emailInput.fill(email);
    await this.passwordInput.fill(password);
    await this.signInButton.click();
  }
}
```

Create `tests/ArchQ.E2E/pages/dashboard.page.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';
import { BasePage } from './base.page';
import { SidebarComponent } from './components/sidebar.component';
import { OrgSwitcherComponent } from './components/org-switcher.component';

export class DashboardPage extends BasePage {
  readonly path = '/adrs';

  readonly sidebar: SidebarComponent;
  readonly orgSwitcher: OrgSwitcherComponent;

  readonly pageTitle: Locator;
  readonly newAdrButton: Locator;
  readonly searchInput: Locator;
  readonly statusFilter: Locator;
  readonly authorFilter: Locator;
  readonly tagFilter: Locator;
  readonly adrTable: Locator;
  readonly adrCards: Locator;
  readonly adrRows: Locator;

  constructor(page: Page) {
    super(page);
    this.sidebar = new SidebarComponent(page);
    this.orgSwitcher = new OrgSwitcherComponent(page);

    this.pageTitle = page.locator('[data-testid="page-title"]');
    this.newAdrButton = page.locator('[data-testid="new-adr-button"]');
    this.searchInput = page.locator('[data-testid="search-input"]');
    this.statusFilter = page.locator('[data-testid="status-filter"]');
    this.authorFilter = page.locator('[data-testid="author-filter"]');
    this.tagFilter = page.locator('[data-testid="tag-filter"]');
    this.adrTable = page.locator('[data-testid="adr-table"]');
    this.adrCards = page.locator('[data-testid="adr-card"]');
    this.adrRows = page.locator('[data-testid="adr-row"]');
  }

  async filterByStatus(status: string): Promise<void> {
    await this.statusFilter.click();
    await this.page.locator(`[data-testid="status-option-${status}"]`).click();
  }

  async searchAdrs(query: string): Promise<void> {
    await this.searchInput.fill(query);
    await this.page.waitForResponse(resp =>
      resp.url().includes('/api/adrs') && resp.status() === 200
    );
  }

  async openAdr(adrNumber: string): Promise<void> {
    await this.page.locator(`[data-testid="adr-link-${adrNumber}"]`).click();
  }

  async getAdrCount(): Promise<number> {
    // Table view on desktop, card view on mobile
    const rows = await this.adrRows.count();
    if (rows > 0) return rows;
    return await this.adrCards.count();
  }
}
```

Create `tests/ArchQ.E2E/pages/adr-editor.page.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';
import { BasePage } from './base.page';
import { MarkdownEditorComponent } from './components/markdown-editor.component';

export class AdrEditorPage extends BasePage {
  readonly path = '/adrs/new';

  readonly markdownEditor: MarkdownEditorComponent;

  readonly titleInput: Locator;
  readonly saveDraftButton: Locator;
  readonly submitForReviewButton: Locator;
  readonly cancelButton: Locator;
  readonly statusBadge: Locator;
  readonly editTab: Locator;
  readonly previewTab: Locator;
  readonly previewPane: Locator;

  constructor(page: Page) {
    super(page);
    this.markdownEditor = new MarkdownEditorComponent(page);

    this.titleInput = page.locator('[data-testid="adr-title-input"]');
    this.saveDraftButton = page.locator('[data-testid="save-draft-button"]');
    this.submitForReviewButton = page.locator('[data-testid="submit-review-button"]');
    this.cancelButton = page.locator('[data-testid="cancel-button"]');
    this.statusBadge = page.locator('[data-testid="status-badge"]');
    this.editTab = page.locator('[data-testid="edit-tab"]');
    this.previewTab = page.locator('[data-testid="preview-tab"]');
    this.previewPane = page.locator('[data-testid="preview-pane"]');
  }

  async createAdr(title: string, content: string): Promise<void> {
    await this.titleInput.fill(title);
    await this.markdownEditor.setContent(content);
    await this.saveDraftButton.click();
  }

  async switchToPreview(): Promise<void> {
    await this.previewTab.click();
    await this.previewPane.waitFor({ state: 'visible' });
  }

  async switchToEdit(): Promise<void> {
    await this.editTab.click();
  }
}
```

Create `tests/ArchQ.E2E/pages/adr-detail.page.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';
import { BasePage } from './base.page';
import { CommentThreadComponent } from './components/comment-thread.component';

export class AdrDetailPage extends BasePage {
  readonly path = '/adrs';

  readonly commentThread: CommentThreadComponent;

  readonly adrNumber: Locator;
  readonly adrTitle: Locator;
  readonly statusBadge: Locator;
  readonly authorName: Locator;
  readonly editButton: Locator;
  readonly approveButton: Locator;
  readonly rejectButton: Locator;
  readonly markdownContent: Locator;
  readonly tagList: Locator;
  readonly approvalPanel: Locator;
  readonly artifactList: Locator;
  readonly backButton: Locator;

  // Mobile accordion sections
  readonly approvalAccordion: Locator;
  readonly meetingNotesAccordion: Locator;
  readonly commentsAccordion: Locator;
  readonly documentsAccordion: Locator;

  constructor(page: Page) {
    super(page);
    this.commentThread = new CommentThreadComponent(page);

    this.adrNumber = page.locator('[data-testid="adr-number"]');
    this.adrTitle = page.locator('[data-testid="adr-title"]');
    this.statusBadge = page.locator('[data-testid="status-badge"]');
    this.authorName = page.locator('[data-testid="author-name"]');
    this.editButton = page.locator('[data-testid="edit-button"]');
    this.approveButton = page.locator('[data-testid="approve-button"]');
    this.rejectButton = page.locator('[data-testid="reject-button"]');
    this.markdownContent = page.locator('[data-testid="markdown-content"]');
    this.tagList = page.locator('[data-testid="tag-list"]');
    this.approvalPanel = page.locator('[data-testid="approval-panel"]');
    this.artifactList = page.locator('[data-testid="artifact-list"]');
    this.backButton = page.locator('[data-testid="back-button"]');

    this.approvalAccordion = page.locator('[data-testid="accordion-approval"]');
    this.meetingNotesAccordion = page.locator('[data-testid="accordion-meeting-notes"]');
    this.commentsAccordion = page.locator('[data-testid="accordion-comments"]');
    this.documentsAccordion = page.locator('[data-testid="accordion-documents"]');
  }

  async navigateToAdr(id: string): Promise<void> {
    await this.page.goto(`${this.path}/${id}`);
    await this.waitForReady();
  }

  async approve(comment?: string): Promise<void> {
    await this.approveButton.click();
    if (comment) {
      await this.page.locator('[data-testid="approval-comment"]').fill(comment);
    }
    await this.page.locator('[data-testid="confirm-approval"]').click();
  }

  async reject(reason: string): Promise<void> {
    await this.rejectButton.click();
    await this.page.locator('[data-testid="rejection-reason"]').fill(reason);
    await this.page.locator('[data-testid="confirm-rejection"]').click();
  }
}
```

### Step 10: Create Shared Component Page Objects

Create `tests/ArchQ.E2E/pages/components/sidebar.component.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';

export class SidebarComponent {
  readonly container: Locator;
  readonly logo: Locator;
  readonly navItems: Locator;
  readonly activeNavItem: Locator;
  readonly userName: Locator;
  readonly userRole: Locator;

  constructor(private readonly page: Page) {
    this.container = page.locator('[data-testid="sidebar"]');
    this.logo = page.locator('[data-testid="sidebar-logo"]');
    this.navItems = page.locator('[data-testid="nav-item"]');
    this.activeNavItem = page.locator('[data-testid="nav-item"].active');
    this.userName = page.locator('[data-testid="user-name"]');
    this.userRole = page.locator('[data-testid="user-role"]');
  }

  async navigateTo(label: string): Promise<void> {
    await this.navItems.filter({ hasText: label }).click();
  }

  async isVisible(): Promise<boolean> {
    return await this.container.isVisible();
  }
}
```

Create `tests/ArchQ.E2E/pages/components/org-switcher.component.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';

export class OrgSwitcherComponent {
  readonly trigger: Locator;
  readonly dropdown: Locator;
  readonly orgOptions: Locator;
  readonly activeOrg: Locator;

  constructor(private readonly page: Page) {
    this.trigger = page.locator('[data-testid="org-switcher"]');
    this.dropdown = page.locator('[data-testid="org-dropdown"]');
    this.orgOptions = page.locator('[data-testid="org-option"]');
    this.activeOrg = page.locator('[data-testid="active-org"]');
  }

  async switchTo(orgName: string): Promise<void> {
    await this.trigger.click();
    await this.dropdown.waitFor({ state: 'visible' });
    await this.orgOptions.filter({ hasText: orgName }).click();
    await this.page.waitForLoadState('networkidle');
  }

  async getActiveOrgName(): Promise<string> {
    return await this.activeOrg.innerText();
  }
}
```

Create `tests/ArchQ.E2E/pages/components/markdown-editor.component.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';

export class MarkdownEditorComponent {
  readonly editArea: Locator;
  readonly toolbar: Locator;
  readonly boldButton: Locator;
  readonly italicButton: Locator;
  readonly codeButton: Locator;
  readonly linkButton: Locator;
  readonly listButton: Locator;

  constructor(private readonly page: Page) {
    this.editArea = page.locator('[data-testid="markdown-edit-area"]');
    this.toolbar = page.locator('[data-testid="markdown-toolbar"]');
    this.boldButton = page.locator('[data-testid="toolbar-bold"]');
    this.italicButton = page.locator('[data-testid="toolbar-italic"]');
    this.codeButton = page.locator('[data-testid="toolbar-code"]');
    this.linkButton = page.locator('[data-testid="toolbar-link"]');
    this.listButton = page.locator('[data-testid="toolbar-list"]');
  }

  async setContent(content: string): Promise<void> {
    await this.editArea.fill(content);
  }

  async getContent(): Promise<string> {
    return await this.editArea.inputValue();
  }

  async applyBold(): Promise<void> {
    await this.boldButton.click();
  }
}
```

Create `tests/ArchQ.E2E/pages/components/comment-thread.component.ts`:

```typescript
import { type Page, type Locator } from '@playwright/test';

export class CommentThreadComponent {
  readonly container: Locator;
  readonly comments: Locator;
  readonly commentInput: Locator;
  readonly submitButton: Locator;

  constructor(private readonly page: Page) {
    this.container = page.locator('[data-testid="comment-thread"]');
    this.comments = page.locator('[data-testid="comment"]');
    this.commentInput = page.locator('[data-testid="comment-input"]');
    this.submitButton = page.locator('[data-testid="submit-comment"]');
  }

  async addComment(text: string): Promise<void> {
    await this.commentInput.fill(text);
    await this.submitButton.click();
  }

  async getCommentCount(): Promise<number> {
    return await this.comments.count();
  }

  async replyTo(commentIndex: number, text: string): Promise<void> {
    const comment = this.comments.nth(commentIndex);
    await comment.locator('[data-testid="reply-button"]').click();
    await comment.locator('[data-testid="reply-input"]').fill(text);
    await comment.locator('[data-testid="submit-reply"]').click();
  }
}
```

### Step 11: Create Test Fixtures

Create `tests/ArchQ.E2E/fixtures/auth.fixture.ts`:

```typescript
import { test as base, type Page } from '@playwright/test';
import { LoginPage } from '../pages/login.page';
import { DashboardPage } from '../pages/dashboard.page';
import { AdrEditorPage } from '../pages/adr-editor.page';
import { AdrDetailPage } from '../pages/adr-detail.page';
import { TEST_USERS } from './test-data';

type AuthFixtures = {
  loginPage: LoginPage;
  dashboardPage: DashboardPage;
  adrEditorPage: AdrEditorPage;
  adrDetailPage: AdrDetailPage;
  authenticatedPage: Page;
};

export const test = base.extend<AuthFixtures>({
  loginPage: async ({ page }, use) => {
    await use(new LoginPage(page));
  },

  dashboardPage: async ({ page }, use) => {
    await use(new DashboardPage(page));
  },

  adrEditorPage: async ({ page }, use) => {
    await use(new AdrEditorPage(page));
  },

  adrDetailPage: async ({ page }, use) => {
    await use(new AdrDetailPage(page));
  },

  authenticatedPage: async ({ page }, use) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.author.email, TEST_USERS.author.password);
    await page.waitForURL('**/adrs');
    await use(page);
  },
});

export { expect } from '@playwright/test';
```

Create `tests/ArchQ.E2E/fixtures/test-data.ts`:

```typescript
export const TEST_USERS = {
  admin: {
    email: 'admin@acme-corp.test',
    password: 'Test@12345678',
    name: 'Admin User',
    role: 'Admin',
  },
  author: {
    email: 'author@acme-corp.test',
    password: 'Test@12345678',
    name: 'Alice Chen',
    role: 'Author',
  },
  reviewer: {
    email: 'reviewer@acme-corp.test',
    password: 'Test@12345678',
    name: 'Bob Kim',
    role: 'Reviewer',
  },
  viewer: {
    email: 'viewer@acme-corp.test',
    password: 'Test@12345678',
    name: 'Carol Mata',
    role: 'Viewer',
  },
} as const;

export const TEST_TENANTS = {
  primary: {
    name: 'Acme Corp',
    slug: 'acme-corp',
  },
  secondary: {
    name: 'Beta Inc',
    slug: 'beta-inc',
  },
} as const;

export const TEST_ADRS = {
  draft: {
    title: 'Use Event-Driven Architecture for Order Processing',
    content: '## Context\n\nOur order processing pipeline...\n\n## Decision\n\nWe will adopt an event-driven architecture...\n\n## Consequences\n\nPositive: Decoupled services...',
  },
  inReview: {
    title: 'Adopt PostgreSQL as Primary Database',
    content: '## Context\n\nOur application currently uses...\n\n## Decision\n\nWe will adopt PostgreSQL...\n\n## Consequences\n\nPositive: Unified database...',
  },
} as const;
```

### Step 12: Write Example E2E Test Specs

Create `tests/ArchQ.E2E/tests/auth/login.spec.ts`:

```typescript
import { test, expect } from '../../fixtures/auth.fixture';
import { TEST_USERS } from '../../fixtures/test-data';

test.describe('Login', () => {
  test('should login with valid credentials and redirect to dashboard', async ({ loginPage, dashboardPage }) => {
    await loginPage.navigate();

    await loginPage.login(TEST_USERS.author.email, TEST_USERS.author.password);

    await expect(dashboardPage.pageTitle).toBeVisible();
    await expect(dashboardPage.pageTitle).toHaveText('Architecture Decision Records');
  });

  test('should show error for invalid credentials', async ({ loginPage }) => {
    await loginPage.navigate();

    await loginPage.login('wrong@email.com', 'wrongpassword');

    await expect(loginPage.errorMessage).toBeVisible();
    await expect(loginPage.errorMessage).toHaveText('Invalid credentials');
  });

  test('should navigate to registration page', async ({ loginPage, page }) => {
    await loginPage.navigate();

    await loginPage.signUpLink.click();

    await expect(page).toHaveURL(/\/register/);
  });
});
```

Create `tests/ArchQ.E2E/tests/adr/adr-create.spec.ts`:

```typescript
import { test, expect } from '../../fixtures/auth.fixture';
import { TEST_ADRS } from '../../fixtures/test-data';

test.describe('ADR Creation', () => {
  test.beforeEach(async ({ authenticatedPage }) => {
    // Each test starts authenticated
  });

  test('should create a new ADR in Draft status', async ({ dashboardPage, adrEditorPage, page }) => {
    await dashboardPage.navigate();
    await dashboardPage.newAdrButton.click();
    await expect(page).toHaveURL(/\/adrs\/new/);

    await adrEditorPage.createAdr(
      TEST_ADRS.draft.title,
      TEST_ADRS.draft.content
    );

    await expect(adrEditorPage.statusBadge).toHaveText('Draft');
    const toast = await adrEditorPage.getToastText();
    expect(toast).toContain('saved');
  });

  test('should show live preview of markdown content', async ({ adrEditorPage }) => {
    await adrEditorPage.navigate();

    await adrEditorPage.titleInput.fill('Test ADR');
    await adrEditorPage.markdownEditor.setContent('## Decision\n\nWe will use **PostgreSQL**.');
    await adrEditorPage.switchToPreview();

    await expect(adrEditorPage.previewPane.locator('h2')).toHaveText('Decision');
    await expect(adrEditorPage.previewPane.locator('strong')).toHaveText('PostgreSQL');
  });

  test('should pre-populate with tenant template', async ({ adrEditorPage }) => {
    await adrEditorPage.navigate();

    const content = await adrEditorPage.markdownEditor.getContent();
    expect(content).toContain('## Context');
    expect(content).toContain('## Decision');
    expect(content).toContain('## Consequences');
  });
});
```

Create `tests/ArchQ.E2E/tests/workflow/approve-adr.spec.ts`:

```typescript
import { test, expect } from '../../fixtures/auth.fixture';
import { TEST_USERS } from '../../fixtures/test-data';
import { LoginPage } from '../../pages/login.page';
import { AdrDetailPage } from '../../pages/adr-detail.page';

test.describe('ADR Approval Workflow', () => {
  test('reviewer can approve an ADR in review', async ({ page }) => {
    // Login as reviewer
    const loginPage = new LoginPage(page);
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.reviewer.email, TEST_USERS.reviewer.password);
    await page.waitForURL('**/adrs');

    const detailPage = new AdrDetailPage(page);
    await detailPage.navigateToAdr('adr-in-review-id');

    await expect(detailPage.statusBadge).toHaveText('In Review');
    await expect(detailPage.approveButton).toBeVisible();

    await detailPage.approve('LGTM - well reasoned decision');

    const toast = await detailPage.getToastText();
    expect(toast).toContain('approved');
  });

  test('reviewer can reject an ADR with reason', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.reviewer.email, TEST_USERS.reviewer.password);
    await page.waitForURL('**/adrs');

    const detailPage = new AdrDetailPage(page);
    await detailPage.navigateToAdr('adr-in-review-id');

    await detailPage.reject('Insufficient cost analysis');

    await expect(detailPage.statusBadge).toHaveText('Rejected');
  });

  test('author cannot approve their own ADR', async ({ authenticatedPage }) => {
    const detailPage = new AdrDetailPage(authenticatedPage);
    await detailPage.navigateToAdr('own-adr-in-review-id');

    await expect(detailPage.approveButton).not.toBeVisible();
  });
});
```

Create `tests/ArchQ.E2E/tests/responsive/mobile-layout.spec.ts`:

```typescript
import { test, expect } from '../../fixtures/auth.fixture';

test.describe('Mobile Responsive Layout', () => {
  test.use({ viewport: { width: 375, height: 812 } });

  test('ADR list shows card layout on mobile', async ({ authenticatedPage, dashboardPage }) => {
    await dashboardPage.navigate();

    // Table should not be visible on mobile
    await expect(dashboardPage.adrTable).not.toBeVisible();

    // Cards should be visible
    const cardCount = await dashboardPage.adrCards.count();
    expect(cardCount).toBeGreaterThan(0);

    // Sidebar should be hidden
    await expect(dashboardPage.sidebar.container).not.toBeVisible();
  });

  test('ADR detail shows accordion sections on mobile', async ({ authenticatedPage }) => {
    const { AdrDetailPage } = await import('../../pages/adr-detail.page');
    const detailPage = new AdrDetailPage(authenticatedPage);
    await detailPage.navigateToAdr('test-adr-id');

    // Sidebar panel should not exist on mobile
    await expect(detailPage.approvalPanel).not.toBeVisible();

    // Accordions should be visible instead
    await expect(detailPage.approvalAccordion).toBeVisible();
    await expect(detailPage.commentsAccordion).toBeVisible();
    await expect(detailPage.documentsAccordion).toBeVisible();

    // Workflow buttons should stack full width
    await expect(detailPage.approveButton).toBeVisible();
    const buttonBox = await detailPage.approveButton.boundingBox();
    expect(buttonBox!.width).toBeGreaterThan(300);
  });

  test('ADR editor shows tabbed edit/preview on mobile', async ({ authenticatedPage }) => {
    const { AdrEditorPage } = await import('../../pages/adr-editor.page');
    const editorPage = new AdrEditorPage(authenticatedPage);
    await editorPage.navigate();

    // Should show edit/preview tabs
    await expect(editorPage.editTab).toBeVisible();
    await expect(editorPage.previewTab).toBeVisible();

    // Preview pane not visible until tab clicked
    await expect(editorPage.previewPane).not.toBeVisible();

    await editorPage.switchToPreview();
    await expect(editorPage.previewPane).toBeVisible();
  });
});
```

### Step 13: Docker Compose Setup

Create `docker-compose.yml`:

```yaml
services:
  couchbase:
    image: couchbase:enterprise-7.6.1
    ports:
      - "8091-8097:8091-8097"
      - "11210-11211:11210-11211"
    volumes:
      - couchbase-data:/opt/couchbase/var
    environment:
      - COUCHBASE_ADMINISTRATOR_USERNAME=Administrator
      - COUCHBASE_ADMINISTRATOR_PASSWORD=password123
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8091/pools"]
      interval: 10s
      timeout: 5s
      retries: 10

  api:
    build:
      context: .
      dockerfile: docker/api/Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - Couchbase__ConnectionString=couchbase://couchbase
      - Couchbase__Username=Administrator
      - Couchbase__Password=password123
    depends_on:
      couchbase:
        condition: service_healthy

  web:
    build:
      context: .
      dockerfile: docker/web/Dockerfile
    ports:
      - "4200:80"
    depends_on:
      - api

volumes:
  couchbase-data:
```

### Step 14: Build and Verify

```bash
# Build the .NET solution
dotnet build

# Run unit tests
dotnet test tests/ArchQ.UnitTests

# Start infrastructure
docker compose up -d couchbase

# Run the API
dotnet run --project src/ArchQ.Api

# In another terminal, start the Angular dev server
cd src/ArchQ.Web && ng serve

# In another terminal, run E2E tests
cd tests/ArchQ.E2E && npx playwright test

# Run E2E tests for a specific browser/viewport
npx playwright test --project=mobile
npx playwright test --project=tablet

# View the HTML report
npx playwright show-report
```

---

## Page Object Model Conventions

| Convention | Rule |
|---|---|
| **File naming** | `<feature>.page.ts` for pages, `<name>.component.ts` for shared components |
| **Locator strategy** | Always use `data-testid` attributes — never CSS classes or tag names |
| **Composition** | Pages compose shared component objects (sidebar, org-switcher, etc.) |
| **No assertions in pages** | Page objects expose locators and actions; tests assert against them |
| **Single responsibility** | One page object per route; one component object per reusable UI element |
| **Wait strategies** | Use Playwright auto-waiting; add explicit waits only for dynamic content |
| **Test isolation** | Each test creates its own state via API helpers or fixtures |

## data-testid Naming Convention

```
data-testid="<component>-<element>[-<variant>]"

Examples:
  data-testid="adr-table"
  data-testid="adr-row"
  data-testid="status-badge"
  data-testid="approve-button"
  data-testid="accordion-comments"
  data-testid="nav-item"
  data-testid="org-switcher"
  data-testid="toast-message"
```

All Angular components must include `data-testid` attributes on interactive and assertable elements to support the Playwright Page Object Model.
