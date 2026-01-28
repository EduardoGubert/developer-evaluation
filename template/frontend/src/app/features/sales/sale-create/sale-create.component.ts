import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { SaleService } from '../../../core/services/sale.service';
import { CreateSaleRequest, CreateSaleItemRequest } from '../../../core/models/sale.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-sale-create',
  templateUrl: './sale-create.component.html',
  styleUrls: ['./sale-create.component.scss']
})
export class SaleCreateComponent {
  sale: CreateSaleRequest = {
    saleNumber: '',
    saleDate: new Date().toISOString(),
    customerId: '',
    customerName: '',
    branchId: '',
    branchName: '',
    items: []
  };

  newItem: CreateSaleItemRequest = {
    productId: '',
    productName: '',
    quantity: 1,
    unitPrice: 0
  };

  isSubmitting = false;

  constructor(
    private saleService: SaleService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  addItem(): void {
    if (!this.newItem.productName || this.newItem.quantity <= 0 || this.newItem.unitPrice <= 0) {
      this.snackBar.open('Please fill all item fields correctly', 'Close', { duration: 3000 });
      return;
    }
    if (this.newItem.quantity > 20) {
      this.snackBar.open('Maximum 20 items per product', 'Close', { duration: 3000 });
      return;
    }

    this.sale.items.push({
      ...this.newItem,
      productId: this.newItem.productId || crypto.randomUUID()
    });
    this.newItem = { productId: '', productName: '', quantity: 1, unitPrice: 0 };
  }

  removeItem(index: number): void {
    this.sale.items.splice(index, 1);
  }

  getEstimatedTotal(): number {
    return this.sale.items.reduce((total, item) => total + (item.quantity * item.unitPrice), 0);
  }

  onSubmit(): void {
    if (!this.sale.saleNumber || !this.sale.customerName || !this.sale.branchName || this.sale.items.length === 0) {
      this.snackBar.open('Please fill all required fields and add at least one item', 'Close', { duration: 3000 });
      return;
    }

    this.isSubmitting = true;
    this.sale.customerId = this.sale.customerId || crypto.randomUUID();
    this.sale.branchId = this.sale.branchId || crypto.randomUUID();

    this.saleService.createSale(this.sale).subscribe({
      next: (result) => {
        this.snackBar.open('Sale created successfully', 'Close', { duration: 3000 });
        this.router.navigate(['/sales', result.id]);
      },
      error: () => {
        this.isSubmitting = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/sales']);
  }
}
