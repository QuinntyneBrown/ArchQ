import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
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
