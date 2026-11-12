import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App', () => {
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('loads and displays products from the API', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    httpTesting.expectOne('/api/products').flush([{
      id: 1,
      name: 'Desk lamp',
      stock: 12,
      createdAt: '2026-01-01T00:00:00Z',
      updatedAt: null,
      active: true,
    }]);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('h1')?.textContent).toContain('Products');
    expect(element.textContent).toContain('Desk lamp');
    const table = element.querySelector('table');
    expect(Array.from(table?.querySelectorAll('thead th') ?? []).map((header) =>
      header.querySelector('.sort-label')?.textContent?.trim() ?? header.textContent?.trim()))
      .toEqual(['Id', 'Product', 'Stock level', 'Actions']);
    expect(Array.from(table?.querySelectorAll('tbody tr:first-child td') ?? []).slice(0, 3).map((cell) => cell.textContent?.trim()))
      .toEqual(['1', 'Desk lamp', '12']);
    expect(table?.querySelector('.stock-track')).toBeNull();
    expect(table?.querySelector('.pagination')).toBeNull();
  });

  it('confirms product deletion in an in-app dialog', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpTesting.expectOne('/api/products').flush([{
      id: 1,
      name: 'Desk lamp',
      stock: 12,
      createdAt: '2026-01-01T00:00:00Z',
      updatedAt: null,
      active: true,
    }]);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    element.querySelector<HTMLButtonElement>('.delete-button')?.click();
    fixture.detectChanges();

    expect(element.querySelector('[role="alertdialog"]')?.textContent).toContain('Desk lamp');
    expect(element.querySelector('.delete-button svg')).not.toBeNull();
    httpTesting.expectNone('/api/products/1');

    element.querySelector<HTMLButtonElement>('.button-danger')?.click();
    fixture.detectChanges();
    httpTesting.expectOne({ method: 'DELETE', url: '/api/products/1' }).flush(null);
    httpTesting.expectOne('/api/products').flush([]);
    fixture.detectChanges();

    expect(element.querySelector('[role="alertdialog"]')).toBeNull();
  });

  it('shows ten products per page and resets to the first page when results change', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const products = Array.from({ length: 12 }, (_, index) => ({
      id: index + 1,
      name: `Product ${index + 1}`,
      stock: 12,
      createdAt: '2026-01-01T00:00:00Z',
      updatedAt: null,
      active: true,
    }));
    httpTesting.expectOne('/api/products').flush(products);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const rows = () => element.querySelectorAll('tbody tr');
    expect(rows().length).toBe(10);
    expect(element.querySelector('.table-footer')?.textContent).toContain('Showing 1–10 of 12 products');
    expect(element.querySelector('.page-indicator')?.textContent).toContain('Page 1 of 2');
    expect(element.querySelector<HTMLButtonElement>('[aria-label="Previous page"]')?.disabled).toBe(true);

    element.querySelector<HTMLButtonElement>('[aria-label="Next page"]')?.click();
    fixture.detectChanges();
    expect(rows().length).toBe(2);
    expect(rows().item(0).textContent).toContain('Product 11');
    expect(element.querySelector('.table-footer')?.textContent).toContain('Showing 11–12 of 12 products');
    expect(element.querySelector<HTMLButtonElement>('[aria-label="Next page"]')?.disabled).toBe(true);

    const app = fixture.componentInstance;
    app.searchInput = 'Product';
    app.applySearch();
    const matchingProducts = Array.from({ length: 11 }, (_, index) => ({
      ...products[index],
      id: index + 101,
      name: `Matching product ${index + 1}`,
    }));
    httpTesting.expectOne('/api/products/search?name=Product').flush(matchingProducts);
    fixture.detectChanges();

    expect(rows().length).toBe(10);
    expect(rows().item(0).textContent).toContain('Matching product 1');
    expect(element.querySelector('.page-indicator')?.textContent).toContain('Page 1 of 2');
  });

  it('sorts the full product list by each sortable column and toggles direction', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    httpTesting.expectOne('/api/products').flush([
      { id: 3, name: 'Zulu', stock: 2, createdAt: '2026-01-01T00:00:00Z', updatedAt: null, active: true },
      { id: 1, name: 'apple', stock: 15, createdAt: '2026-01-01T00:00:00Z', updatedAt: null, active: true },
      { id: 2, name: 'Maple', stock: 7, createdAt: '2026-01-01T00:00:00Z', updatedAt: null, active: true },
    ]);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const rows = () => Array.from(element.querySelectorAll('tbody tr')).map((row) => row.textContent ?? '');

    element.querySelector<HTMLButtonElement>('[aria-label="Sort by Product"]')?.click();
    fixture.detectChanges();
    expect(rows()[0]).toContain('apple');
    expect(element.querySelector('th[aria-sort="ascending"] .sort-button')?.getAttribute('data-sort-state')).toBe('asc');

    element.querySelector<HTMLButtonElement>('[aria-label="Sort by Product"]')?.click();
    fixture.detectChanges();
    expect(rows()[0]).toContain('Zulu');
    expect(element.querySelector('th[aria-sort="descending"] .sort-button')?.getAttribute('data-sort-state')).toBe('desc');

    element.querySelector<HTMLButtonElement>('[aria-label="Sort by Stock level"]')?.click();
    fixture.detectChanges();
    expect(rows()[0]).toContain('Zulu');
    expect(rows()[1]).toContain('Maple');
    expect(rows()[2]).toContain('apple');

    element.querySelector<HTMLButtonElement>('[aria-label="Sort by Id"]')?.click();
    fixture.detectChanges();
    expect(rows()[0]).toContain('apple');
    expect(rows()[1]).toContain('Maple');
    expect(rows()[2]).toContain('Zulu');
  });

  it('disables the remove stock action when stock is zero', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    httpTesting.expectOne('/api/products').flush([{
      id: 1,
      name: 'Out of stock item',
      stock: 0,
      createdAt: '2026-01-01T00:00:00Z',
      updatedAt: null,
      active: true,
    }]);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector<HTMLButtonElement>('.stock-remove')?.disabled).toBe(true);
    expect(element.querySelector<HTMLButtonElement>('.stock-add')?.disabled).toBe(false);
  });

  it('prevents removing more stock than the product has', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    httpTesting.expectOne('/api/products').flush([{
      id: 1,
      name: 'Desk lamp',
      stock: 5,
      createdAt: '2026-01-01T00:00:00Z',
      updatedAt: null,
      active: true,
    }]);
    fixture.detectChanges();

    const app = fixture.componentInstance;
    app.openStock(app.products()[0], 'remove');
    app.stockQuantity = 6;
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector<HTMLInputElement>('#stock-quantity')?.max).toBe('5');
    expect(element.querySelector('.field-help')?.textContent).toContain('from 1 to 5');

    app.saveStock();
    fixture.detectChanges();
    expect(app.modalError()).toContain('You can remove at most 5 units');
    expect(element.querySelector('[role="alert"]')?.textContent).toContain('You can remove at most 5 units');
    expect(element.querySelector('.error-banner')).toBeNull();
    httpTesting.expectNone('/api/products/1/decrement-stock/6');
  });

  it('shows mutation API errors inside the open modal', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    httpTesting.expectOne('/api/products').flush([{
      id: 1,
      name: 'Desk lamp',
      stock: 5,
      createdAt: '2026-01-01T00:00:00Z',
      updatedAt: null,
      active: true,
    }]);
    fixture.detectChanges();

    const app = fixture.componentInstance;
    app.openStock(app.products()[0], 'remove');
    app.stockQuantity = 3;
    app.saveStock();
    httpTesting.expectOne('/api/products/1/decrement-stock/3').flush(
      { title: 'Stock changed elsewhere.' },
      { status: 409, statusText: 'Conflict' },
    );
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('[aria-labelledby="stock-dialog-title"]')).not.toBeNull();
    expect(element.querySelector('[role="alert"]')?.textContent).toContain('Stock changed elsewhere.');
    expect(element.querySelector('.error-banner')).toBeNull();
  });

  it('keeps page-level validation errors in the main banner', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    httpTesting.expectOne('/api/products').flush([]);
    fixture.detectChanges();

    const app = fixture.componentInstance;
    app.minStock = -1;
    app.maxStock = 5;
    app.applyStockFilter();
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('.error-banner')?.textContent).toContain('Enter a valid stock range');
    expect(element.querySelector('.modal-error')).toBeNull();
  });
});
