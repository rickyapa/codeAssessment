import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateProductRequest, Product, UpdateProductRequest } from './product.model';

const PRODUCTS_URL = '/api/products';

@Injectable({ providedIn: 'root' })
export class ProductApiService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(PRODUCTS_URL);
  }

  search(name: string): Observable<Product[]> {
    const params = new HttpParams().set('name', name);
    return this.http.get<Product[]>(`${PRODUCTS_URL}/search`, { params });
  }

  getByStockRange(min: number, max: number): Observable<Product[]> {
    const params = new HttpParams().set('min', min).set('max', max);
    return this.http.get<Product[]>(`${PRODUCTS_URL}/stock-level`, { params });
  }

  create(product: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(PRODUCTS_URL, product);
  }

  update(id: number, product: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${PRODUCTS_URL}/${id}`, product);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${PRODUCTS_URL}/${id}`);
  }

  addStock(id: number, quantity: number): Observable<Product> {
    return this.http.post<Product>(`${PRODUCTS_URL}/${id}/add-to-stock/${quantity}`, null);
  }

  decrementStock(id: number, quantity: number): Observable<Product> {
    return this.http.post<Product>(`${PRODUCTS_URL}/${id}/decrement-stock/${quantity}`, null);
  }
}
