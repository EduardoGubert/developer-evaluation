import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SaleService } from '../../../core/services/sale.service';
import { Sale } from '../../../core/models/sale.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-sale-detail',
  templateUrl: './sale-detail.component.html',
  styleUrls: ['./sale-detail.component.scss']
})
export class SaleDetailComponent implements OnInit {
  sale: Sale | null = null;
  isLoading = false;
  itemColumns = ['productName', 'quantity', 'unitPrice', 'discount', 'totalAmount', 'actions'];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private saleService: SaleService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadSale(id);
    }
  }

  loadSale(id: string): void {
    this.isLoading = true;
    this.saleService.getSale(id).subscribe({
      next: (sale) => {
        this.sale = sale;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['/sales']);
      }
    });
  }

  cancelSale(): void {
    if (this.sale && confirm('Are you sure you want to cancel this sale?')) {
      this.saleService.cancelSale(this.sale.id).subscribe({
        next: () => {
          this.snackBar.open('Sale cancelled successfully', 'Close', { duration: 3000 });
          this.loadSale(this.sale!.id);
        }
      });
    }
  }

  cancelItem(itemId: string): void {
    if (this.sale && confirm('Are you sure you want to cancel this item?')) {
      this.saleService.cancelSaleItem(this.sale.id, itemId).subscribe({
        next: () => {
          this.snackBar.open('Item cancelled successfully', 'Close', { duration: 3000 });
          this.loadSale(this.sale!.id);
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/sales']);
  }
}
