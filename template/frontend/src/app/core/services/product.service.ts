import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponseWithData, PaginatedList } from '../models/api-response.model';
import { Product, CreateProductRequest } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = `${environment.apiUrl}/products`;

  constructor(private http: HttpClient) {}

  getProducts(page = 1, size = 10, order?: string): Observable<PaginatedList<Product>> {
    let params = new HttpParams()
      .set('_page', page.toString())
      .set('_size', size.toString());
    if (order) params = params.set('_order', order);

    return this.http.get<ApiResponseWithData<PaginatedList<Product>>>(this.apiUrl, { params }).pipe(
      map(res => res.data)
    );
  }

  getProduct(id: string): Observable<Product> {
    return this.http.get<ApiResponseWithData<Product>>(`${this.apiUrl}/${id}`).pipe(
      map(res => res.data)
    );
  }

  createProduct(product: CreateProductRequest): Observable<Product> {
    return this.http.post<ApiResponseWithData<Product>>(this.apiUrl, product).pipe(
      map(res => res.data)
    );
  }

  updateProduct(id: string, product: CreateProductRequest): Observable<Product> {
    return this.http.put<ApiResponseWithData<Product>>(`${this.apiUrl}/${id}`, product).pipe(
      map(res => res.data)
    );
  }

  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getCategories(): Observable<string[]> {
    return this.http.get<ApiResponseWithData<string[]>>(`${this.apiUrl}/categories`).pipe(
      map(res => res.data)
    );
  }

  getProductsByCategory(category: string, page = 1, size = 10, order?: string): Observable<PaginatedList<Product>> {
    let params = new HttpParams()
      .set('_page', page.toString())
      .set('_size', size.toString());
    if (order) params = params.set('_order', order);

    return this.http.get<ApiResponseWithData<PaginatedList<Product>>>(`${this.apiUrl}/category/${category}`, { params }).pipe(
      map(res => res.data)
    );
  }
}
