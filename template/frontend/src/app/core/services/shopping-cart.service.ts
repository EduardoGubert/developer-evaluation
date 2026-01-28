import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, tap, catchError, switchMap } from 'rxjs/operators';
import { CartService } from './cart.service';
import { AuthService } from './auth.service';
import { Cart, CartProduct, CheckoutCartRequest, CheckoutCartResponse } from '../models/cart.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class ShoppingCartService {
  private cartSubject = new BehaviorSubject<Cart | null>(null);
  public cart$ = this.cartSubject.asObservable();
  public itemCount$ = this.cart$.pipe(
    map(cart => cart?.products?.reduce((sum, p) => sum + p.quantity, 0) || 0)
  );

  constructor(
    private cartService: CartService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.authService.currentUser$.subscribe(user => {
      if (user?.userId) {
        this.loadUserCart(user.userId);
      } else {
        this.cartSubject.next(null);
      }
    });
  }

  get currentCart(): Cart | null {
    return this.cartSubject.value;
  }

  loadUserCart(userId: string): void {
    this.cartService.getCarts(1, 100).subscribe({
      next: (res) => {
        const userCart = res.data.find(c => c.userId === userId);
        this.cartSubject.next(userCart || null);
      },
      error: () => {
        this.cartSubject.next(null);
      }
    });
  }

  addToCart(productId: string, quantity: number = 1): Observable<Cart> {
    const userId = this.authService.currentUser?.userId;
    if (!userId) {
      this.snackBar.open('Please login to add items to cart', 'Close', { duration: 3000 });
      return of(null as any);
    }

    const currentCart = this.cartSubject.value;

    if (currentCart) {
      const products = [...currentCart.products];
      const existing = products.find(p => p.productId === productId);
      if (existing) {
        const newQty = existing.quantity + quantity;
        if (newQty > 20) {
          this.snackBar.open('Cannot add more than 20 identical items', 'Close', { duration: 3000 });
          return of(currentCart);
        }
        existing.quantity = newQty;
      } else {
        if (quantity > 20) {
          this.snackBar.open('Cannot add more than 20 identical items', 'Close', { duration: 3000 });
          return of(currentCart);
        }
        products.push({ productId, quantity });
      }

      return this.cartService.updateCart(currentCart.id, {
        userId,
        date: new Date().toISOString(),
        products
      }).pipe(
        tap(cart => {
          this.cartSubject.next(cart);
          this.snackBar.open('Cart updated', 'Close', { duration: 2000 });
        }),
        catchError(err => {
          this.snackBar.open('Failed to update cart', 'Close', { duration: 3000 });
          return of(currentCart);
        })
      );
    } else {
      if (quantity > 20) {
        this.snackBar.open('Cannot add more than 20 identical items', 'Close', { duration: 3000 });
        return of(null as any);
      }
      return this.cartService.createCart({
        userId,
        date: new Date().toISOString(),
        products: [{ productId, quantity }]
      }).pipe(
        tap(cart => {
          this.cartSubject.next(cart);
          this.snackBar.open('Product added to cart', 'Close', { duration: 2000 });
        }),
        catchError(err => {
          this.snackBar.open('Failed to create cart', 'Close', { duration: 3000 });
          return of(null as any);
        })
      );
    }
  }

  updateItemQuantity(productId: string, quantity: number): Observable<Cart | null> {
    const currentCart = this.cartSubject.value;
    const userId = this.authService.currentUser?.userId;
    if (!currentCart || !userId) return of(null);

    if (quantity > 20) {
      this.snackBar.open('Cannot have more than 20 identical items', 'Close', { duration: 3000 });
      return of(currentCart);
    }

    const products = currentCart.products.map(p =>
      p.productId === productId ? { ...p, quantity } : { ...p }
    );

    return this.cartService.updateCart(currentCart.id, {
      userId,
      date: new Date().toISOString(),
      products
    }).pipe(
      tap(cart => this.cartSubject.next(cart)),
      catchError(err => {
        this.snackBar.open('Failed to update quantity', 'Close', { duration: 3000 });
        return of(currentCart);
      })
    );
  }

  removeItem(productId: string): Observable<Cart | null> {
    const currentCart = this.cartSubject.value;
    const userId = this.authService.currentUser?.userId;
    if (!currentCart || !userId) return of(null);

    const products = currentCart.products.filter(p => p.productId !== productId);

    if (products.length === 0) {
      return this.cartService.deleteCart(currentCart.id).pipe(
        tap(() => {
          this.cartSubject.next(null);
          this.snackBar.open('Cart cleared', 'Close', { duration: 2000 });
        }),
        map(() => null),
        catchError(err => {
          this.snackBar.open('Failed to remove item', 'Close', { duration: 3000 });
          return of(currentCart);
        })
      );
    }

    return this.cartService.updateCart(currentCart.id, {
      userId,
      date: new Date().toISOString(),
      products
    }).pipe(
      tap(cart => {
        this.cartSubject.next(cart);
        this.snackBar.open('Item removed', 'Close', { duration: 2000 });
      }),
      catchError(err => {
        this.snackBar.open('Failed to remove item', 'Close', { duration: 3000 });
        return of(currentCart);
      })
    );
  }

  checkout(branchId: string, branchName: string): Observable<CheckoutCartResponse | null> {
    const currentCart = this.cartSubject.value;
    if (!currentCart) {
      this.snackBar.open('No cart to checkout', 'Close', { duration: 3000 });
      return of(null);
    }

    const request: CheckoutCartRequest = { branchId, branchName };
    return this.cartService.checkout(currentCart.id, request).pipe(
      tap(response => {
        this.cartSubject.next(null);
        this.snackBar.open(
          `Checkout complete! Sale #${response.saleNumber} - Total: R$ ${response.totalAmount.toFixed(2)}`,
          'Close',
          { duration: 5000 }
        );
      }),
      catchError(err => {
        this.snackBar.open('Checkout failed. Please try again.', 'Close', { duration: 3000 });
        return of(null);
      })
    );
  }
}
