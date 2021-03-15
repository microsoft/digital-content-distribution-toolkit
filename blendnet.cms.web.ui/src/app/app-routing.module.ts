import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { BroadcastedComponent } from './broadcasted/broadcasted.component';
import { ContentProviderComponent } from './content-provider/content-provider.component';
import { DevicesComponent } from './devices/devices.component';
import { HomeComponent } from './home/home.component';
import { ManageContentComponent } from './manage-content/manage-content.component';
import { ProcessedComponent } from './processed/processed.component';
import { SasKeyComponent } from './sas-key/sas-key.component';
import { UnprocessedComponent } from './unprocessed/unprocessed.component';


const appRoutes: Routes = [
  {
    path: 'unprocessed-content', 
    component: UnprocessedComponent,
    canActivate: [
      MsalGuard,
    ]
  },
  {
    path: 'processed-content',
     component: ProcessedComponent,
     canActivate: [
      MsalGuard,
    ]
    },
  {
    
    path: 'broadcasted-content',
   component: BroadcastedComponent,
   canActivate: [
    MsalGuard,
  ]
  },
  {
    path: 'devices',
   component: DevicesComponent,
   canActivate: [
    MsalGuard,
  ]
  },
  {
    path: 'manage-content',
   component: ManageContentComponent,
   canActivate: [
    MsalGuard,
  ]
  },
  {
    path: 'content-providers',
     component: ContentProviderComponent,
     canActivate: [
      MsalGuard,
    ]
    },
  {
    path: 'sas-key', 
    component: SasKeyComponent,
    canActivate: [
      MsalGuard,
    ]
  },
  {
    // Needed for hash routing
    path: 'error',
    component: HomeComponent
  },
  {
    // Needed for hash routing
    path: 'state',
    component: HomeComponent
  },
  {
    // Needed for hash routing
    path: 'code',
    component: HomeComponent
  },
  {
    path: '',
    component: HomeComponent
  }

];
const isIframe = window !== window.parent && !window.opener;

@NgModule({
  imports: [RouterModule.forRoot(appRoutes, 
    { relativeLinkResolution: 'legacy',
    useHash: true,
    // Don't perform initial navigation in iframes
    initialNavigation: !isIframe ? 'enabled' : 'disabled'})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
