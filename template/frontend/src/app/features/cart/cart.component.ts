import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription, forkJoin } from 'rxjs';
import { ShoppingCartService } from '../../core/services/shopping-cart.service';
import { ProductService } from '../../core/services/product.service';
import { Cart, CartProduct } from '../../core/models/cart.model';
import { Product } from '../../core/models/product.model';
import { MatSnackBar } from '@angular/material/snack-bar';

export interface CartItemDisplay {
  productId: string;
  productName: string;
  productImage: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit, OnDestroy {
  cart: Cart | null = null;
  cartItems: CartItemDisplay[] = [];
  isLoading = false;
  isCheckingOut = false;
  branchName = '';
  displayedColumns = ['image', 'product', 'price', 'quantity', 'subtotal', 'actions'];

  private cartSub!: Subscription;

  constructor(
    private shoppingCartService: ShoppingCartService,
    private productService: ProductService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.cartSub = this.shoppingCartService.cart$.subscribe(cart => {
      this.cart = cart;
      if (cart && cart.products && cart.products.length > 0) {
        this.loadProductDetails(cart.products);
      } else {
        this.cartItems = [];
      }
    });
  }

  ngOnDestroy(): void {
    if (this.cartSub) {
      this.cartSub.unsubscribe();
    }
  }

  loadProductDetails(products: CartProduct[]): void {
    this.isLoading = true;
    const requests = products.map(p => this.productService.getProduct(p.productId));
    forkJoin(requests).subscribe({
      next: (productDetails: Product[]) => {
        this.cartItems = products.map((cartItem, index) => {
          const product = productDetails[index];
          return {
            productId: cartItem.productId,
            productName: product?.title || 'Unknown Product',
            productImage: product?.image || '',
            unitPrice: product?.price || 0,
            quantity: cartItem.quantity,
            subtotal: (product?.price || 0) * cartItem.quantity
          };
        });
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open('Failed to load product details', 'Close', { duration: 3000 });
      }
    });
  }

  get totalAmount(): number {
    return this.cartItems.reduce((sum, item) => sum + item.subtotal, 0);
  }

  get totalItemCount(): number {
    return this.cartItems.reduce((sum, item) => sum + item.quantity, 0);
  }

  getDiscountInfo(quantity: number): { rate: number; label: string } {
    if (quantity >= 10) return { rate: 20, label: '20% off' };
    if (quantity >= 4) return { rate: 10, label: '10% off' };
    return { rate: 0, label: '' };
  }

  updateQuantity(item: CartItemDisplay, newQuantity: number): void {
    if (newQuantity < 1 || newQuantity > 20) return;
    this.shoppingCartService.updateItemQuantity(item.productId, newQuantity).subscribe({
      next: () => {
        item.quantity = newQuantity;
        item.subtotal = item.unitPrice * newQuantity;
      }
    });
  }

  removeItem(item: CartItemDisplay): void {
    this.shoppingCartService.removeItem(item.productId).subscribe();
  }

  checkout(): void {
    if (!this.branchName.trim()) {
      this.snackBar.open('Please enter a branch name', 'Close', { duration: 3000 });
      return;
    }

    this.isCheckingOut = true;
    const branchId = crypto.randomUUID();
    this.shoppingCartService.checkout(branchId, this.branchName.trim()).subscribe({
      next: (response) => {
        this.isCheckingOut = false;
        if (response) {
          this.router.navigate(['/sales']);
        }
      },
      error: () => {
        this.isCheckingOut = false;
      }
    });
  }

  continueShopping(): void {
    this.router.navigate(['/products']);
  }
}
