import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { BroadcastComponent } from './broadcast/broadcast.component';
import { ContentProviderComponent } from './content-provider/content-provider.component';
import { DevicesComponent } from './devices/devices.component';
import { LoginComponent } from './login/login.component';
import { ManageContentComponent } from './manage-content/manage-content.component';
import { ProcessedComponent } from './processed/processed.component';
import { SasKeyComponent } from './sas-key/sas-key.component';
import { UnprocessedComponent } from './unprocessed/unprocessed.component';
import { roles } from './b2c-config';
import { RoleGuardService } from './services/role-guard.service';
import { SubscriptionComponent } from './subscription/subscription.component';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { AuthGuardService } from './services/auth-guard.service';


const appRoutes: Routes = [

  {
    path: 'unprocessed-content', 
    component: UnprocessedComponent,
    canActivate: [
      // MsalGuard,
      RoleGuardService
    ],
    data: { 
      // expectedRole: roles.SuperUser
      expectedRole: roles.NormalUser
    } 
  },
  {
    path: 'processed-content',
     component: ProcessedComponent,
     canActivate: [
      // MsalGuard,
      RoleGuardService
    ],
    data: { 
      // expectedRole: roles.SuperUser
      expectedRole: roles.NormalUser
    } 
    },
  {
    
    path: 'broadcast-content',
   component: BroadcastComponent,
   canActivate: [
    // MsalGuard,
    RoleGuardService
  ],
    data: { 
    // expectedRole: roles.SuperUser
    expectedRole: roles.NormalUser    } 
  },
  {
    path: 'devices',
   component: DevicesComponent,
   canActivate: [
    // MsalGuard,
    RoleGuardService
  ],
    data: { 
     // expectedRole: roles.SuperUser
     expectedRole: roles.NormalUser
    } 
  },
  {
    path: 'manage-content',
   component: ManageContentComponent,
   canActivate: [
    // MsalGuard,
    RoleGuardService
  ],
    data: { 
      // expectedRole: roles.SuperUser
      expectedRole: roles.NormalUser
    } 
  },
  {
    path: 'content-providers',
     component: ContentProviderComponent,
     canActivate: [
      // MsalGuard,
      RoleGuardService
    ],
    data: { 
      // expectedRole: roles.SuperUser
      expectedRole: roles.NormalUser,
      isContentProviderSelectPage: true
    } 
    },
  {
    path: 'sas-key', 
    component: SasKeyComponent,
    canActivate: [
      // MsalGuard,
      RoleGuardService
    ],
    data: { 
     // expectedRole: roles.SuperUser
     expectedRole: roles.NormalUser
    } 
  },
  {
    path: 'subscriptions', 
    component: SubscriptionComponent,
    canActivate: [
      // MsalGuard,
      RoleGuardService
    ],
    data: { 
      // expectedRole: roles.SuperUser
      expectedRole: roles.NormalUser
    } 
  },
  // {
  //   // Needed for hash routing
  //   path: 'error',
  //   component: HomeComponent
  // },
  // {
  //   // Needed for hash routing
  //   path: 'state',
  //   component: HomeComponent
  // },
  // {
  //   // Needed for hash routing
  //   path: 'code',
  //   component: HomeComponent
  // },
  { 
    path: '', 
    component: HomeComponent, 
    canActivate: [AuthGuardService] },
  {
    path: 'login',
    component: LoginComponent
  },
  // otherwise redirect to home
  { 
    path: '**', 
    redirectTo: '' 
  }

];
const isIframe = window !== window.parent && !window.opener;

@NgModule({
  imports: [RouterModule.forRoot(appRoutes, 
    { 
      // relativeLinkResolution: 'legacy',
    useHash: true,
    // Don't perform initial navigation in iframes
    initialNavigation: !isIframe ? 'enabled' : 'disabled'})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
