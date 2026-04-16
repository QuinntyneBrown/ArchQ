import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ToastComponent } from './shared/components/toast/toast.component';
import { OrgSwitcherComponent } from './shared/components/org-switcher/org-switcher.component';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastComponent, OrgSwitcherComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ArchQ.Web';
  readonly sidebarOpen = signal(false);

  constructor(
    public readonly authService: AuthService,
    private readonly router: Router
  ) {}

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  onLogout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }
}
