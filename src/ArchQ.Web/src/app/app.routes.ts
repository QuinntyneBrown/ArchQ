import { Routes } from '@angular/router';
import { TenantCreateComponent } from './features/settings/tenant-create/tenant-create.component';
import { TenantDetailComponent } from './features/settings/tenant-detail/tenant-detail.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { VerifyEmailComponent } from './features/auth/verify-email/verify-email.component';
import { LoginComponent } from './features/auth/login/login.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'verify-email', component: VerifyEmailComponent },
  { path: 'tenants/new', component: TenantCreateComponent },
  { path: 'tenants/:id', component: TenantDetailComponent },
  { path: 'adrs', children: [] },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];
