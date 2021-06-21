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


const appRoutes: Routes = [

  {
    path: 'unprocessed-content', 
    component: UnprocessedComponent,
    canActivate: [
      RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'processed-content',
     component: ProcessedComponent,
     canActivate: [
      RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
    },
  {
    
    path: 'broadcast-content',
   component: BroadcastComponent,
   canActivate: [
    RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'devices',
   component: DevicesComponent,
   canActivate: [
    RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'manage-content',
   component: ManageContentComponent,
   canActivate: [
    RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'incentive-management',
   component: IncentiveManagementComponent,
   canActivate: [
    RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'content-providers',
     component: ContentProviderComponent,
     canActivate: [
      RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin, environment.roles.ContentAdmin]
    } 
    },
  {
    path: 'sas-key', 
    component: SasKeyComponent,
    canActivate: [
      RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin, environment.roles.ContentAdmin]
    } 
  },
  {
    path: 'profile', 
    component: ProfileComponent
  },
  {
    path: 'subscriptions', 
    component: SubscriptionComponent,
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
