import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  width?: string;
  template?: any;
}

export interface TableRow {
  [key: string]: any;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="table-container">
      <table class="table" [class.table-hover]="hover">
        <thead>
          <tr>
            <th *ngFor="let col of columns" [style.width]="col.width">
              <span [class.sortable]="col.sortable" (click)="onSort(col.key)">
                {{ col.label }}
              </span>
            </th>
            <th *ngIf="actions" style="width: 120px;">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let row of rows">
            <td *ngFor="let col of columns">{{ row[col.key] }}</td>
            <td *ngIf="actions" class="table-actions">
              <button class="btn btn-icon" (click)="onEdit(row)" title="Edit">
                <i class="fas fa-edit"></i>
              </button>
              <button class="btn btn-icon btn-danger" (click)="onDelete(row)" title="Delete">
                <i class="fas fa-trash"></i>
              </button>
            </td>
          </tr>
          <tr *ngIf="rows.length === 0">
            <td [attr.colspan]="columns.length + (actions ? 1 : 0)" class="text-center text-muted">
              {{ emptyMessage }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  styles: [`
    .table-container {
      overflow-x: auto;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .table {
      width: 100%;
      border-collapse: collapse;
      background-color: white;
    }

    .table thead {
      background-color: #f9fafb;
      border-bottom: 2px solid #e5e7eb;
    }

    .table th {
      padding: 12px;
      text-align: left;
      font-weight: 600;
      color: #1f2937;
    }

    .table th .sortable {
      cursor: pointer;
      user-select: none;
    }

    .table th .sortable:hover {
      color: #667eea;
    }

    .table td {
      padding: 12px;
      border-bottom: 1px solid #e5e7eb;
      color: #374151;
    }

    .table tbody tr:hover {
      background-color: #f9fafb;
    }

    .table tbody tr:last-child td {
      border-bottom: none;
    }

    .table-actions {
      display: flex;
      gap: 8px;
    }

    .btn-icon {
      width: 36px;
      height: 36px;
      padding: 0;
      display: flex;
      align-items: center;
      justify-content: center;
      border: none;
      background-color: #e5e7eb;
      border-radius: 4px;
      cursor: pointer;
      transition: all 0.3s;
      color: #667eea;
    }

    .btn-icon:hover {
      background-color: #d1d5db;
    }

    .btn-icon.btn-danger {
      color: #dc3545;
    }

    .btn-icon.btn-danger:hover {
      background-color: #f8d7da;
    }

    .text-center {
      text-align: center;
    }

    .text-muted {
      color: #6b7280;
    }
  `]
})
export class DataTableComponent {
  @Input() columns: TableColumn[] = [];
  @Input() rows: TableRow[] = [];
  @Input() hover: boolean = true;
  @Input() actions: boolean = false;
  @Input() emptyMessage: string = 'No data available';
  @Output() edit = new EventEmitter<TableRow>();
  @Output() delete = new EventEmitter<TableRow>();
  @Output() sort = new EventEmitter<string>();

  onSort(key: string): void {
    this.sort.emit(key);
  }

  onEdit(row: TableRow): void {
    this.edit.emit(row);
  }

  onDelete(row: TableRow): void {
    this.delete.emit(row);
  }
}
