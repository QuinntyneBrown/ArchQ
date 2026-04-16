import { Component, computed } from '@angular/core';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  template: `
    @if (visible()) {
      <div
        class="toast"
        [class.toast-success]="toastType() === 'success'"
        [class.toast-error]="toastType() === 'error'"
        data-testid="toast-message"
      >
        {{ message() }}
      </div>
    }
  `,
  styles: [`
    .toast {
      position: fixed;
      top: 1rem;
      right: 1rem;
      padding: 0.75rem 1.5rem;
      border-radius: 0.5rem;
      color: #ffffff;
      font-weight: 500;
      font-size: 0.875rem;
      z-index: 9999;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.4);
    }
    .toast-success {
      border-left: 3px solid #22c55e;
    }
    .toast-error {
      border-left: 3px solid #ef4444;
    }
  `]
})
export class ToastComponent {
  constructor(private readonly toastService: ToastService) {}

  readonly visible = computed(() => this.toastService.state().visible);
  readonly message = computed(() => this.toastService.state().message);
  readonly toastType = computed(() => this.toastService.state().type);
}
