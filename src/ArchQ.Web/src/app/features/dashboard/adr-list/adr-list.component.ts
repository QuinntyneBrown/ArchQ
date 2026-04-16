import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-adr-list',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="adr-list-container">
      <div class="adr-list-card">
        <div class="header-row">
          <h1 class="heading">Architecture Decision Records</h1>
          <a routerLink="/adrs/new" class="btn-primary" data-testid="create-adr-button">+ New ADR</a>
        </div>
        <p class="placeholder-text">ADR dashboard coming soon.</p>
      </div>
    </div>
  `,
  styles: [`
    .adr-list-container {
      min-height: 100vh;
      padding: 2rem;
    }
    .adr-list-card {
      max-width: 60rem;
      margin: 0 auto;
      padding: 2rem;
      background-color: #1a1d2e;
      border: 1px solid #2a2d3e;
      border-radius: 0.75rem;
    }
    .header-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }
    .heading {
      font-size: 1.5rem;
      font-weight: 700;
      color: #ffffff;
      margin: 0;
    }
    .btn-primary {
      padding: 0.5rem 1rem;
      background-color: #2563eb;
      color: #fff;
      border: none;
      border-radius: 0.375rem;
      cursor: pointer;
      font-size: 0.875rem;
      font-weight: 500;
      text-decoration: none;
      transition: background-color 0.15s ease;
    }
    .btn-primary:hover {
      background-color: #1d4ed8;
    }
    .placeholder-text {
      color: #9ca3af;
      font-size: 0.875rem;
    }
  `]
})
export class AdrListComponent {}
