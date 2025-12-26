// import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
// import { isPlatformBrowser } from '@angular/common';
// import {
//   HttpRequest,
//   HttpHandler,
//   HttpEvent,
//   HttpInterceptor
// } from '@angular/common/http';
// import { Observable } from 'rxjs';

// @Injectable()
// export class AuthInterceptor implements HttpInterceptor {
//   constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

//   intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
//     // Only add token if running in browser
//     if (!isPlatformBrowser(this.platformId)) {
//       return next.handle(request);
//     }

//     // Check if localStorage is available
//     if (typeof localStorage === 'undefined') {
//       return next.handle(request);
//     }

//     const token = localStorage.getItem('auth_token');
    
//     if (token) {
//       const cloned = request.clone({
//         headers: request.headers.set('Authorization', `Bearer ${token}`)
//       });
//       return next.handle(cloned);
//     }
    
//     return next.handle(request);
//   }
// }

import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Only add headers if running in browser
    if (!isPlatformBrowser(this.platformId)) {
      return next.handle(request);
    }

    // Check if localStorage is available
    if (typeof localStorage === 'undefined') {
      return next.handle(request);
    }

    const token = localStorage.getItem('auth_token');
    const subdomain = localStorage.getItem('tenant_subdomain');
    
    // Clone request and add headers
    let cloned = request;
    
    // Add tenant subdomain header
    if (subdomain) {
      cloned = cloned.clone({
        headers: cloned.headers.set('X-Tenant-Subdomain', subdomain)
      });
    }
    
    // Add authorization token
    if (token) {
      cloned = cloned.clone({
        headers: cloned.headers.set('Authorization', `Bearer ${token}`)
      });
    }
    
    return next.handle(cloned);
  }
}