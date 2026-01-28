import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CreateProductRequest } from '../../../core/models/product.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.scss']
})
export class ProductEditComponent implements OnInit {
  productId = '';
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
  isLoadingProduct = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.filteredCategories = categories;
      }
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.productId = id;
      this.loadProduct(id);
    }
  }

  filterCategories(value: string): void {
    const filter = value.toLowerCase();
    this.filteredCategories = this.categories.filter(cat => cat.toLowerCase().includes(filter));
  }

  loadProduct(id: string): void {
    this.productService.getProduct(id).subscribe({
      next: (product) => {
        this.product = {
          title: product.title,
          price: product.price,
          description: product.description,
          category: product.category,
          image: product.image,
          rating: product.rating || { rate: 0, count: 0 }
        };
        this.isLoadingProduct = false;
      },
      error: () => {
        this.isLoadingProduct = false;
        this.router.navigate(['/products']);
      }
    });
  }

  onSubmit(): void {
    if (!this.product.title || !this.product.category || this.product.price <= 0) {
      this.snackBar.open('Title, category and a valid price are required.', 'Close', { duration: 3000 });
      return;
    }

    this.isLoading = true;
    this.productService.updateProduct(this.productId, this.product).subscribe({
      next: () => {
        this.snackBar.open('Product updated successfully!', 'Close', { duration: 3000 });
        this.router.navigate(['/products', this.productId]);
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/products', this.productId]);
  }
}
