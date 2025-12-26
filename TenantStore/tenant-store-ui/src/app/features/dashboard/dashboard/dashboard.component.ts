import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../core/services/product.service';
import { TenantService } from '../../../core/services/tenant.service';
import { AuthService } from '../../../core/services/auth.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  stats = {
    totalProducts: 0,
    activeProducts: 0,
    totalValue: 0,
    lowStockProducts: 0
  };
  
  limits = {
    maxProducts: 0,
    maxUsers: 0,
    currentProducts: 0,
    currentUsers: 0,
    productPercentage: 0,
    userPercentage: 0
  };
  
  tenantInfo: any = null;
  currentUser: any = null;
  loading = true;

  constructor(
    private productService: ProductService,
    private tenantService: TenantService,
    public authService: AuthService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;

    // Get tenant info
    this.tenantService.tenant$.subscribe(tenant => {
      if (tenant) {
        this.tenantInfo = tenant;
        this.loadTenantStats(tenant.id);
      }
    });

    // Get current user
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    // Get products and calculate stats
    this.productService.getAll().subscribe({
      next: (products) => {
        this.stats.totalProducts = products.length;
        this.stats.activeProducts = products.filter(p => p.isActive).length;
        this.stats.totalValue = products.reduce((sum, p) => sum + (p.price * p.stock), 0);
        this.stats.lowStockProducts = products.filter(p => p.stock < 10).length;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading dashboard:', error);
        this.loading = false;
      }
    });
  }

  loadTenantStats(tenantId: string): void {
    this.http.get<any>(`${environment.apiUrl}/Tenants/${tenantId}/stats`).subscribe({
      next: (stats) => {
        this.limits = {
          maxProducts: stats.products.max,
          maxUsers: stats.users.max,
          currentProducts: stats.products.current,
          currentUsers: stats.users.current,
          productPercentage: stats.products.percentUsed,
          userPercentage: stats.users.percentUsed
        };
      },
      error: (error) => {
        console.error('Error loading stats:', error);
      }
    });
  }

  getProgressBarClass(percentage: number): string {
    if (percentage >= 90) return 'danger';
    if (percentage >= 70) return 'warning';
    return 'success';
  }
}