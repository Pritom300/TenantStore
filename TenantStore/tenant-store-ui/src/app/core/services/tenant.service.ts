import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { TenantInfo } from '../models/tenant.model';

@Injectable({
  providedIn: 'root'
})
export class TenantService {
  private tenantSubject = new BehaviorSubject<TenantInfo | null>(null);
  public tenant$ = this.tenantSubject.asObservable();

  constructor() {
    this.detectTenant();
  }

  private detectTenant(): void {
    const hostname = window.location.hostname;
    let subdomain: string | null = null;

    // Extract subdomain
    if (hostname.includes('localhost')) {
      const parts = hostname.split('.');
      if (parts.length > 1) {
        subdomain = parts[0];
      }
    } else {
      const parts = hostname.split('.');
      if (parts.length >= 3) {
        subdomain = parts[0];
      }
    }

    if (subdomain && subdomain !== 'www') {
      // Store subdomain for API calls
      localStorage.setItem('tenant_subdomain', subdomain);
    }
  }

  getTenantSubdomain(): string | null {
    return localStorage.getItem('tenant_subdomain');
  }

  setTenant(tenant: TenantInfo): void {
    this.tenantSubject.next(tenant);
    // Apply theme
    this.applyTheme(tenant.themeColor);
  }

  getCurrentTenant(): TenantInfo | null {
    return this.tenantSubject.value;
  }

  private applyTheme(color: string): void {
    document.documentElement.style.setProperty('--primary-color', color);
  }
}