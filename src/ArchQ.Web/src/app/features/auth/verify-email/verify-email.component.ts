import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './verify-email.component.html',
  styleUrl: './verify-email.component.scss'
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
