import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BroadcastComponent } from './broadcast/broadcast.component';
import { ContentProviderComponent } from './content-provider/content-provider.component';
import { DevicesComponent } from './devices/devices.component';
import { LoginComponent } from './login/login.component';
import { ManageContentComponent } from './manage-content/manage-content.component';
import { ProcessedComponent } from './processed/processed.component';
import { SasKeyComponent } from './sas-key/sas-key.component';
import { UnprocessedComponent } from './unprocessed/unprocessed.component';
import { RoleGuardService } from './services/role-guard.service';
import { SubscriptionComponent } from './subscription/subscription.component';
import { HomeComponent } from './home/home.component';
import { AuthGuardService } from './services/auth-guard.service';
import { environment } from 'src/environments/environment';
import { ProfileComponent } from './profile/profile.component';
import { IncentiveManagementComponent } from './incentive-management/incentive-management.component';

import { NotificationsComponent } from './notifications/notifications.component';
import { SeamlessLoginComponent } from './retailer/seamless-login/seamless-login.component';
import { DeviceHistoryComponent } from './devices/device-history.component';

const appRoutes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'admin',
    loadChildren: () => import('./authenticated-user/authenticated-user.module').then(m => m.AuthenticatedUserModule)
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
    path: 'unprocessed-content', 
    component: UnprocessedComponent,
    canActivate: [
      AuthGuardService, RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'processed-content',
     component: ProcessedComponent,
     canActivate: [
      AuthGuardService, RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
    },
  {
    
    path: 'broadcast-content',
   component: BroadcastComponent,
   canActivate: [
    AuthGuardService, RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'devices',
   component: DevicesComponent,
   canActivate: [
    AuthGuardService, RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'devices/:id',
   component: DeviceHistoryComponent,
   canActivate: [
    AuthGuardService, RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'manage-content',
   component: ManageContentComponent,
   canActivate: [
    AuthGuardService, RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'incentive-management',
   component: IncentiveManagementComponent,
   canActivate: [
    AuthGuardService, RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'content-providers',
     component: ContentProviderComponent,
     canActivate: [
      AuthGuardService, RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin, environment.roles.ContentAdmin]
    } 
    },
  {
    path: 'sas-key', 
    component: SasKeyComponent,
    canActivate: [
      AuthGuardService, RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin, environment.roles.ContentAdmin]
    } 
  },
  {
    path: 'profile', 
    component: ProfileComponent,
    canActivate: [
      AuthGuardService
    ],
  },
  {
    path: 'subscriptions', 
    component: SubscriptionComponent,
    canActivate: [
      AuthGuardService, RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'notifications', 
    component: NotificationsComponent,
    canActivate: [
      RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  { 
    path: '', 
    component: HomeComponent, 
    canActivate: [AuthGuardService] },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'profile',
    component: ProfileComponent
  },
  { 
    path: '**', 
    redirectTo: '' 
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
