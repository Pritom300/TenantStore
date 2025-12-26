import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ProductListComponent } from './features/products/product-list/product-list.component';
import { ProductFormComponent } from './features/products/product-form/product-form.component';
import { DashboardComponent } from './features/dashboard/dashboard/dashboard.component';
import { TenantCreateComponent } from './features/admin/tenant-create/tenant-create.component';
import { SubscriptionComponent } from './features/admin/subscription/subscription.component';
import { ThemeSettingsComponent } from './features/admin/theme-settings/theme-settings.component';
import { UserListComponent } from './features/admin/user-list/user-list.component';
import { ManageTenantsComponent } from './features/admin/manage-tenants/manage-tenants.component';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'admin/manage-tenants', component: ManageTenantsComponent},
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent }, 
  { path: 'products', component: ProductListComponent },
  { path: 'products/add', component: ProductFormComponent },
  { path: 'products/edit/:id', component: ProductFormComponent },
  { path: 'admin/create-tenant', component: TenantCreateComponent },
  { path: 'admin/subscription', component: SubscriptionComponent },
  { path: 'admin/theme', component: ThemeSettingsComponent },
  { path: 'admin/users', component: UserListComponent },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }