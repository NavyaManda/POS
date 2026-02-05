import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="modal-overlay" *ngIf="isOpen" (click)="onClose()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h3 class="modal-title">{{ title }}</h3>
          <button class="modal-close" (click)="onClose()">&times;</button>
        </div>
        <div class="modal-body">
          <ng-content></ng-content>
        </div>
        <div class="modal-footer" *ngIf="showFooter">
          <button class="btn btn-secondary" (click)="onClose()">Cancel</button>
          <button class="btn btn-primary" (click)="onConfirm()">{{ confirmText }}</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1050;
    }

    .modal-content {
      background-color: white;
      border-radius: 8px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
      max-width: 500px;
      width: 90%;
      max-height: 90vh;
      overflow-y: auto;
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 20px;
      border-bottom: 1px solid #e5e7eb;
    }

    .modal-title {
      margin: 0;
      font-size: 20px;
      font-weight: 700;
      color: #1f2937;
    }

    .modal-close {
      background: none;
      border: none;
      font-size: 28px;
      cursor: pointer;
      color: #6b7280;
      padding: 0;
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .modal-close:hover {
      color: #1f2937;
    }

    .modal-body {
      padding: 20px;
    }

    .modal-footer {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
      padding: 20px;
      border-top: 1px solid #e5e7eb;
    }

    .btn {
      padding: 10px 20px;
      font-size: 14px;
      font-weight: 600;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      transition: all 0.3s;
    }

    .btn-secondary {
      background-color: #e5e7eb;
      color: #1f2937;
    }

    .btn-secondary:hover {
      background-color: #d1d5db;
    }

    .btn-primary {
      background-color: #667eea;
      color: white;
    }

    .btn-primary:hover {
      background-color: #764ba2;
    }
  `]
})
export class ModalComponent {
  @Input() isOpen: boolean = false;
  @Input() title: string = 'Modal';
  @Input() confirmText: string = 'Confirm';
  @Input() showFooter: boolean = true;
  @Output() closed = new EventEmitter<void>();
  @Output() confirmed = new EventEmitter<void>();

  onClose(): void {
    this.closed.emit();
  }

  onConfirm(): void {
    this.confirmed.emit();
  }
}
