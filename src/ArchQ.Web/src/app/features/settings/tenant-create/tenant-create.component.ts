import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { TenantService } from '../../../core/services/tenant.service';
import { ToastService } from '../../../shared/components/toast/toast.service';

const RESERVED_SLUGS = ['_system', '_default', 'admin', 'api', 'www', 'app'];
const SLUG_PATTERN = /^[a-z0-9][a-z0-9-]{1,61}[a-z0-9]$/;

@Component({
  selector: 'app-tenant-create',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="tenant-create-container">
      <h1>Create Organization</h1>

      @if (errorMessage()) {
        <div class="error-banner" data-testid="error-message">{{ errorMessage() }}</div>
      }

      <form (ngSubmit)="onSubmit()" novalidate>
        <div class="form-group">
          <label for="displayName">Display Name</label>
          <input
            id="displayName"
            type="text"
            [(ngModel)]="displayName"
            name="displayName"
            (ngModelChange)="onDisplayNameChange($event)"
            data-testid="tenant-display-name-input"
            placeholder="Enter organization name"
          />
          @if (displayNameError()) {
            <div class="field-error" data-testid="display-name-error">{{ displayNameError() }}</div>
          }
        </div>

        <div class="form-group">
          <label for="slug">Slug</label>
          <input
            id="slug"
            type="text"
            [(ngModel)]="slug"
            name="slug"
            data-testid="tenant-slug-input"
            placeholder="organization-slug"
          />
          <div class="slug-preview" data-testid="tenant-slug-preview">{{ slugPreview }}</div>
          @if (slugError()) {
            <div class="field-error" data-testid="slug-error">{{ slugError() }}</div>
          }
        </div>

        <div class="form-actions">
          <button
            type="button"
            data-testid="cancel-button"
            (click)="onCancel()"
            class="btn-secondary"
          >
            Cancel
          </button>
          <button
            type="submit"
            data-testid="create-tenant-button"
            [disabled]="submitting()"
            class="btn-primary"
          >
            {{ submitting() ? 'Creating...' : 'Create Organization' }}
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .tenant-create-container {
      max-width: 28rem;
      margin: 3rem auto;
      padding: 2rem 2.5rem;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.75rem;
    }
    h1 {
      margin-bottom: 1.5rem;
      font-size: 1.5rem;
      font-weight: 600;
      color: #ffffff;
      text-align: center;
    }
    .form-group {
      margin-bottom: 1rem;
    }
    label {
      display: block;
      margin-bottom: 0.375rem;
      font-weight: 500;
      font-size: 0.875rem;
      color: #d1d5db;
    }
    input {
      width: 100%;
      padding: 0.625rem 0.75rem;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      font-size: 0.875rem;
      box-sizing: border-box;
      background-color: #252836;
      color: #ffffff;
      outline: none;
      transition: border-color 0.15s ease;
    }
    input::placeholder {
      color: #6b7280;
    }
    input:focus {
      border-color: #2563eb;
    }
    .slug-preview {
      margin-top: 0.25rem;
      font-size: 0.875rem;
      color: #6b7280;
    }
    .field-error {
      margin-top: 0.25rem;
      font-size: 0.875rem;
      color: #ef4444;
    }
    .error-banner {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      background-color: rgba(239, 68, 68, 0.1);
      border: 1px solid #ef4444;
      border-radius: 0.375rem;
      color: #ef4444;
    }
    .form-actions {
      display: flex;
      gap: 0.75rem;
      margin-top: 1.5rem;
    }
    .btn-primary {
      padding: 0.625rem 1.25rem;
      background-color: #2563eb;
      color: #fff;
      border: none;
      border-radius: 0.375rem;
      cursor: pointer;
      font-size: 0.875rem;
      font-weight: 500;
      transition: background-color 0.15s ease;
    }
    .btn-primary:hover {
      background-color: #1d4ed8;
    }
    .btn-primary:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .btn-secondary {
      padding: 0.625rem 1.25rem;
      background-color: transparent;
      color: #9ca3af;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      cursor: pointer;
      font-size: 0.875rem;
      font-weight: 500;
      transition: border-color 0.15s ease;
    }
    .btn-secondary:hover {
      border-color: #6b7280;
    }
    @media (max-width: 640px) {
      .tenant-create-container {
        margin: 1rem;
        padding: 1.5rem;
        border-radius: 0.5rem;
      }
      .form-actions {
        flex-direction: column;
      }
    }
  `]
})
export class TenantCreateComponent {
  displayName = '';
  slug = '';

  readonly submitting = signal(false);
  readonly errorMessage = signal('');
  readonly displayNameError = signal('');
  readonly slugError = signal('');

  get slugPreview(): string {
    return this.slug || this.generateSlug(this.displayName);
  }

  constructor(
    private readonly tenantService: TenantService,
    private readonly toastService: ToastService,
    private readonly router: Router
  ) {}

  onDisplayNameChange(value: string): void {
    this.displayName = value;
    if (!this.slug) {
      // slug preview updates via computed, but don't overwrite manual slug
    }
  }

  onCancel(): void {
    this.router.navigate(['/adrs']);
  }

  onSubmit(): void {
    this.errorMessage.set('');
    this.displayNameError.set('');
    this.slugError.set('');

    const effectiveSlug = this.slug || this.generateSlug(this.displayName);

    // Validate display name
    if (!this.displayName.trim()) {
      this.displayNameError.set('Display name is required');
      return;
    }

    // Validate slug
    if (RESERVED_SLUGS.includes(effectiveSlug)) {
      this.slugError.set('This slug is reserved');
      return;
    }

    if (effectiveSlug.length < 3) {
      this.slugError.set('Slug must be at least 3 characters');
      return;
    }

    if (!SLUG_PATTERN.test(effectiveSlug)) {
      this.slugError.set('Slug must contain only lowercase alphanumeric characters and hyphens');
      return;
    }

    this.submitting.set(true);

    this.tenantService.create({ displayName: this.displayName, slug: effectiveSlug }).subscribe({
      next: (tenant) => {
        this.submitting.set(false);
        this.toastService.show('Organization created successfully', 'success');
        this.router.navigate(['/tenants', tenant.id]);
      },
      error: (err: HttpErrorResponse) => {
        this.submitting.set(false);
        if (err.status === 409) {
          this.errorMessage.set('This slug is already in use');
        } else if (err.status === 400) {
          const body = err.error;
          this.errorMessage.set(body?.message || 'Validation error');
        } else {
          this.errorMessage.set('An unexpected error occurred');
        }
      }
    });
  }

  private generateSlug(name: string): string {
    return name
      .toLowerCase()
      .replace(/\s+/g, '-')
      .replace(/[^a-z0-9-]/g, '');
  }
}
