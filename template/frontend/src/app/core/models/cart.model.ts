export interface CartProduct {
  productId: string;
  quantity: number;
}

export interface Cart {
  id: string;
  userId: string;
  date: string;
  products: CartProduct[];
}

export interface CreateCartRequest {
  userId: string;
  date: string;
  products: CartProduct[];
}

export interface CheckoutCartRequest {
  branchId: string;
  branchName: string;
}

export interface CheckoutCartResponse {
  saleId: string;
  saleNumber: string;
  totalAmount: number;
  items: CheckoutCartItemResponse[];
}

export interface CheckoutCartItemResponse {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  totalAmount: number;
}
