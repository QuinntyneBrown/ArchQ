import { Component, OnInit, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { AdrService } from '../../../core/services/adr.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../shared/components/toast/toast.service';
import { AdrSummary } from '../../../core/models/adr.model';

@Component({
  selector: 'app-adr-list',
  standalone: true,
  imports: [RouterLink, FormsModule, NgClass],
  template: `
    <div class="adr-list-container">
      <div class="adr-list-card">
        <!-- Header -->
        <div class="header-row">
          <h1 class="heading" data-testid="page-title">Architecture Decision Records</h1>
          <a routerLink="/adrs/new" class="btn-primary btn-new-desktop" data-testid="new-adr-button">+ New ADR</a>
        </div>

        <!-- Filters Row -->
        <div class="filters-row">
          <input
            type="text"
            class="search-input"
            data-testid="search-input"
            placeholder="Search ADRs..."
            [(ngModel)]="searchTerm"
            (ngModelChange)="onSearchChange()"
          />
          <select
            class="status-filter"
            data-testid="status-filter"
            [(ngModel)]="selectedStatus"
            (ngModelChange)="onFilterChange()"
          >
            <option value="All">All</option>
            <option value="Draft">Draft</option>
            <option value="In Review">In Review</option>
            <option value="Approved">Approved</option>
            <option value="Rejected">Rejected</option>
            <option value="Superseded">Superseded</option>
          </select>
        </div>

        <!-- Mobile Filter Pills -->
        <div class="filter-pills">
          @for (status of statusOptions; track status) {
            <button
              class="pill"
              [class.pill-active]="selectedStatus === status"
              (click)="selectedStatus = status; onFilterChange()"
            >{{ status }}</button>
          }
        </div>

        <!-- Loading -->
        @if (loading()) {
          <div class="loading-state">
            <p class="muted-text">Loading ADRs...</p>
          </div>
        }

        <!-- Empty State -->
        @if (!loading() && adrs().length === 0) {
          <div class="empty-state">
            <p class="muted-text">No ADRs found. Create your first Architecture Decision Record.</p>
          </div>
        }

        <!-- Desktop Table -->
        @if (!loading() && adrs().length > 0) {
          <div class="table-wrapper">
            <table class="adr-table" data-testid="adr-table">
              <thead>
                <tr>
                  <th>Number</th>
                  <th>Title</th>
                  <th>Status</th>
                  <th>Author</th>
                  <th>Modified</th>
                </tr>
              </thead>
              <tbody>
                @for (adr of adrs(); track adr.id) {
                  <tr
                    class="adr-row"
                    data-testid="adr-row"
                    (click)="navigateToAdr(adr.id)"
                  >
                    <td>
                      <a
                        [routerLink]="['/adrs', adr.id, 'edit']"
                        class="adr-number-link"
                        [attr.data-testid]="'adr-link-' + adr.adrNumber"
                        (click)="$event.stopPropagation()"
                      >{{ adr.adrNumber }}</a>
                    </td>
                    <td class="title-cell">{{ adr.title }}</td>
                    <td>
                      <span class="status-badge" [ngClass]="getStatusClass(adr.status)">{{ adr.status }}</span>
                    </td>
                    <td>
                      <div class="author-cell">
                        <span class="author-avatar">{{ getInitials(adr.authorName) }}</span>
                        <span class="author-name">{{ adr.authorName || 'Unknown' }}</span>
                      </div>
                    </td>
                    <td class="date-cell">{{ formatDate(adr.updatedAt) }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }

        <!-- Mobile Cards -->
        @if (!loading() && adrs().length > 0) {
          <div class="card-list">
            @for (adr of adrs(); track adr.id) {
              <div
                class="adr-card"
                data-testid="adr-card"
                (click)="navigateToAdr(adr.id)"
              >
                <div class="card-top-row">
                  <span class="card-number">{{ adr.adrNumber }}</span>
                  <span class="status-badge" [ngClass]="getStatusClass(adr.status)">{{ adr.status }}</span>
                </div>
                <div class="card-title">{{ adr.title }}</div>
                <div class="card-bottom-row">
                  <div class="author-cell">
                    <span class="author-avatar author-avatar-sm">{{ getInitials(adr.authorName) }}</span>
                    <span class="card-author">{{ adr.authorName || 'Unknown' }}</span>
                  </div>
                  <span class="card-date">{{ formatDate(adr.updatedAt) }}</span>
                </div>
              </div>
            }
          </div>
        }

        <!-- Pagination -->
        @if (!loading() && (prevCursor() || nextCursor())) {
          <div class="pagination-row">
            <button
              class="btn-page"
              [disabled]="!prevCursor()"
              (click)="loadPage(prevCursor()!)"
            >Previous</button>
            <span class="total-count">{{ totalCount() }} total</span>
            <button
              class="btn-page"
              [disabled]="!nextCursor()"
              (click)="loadPage(nextCursor()!)"
            >Next</button>
          </div>
        }
      </div>

      <!-- Mobile FAB -->
      <a routerLink="/adrs/new" class="fab" data-testid="new-adr-button-mobile">+</a>
    </div>
  `,
  styles: [`
    .adr-list-container {
      min-height: 100vh;
      padding: 2rem;
      position: relative;
    }
    .adr-list-card {
      max-width: 72rem;
      margin: 0 auto;
      padding: 2rem;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.75rem;
    }

    /* Header */
    .header-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }
    .heading {
      font-size: 1.5rem;
      font-weight: 700;
      color: #ffffff;
      margin: 0;
    }
    .btn-primary {
      padding: 0.5rem 1rem;
      background-color: #2563eb;
      color: #fff;
      border: none;
      border-radius: 0.375rem;
      cursor: pointer;
      font-size: 0.875rem;
      font-weight: 500;
      text-decoration: none;
      transition: background-color 0.15s ease;
    }
    .btn-primary:hover {
      background-color: #1d4ed8;
    }

    /* Filters */
    .filters-row {
      display: flex;
      gap: 0.75rem;
      margin-bottom: 1.25rem;
    }
    .search-input {
      flex: 1;
      padding: 0.5rem 0.75rem;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      font-size: 0.875rem;
      background-color: #252836;
      color: #ffffff;
      outline: none;
      transition: border-color 0.15s ease;
    }
    .search-input::placeholder { color: #6b7280; }
    .search-input:focus { border-color: #2563eb; }
    .status-filter {
      padding: 0.5rem 0.75rem;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      font-size: 0.875rem;
      background-color: #252836;
      color: #ffffff;
      outline: none;
      cursor: pointer;
      min-width: 8rem;
    }
    .status-filter:focus { border-color: #2563eb; }

    /* Filter Pills (mobile only) */
    .filter-pills {
      display: none;
      gap: 0.5rem;
      margin-bottom: 1rem;
      flex-wrap: wrap;
    }
    .pill {
      padding: 0.375rem 0.75rem;
      background: transparent;
      color: #9ca3af;
      border: 1px solid #3a3f54;
      border-radius: 9999px;
      cursor: pointer;
      font-size: 0.75rem;
      font-weight: 500;
      transition: all 0.15s ease;
    }
    .pill-active {
      background-color: #2563eb;
      color: #fff;
      border-color: #2563eb;
    }

    /* Table */
    .table-wrapper {
      overflow-x: auto;
    }
    .adr-table {
      width: 100%;
      border-collapse: collapse;
    }
    .adr-table th {
      text-align: left;
      padding: 0.75rem 1rem;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: #6b7280;
      border-bottom: 1px solid #2a2d3e;
    }
    .adr-table td {
      padding: 0.75rem 1rem;
      font-size: 0.875rem;
      color: #d1d5db;
      border-bottom: 1px solid #2a2d3e;
    }
    .adr-row {
      cursor: pointer;
      transition: background-color 0.15s ease;
    }
    .adr-row:hover {
      background-color: #252836;
    }
    .adr-number-link {
      color: #3b82f6;
      text-decoration: none;
      font-weight: 600;
    }
    .adr-number-link:hover {
      text-decoration: underline;
    }
    .title-cell {
      max-width: 20rem;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    /* Status Badge */
    .status-badge {
      display: inline-block;
      padding: 0.125rem 0.5rem;
      font-size: 0.75rem;
      font-weight: 500;
      border-radius: 9999px;
    }
    .status-draft {
      color: #22c55e;
      background-color: rgba(34, 197, 94, 0.1);
      border: 1px solid rgba(34, 197, 94, 0.3);
    }
    .status-in-review {
      color: #3b82f6;
      background-color: rgba(59, 130, 246, 0.1);
      border: 1px solid rgba(59, 130, 246, 0.3);
    }
    .status-approved {
      color: #22c55e;
      background-color: rgba(34, 197, 94, 0.1);
      border: 1px solid rgba(34, 197, 94, 0.3);
    }
    .status-rejected {
      color: #ef4444;
      background-color: rgba(239, 68, 68, 0.1);
      border: 1px solid rgba(239, 68, 68, 0.3);
    }
    .status-superseded {
      color: #6b7280;
      background-color: rgba(107, 114, 128, 0.1);
      border: 1px solid rgba(107, 114, 128, 0.3);
    }

    /* Author */
    .author-cell {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    .author-avatar {
      width: 1.75rem;
      height: 1.75rem;
      border-radius: 50%;
      background-color: #374151;
      color: #d1d5db;
      font-size: 0.625rem;
      font-weight: 600;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }
    .author-avatar-sm {
      width: 1.5rem;
      height: 1.5rem;
      font-size: 0.5625rem;
    }
    .author-name {
      color: #d1d5db;
      font-size: 0.875rem;
    }
    .date-cell {
      white-space: nowrap;
      color: #9ca3af !important;
      font-size: 0.8125rem !important;
    }

    /* Card List (mobile) */
    .card-list {
      display: none;
      flex-direction: column;
      gap: 0.75rem;
    }
    .adr-card {
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.5rem;
      padding: 1rem;
      cursor: pointer;
      transition: background-color 0.15s ease;
    }
    .adr-card:hover {
      background-color: #252836;
    }
    .card-top-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.5rem;
    }
    .card-number {
      color: #3b82f6;
      font-weight: 600;
      font-size: 0.875rem;
    }
    .card-title {
      color: #ffffff;
      font-weight: 500;
      font-size: 0.9375rem;
      margin-bottom: 0.75rem;
      line-height: 1.4;
    }
    .card-bottom-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .card-author {
      color: #9ca3af;
      font-size: 0.8125rem;
    }
    .card-date {
      color: #6b7280;
      font-size: 0.75rem;
    }

    /* Pagination */
    .pagination-row {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 1rem;
      margin-top: 1.25rem;
      padding-top: 1rem;
      border-top: 1px solid #2a2d3e;
    }
    .btn-page {
      padding: 0.375rem 0.75rem;
      background-color: #252836;
      color: #d1d5db;
      border: 1px solid #3a3f54;
      border-radius: 0.375rem;
      cursor: pointer;
      font-size: 0.8125rem;
      transition: all 0.15s ease;
    }
    .btn-page:hover:not(:disabled) {
      background-color: #2a2d3e;
      border-color: #4b5563;
    }
    .btn-page:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }
    .total-count {
      color: #6b7280;
      font-size: 0.8125rem;
    }

    /* Loading / Empty */
    .loading-state, .empty-state {
      padding: 3rem 1rem;
      text-align: center;
    }
    .muted-text {
      color: #9ca3af;
      font-size: 0.875rem;
    }

    /* FAB (mobile only) */
    .fab {
      display: none;
      position: fixed;
      bottom: 1.5rem;
      right: 1.5rem;
      width: 3.5rem;
      height: 3.5rem;
      background-color: #2563eb;
      color: #fff;
      border-radius: 50%;
      font-size: 1.75rem;
      font-weight: 300;
      text-decoration: none;
      align-items: center;
      justify-content: center;
      box-shadow: 0 4px 12px rgba(37, 99, 235, 0.4);
      transition: background-color 0.15s ease;
      z-index: 10;
    }
    .fab:hover {
      background-color: #1d4ed8;
    }

    /* Responsive: Tablet */
    @media (max-width: 1024px) {
      .adr-list-container { padding: 1.5rem; }
      .adr-list-card { padding: 1.5rem; }
    }

    /* Responsive: Mobile */
    @media (max-width: 575px) {
      .adr-list-container { padding: 1rem; }
      .adr-list-card { padding: 1rem; }
      .heading { font-size: 1.25rem; }
      .btn-new-desktop { display: none; }
      .filters-row { flex-direction: column; }
      .status-filter { display: none; }
      .filter-pills { display: flex; }
      .table-wrapper { display: none; }
      .card-list { display: flex; }
      .fab { display: flex; }
    }

    /* Responsive: Tablet range — condensed table */
    @media (min-width: 576px) and (max-width: 767px) {
      .adr-table th:nth-child(5),
      .adr-table td:nth-child(5) {
        display: none;
      }
    }
  `]
})
export class AdrListComponent implements OnInit {
  readonly statusOptions = ['All', 'Draft', 'In Review', 'Approved', 'Rejected', 'Superseded'];

  readonly adrs = signal<AdrSummary[]>([]);
  readonly loading = signal(false);
  readonly nextCursor = signal<string | null>(null);
  readonly prevCursor = signal<string | null>(null);
  readonly totalCount = signal(0);

  searchTerm = '';
  selectedStatus = 'All';

  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private readonly adrService: AdrService,
    private readonly authService: AuthService,
    private readonly toastService: ToastService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loadAdrs();
  }

  onFilterChange(): void {
    this.loadAdrs();
  }

  onSearchChange(): void {
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
    }
    this.searchTimeout = setTimeout(() => {
      this.loadAdrs();
    }, 300);
  }

  loadPage(cursor: string): void {
    this.loadAdrs(cursor);
  }

  navigateToAdr(id: string): void {
    this.router.navigate(['/adrs', id, 'edit']);
  }

  getStatusClass(status: string): string {
    const normalized = status.toLowerCase().replace(/\s+/g, '-');
    switch (normalized) {
      case 'draft': return 'status-draft';
      case 'in-review': return 'status-in-review';
      case 'approved': return 'status-approved';
      case 'rejected': return 'status-rejected';
      case 'superseded': return 'status-superseded';
      default: return 'status-draft';
    }
  }

  getInitials(name: string): string {
    if (!name) return '?';
    return name.split(' ').map(p => p[0]).join('').toUpperCase().slice(0, 2);
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
  }

  private loadAdrs(cursor?: string): void {
    const tenant = this.authService.currentTenant();
    if (!tenant) {
      this.toastService.show('No tenant selected', 'error');
      return;
    }

    this.loading.set(true);

    this.adrService.listAdrs(tenant.slug, {
      status: this.selectedStatus !== 'All' ? this.selectedStatus : undefined,
      search: this.searchTerm || undefined,
      cursor: cursor
    }).subscribe({
      next: (response) => {
        this.adrs.set(response.items);
        this.nextCursor.set(response.nextCursor);
        this.prevCursor.set(response.prevCursor);
        this.totalCount.set(response.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.toastService.show('Failed to load ADRs', 'error');
        this.loading.set(false);
      }
    });
  }
}
