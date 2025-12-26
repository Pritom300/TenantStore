export interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  stock: number;
  imageUrl?: string;
  isActive: boolean;
  createdAt: Date;
}

export interface CreateProductDto {
  name: string;
  description?: string;
  price: number;
  stock: number;
  imageUrl?:string
}

export interface UpdateProductDto {
  name: string;
  description?: string;
  price: number;
  stock: number;
  imageUrl?:string
}