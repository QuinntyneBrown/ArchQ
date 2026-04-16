import { Component, computed, signal, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { OrgService, MembershipsResponse } from '../../../core/services/org.service';

@Component({
  selector: 'app-org-switcher',
  standalone: true,
  template: `
    <div class="org-switcher-wrapper">
      <button
        class="org-pill"
        data-testid="org-switcher"
        [class.has-multiple]="hasMultipleOrgs()"
        [disabled]="switching()"
        (click)="toggle()"
      >
        @if (switching()) {
          <span class="switching-indicator" data-testid="switching-indicator">Switching...</span>
        } @else {
          <span class="org-avatar">{{ activeInitial() }}</span>
          <span class="org-name" data-testid="active-org">{{ activeTenantName() }}</span>
          @if (hasMultipleOrgs()) {
            <svg class="chevron-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="m7 15 5 5 5-5"/>
              <path d="m7 9 5-5 5 5"/>
            </svg>
          }
        }
      </button>

      @if (isOpen()) {
        <div class="org-dropdown" data-testid="org-dropdown">
          @for (org of orgs(); track org.tenantSlug) {
            <button
              class="org-option"
              data-testid="org-option"
              [class.active]="org.tenantSlug === activeSlug()"
              (click)="selectOrg(org.tenantSlug)"
            >
              <span class="org-option-avatar">{{ org.tenantDisplayName.charAt(0).toUpperCase() }}</span>
              <span class="org-option-name">{{ org.tenantDisplayName }}</span>
              @if (org.tenantSlug === activeSlug()) {
                <svg class="check-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M20 6 9 17l-5-5"/>
                </svg>
              }
            </button>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .org-switcher-wrapper {
      position: relative;
    }

    .org-pill {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      width: 100%;
      padding: 0.5rem 0.75rem;
      background-color: #252836;
      border: 1px solid #3a3f54;
      border-radius: 0.5rem;
      color: #ffffff;
      font-size: 0.875rem;
      cursor: default;
      transition: background-color 0.15s ease;
    }

    .org-pill:disabled {
      opacity: 0.7;
      cursor: wait;
    }

    .org-pill.has-multiple:not(:disabled) {
      cursor: pointer;
    }

    .org-pill.has-multiple:not(:disabled):hover {
      background-color: #2a2d3e;
    }

    .org-avatar, .org-option-avatar {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 1.5rem;
      height: 1.5rem;
      border-radius: 50%;
      background-color: #6366f1;
      color: #ffffff;
      font-size: 0.75rem;
      font-weight: 600;
      flex-shrink: 0;
    }

    .org-option-avatar {
      width: 1.25rem;
      height: 1.25rem;
      font-size: 0.625rem;
      background-color: #3a3f54;
    }

    .switching-indicator {
      color: #9ca3af;
      font-size: 0.875rem;
    }

    .org-name {
      flex: 1;
      text-align: left;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .chevron-icon {
      flex-shrink: 0;
      opacity: 0.6;
    }

    .org-dropdown {
      position: absolute;
      top: calc(100% + 0.25rem);
      left: 0;
      right: 0;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.5rem;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
      z-index: 100;
      padding: 0.25rem;
      max-height: 15rem;
      overflow-y: auto;
    }

    .org-option {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      width: 100%;
      padding: 0.5rem 0.75rem;
      background: none;
      border: none;
      border-radius: 0.375rem;
      color: #9ca3af;
      font-size: 0.875rem;
      cursor: pointer;
      transition: background-color 0.15s ease, color 0.15s ease;
    }

    .org-option:hover {
      background-color: #252836;
      color: #ffffff;
    }

    .org-option:hover .org-option-avatar {
      background-color: #6366f1;
    }

    .org-option.active {
      color: #ffffff;
    }

    .org-option.active .org-option-avatar {
      background-color: #6366f1;
    }

    .org-option-name {
      flex: 1;
      text-align: left;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .check-icon {
      flex-shrink: 0;
      color: #6366f1;
    }

    @media (max-width: 767px) {
      .org-pill {
        padding: 0.375rem 0.5rem;
        font-size: 0.8125rem;
        border-radius: 0.375rem;
      }

      .org-avatar {
        width: 1.25rem;
        height: 1.25rem;
        font-size: 0.625rem;
      }

      .org-dropdown {
        position: fixed;
        top: auto;
        bottom: 0;
        left: 0;
        right: 0;
        border-radius: 0.75rem 0.75rem 0 0;
        max-height: 50vh;
        padding: 0.5rem;
      }

      .org-option {
        padding: 0.75rem;
        font-size: 1rem;
      }
    }
  `]
})
export class OrgSwitcherComponent {
  readonly isOpen = signal(false);
  readonly switching = signal(false);
  readonly orgs = signal<MembershipsResponse['memberships']>([]);

  readonly activeTenantName = computed(() => {
    const tenant = this.authService.currentTenant();
    return tenant?.displayName ?? '';
  });

  readonly activeInitial = computed(() => {
    const name = this.activeTenantName();
    return name ? name.charAt(0).toUpperCase() : '';
  });

  readonly activeSlug = computed(() => {
    const tenant = this.authService.currentTenant();
    return tenant?.slug ?? '';
  });

  readonly hasMultipleOrgs = computed(() => this.orgs().length > 1);

  constructor(
    private readonly authService: AuthService,
    private readonly orgService: OrgService,
    private readonly router: Router
  ) {
    this.loadOrgs();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('app-org-switcher')) {
      this.isOpen.set(false);
    }
  }

  toggle(): void {
    if (!this.hasMultipleOrgs() || this.switching()) return;
    this.isOpen.update(v => !v);
  }

  selectOrg(tenantSlug: string): void {
    if (tenantSlug === this.activeSlug()) {
      this.isOpen.set(false);
      return;
    }

    this.switching.set(true);
    this.isOpen.set(false);

    this.orgService.switchOrg(tenantSlug).subscribe({
      next: (response) => {
        this.authService.updateTenant(response.tenant);
        this.loadOrgs();
        this.switching.set(false);
        this.router.navigate(['/adrs']);
      },
      error: () => {
        this.switching.set(false);
      }
    });
  }

  private loadOrgs(): void {
    this.orgService.listMemberships().subscribe({
      next: (response) => {
        this.orgs.set(response.memberships);
      }
    });
  }
}
