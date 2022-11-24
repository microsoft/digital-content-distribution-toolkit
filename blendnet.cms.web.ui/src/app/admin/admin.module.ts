// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { BroadcastComponent } from '../broadcast/broadcast.component';
import { ContentProviderComponent } from '../content-provider/content-provider.component';
import { DevicesComponent } from '../devices/devices.component';
import { ManageContentComponent } from '../manage-content/manage-content.component';
import { ProcessedComponent } from '../processed/processed.component';
import { SasKeyComponent } from '../sas-key/sas-key.component';
import { UnprocessedComponent } from '../unprocessed/unprocessed.component';
import { RoleGuardService } from '../services/role-guard.service';
import { SubscriptionComponent } from '../subscription/subscription.component';
import { environment } from 'src/environments/environment';
import { NotificationsComponent } from '../notifications/notifications.component';
import { IncentiveManagementComponent } from '../incentive-management/incentive-management.component';
import { DeviceFilterHistoryComponent } from '../devices/device-filter-history.component';
import { DeviceRetailerHistoryComponent } from '../devices/device-retailer-history.component';
import { DeviceContentsComponent } from '../devices/device-contents.component';
import { AdminRetailerDashboardComponent } from '../admin-retailer-dashboard/admin-retailer-dashboard.component';
import { ExportUserDataComponent } from '../export-user-data/export-user-data.component';
import { DeleteUserDataComponent } from '../delete-user-data/delete-user-data.component';

const routes: Routes = [
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
      expectedRole: [environment.roles.SuperAdmin, environment.roles.HubDeviceManagement]
    } 
  },
  {
    path: 'devices/filters-history/:id',
   component: DeviceFilterHistoryComponent,
   canActivate: [
    RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin, environment.roles.HubDeviceManagement]
    } 
  },
  {
    path: 'devices/assignment-history/:id',
   component: DeviceRetailerHistoryComponent,
   canActivate: [
    RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin, environment.roles.HubDeviceManagement]
    } 
  },
  {
    path: 'devices/contents/:id',
   component: DeviceContentsComponent,
   canActivate: [
    RoleGuardService
  ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin, environment.roles.HubDeviceManagement]
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
      expectedRole: [environment.roles.SuperAdmin, environment.roles.ContentAdmin],
      isContentProviderSelectPage: true
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
    path: 'retailer-dashboard',
    component: AdminRetailerDashboardComponent,
    canActivate: [
      RoleGuardService
    ],
    data: {
      expectedRole: [environment.roles.SuperAdmin]
    }
  },
  {
    path: 'export', 
    component: ExportUserDataComponent,
    canActivate: [
      RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  },
  {
    path: 'delete', 
    component: DeleteUserDataComponent,
    canActivate: [
      RoleGuardService
    ],
    data: { 
      expectedRole: [environment.roles.SuperAdmin]
    } 
  }

];


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class AdminModule { }