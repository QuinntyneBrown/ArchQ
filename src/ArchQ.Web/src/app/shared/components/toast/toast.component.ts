import { Component, computed } from '@angular/core';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.scss'
})
export class ToastComponent {
  constructor(private readonly toastService: ToastService) {}

  readonly visible = computed(() => this.toastService.state().visible);
  readonly message = computed(() => this.toastService.state().message);
  readonly toastType = computed(() => this.toastService.state().type);
}
