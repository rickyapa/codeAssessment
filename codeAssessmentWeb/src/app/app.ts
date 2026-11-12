import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { Product, StockOperation } from './product.model';
import { ProductApiService } from './product-api.service';

type SortColumn = 'id' | 'name' | 'stock';
type SortDirection = 'asc' | 'desc';

@Component({
  imports: [CommonModule, FormsModule],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  private readonly api = inject(ProductApiService);

  readonly products = signal<Product[]>([]);
  readonly loading = signal(false);
  readonly mutating = signal(false);
  readonly error = signal<string | null>(null);
  readonly modalError = signal<string | null>(null);
  readonly pageSize = 10;
  readonly page = signal(1);
  readonly sortColumn = signal<SortColumn | null>(null);
  readonly sortDirection = signal<SortDirection>('asc');
  readonly dialog = signal<'product' | 'stock' | 'delete' | null>(null);
  readonly editingProduct = signal<Product | null>(null);
  readonly stockProduct = signal<Product | null>(null);
  readonly deletingProduct = signal<Product | null>(null);
  readonly stockOperation = signal<StockOperation>('add');
  readonly activeCount = computed(() => this.products().filter((product) => product.active).length);
  readonly stockUnits = computed(() => this.products().reduce((total, product) => total + product.stock, 0));
  readonly lowStockCount = computed(() => this.products().filter((product) => product.stock < 10).length);
  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.products().length / this.pageSize)));
  readonly sortedProducts = computed(() => {
    const column = this.sortColumn();
    if (!column) {
      return this.products();
    }

    const direction = this.sortDirection() === 'asc' ? 1 : -1;
    return [...this.products()].sort((first, second) => {
      const comparison = column === 'name'
        ? first.name.localeCompare(second.name, undefined, { sensitivity: 'base' })
        : first[column] - second[column];
      return comparison * direction;
    });
  });
  readonly pageProducts = computed(() => {
    const start = (this.page() - 1) * this.pageSize;
    return this.sortedProducts().slice(start, start + this.pageSize);
  });
  readonly firstVisibleProduct = computed(() => this.products().length ? (this.page() - 1) * this.pageSize + 1 : 0);
  readonly lastVisibleProduct = computed(() => Math.min(this.page() * this.pageSize, this.products().length));

  searchInput = '';
  minStock: number | null = null;
  maxStock: number | null = null;
  productName = '';
  productStock = 0;
  stockQuantity = 1;

  private filterMode: 'all' | 'search' | 'stock' = 'all';
  private appliedSearch = '';
  private appliedMin = 0;
  private appliedMax = 0;

  ngOnInit(): void {
    this.loadProducts();
  }

  applySearch(): void {
    const name = this.searchInput.trim();
    if (!name) {
      this.clearFilters();
      return;
    }

    this.error.set(null);
    this.minStock = null;
    this.maxStock = null;
    this.appliedSearch = name;
    this.filterMode = 'search';
    this.loadProducts();
  }

  applyStockFilter(): void {
    if (this.minStock === null || this.maxStock === null || this.minStock < 0 || this.maxStock < this.minStock) {
      this.error.set('Enter a valid stock range. The maximum must be at least the minimum.');
      return;
    }

    this.error.set(null);
    this.searchInput = '';
    this.appliedMin = this.minStock;
    this.appliedMax = this.maxStock;
    this.filterMode = 'stock';
    this.loadProducts();
  }

  clearFilters(): void {
    this.error.set(null);
    this.searchInput = '';
    this.minStock = null;
    this.maxStock = null;
    this.filterMode = 'all';
    this.loadProducts();
  }

  refresh(): void {
    this.loadProducts();
  }

  previousPage(): void {
    this.page.update((currentPage) => Math.max(1, currentPage - 1));
  }

  nextPage(): void {
    this.page.update((currentPage) => Math.min(this.totalPages(), currentPage + 1));
  }

  sortBy(column: SortColumn): void {
    if (this.sortColumn() === column) {
      this.sortDirection.update((direction) => direction === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
    this.page.set(1);
  }

  openCreate(): void {
    this.clearErrors();
    this.editingProduct.set(null);
    this.productName = '';
    this.productStock = 0;
    this.dialog.set('product');
  }

  openEdit(product: Product): void {
    this.clearErrors();
    this.editingProduct.set(product);
    this.productName = product.name;
    this.productStock = product.stock;
    this.dialog.set('product');
  }

  openStock(product: Product, operation: StockOperation): void {
    this.clearErrors();
    this.stockProduct.set(product);
    this.stockOperation.set(operation);
    this.stockQuantity = 1;
    this.dialog.set('stock');
  }

  closeDialog(): void {
    if (!this.mutating()) {
      this.dialog.set(null);
      this.deletingProduct.set(null);
      this.clearErrors();
    }
  }

  saveProduct(): void {
    const name = this.productName.trim();
    if (!name || !Number.isInteger(this.productStock) || this.productStock < 0) {
      this.setOperationError('Enter a product name and a whole-number stock value of zero or more.');
      return;
    }

    const editing = this.editingProduct();
    const request = editing
      ? this.api.update(editing.id, { name, stock: this.productStock })
      : this.api.create({ name, stock: this.productStock });

    this.performMutation(request);
  }

  saveStock(): void {
    const product = this.stockProduct();
    if (!product || !Number.isInteger(this.stockQuantity) || this.stockQuantity < 1) {
      this.setOperationError('Enter a whole-number quantity greater than zero.');
      return;
    }

    if (this.stockOperation() === 'remove' && this.stockQuantity > product.stock) {
      this.setOperationError(`You can remove at most ${product.stock} units from this product.`);
      return;
    }

    const request = this.stockOperation() === 'add'
      ? this.api.addStock(product.id, this.stockQuantity)
      : this.api.decrementStock(product.id, this.stockQuantity);
    this.performMutation(request);
  }

  openDelete(product: Product): void {
    this.clearErrors();
    this.deletingProduct.set(product);
    this.dialog.set('delete');
  }

  confirmDelete(): void {
    const product = this.deletingProduct();
    if (this.mutating() || !product) {
      return;
    }

    this.performMutation(this.api.delete(product.id));
  }

  private loadProducts(): void {
    this.page.set(1);
    this.loading.set(true);
    const request = this.filterMode === 'search'
      ? this.api.search(this.appliedSearch)
      : this.filterMode === 'stock'
        ? this.api.getByStockRange(this.appliedMin, this.appliedMax)
        : this.api.getAll();

    request.subscribe({
      next: (products) => {
        this.products.set(products);
        this.loading.set(false);
      },
      error: (requestError: unknown) => {
        this.error.set(this.getErrorMessage(requestError));
        this.loading.set(false);
      },
    });
  }

  private performMutation(request: Observable<unknown>): void {
    this.clearErrors();
    this.mutating.set(true);
    request.subscribe({
      next: () => {
        this.mutating.set(false);
        this.dialog.set(null);
        this.deletingProduct.set(null);
        this.loadProducts();
      },
      error: (requestError: unknown) => {
        this.setOperationError(this.getErrorMessage(requestError));
        this.mutating.set(false);
      },
    });
  }

  private setOperationError(message: string): void {
    if (this.dialog()) {
      this.modalError.set(message);
      return;
    }

    this.error.set(message);
  }

  private clearErrors(): void {
    this.error.set(null);
    this.modalError.set(null);
  }

  private getErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      const body = error.error as { detail?: unknown; title?: unknown; message?: unknown } | string | null;
      const detail = typeof body === 'string'
        ? body
        : body?.detail ?? body?.message ?? body?.title;
      const message = typeof detail === 'string' ? detail : error.message;
      return `Request failed${error.status ? ` (${error.status})` : ''}: ${message}`;
    }

    return error instanceof Error ? error.message : 'An unexpected error occurred.';
  }
}
