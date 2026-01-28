import { Component, OnInit } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { Cart } from '../../core/models/cart.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  carts: Cart[] = [];
  currentPage = 1;
  totalPages = 1;
  totalCount = 0;
  pageSize = 10;
  isLoading = false;
  displayedColumns = ['id', 'userId', 'date', 'products', 'actions'];

  constructor(
    private cartService: CartService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadCarts();
  }

  loadCarts(): void {
    this.isLoading = true;
    this.cartService.getCarts(this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        this.carts = res.data;
        this.totalPages = res.totalPages;
        this.totalCount = res.totalCount;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  deleteCart(id: string): void {
    if (confirm('Are you sure you want to delete this cart?')) {
      this.cartService.deleteCart(id).subscribe({
        next: () => {
          this.snackBar.open('Cart deleted successfully', 'Close', { duration: 3000 });
          this.loadCarts();
        }
      });
    }
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadCarts();
  }
}
