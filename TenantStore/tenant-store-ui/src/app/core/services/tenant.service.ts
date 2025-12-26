import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { TenantInfo } from '../models/tenant.model';

@Injectable({
  providedIn: 'root'
})
export class TenantService {
  private tenantSubject = new BehaviorSubject<TenantInfo | null>(null);
  public tenant$ = this.tenantSubject.asObservable();

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    // Only detect tenant if running in browser
    if (isPlatformBrowser(this.platformId)) {
      this.detectTenant();
    }
  }

  private detectTenant(): void {
    // Check if window is available
    if (typeof window === 'undefined') {
      return;
    }

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
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem('tenant_subdomain', subdomain);
      }
    }
  }

  getTenantSubdomain(): string | null {
    if (typeof localStorage !== 'undefined') {
      return localStorage.getItem('tenant_subdomain');
    }
    return null;
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
    if (typeof document !== 'undefined') {
      document.documentElement.style.setProperty('--primary-color', color);
    }
  }
}