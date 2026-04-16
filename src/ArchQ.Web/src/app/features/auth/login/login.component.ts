import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="login-container">
      <div class="login-card">
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
        <h2 class="subtitle">Sign in to your account</h2>

        @if (errorMessage()) {
          <div class="error-banner" data-testid="error-message">{{ errorMessage() }}</div>
        }

        <form (ngSubmit)="onSubmit()" novalidate>
          <div class="form-group">
            <label for="email">Email Address</label>
            <input
              id="email"
              type="email"
              [(ngModel)]="email"
              name="email"
              data-testid="email-input"
              placeholder="you&#64;company.com"
            />
          </div>

          <div class="form-group">
            <div class="password-label-row">
              <label for="password">Password</label>
              <a href="javascript:void(0)" class="forgot-password" data-testid="forgot-password">Forgot password?</a>
            </div>
            <input
              id="password"
              type="password"
              [(ngModel)]="password"
              name="password"
              data-testid="password-input"
              placeholder="Enter your password"
            />
          </div>

          <button
            type="submit"
            class="btn-primary"
            data-testid="sign-in-button"
            [disabled]="submitting()"
          >
            {{ submitting() ? 'Signing In...' : 'Sign In' }}
          </button>
        </form>

        <div class="divider">
          <span class="divider-text">OR</span>
        </div>

        <p class="sign-up-prompt">
          Don't have an account? <a routerLink="/register" data-testid="sign-up-link">Sign up</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      padding: 1rem;
    }
    .login-card {
      width: 100%;
      max-width: 27.5rem;
      padding: 2rem 2.5rem;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.75rem;
    }
    .brand {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      margin-bottom: 0.5rem;
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
    h2.subtitle {
      text-align: center;
      color: #9ca3af;
      font-size: 0.875rem;
      font-weight: 400;
      margin: 0 0 1.5rem;
    }
    .form-group {
      margin-bottom: 1rem;
    }
    .password-label-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.375rem;
    }
    .password-label-row label {
      margin-bottom: 0;
    }
    .forgot-password {
      font-size: 0.8125rem;
      color: #3b82f6;
      text-decoration: none;
    }
    .forgot-password:hover {
      text-decoration: underline;
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
    .error-banner {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      background-color: rgba(239, 68, 68, 0.1);
      border: 1px solid #ef4444;
      border-radius: 0.375rem;
      color: #ef4444;
    }
    .btn-primary {
      width: 100%;
      padding: 0.625rem 1.25rem;
      margin-top: 1.5rem;
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
    .divider {
      display: flex;
      align-items: center;
      margin: 1.5rem 0;
    }
    .divider::before,
    .divider::after {
      content: '';
      flex: 1;
      height: 1px;
      background-color: #2a2d3e;
    }
    .divider-text {
      padding: 0 0.75rem;
      font-size: 0.75rem;
      color: #6b7280;
      text-transform: uppercase;
    }
    .sign-up-prompt {
      text-align: center;
      font-size: 0.875rem;
      color: #9ca3af;
      margin: 0;
    }
    .sign-up-prompt a {
      color: #3b82f6;
      text-decoration: none;
    }
    .sign-up-prompt a:hover {
      text-decoration: underline;
    }
    @media (max-width: 768px) {
      .login-card {
        padding: 1.5rem;
        border-radius: 0.5rem;
      }
    }
  `]
})
export class LoginComponent {
  email = '';
  password = '';

  readonly submitting = signal(false);
  readonly errorMessage = signal('');

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  onSubmit(): void {
    this.errorMessage.set('');

    if (!this.email.trim() || !this.password.trim()) {
      this.errorMessage.set('Email and password are required');
      return;
    }

    this.submitting.set(true);

    this.authService.login(this.email, this.password).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigate(['/adrs']);
      },
      error: (err: HttpErrorResponse) => {
        this.submitting.set(false);
        if (err.status === 401) {
          this.errorMessage.set('Invalid email or password');
        } else if (err.status === 403) {
          this.errorMessage.set('Your account has not been verified. Please check your email.');
        } else if (err.status === 400) {
          this.errorMessage.set(err.error?.message || 'Validation error');
        } else {
          this.errorMessage.set('An unexpected error occurred');
        }
      }
    });
  }
}
