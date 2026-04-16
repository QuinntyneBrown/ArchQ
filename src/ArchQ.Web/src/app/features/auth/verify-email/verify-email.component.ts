import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="verify-container">
      <div class="verify-card">
        <div class="brand" data-testid="app-logo">
          <svg class="logo-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <line x1="3" y1="22" x2="21" y2="22"></line>
            <line x1="6" y1="18" x2="6" y2="11"></line>
            <line x1="10" y1="18" x2="10" y2="11"></line>
            <line x1="14" y1="18" x2="14" y2="11"></line>
            <line x1="18" y1="18" x2="18" y2="11"></line>
            <polygon points="12 2 20 7 4 7"></polygon>
          </svg>
          <span class="logo-text">ArchQ</span>
        </div>

        @if (loading()) {
          <p class="status-text">Verifying your email...</p>
        }

        @if (verificationStatus()) {
          <div class="success-banner" data-testid="verification-status">{{ verificationStatus() }}</div>
        }

        @if (verificationError()) {
          <div class="error-banner" data-testid="verification-error">{{ verificationError() }}</div>
        }

        @if (!loading()) {
          <p class="sign-in-prompt">
            <a routerLink="/login" data-testid="sign-in-link">Sign in to your account</a>
          </p>
        }
      </div>
    </div>
  `,
  styles: [`
    .verify-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      padding: 1rem;
    }
    .verify-card {
      width: 100%;
      max-width: 28rem;
      padding: 2rem 2.5rem;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.75rem;
      text-align: center;
    }
    .brand {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      margin-bottom: 1.5rem;
    }
    .logo-icon {
      width: 2rem;
      height: 2rem;
      color: #3b82f6;
    }
    .logo-text {
      font-size: 1.5rem;
      font-weight: 700;
      color: #ffffff;
    }
    .status-text {
      color: #9ca3af;
      font-size: 0.875rem;
      margin-bottom: 1rem;
    }
    .success-banner {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      background-color: rgba(34, 197, 94, 0.1);
      border: 1px solid #22c55e;
      border-radius: 0.375rem;
      color: #22c55e;
    }
    .error-banner {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      background-color: rgba(239, 68, 68, 0.1);
      border: 1px solid #ef4444;
      border-radius: 0.375rem;
      color: #ef4444;
    }
    .sign-in-prompt {
      margin-top: 1.25rem;
      font-size: 0.875rem;
    }
    .sign-in-prompt a {
      color: #3b82f6;
      text-decoration: none;
    }
    .sign-in-prompt a:hover {
      text-decoration: underline;
    }
    @media (max-width: 640px) {
      .verify-card {
        padding: 1.5rem;
        border-radius: 0.5rem;
      }
    }
  `]
})
export class VerifyEmailComponent implements OnInit {
  readonly loading = signal(true);
  readonly verificationStatus = signal('');
  readonly verificationError = signal('');

  constructor(
    private readonly route: ActivatedRoute,
    private readonly authService: AuthService
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');

    if (!token) {
      this.loading.set(false);
      this.verificationError.set('Verification link is invalid or has expired.');
      return;
    }

    this.authService.verifyEmail(token).subscribe({
      next: () => {
        this.loading.set(false);
        this.verificationStatus.set('Email verified successfully. You may now sign in.');
      },
      error: (_err: HttpErrorResponse) => {
        this.loading.set(false);
        this.verificationError.set('Verification link is invalid or has expired.');
      }
    });
  }
}
