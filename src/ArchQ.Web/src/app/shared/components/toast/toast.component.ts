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
      border-radius: 0.375rem;
      color: #fff;
      font-weight: 500;
      z-index: 9999;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }
    .toast-success {
      background-color: #16a34a;
    }
    .toast-error {
      background-color: #dc2626;
    }
  `]
})
export class ToastComponent {
  constructor(private readonly toastService: ToastService) {}

  readonly visible = computed(() => this.toastService.state().visible);
  readonly message = computed(() => this.toastService.state().message);
  readonly toastType = computed(() => this.toastService.state().type);
}
