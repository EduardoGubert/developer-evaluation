import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CreateProductRequest } from '../../../core/models/product.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-product-create',
  templateUrl: './product-create.component.html',
  styleUrls: ['./product-create.component.scss']
})
export class ProductCreateComponent implements OnInit {
  product: CreateProductRequest = {
    title: '',
    price: 0,
    description: '',
    category: '',
    image: '',
    rating: { rate: 0, count: 0 }
  };

  categories: string[] = [];
  filteredCategories: string[] = [];
  isLoading = false;

  constructor(
    private productService: ProductService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.filteredCategories = categories;
      }
    });
  }

  filterCategories(value: string): void {
    const filter = value.toLowerCase();
    this.filteredCategories = this.categories.filter(cat => cat.toLowerCase().includes(filter));
  }

  onSubmit(): void {
    if (!this.product.title || !this.product.category || this.product.price <= 0) {
      this.snackBar.open('Title, category and a valid price are required.', 'Close', { duration: 3000 });
      return;
    }

    this.isLoading = true;
    this.productService.createProduct(this.product).subscribe({
      next: (created) => {
        this.snackBar.open('Product created successfully!', 'Close', { duration: 3000 });
        this.router.navigate(['/products', created.id]);
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/products']);
  }
}
