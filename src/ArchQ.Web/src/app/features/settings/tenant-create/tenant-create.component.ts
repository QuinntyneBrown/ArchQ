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
  templateUrl: './tenant-create.component.html',
  styleUrl: './tenant-create.component.scss'
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
