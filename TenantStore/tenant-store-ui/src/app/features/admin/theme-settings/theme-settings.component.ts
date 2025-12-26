import { Component, OnInit } from '@angular/core';
import { TenantService } from '../../../core/services/tenant.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-theme-settings',
  templateUrl: './theme-settings.component.html',
  styleUrls: ['./theme-settings.component.scss']
})
export class ThemeSettingsComponent implements OnInit {
  tenantInfo: any = null;
  selectedColor = '#007bff';
  loading = false;
  successMessage = '';
  errorMessage = '';

  presetColors = [
    { name: 'Blue', color: '#007bff' },
    { name: 'Green', color: '#28a745' },
    { name: 'Red', color: '#dc3545' },
    { name: 'Purple', color: '#6f42c1' },
    { name: 'Orange', color: '#fd7e14' },
    { name: 'Teal', color: '#20c997' },
    { name: 'Pink', color: '#e83e8c' },
    { name: 'Indigo', color: '#6610f2' }
  ];

  constructor(
    private tenantService: TenantService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.tenantService.tenant$.subscribe(tenant => {
      if (tenant) {
        this.tenantInfo = tenant;
        this.selectedColor = tenant.themeColor;
      }
    });
  }

  selectPreset(color: string): void {
    this.selectedColor = color;
    this.applyThemePreview(color);
  }

  applyThemePreview(color: string): void {
    document.documentElement.style.setProperty('--primary-color', color);
  }

  saveTheme(): void {
    if (!this.tenantInfo) return;

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.http.put(
      `${environment.apiUrl}/Tenants/${this.tenantInfo.id}/theme`,
      { themeColor: this.selectedColor }
    ).subscribe({
      next: () => {
        this.loading = false;
        this.successMessage = 'Theme updated successfully!';
        this.tenantService.setTenant({
          ...this.tenantInfo,
          themeColor: this.selectedColor
        });
        
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'Failed to update theme';
      }
    });
  }
}