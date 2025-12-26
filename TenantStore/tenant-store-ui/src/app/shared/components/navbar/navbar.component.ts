import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { TenantService } from '../../../core/services/tenant.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  currentUser: any = null;
  tenantInfo: any = null;

  constructor(
    public authService: AuthService,
    private tenantService: TenantService
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    this.tenantService.tenant$.subscribe(tenant => {
      this.tenantInfo = tenant;
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
