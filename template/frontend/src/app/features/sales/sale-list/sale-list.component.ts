import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SaleService } from '../../../core/services/sale.service';
import { Sale } from '../../../core/models/sale.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-sale-list',
  templateUrl: './sale-list.component.html',
  styleUrls: ['./sale-list.component.scss']
})
export class SaleListComponent implements OnInit {
  sales: Sale[] = [];
  currentPage = 1;
  totalPages = 1;
  totalItems = 0;
  pageSize = 10;
  isLoading = false;
  displayedColumns = ['saleNumber', 'customer', 'branch', 'totalAmount', 'status', 'actions'];

  constructor(
    private saleService: SaleService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadSales();
  }

  loadSales(): void {
    this.isLoading = true;
    this.saleService.getSales(this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        this.sales = res.data;
        this.totalPages = res.totalPages;
        this.totalItems = res.totalItems;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  viewSale(id: string): void {
    this.router.navigate(['/sales', id]);
  }

  createSale(): void {
    this.router.navigate(['/sales', 'new']);
  }

  cancelSale(id: string, event: Event): void {
    event.stopPropagation();
    if (confirm('Are you sure you want to cancel this sale?')) {
      this.saleService.cancelSale(id).subscribe({
        next: () => {
          this.snackBar.open('Sale cancelled successfully', 'Close', { duration: 3000 });
          this.loadSales();
        }
      });
    }
  }

  deleteSale(id: string, event: Event): void {
    event.stopPropagation();
    if (confirm('Are you sure you want to delete this sale?')) {
      this.saleService.deleteSale(id).subscribe({
        next: () => {
          this.snackBar.open('Sale deleted successfully', 'Close', { duration: 3000 });
          this.loadSales();
        }
      });
    }
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadSales();
  }
}
