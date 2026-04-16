import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { TenantService } from '../../../core/services/tenant.service';
import { ToastService } from '../../../shared/components/toast/toast.service';
import { Tenant } from '../../../core/models/tenant.model';

@Component({
  selector: 'app-tenant-detail',
  standalone: true,
  imports: [FormsModule],
  template: `
    @if (notFound()) {
      <div class="not-found" data-testid="not-found-message">
        Tenant not found
      </div>
    } @else if (tenant()) {
      <div class="tenant-detail-container">
        <div class="detail-row">
          <span class="label">Name</span>
          @if (editing()) {
            <div class="edit-row">
              <input
                type="text"
                [(ngModel)]="editName"
                data-testid="tenant-display-name-input"
              />
              <button data-testid="save-name-button" class="btn-primary" (click)="saveName()">Save</button>
              <button data-testid="cancel-edit-button" class="btn-secondary" (click)="cancelEdit()">Cancel</button>
            </div>
          } @else {
            <span data-testid="tenant-name">{{ tenant()!.displayName }}</span>
            <button data-testid="edit-name-button" class="btn-icon" (click)="startEdit()">Edit</button>
          }
        </div>

        <div class="detail-row">
          <span class="label">Slug</span>
          <span data-testid="tenant-slug">{{ tenant()!.slug }}</span>
        </div>

        <div class="detail-row">
          <span class="label">Status</span>
          <span data-testid="tenant-status">{{ tenant()!.status }}</span>
        </div>
      </div>
    } @else {
      <div class="loading">Loading...</div>
    }
  `,
  styles: [`
    .tenant-detail-container {
      max-width: 40rem;
      margin: 3rem auto;
      padding: 2rem 2.5rem;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.75rem;
    }
    .not-found, .loading {
      max-width: 40rem;
      margin: 3rem auto;
      padding: 2rem 2.5rem;
      text-align: center;
      color: #6b7280;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.75rem;
    }
    .detail-row {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 0.75rem 0;
      border-bottom: 1px solid #2a2d3e;
    }
    .detail-row:last-child {
      border-bottom: none;
    }
    .label {
      font-weight: 500;
      font-size: 0.875rem;
      min-width: 5rem;
      color: #9ca3af;
    }
    .edit-row {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    .edit-row input {
      padding: 0.625rem 0.75rem;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      font-size: 0.875rem;
      background-color: #252836;
      color: #ffffff;
      outline: none;
    }
    .edit-row input:focus {
      border-color: #2563eb;
    }
    .btn-primary {
      padding: 0.375rem 0.75rem;
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
    .btn-secondary {
      padding: 0.375rem 0.75rem;
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
    .btn-icon {
      padding: 0.25rem 0.5rem;
      background: transparent;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      cursor: pointer;
      color: #3b82f6;
      font-size: 0.875rem;
      transition: border-color 0.15s ease;
    }
    .btn-icon:hover {
      border-color: #3b82f6;
    }
    @media (max-width: 640px) {
      .tenant-detail-container {
        margin: 1rem;
        padding: 1.5rem;
        border-radius: 0.5rem;
      }
    }
  `]
})
export class TenantDetailComponent implements OnInit {
  readonly tenant = signal<Tenant | null>(null);
  readonly notFound = signal(false);
  readonly editing = signal(false);
  editName = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly tenantService: TenantService,
    private readonly toastService: ToastService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.tenantService.getById(id).subscribe({
        next: (tenant) => this.tenant.set(tenant),
        error: (err: HttpErrorResponse) => {
          if (err.status === 404) {
            this.notFound.set(true);
          }
        }
      });
    }
  }

  startEdit(): void {
    this.editName = this.tenant()!.displayName;
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
  }

  saveName(): void {
    const current = this.tenant();
    if (!current) return;

    this.tenantService.update(current.id, { displayName: this.editName }).subscribe({
      next: (updated) => {
        this.tenant.set(updated);
        this.editing.set(false);
        this.toastService.show('Organization updated successfully', 'success');
      },
      error: () => {
        this.toastService.show('Failed to update organization', 'error');
      }
    });
  }
}
