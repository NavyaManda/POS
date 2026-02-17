import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MenuService, MenuItem, Category } from '../../services/menu.service';

@Component({
  selector: 'app-menu-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  template: `
    <div class="menu-container">
      <div class="menu-header">
        <h2>Menu Management</h2>
        <button class="btn btn-primary" (click)="openAddForm()">+ Add Menu Item</button>
      </div>

      <div class="menu-filters">
        <select [(ngModel)]="selectedCategoryId" (change)="filterByCategory()">
          <option value="">All Categories</option>
          <option *ngFor="let category of categories" [value]="category.id">
            {{ category.name }}
          </option>
        </select>
      </div>

      <div class="menu-items-grid">
        <div *ngIf="loading" class="loading">Loading menu items...</div>
        <div *ngIf="!loading && menuItems.length === 0" class="empty-state">
          No menu items found
        </div>
        
        <div class="menu-item-card" *ngFor="let item of menuItems">
          <div class="item-header">
            <h3>{{ item.name }}</h3>
            <span class="price">${{ item.price }}</span>
          </div>
          
          <p class="description">{{ item.description }}</p>
          
          <div class="item-tags">
            <span *ngIf="item.isVegetarian" class="tag vegetarian">🌱 Vegetarian</span>
            <span *ngIf="item.isSpicy" class="tag spicy">🌶️ Spicy</span>
            <span class="tag calories">{{ item.calories }} cal</span>
            <span [class.tag, class.active]="item.isAvailable">{{ item.isAvailable ? '✓ Available' : '✗ Unavailable' }}</span>
          </div>
          
          <div class="item-actions">
            <button class="btn btn-sm btn-secondary" (click)="editItem(item)">Edit</button>
            <button class="btn btn-sm btn-danger" (click)="deleteItem(item.id)">Delete</button>
          </div>
        </div>
      </div>

      <!-- Add/Edit Modal -->
      <div class="modal" *ngIf="showForm" [@fadeIn]>
        <div class="modal-content">
          <div class="modal-header">
            <h3>{{ editingId ? 'Edit Menu Item' : 'Add Menu Item' }}</h3>
            <button class="close-btn" (click)="closeForm()">&times;</button>
          </div>
          
          <form (ngSubmit)="saveItem()" [formGroup]="itemForm" class="form">
            <div class="form-group">
              <label>Name *</label>
              <input type="text" formControlName="name" required />
            </div>
            
            <div class="form-group">
              <label>Description *</label>
              <textarea formControlName="description" required></textarea>
            </div>
            
            <div class="form-row">
              <div class="form-group">
                <label>Price *</label>
                <input type="number" step="0.01" formControlName="price" required />
              </div>
              
              <div class="form-group">
                <label>Category *</label>
                <select formControlName="categoryId" required>
                  <option value="">Select category</option>
                  <option *ngFor="let category of categories" [value]="category.id">
                    {{ category.name }}
                  </option>
                </select>
              </div>
            </div>
            
            <div class="form-group">
              <label>Calories</label>
              <input type="number" formControlName="calories" />
            </div>
            
            <div class="form-group">
              <label>Image URL</label>
              <input type="text" formControlName="imageUrl" />
            </div>
            
            <div class="form-group">
              <label class="checkbox">
                <input type="checkbox" formControlName="isVegetarian" />
                Vegetarian
              </label>
            </div>
            
            <div class="form-group">
              <label class="checkbox">
                <input type="checkbox" formControlName="isSpicy" />
                Spicy
              </label>
            </div>
            
            <div class="form-group">
              <label class="checkbox">
                <input type="checkbox" formControlName="isAvailable" />
                Available
              </label>
            </div>
            
            <div class="form-actions">
              <button type="submit" class="btn btn-primary" [disabled]="!itemForm.valid || saving">
                {{ saving ? 'Saving...' : 'Save' }}
              </button>
              <button type="button" class="btn btn-secondary" (click)="closeForm()">Cancel</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .menu-container {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .menu-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }

    .menu-filters {
      margin-bottom: 20px;
    }

    .menu-filters select {
      padding: 10px;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 14px;
    }

    .menu-items-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 20px;
    }

    .menu-item-card {
      border: 1px solid #eee;
      border-radius: 8px;
      padding: 15px;
      background: white;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      transition: transform 0.2s;
    }

    .menu-item-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0,0,0,0.15);
    }

    .item-header {
      display: flex;
      justify-content: space-between;
      align-items: start;
      margin-bottom: 10px;
    }

    .item-header h3 {
      margin: 0;
      font-size: 18px;
    }

    .price {
      font-size: 20px;
      font-weight: bold;
      color: #667eea;
    }

    .description {
      color: #666;
      font-size: 14px;
      margin: 10px 0;
      line-height: 1.4;
    }

    .item-tags {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin: 10px 0;
    }

    .tag {
      display: inline-block;
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
      background: #f0f0f0;
    }

    .tag.vegetarian {
      background: #d4edda;
      color: #155724;
    }

    .tag.spicy {
      background: #f8d7da;
      color: #721c24;
    }

    .tag.active {
      background: #d1ecf1;
      color: #0c5460;
    }

    .item-actions {
      display: flex;
      gap: 10px;
      margin-top: 15px;
    }

    .btn {
      padding: 8px 12px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
      transition: all 0.2s;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-secondary {
      background: #f0f0f0;
      color: #333;
    }

    .btn-danger {
      background: #dc3545;
      color: white;
    }

    .btn-sm {
      padding: 6px 10px;
      font-size: 12px;
      flex: 1;
    }

    .modal {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }

    .modal-content {
      background: white;
      border-radius: 8px;
      padding: 30px;
      width: 90%;
      max-width: 500px;
      max-height: 90vh;
      overflow-y: auto;
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }

    .close-btn {
      background: none;
      border: none;
      font-size: 24px;
      cursor: pointer;
    }

    .form-group {
      margin-bottom: 15px;
    }

    .form-group label {
      display: block;
      margin-bottom: 5px;
      font-weight: 500;
    }

    .form-group input[type="text"],
    .form-group input[type="number"],
    .form-group textarea,
    .form-group select {
      width: 100%;
      padding: 10px;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 14px;
    }

    .form-group textarea {
      resize: vertical;
      min-height: 80px;
    }

    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 15px;
    }

    .form-group.checkbox {
      display: flex;
      align-items: center;
    }

    .form-group.checkbox input {
      width: auto;
      margin-right: 8px;
    }

    .form-actions {
      display: flex;
      gap: 10px;
      margin-top: 20px;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .loading, .empty-state {
      text-align: center;
      padding: 40px;
      color: #666;
    }
  `]
})
export class MenuListComponent implements OnInit {
  menuItems: MenuItem[] = [];
  categories: Category[] = [];
  loading = false;
  saving = false;
  showForm = false;
  editingId: number | null = null;
  selectedCategoryId: string = '';

  itemForm = this.createEmptyForm();

  constructor(private menuService: MenuService) {}

  ngOnInit() {
    this.loadCategories();
    this.loadMenuItems();
  }

  private createEmptyForm() {
    return {
      name: '',
      description: '',
      price: 0,
      categoryId: '',
      isAvailable: true,
      imageUrl: '',
      calories: 0,
      isVegetarian: false,
      isSpicy: false
    };
  }

  loadCategories() {
    this.menuService.getCategories().subscribe(
      (response) => {
        if (response.success) {
          this.categories = response.data || [];
        }
      },
      (error) => console.error('Error loading categories:', error)
    );
  }

  loadMenuItems() {
    this.loading = true;
    this.menuService.getMenuItems().subscribe(
      (response) => {
        if (response.success) {
          this.menuItems = response.data || [];
        }
        this.loading = false;
      },
      (error) => {
        console.error('Error loading menu items:', error);
        this.loading = false;
      }
    );
  }

  filterByCategory() {
    if (!this.selectedCategoryId) {
      this.loadMenuItems();
      return;
    }

    this.loading = true;
    this.menuService.getMenuItemsByCategory(parseInt(this.selectedCategoryId)).subscribe(
      (response) => {
        if (response.success) {
          this.menuItems = response.data || [];
        }
        this.loading = false;
      },
      (error) => {
        console.error('Error filtering menu items:', error);
        this.loading = false;
      }
    );
  }

  openAddForm() {
    this.editingId = null;
    this.itemForm = this.createEmptyForm();
    this.showForm = true;
  }

  editItem(item: MenuItem) {
    this.editingId = item.id;
    this.itemForm = {
      name: item.name,
      description: item.description,
      price: item.price,
      categoryId: item.categoryId.toString(),
      isAvailable: item.isAvailable,
      imageUrl: item.imageUrl || '',
      calories: item.calories,
      isVegetarian: item.isVegetarian,
      isSpicy: item.isSpicy
    };
    this.showForm = true;
  }

  closeForm() {
    this.showForm = false;
    this.editingId = null;
  }

  saveItem() {
    this.saving = true;
    const request = {
      ...this.itemForm,
      categoryId: parseInt(this.itemForm.categoryId as any)
    };

    const operation = this.editingId 
      ? this.menuService.updateMenuItem(this.editingId, request as any)
      : this.menuService.createMenuItem(request as any);

    operation.subscribe(
      (response) => {
        if (response.success) {
          this.closeForm();
          this.loadMenuItems();
        }
        this.saving = false;
      },
      (error) => {
        console.error('Error saving item:', error);
        this.saving = false;
      }
    );
  }

  deleteItem(id: number) {
    if (confirm('Are you sure you want to delete this item?')) {
      this.menuService.deleteMenuItem(id).subscribe(
        (response) => {
          if (response.success) {
            this.loadMenuItems();
          }
        },
        (error) => console.error('Error deleting item:', error)
      );
    }
  }
}
