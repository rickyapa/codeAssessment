import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ProductApiService } from './product-api.service';

describe('ProductApiService', () => {
  let api: ProductApiService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProductApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(ProductApiService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('uses the product API endpoints for list, filters, and mutations', () => {
    api.getAll().subscribe();
    api.search('lamp').subscribe();
    api.getByStockRange(2, 20).subscribe();
    api.create({ name: 'Lamp', stock: 4 }).subscribe();
    api.update(3, { name: 'Desk lamp', stock: 6 }).subscribe();
    api.delete(3).subscribe();
    api.addStock(3, 5).subscribe();
    api.decrementStock(3, 2).subscribe();

    expect(httpTesting.expectOne((request) =>
      request.url === '/api/products' && request.method === 'GET',
    )).toBeTruthy();
    expect(httpTesting.expectOne((request) =>
      request.url === '/api/products/search' && request.params.get('name') === 'lamp',
    )).toBeTruthy();
    expect(httpTesting.expectOne((request) =>
      request.url === '/api/products/stock-level'
        && request.params.get('min') === '2'
        && request.params.get('max') === '20',
    )).toBeTruthy();

    const createRequest = httpTesting.expectOne((request) =>
      request.url === '/api/products' && request.method === 'POST',
    );
    expect(createRequest.request.body).toEqual({ name: 'Lamp', stock: 4 });

    const updateRequest = httpTesting.expectOne((request) =>
      request.url === '/api/products/3' && request.method === 'PUT',
    );
    expect(updateRequest.request.body).toEqual({ name: 'Desk lamp', stock: 6 });
    expect(httpTesting.expectOne((request) =>
      request.url === '/api/products/3' && request.method === 'DELETE',
    )).toBeTruthy();
    expect(httpTesting.expectOne('/api/products/3/add-to-stock/5').request.method).toBe('POST');
    expect(httpTesting.expectOne('/api/products/3/decrement-stock/2').request.method).toBe('POST');
  });
});
