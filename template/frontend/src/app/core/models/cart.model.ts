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
