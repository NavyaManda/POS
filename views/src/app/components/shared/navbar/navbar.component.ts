import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <nav class="navbar">
      <div class="navbar-container">
        <div class="navbar-brand">
          <h2>POS System</h2>
        </div>
        <ul class="navbar-menu">
          <li class="navbar-item" *ngIf="user">
            <span class="navbar-text">Welcome, {{ user.firstName }} {{ user.lastName }}</span>
          </li>
          <li class="navbar-item">
            <button class="btn btn-secondary" (click)="onLogout()">Logout</button>
          </li>
        </ul>
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      background-color: #667eea;
      color: white;
      padding: 0;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .navbar-container {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 20px;
      max-width: 100%;
    }

    .navbar-brand h2 {
      margin: 0;
      font-size: 20px;
      font-weight: 700;
    }

    .navbar-menu {
      display: flex;
      list-style: none;
      margin: 0;
      padding: 0;
      gap: 20px;
      align-items: center;
    }

    .navbar-item {
      margin: 0;
    }

    .navbar-text {
      color: white;
      font-size: 14px;
    }

    .btn-secondary {
      background-color: rgba(255, 255, 255, 0.2);
      border: 1px solid rgba(255, 255, 255, 0.3);
      color: white;
      padding: 8px 16px;
      cursor: pointer;
      border-radius: 4px;
      transition: all 0.3s;
    }

    .btn-secondary:hover {
      background-color: rgba(255, 255, 255, 0.3);
    }
  `]
})
export class NavbarComponent {
  @Input() user: any;

  onLogout(): void {
    this.logout.emit();
  }

  logout: any = { emit: () => {} };
}
