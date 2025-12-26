import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// Components
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ProductListComponent } from './features/products/product-list/product-list.component';
import { ProductFormComponent } from './features/products/product-form/product-form.component';

// Interceptor
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { DashboardComponent } from './features/dashboard/dashboard/dashboard.component';
import { TenantCreateComponent } from './features/admin/tenant-create/tenant-create.component';
import { SubscriptionComponent } from './features/admin/subscription/subscription.component';
import { ThemeSettingsComponent } from './features/admin/theme-settings/theme-settings.component';
import { UserListComponent } from './features/admin/user-list/user-list.component';
import { ManageTenantsComponent } from './features/admin/manage-tenants/manage-tenants.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    LoginComponent,
    RegisterComponent,
    ProductListComponent,
    ProductFormComponent,
    DashboardComponent,
    TenantCreateComponent,
    SubscriptionComponent,
    ThemeSettingsComponent,
    UserListComponent,
    ManageTenantsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }