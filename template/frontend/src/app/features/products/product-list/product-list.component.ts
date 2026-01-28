import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ShoppingCartService } from '../../../core/services/shopping-cart.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  categories: string[] = [];
  selectedCategory = '';
  currentPage = 1;
  totalPages = 1;
  totalItems = 0;
  pageSize = 10;
  isLoading = false;
  orderBy = '';

  constructor(
    private productService: ProductService,
    private router: Router,
    private snackBar: MatSnackBar,
    private shoppingCartService: ShoppingCartService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    const service$ = this.selectedCategory
      ? this.productService.getProductsByCategory(this.selectedCategory, this.currentPage, this.pageSize, this.orderBy || undefined)
      : this.productService.getProducts(this.currentPage, this.pageSize, this.orderBy || undefined);

    service$.subscribe({
      next: (res) => {
        this.products = res.data;
        this.totalPages = res.totalPages;
        this.totalItems = res.totalItems;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => this.categories = categories
    });
  }

  onCategoryChange(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadProducts();
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  viewProduct(id: string): void {
    this.router.navigate(['/products', id]);
  }

  deleteProduct(id: string): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(id).subscribe({
        next: () => {
          this.snackBar.open('Product deleted successfully', 'Close', { duration: 3000 });
          this.loadProducts();
        }
      });
    }
  }

  addToCart(product: Product, event: Event): void {
    event.stopPropagation();
    this.shoppingCartService.addToCart(product.id).subscribe();
  }
}
