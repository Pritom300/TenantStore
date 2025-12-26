// import { Component, OnInit } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { FormsModule } from '@angular/forms';
// import { Router, ActivatedRoute } from '@angular/router';
// import { ProductService } from '../../../core/services/product.service';
// import { CreateProductDto, UpdateProductDto } from '../../../core/models/product.model';

// @Component({
//   selector: 'app-product-form',
  
//   templateUrl: './product-form.component.html',
//   styleUrl: './product-form.component.scss'
// })
// export class ProductFormComponent implements OnInit {
//   productData: CreateProductDto | UpdateProductDto = {
//     name: '',
//     description: '',
//     price: 0,
//     stock: 0
//   };
  
//   productId: string | null = null;
//   isEditMode = false;
//   loading = false;
//   errorMessage = '';

//   constructor(
//     private productService: ProductService,
//     private router: Router,
//     private route: ActivatedRoute
//   ) {}

//   ngOnInit(): void {
//     this.productId = this.route.snapshot.paramMap.get('id');
    
//     if (this.productId) {
//       this.isEditMode = true;
//       this.loadProduct(this.productId);
//     }
//   }

//   loadProduct(id: string): void {
//     this.productService.getById(id).subscribe({
//       next: (product) => {
//         this.productData = {
//           name: product.name,
//           description: product.description,
//           price: product.price,
//           stock: product.stock
//         };
//       },
//       error: (error) => {
//         this.errorMessage = 'Failed to load product';
//         console.error('Error loading product:', error);
//       }
//     });
//   }

// onSubmit(): void {
//   if (!this.productData.name || this.productData.price < 0 || this.productData.stock < 0) {
//     this.errorMessage = 'Please fill in all required fields correctly';
//     return;
//   }

//    this.loading = true;
//   this.errorMessage = '';

//   if (this.isEditMode && this.productId) {
//     this.productService.update(this.productId, this.productData as UpdateProductDto).subscribe({
//       next: () => {
//         this.loading = false;
//         this.router.navigate(['/products']);
//       },
//       error: (error) => {
//         this.loading = false;
//         // Show detailed error message from backend
//         this.errorMessage = error.error?.message || 'Failed to update product';
        
//         // If it's a limit error, show upgrade prompt
//         if (error.error?.message?.includes('limit')) {
//           this.errorMessage += '\n\nWould you like to upgrade your subscription?';
//           setTimeout(() => {
//             if (confirm('Go to subscription page?')) {
//               this.router.navigate(['/admin/subscription']);
//             }
//           }, 100);
//         }
//       }
//     });
//   } else {
//     this.productService.create(this.productData as CreateProductDto).subscribe({
//       next: () => {
//         this.loading = false;
//         this.router.navigate(['/products']);
//       },
//       error: (error) => {
//         this.loading = false;
//         // Show detailed error message from backend
//         this.errorMessage = error.error?.message || 'Failed to create product';
        
//         // If it's a limit error, show upgrade prompt
//         if (error.error?.message?.includes('limit')) {
//           setTimeout(() => {
//             if (confirm('Product limit reached! Would you like to upgrade your subscription?')) {
//               this.router.navigate(['/admin/subscription']);
//             }
//           }, 100);
//         }
//       }
//     });
//   }
// }

//   cancel(): void {
//     this.router.navigate(['/products']);
//   }
// }

import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CreateProductDto, UpdateProductDto } from '../../../core/models/product.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  productData: CreateProductDto | UpdateProductDto = {
    name: '',
    description: '',
    price: 0,
    stock: 0,
    imageUrl: ''
  };
  
  productId: string | null = null;
  isEditMode = false;
  loading = false;
  errorMessage = '';
  
  // Image upload properties
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  uploadingImage = false;
  apiUrl = environment.apiUrl.replace('/api', ''); // Base URL for image display

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
          stock: product.stock,
          imageUrl: product.imageUrl
        };
        
        // Set image preview if product has image
        if (product.imageUrl) {
          this.imagePreview = `${this.apiUrl}${product.imageUrl}`;
        }
      },
      error: (error) => {
        this.errorMessage = 'Failed to load product';
        console.error('Error loading product:', error);
      }
    });
  }

  // Handle file selection
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file type
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
      if (!allowedTypes.includes(file.type)) {
        this.errorMessage = 'Please select a valid image file (JPG, PNG, GIF, WEBP)';
        return;
      }

      // Validate file size (5MB max)
      const maxSize = 5 * 1024 * 1024; // 5MB
      if (file.size > maxSize) {
        this.errorMessage = 'File size must be less than 5MB';
        return;
      }

      this.selectedFile = file;
      this.errorMessage = '';

      // Show image preview
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreview = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  // Upload image to server
  uploadImage(): Promise<string> {
    return new Promise((resolve, reject) => {
      if (!this.selectedFile) {
        resolve(this.productData.imageUrl || '');
        return;
      }

      this.uploadingImage = true;

      this.productService.uploadImage(this.selectedFile).subscribe({
        next: (response) => {
          this.uploadingImage = false;
          resolve(response.imageUrl);
        },
        error: (error) => {
          this.uploadingImage = false;
          this.errorMessage = error.error?.message || 'Failed to upload image';
          reject(error);
        }
      });
    });
  }

  // Remove image
  removeImage(): void {
    this.selectedFile = null;
    this.imagePreview = null;
    this.productData.imageUrl = '';
  }

  async onSubmit(): Promise<void> {
    if (!this.productData.name || this.productData.price < 0 || this.productData.stock < 0) {
      this.errorMessage = 'Please fill in all required fields correctly';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    try {
      // Upload image first if selected
      if (this.selectedFile) {
        const imageUrl = await this.uploadImage();
        this.productData.imageUrl = imageUrl;
      }

      // Then create/update product
      if (this.isEditMode && this.productId) {
        this.productService.update(this.productId, this.productData as UpdateProductDto).subscribe({
          next: () => {
            this.loading = false;
            this.router.navigate(['/products']);
          },
          error: (error) => {
            this.loading = false;
            this.errorMessage = error.error?.message || 'Failed to update product';
            
            if (error.error?.message?.includes('limit')) {
              setTimeout(() => {
                if (confirm('Go to subscription page?')) {
                  this.router.navigate(['/admin/subscription']);
                }
              }, 100);
            }
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
            
            if (error.error?.message?.includes('limit')) {
              setTimeout(() => {
                if (confirm('Product limit reached! Would you like to upgrade your subscription?')) {
                  this.router.navigate(['/admin/subscription']);
                }
              }, 100);
            }
          }
        });
      }
    } catch (error) {
      this.loading = false;
      // Error already handled in uploadImage
    }
  }

  cancel(): void {
    this.router.navigate(['/products']);
  }
}