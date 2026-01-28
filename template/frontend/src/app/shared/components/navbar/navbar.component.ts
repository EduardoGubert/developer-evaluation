import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { ShoppingCartService } from '../../../core/services/shopping-cart.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  cartItemCount$: Observable<number>;

  constructor(
    public authService: AuthService,
    private shoppingCartService: ShoppingCartService
  ) {
    this.cartItemCount$ = this.shoppingCartService.itemCount$;
  }

  logout(): void {
    this.authService.logout();
  }
}
