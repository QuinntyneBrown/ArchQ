import { Component, OnInit, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { AdrService } from '../../../core/services/adr.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../shared/components/toast/toast.service';
import { AdrSummary, SearchResultItem } from '../../../core/models/adr.model';

@Component({
  selector: 'app-adr-list',
  standalone: true,
  imports: [RouterLink, FormsModule, NgClass],
  templateUrl: './adr-list.component.html',
  styleUrl: './adr-list.component.scss'
})
export class AdrListComponent implements OnInit {
  readonly statusOptions = ['All', 'Draft', 'In Review', 'Approved', 'Rejected', 'Superseded'];

  readonly adrs = signal<AdrSummary[]>([]);
  readonly loading = signal(false);
  readonly nextCursor = signal<string | null>(null);
  readonly prevCursor = signal<string | null>(null);
  readonly totalCount = signal(0);
  readonly searchResults = signal<SearchResultItem[]>([]);
  readonly isSearchMode = signal(false);
  readonly searchTotalHits = signal(0);

  searchTerm = '';
  selectedStatus = 'All';

  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private readonly adrService: AdrService,
    public readonly authService: AuthService,
    private readonly toastService: ToastService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loadAdrs();
  }

  onFilterChange(): void {
    if (this.isSearchMode() && this.searchTerm.trim().length >= 2) {
      this.performSearch();
    } else {
      this.loadAdrs();
    }
  }

  onSearchChange(): void {
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
    }
    this.searchTimeout = setTimeout(() => {
      if (this.searchTerm.trim().length >= 2) {
        this.performSearch();
      } else {
        this.isSearchMode.set(false);
        this.searchResults.set([]);
        this.loadAdrs();
      }
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

  private performSearch(): void {
    const tenant = this.authService.currentTenant();
    if (!tenant) {
      this.toastService.show('No tenant selected', 'error');
      return;
    }

    this.loading.set(true);
    const statusFilter = this.selectedStatus !== 'All' ? this.selectedStatus : undefined;

    this.adrService.searchAdrs(tenant.slug, this.searchTerm.trim(), statusFilter).subscribe({
      next: (response) => {
        this.searchResults.set(response.results);
        this.searchTotalHits.set(response.totalHits);
        this.isSearchMode.set(true);
        this.loading.set(false);
      },
      error: () => {
        this.toastService.show('Search failed', 'error');
        this.loading.set(false);
      }
    });
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
