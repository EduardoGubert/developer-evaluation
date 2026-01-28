import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponseWithData, PaginatedList } from '../models/api-response.model';
import { Sale, CreateSaleRequest } from '../models/sale.model';

@Injectable({ providedIn: 'root' })
export class SaleService {
  private apiUrl = `${environment.apiUrl}/sales`;

  constructor(private http: HttpClient) {}

  getSales(page = 1, size = 10, order?: string): Observable<PaginatedList<Sale>> {
    let params = new HttpParams()
      .set('_page', page.toString())
      .set('_size', size.toString());
    if (order) params = params.set('_order', order);

    return this.http.get<ApiResponseWithData<PaginatedList<Sale>>>(this.apiUrl, { params }).pipe(
      map(res => res.data)
    );
  }

  getSale(id: string): Observable<Sale> {
    return this.http.get<ApiResponseWithData<Sale>>(`${this.apiUrl}/${id}`).pipe(
      map(res => res.data)
    );
  }

  createSale(sale: CreateSaleRequest): Observable<Sale> {
    return this.http.post<ApiResponseWithData<Sale>>(this.apiUrl, sale).pipe(
      map(res => res.data)
    );
  }

  updateSale(id: string, sale: CreateSaleRequest): Observable<Sale> {
    return this.http.put<ApiResponseWithData<Sale>>(`${this.apiUrl}/${id}`, sale).pipe(
      map(res => res.data)
    );
  }

  deleteSale(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  cancelSale(id: string): Observable<any> {
    return this.http.patch<ApiResponseWithData<any>>(`${this.apiUrl}/${id}/cancel`, {}).pipe(
      map(res => res.data)
    );
  }

  cancelSaleItem(saleId: string, itemId: string): Observable<any> {
    return this.http.patch<ApiResponseWithData<any>>(`${this.apiUrl}/${saleId}/items/${itemId}/cancel`, {}).pipe(
      map(res => res.data)
    );
  }
}
