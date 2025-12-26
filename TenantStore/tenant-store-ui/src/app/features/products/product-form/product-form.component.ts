import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CreateProductDto, UpdateProductDto } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
})
export class ProductFormComponent implements OnInit {
  productData: CreateProductDto | UpdateProductDto = {
    name: '',
    description: '',
    price: 0,
    stock: 0
  };
  
  productId: string | null = null;
  isEditMode = false;
  loading = false;
  errorMessage = '';

  constructor(
    private productService: ProductService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id');
    
    if (this.productId) {
      this.isEditMode = true;
      this.loadProduct(this.productId);
    }
  }

  loadProduct(id: string): void {
    this.productService.getById(id).subscribe({
      next: (product) => {
        this.productData = {
          name: product.name,
          description: product.description,
          price: product.price,
          stock: product.stock
        };
      },
      error: (error) => {
        this.errorMessage = 'Failed to load product';
        console.error('Error loading product:', error);
      }
    });
  }

  onSubmit(): void {
    if (!this.productData.name || this.productData.price < 0 || this.productData.stock < 0) {
      this.errorMessage = 'Please fill in all required fields correctly';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    if (this.isEditMode && this.productId) {
      this.productService.update(this.productId, this.productData as UpdateProductDto).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/products']);
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error.error?.message || 'Failed to update product';
        }
      });
    } else {
      this.productService.create(this.productData as CreateProductDto).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/products']);
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error.error?.message || 'Failed to create product';
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/products']);
  }
}