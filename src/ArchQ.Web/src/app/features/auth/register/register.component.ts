import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="register-container">
      <div class="register-card">
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
        <p class="subtitle">Create your account</p>

        @if (errorMessage()) {
          <div class="error-banner" data-testid="error-message">{{ errorMessage() }}</div>
        }

        @if (successMessage()) {
          <div class="success-banner" data-testid="success-message">{{ successMessage() }}</div>
        } @else {
          <form (ngSubmit)="onSubmit()" novalidate>
            <div class="form-group">
              <label for="fullName">Full Name</label>
              <input
                id="fullName"
                type="text"
                [(ngModel)]="fullName"
                name="fullName"
                data-testid="full-name-input"
                placeholder="John Smith"
              />
              @if (fullNameError()) {
                <div class="field-error" data-testid="full-name-error">{{ fullNameError() }}</div>
              }
            </div>

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
              @if (emailError()) {
                <div class="field-error" data-testid="email-error">{{ emailError() }}</div>
              }
            </div>

            <div class="form-group">
              <label for="password">Password</label>
              <input
                id="password"
                type="password"
                [(ngModel)]="password"
                name="password"
                (ngModelChange)="onPasswordChange()"
                data-testid="password-input"
                placeholder="Minimum 8 characters"
              />
              <div class="password-strength" data-testid="password-strength">
                <div
                  class="password-strength-bar"
                  [style.width.%]="passwordStrength()"
                  [style.background-color]="passwordStrengthColor()"
                ></div>
              </div>
              @if (passwordError()) {
                <div class="field-error" data-testid="password-error">{{ passwordError() }}</div>
              }
            </div>

            <div class="form-group">
              <label for="organizationName">Organization Name</label>
              <input
                id="organizationName"
                type="text"
                [(ngModel)]="organizationName"
                name="organizationName"
                data-testid="organization-name-input"
                placeholder="Acme Corp"
              />
              @if (organizationError()) {
                <div class="field-error" data-testid="organization-error">{{ organizationError() }}</div>
              }
            </div>

            <button
              type="submit"
              class="btn-primary"
              data-testid="create-account-button"
              [disabled]="submitting()"
            >
              {{ submitting() ? 'Creating Account...' : 'Create Account' }}
            </button>
          </form>

          <p class="sign-in-prompt">
            Already have an account? <a routerLink="/login" data-testid="sign-in-link">Sign in</a>
          </p>
        }
      </div>
    </div>
  `,
  styles: [`
    .register-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      padding: 1rem;
    }
    .register-card {
      width: 100%;
      max-width: 28rem;
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
    .subtitle {
      text-align: center;
      color: #9ca3af;
      font-size: 0.875rem;
      margin-bottom: 1.5rem;
    }
    .form-group {
      margin-bottom: 1rem;
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
    .password-strength {
      height: 4px;
      background-color: #252836;
      border-radius: 2px;
      margin-top: 0.5rem;
      overflow: hidden;
    }
    .password-strength-bar {
      height: 100%;
      border-radius: 2px;
      transition: width 0.3s ease, background-color 0.3s ease;
    }
    .field-error {
      margin-top: 0.25rem;
      font-size: 0.875rem;
      color: #ef4444;
    }
    .error-banner {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      background-color: rgba(239, 68, 68, 0.1);
      border: 1px solid #ef4444;
      border-radius: 0.375rem;
      color: #ef4444;
    }
    .success-banner {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      background-color: rgba(34, 197, 94, 0.1);
      border: 1px solid #22c55e;
      border-radius: 0.375rem;
      color: #22c55e;
      text-align: center;
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
    .sign-in-prompt {
      text-align: center;
      margin-top: 1.25rem;
      font-size: 0.875rem;
      color: #9ca3af;
    }
    .sign-in-prompt a {
      color: #3b82f6;
      text-decoration: none;
    }
    .sign-in-prompt a:hover {
      text-decoration: underline;
    }
    @media (max-width: 640px) {
      .register-card {
        padding: 1.5rem;
        border-radius: 0.5rem;
      }
    }
  `]
})
export class RegisterComponent {
  fullName = '';
  email = '';
  password = '';
  organizationName = '';

  readonly submitting = signal(false);
  readonly errorMessage = signal('');
  readonly successMessage = signal('');
  readonly fullNameError = signal('');
  readonly emailError = signal('');
  readonly passwordError = signal('');
  readonly organizationError = signal('');
  readonly passwordStrength = signal(0);
  readonly passwordStrengthColor = signal('#ef4444');

  constructor(private readonly authService: AuthService) {}

  onPasswordChange(): void {
    let strength = 0;
    if (this.password.length >= 8) strength += 25;
    if (/[A-Z]/.test(this.password)) strength += 25;
    if (/[a-z]/.test(this.password)) strength += 25;
    if (/[^a-zA-Z0-9]/.test(this.password)) strength += 25;

    this.passwordStrength.set(strength);

    if (strength <= 25) {
      this.passwordStrengthColor.set('#ef4444');
    } else if (strength <= 50) {
      this.passwordStrengthColor.set('#f97316');
    } else if (strength <= 75) {
      this.passwordStrengthColor.set('#eab308');
    } else {
      this.passwordStrengthColor.set('#22c55e');
    }
  }

  onSubmit(): void {
    this.errorMessage.set('');
    this.fullNameError.set('');
    this.emailError.set('');
    this.passwordError.set('');
    this.organizationError.set('');

    let valid = true;

    if (!this.fullName.trim()) {
      this.fullNameError.set('Full name is required');
      valid = false;
    }

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!this.email.trim()) {
      this.emailError.set('Email is required');
      valid = false;
    } else if (!emailPattern.test(this.email)) {
      this.emailError.set('Please enter a valid email address');
      valid = false;
    }

    if (this.password.length < 8) {
      this.passwordError.set('Password must be at least 8 characters');
      valid = false;
    } else if (!/[A-Z]/.test(this.password)) {
      this.passwordError.set('Password must contain at least one uppercase letter');
      valid = false;
    } else if (!/[a-z]/.test(this.password)) {
      this.passwordError.set('Password must contain at least one lowercase letter');
      valid = false;
    } else if (!/[0-9]/.test(this.password)) {
      this.passwordError.set('Password must contain at least one digit');
      valid = false;
    } else if (!/[^a-zA-Z0-9]/.test(this.password)) {
      this.passwordError.set('Password must contain at least one special character');
      valid = false;
    }

    if (!this.organizationName.trim()) {
      this.organizationError.set('Organization name is required');
      valid = false;
    }

    if (!valid) return;

    this.submitting.set(true);

    this.authService.register({
      fullName: this.fullName,
      email: this.email,
      password: this.password,
      organizationName: this.organizationName
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.successMessage.set('Registration successful. Please check your email to verify your account.');
      },
      error: (err: HttpErrorResponse) => {
        this.submitting.set(false);
        if (err.status === 409) {
          this.errorMessage.set('An account with this email already exists');
        } else if (err.status === 400) {
          this.errorMessage.set(err.error?.message || 'Validation error');
        } else {
          this.errorMessage.set('An unexpected error occurred');
        }
      }
    });
  }
}
