export interface Product {
  id: number;
  name: string;
  stock: number;
  createdAt: string;
  updatedAt: string | null;
  active: boolean;
}

export interface CreateProductRequest {
  name: string;
  stock: number;
}

export type UpdateProductRequest = CreateProductRequest;

export type StockOperation = 'add' | 'remove';
