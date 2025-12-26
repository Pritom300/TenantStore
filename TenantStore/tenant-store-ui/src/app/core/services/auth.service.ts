import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, RegisterRequest } from '../models/user.model';
import { TenantService } from './tenant.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private router: Router,
    private tenantService: TenantService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.loadUserFromStorage();
    }
  }

  private loadUserFromStorage(): void {
    if (typeof localStorage === 'undefined') {
      return;
    }

    const token = this.getToken();
    const user = localStorage.getItem('current_user');
    if (token && user) {
      this.currentUserSubject.next(JSON.parse(user));
    }
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, request)
      .pipe(
        tap(response => {
          if (typeof localStorage !== 'undefined') {
            localStorage.setItem('auth_token', response.token);
            localStorage.setItem('current_user', JSON.stringify(response.user));
          }
          this.currentUserSubject.next(response.user);
          this.tenantService.setTenant(response.tenant);
        })
      );
  }

  register(request: RegisterRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/register`, request)
      .pipe(
        tap(response => {
          if (typeof localStorage !== 'undefined') {
            localStorage.setItem('auth_token', response.token);
            localStorage.setItem('current_user', JSON.stringify(response.user));
          }
          this.currentUserSubject.next(response.user);
          this.tenantService.setTenant(response.tenant);
        })
      );
  }

  logout(): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('current_user');
    }
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    if (typeof localStorage !== 'undefined') {
      return localStorage.getItem('auth_token');
    }
    return null;
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  getCurrentUser(): any {
    return this.currentUserSubject.value;
  }



  isSuperAdmin(): boolean {
  const user = this.getCurrentUser();
  return user?.role === 'SuperAdmin';
}

isAdmin(): boolean {
  const user = this.getCurrentUser();
  return user?.role === 'Admin';
}

isUser(): boolean {
  const user = this.getCurrentUser();
  return user?.role === 'User';
}

canDelete(): boolean {
  return this.isSuperAdmin() || this.isAdmin();
}

canManageTenants(): boolean {
  return this.isSuperAdmin();
}

canManageUsers(): boolean {
  return this.isSuperAdmin() || this.isAdmin();
}
}