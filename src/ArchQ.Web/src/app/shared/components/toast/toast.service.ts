import { Injectable, signal } from '@angular/core';

export interface ToastState {
  message: string;
  type: 'success' | 'error';
  visible: boolean;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly state = signal<ToastState>({ message: '', type: 'success', visible: false });

  private timeoutId: ReturnType<typeof setTimeout> | null = null;

  show(message: string, type: 'success' | 'error'): void {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
    this.state.set({ message, type, visible: true });
    this.timeoutId = setTimeout(() => {
      this.state.set({ message: '', type: 'success', visible: false });
      this.timeoutId = null;
    }, 5000);
  }
}
