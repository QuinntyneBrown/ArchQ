import { Component, computed, signal, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { OrgService, MembershipsResponse } from '../../../core/services/org.service';

@Component({
  selector: 'app-org-switcher',
  standalone: true,
  templateUrl: './org-switcher.component.html',
  styleUrl: './org-switcher.component.scss'
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
