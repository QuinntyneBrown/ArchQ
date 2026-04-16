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
  templateUrl: './tenant-detail.component.html',
  styleUrl: './tenant-detail.component.scss'
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
