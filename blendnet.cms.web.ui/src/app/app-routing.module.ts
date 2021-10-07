import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminLoginComponent } from './admin-login/admin-login.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { ProfileComponent } from './profile/profile.component';
import { RetailerLoginComponent } from './retailer-login/retailer-login.component';
import { SeamlessLoginComponent } from './retailer/seamless-login/seamless-login.component';
import { RoleGuardService } from './services/role-guard.service';

const appRoutes: Routes = [
  { 
    path: 'login', 
    component: LoginComponent
  },
  {
    path: 'admin-login',
    component: AdminLoginComponent
  },
  {
    path: 'admin',
    loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule)
  },
  {
    path: 'retailer',
    loadChildren: () => import('./retailer/retailer.module').then(m => m.RetailerModule)
  },
  {
    path: 'retailer-login',
    component: SeamlessLoginComponent
  },
  {
    path: 'common-retailer-login',
    component: RetailerLoginComponent
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [
      RoleGuardService
    ]
  },
  { 
    path: '**', 
    redirectTo: '/home',
  },
  {
    path: 'home', 
    component: HomeComponent
  }

];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes, 
    { 
    useHash: true
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
