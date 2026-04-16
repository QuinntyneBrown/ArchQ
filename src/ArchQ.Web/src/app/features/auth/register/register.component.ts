import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
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
    let score = 0;
    if (this.password.length >= 8) score++;
    if (/[A-Z]/.test(this.password)) score++;
    if (/[a-z]/.test(this.password)) score++;
    if (/[0-9]/.test(this.password)) score++;
    if (/[^a-zA-Z0-9]/.test(this.password)) score++;

    const strength = (score / 5) * 100;
    this.passwordStrength.set(strength);

    if (score <= 1) {
      this.passwordStrengthColor.set('#ef4444');
    } else if (score <= 2) {
      this.passwordStrengthColor.set('#f97316');
    } else if (score <= 3) {
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
