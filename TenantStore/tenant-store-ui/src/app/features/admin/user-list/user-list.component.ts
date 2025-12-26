import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../core/services/user.service';
import { TenantService } from '../../../core/services/tenant.service';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  tenantInfo: any = null;
  loading = true;
  errorMessage = '';

  constructor(
    private userService: UserService,
    private tenantService: TenantService,
    public authService: AuthService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadTenantInfo();
    this.loadUsers();
  }

  loadTenantInfo(): void {
    this.tenantService.tenant$.subscribe(tenant => {
      if (tenant) {
        this.http.get(`${environment.apiUrl}/Tenants/${tenant.id}`).subscribe({
          next: (data: any) => {
            this.tenantInfo = data;
          },
          error: (error) => {
            console.error('Error loading tenant:', error);
          }
        });
      }
    });
  }

  loadUsers(): void {
    this.loading = true;
    this.errorMessage = '';

    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load users';
        this.loading = false;
        console.error('Error loading users:', error);
      }
    });
  }

  changeRole(userId: string, newRole: string): void {
    if (!confirm(`Change user role to ${newRole}?`)) {
      return;
    }

    this.userService.changeRole(userId, newRole).subscribe({
      next: () => {
        this.loadUsers(); // Reload users
      },
      error: (error) => {
        alert('Failed to change user role');
        console.error('Error changing role:', error);
      }
    });
  }

  getRoleBadgeClass(role: string): string {
    switch (role) {
      case 'SuperAdmin': return 'badge-superadmin';
      case 'Admin': return 'badge-admin';
      case 'User': return 'badge-user';
      default: return '';
    }
  }
}