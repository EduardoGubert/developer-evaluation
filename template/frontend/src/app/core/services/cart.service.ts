import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponseWithData, PaginatedList } from '../models/api-response.model';
import { Cart, CreateCartRequest, CheckoutCartRequest, CheckoutCartResponse } from '../models/cart.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private apiUrl = `${environment.apiUrl}/carts`;

  constructor(private http: HttpClient) {}

  getCarts(page = 1, size = 10, order?: string): Observable<PaginatedList<Cart>> {
    let params = new HttpParams()
      .set('_page', page.toString())
      .set('_size', size.toString());
    if (order) params = params.set('_order', order);

    return this.http.get<ApiResponseWithData<PaginatedList<Cart>>>(this.apiUrl, { params }).pipe(
      map(res => res.data)
    );
  }

  getCart(id: string): Observable<Cart> {
    return this.http.get<ApiResponseWithData<Cart>>(`${this.apiUrl}/${id}`).pipe(
      map(res => res.data)
    );
  }

  createCart(cart: CreateCartRequest): Observable<Cart> {
    return this.http.post<ApiResponseWithData<Cart>>(this.apiUrl, cart).pipe(
      map(res => res.data)
    );
  }

  updateCart(id: string, cart: CreateCartRequest): Observable<Cart> {
    return this.http.put<ApiResponseWithData<Cart>>(`${this.apiUrl}/${id}`, cart).pipe(
      map(res => res.data)
    );
  }

  deleteCart(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  checkout(cartId: string, request: CheckoutCartRequest): Observable<CheckoutCartResponse> {
    return this.http.post<ApiResponseWithData<CheckoutCartResponse>>(`${this.apiUrl}/${cartId}/checkout`, request).pipe(
      map(res => res.data)
    );
  }
}
