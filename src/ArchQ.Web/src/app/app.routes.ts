import { Routes } from '@angular/router';
import { TenantCreateComponent } from './features/settings/tenant-create/tenant-create.component';
import { TenantDetailComponent } from './features/settings/tenant-detail/tenant-detail.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { VerifyEmailComponent } from './features/auth/verify-email/verify-email.component';
import { LoginComponent } from './features/auth/login/login.component';
import { AdrListComponent } from './features/dashboard/adr-list/adr-list.component';
import { AdrEditorComponent } from './features/adr/adr-editor/adr-editor.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'verify-email', component: VerifyEmailComponent },
  { path: 'tenants/new', component: TenantCreateComponent, canActivate: [authGuard] },
  { path: 'tenants/:id', component: TenantDetailComponent, canActivate: [authGuard] },
  { path: 'adrs', component: AdrListComponent, canActivate: [authGuard] },
  { path: 'adrs/new', component: AdrEditorComponent, canActivate: [authGuard] },
  { path: 'adrs/:id/edit', component: AdrEditorComponent, canActivate: [authGuard] },
  { path: '', redirectTo: '/adrs', pathMatch: 'full' },
];
