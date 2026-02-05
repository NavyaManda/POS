import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-primary-btn',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button 
      [class.btn]="true"
      [class.btn-primary]="true"
      [class.btn-sm]="size === 'sm'"
      [class.btn-lg]="size === 'lg'"
      [class.btn-block]="block"
      [disabled]="disabled || loading"
      (click)="onClick()"
      [type]="type"
    >
      <span *ngIf="loading" class="spinner-border spinner-border-sm me-2"></span>
      {{ text }}
    </button>
  `,
  styles: [`
    .btn {
      padding: 10px 20px;
      font-size: 14px;
      font-weight: 600;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      transition: all 0.3s ease;
      display: inline-flex;
      align-items: center;
      gap: 8px;
    }

    .btn-primary {
      background-color: #667eea;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background-color: #764ba2;
      transform: translateY(-2px);
    }

    .btn-primary:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-sm {
      padding: 8px 16px;
      font-size: 12px;
    }

    .btn-lg {
      padding: 12px 24px;
      font-size: 16px;
    }

    .btn-block {
      width: 100%;
      justify-content: center;
    }

    .spinner-border {
      display: inline-block;
      width: 14px;
      height: 14px;
      vertical-align: text-bottom;
      border: 2px solid currentColor;
      border-right-color: transparent;
      border-radius: 50%;
      animation: spinner-border 0.75s linear infinite;
    }

    @keyframes spinner-border {
      to {
        transform: rotate(360deg);
      }
    }

    .me-2 {
      margin-right: 8px;
    }
  `]
})
export class PrimaryBtnComponent {
  @Input() text: string = 'Button';
  @Input() disabled: boolean = false;
  @Input() loading: boolean = false;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() block: boolean = false;
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Output() clicked = new EventEmitter<void>();

  onClick(): void {
    this.clicked.emit();
  }
}
