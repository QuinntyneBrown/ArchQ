import { Routes } from '@angular/router';
import { TenantCreateComponent } from './features/settings/tenant-create/tenant-create.component';
import { TenantDetailComponent } from './features/settings/tenant-detail/tenant-detail.component';

export const routes: Routes = [
  { path: 'tenants/new', component: TenantCreateComponent },
  { path: 'tenants/:id', component: TenantDetailComponent },
  { path: 'adrs', children: [] },
  { path: 'login', children: [] },
  { path: '', redirectTo: '/adrs', pathMatch: 'full' },
];
