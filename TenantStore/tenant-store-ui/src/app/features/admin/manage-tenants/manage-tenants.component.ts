import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

interface Tenant {
  id: string;
  name: string;
  subdomain: string;
  themeColor: string;
  isActive: boolean;
  subscriptionPlan: string;
  subscriptionExpiresAt?: Date;
  maxProducts: number;
  maxUsers: number;
  createdAt: Date;
}

@Component({
  selector: 'app-manage-tenants',
  templateUrl: './manage-tenants.component.html',
  styleUrls: ['./manage-tenants.component.scss']
})
export class ManageTenantsComponent implements OnInit {
  tenants: Tenant[] = [];
  loading = true;
  errorMessage = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadTenants();
  }

  loadTenants(): void {
    this.loading = true;
    this.errorMessage = '';

    this.http.get<Tenant[]>(`${environment.apiUrl}/Tenants`).subscribe({
      next: (tenants) => {
        this.tenants = tenants;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load tenants';
        this.loading = false;
        console.error('Error loading tenants:', error);
      }
    });
  }
//Deepseek
    get activeTenantsCount(): number {
    return this.tenants.filter(t => t.isActive).length;
  }

  get inactiveTenantsCount(): number {
    return this.tenants.filter(t => !t.isActive).length;
  }
//Deepseek end
  viewTenant(subdomain: string): void {
    window.open(`http://${subdomain}.localhost:4200`, '_blank');
  }

  toggleTenantStatus(tenantId: string, currentStatus: boolean): void {
    const action = currentStatus ? 'deactivate' : 'activate';
    if (!confirm(`Are you sure you want to ${action} this tenant?`)) {
      return;
    }

    // TODO: Implement activate/deactivate API endpoint
    alert(`Tenant ${action} feature - API endpoint coming soon!`);
  }

  getPlanBadgeClass(plan: string): string {
    switch (plan) {
      case 'Free': return 'badge-free';
      case 'Basic': return 'badge-basic';
      case 'Premium': return 'badge-premium';
      case 'Enterprise': return 'badge-enterprise';
      default: return '';
    }
  }
}
