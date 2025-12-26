import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-tenant-create',
  templateUrl: './tenant-create.component.html',
  styleUrls: ['./tenant-create.component.scss']
})
export class TenantCreateComponent {
  tenantData = {
    name: '',
    subdomain: '',
    themeColor: '#007bff',
    adminName: '',
    adminEmail: '',
    adminPassword: ''
  };

  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  onSubmit(): void {
    if (!this.tenantData.name || !this.tenantData.subdomain || 
        !this.tenantData.adminName || !this.tenantData.adminEmail || 
        !this.tenantData.adminPassword) {
      this.errorMessage = 'Please fill in all required fields';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.http.post(`${environment.apiUrl}/Tenants`, this.tenantData).subscribe({
      next: (response) => {
        this.loading = false;
        this.successMessage = `Tenant "${this.tenantData.name}" created successfully!`;
        setTimeout(() => {
          // Reset form
          this.tenantData = {
            name: '',
            subdomain: '',
            themeColor: '#007bff',
            adminName: '',
            adminEmail: '',
            adminPassword: ''
          };
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Failed to create tenant';
      }
    });
  }
}