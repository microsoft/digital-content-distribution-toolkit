import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { environment } from 'src/environments/environment';
import { RoleGuardService } from './../services/role-guard.service';

import { BroadcastComponent } from './../broadcast/broadcast.component';
import { ContentProviderComponent } from './../content-provider/content-provider.component';
import { DevicesComponent } from './../devices/devices.component';
import { LoginComponent } from './../login/login.component';
import { ManageContentComponent } from './../manage-content/manage-content.component';
import { ProcessedComponent } from './../processed/processed.component';
import { SasKeyComponent } from './../sas-key/sas-key.component';
import { UnprocessedComponent } from './../unprocessed/unprocessed.component';
import { SubscriptionComponent } from './../subscription/subscription.component';
import { HomeComponent } from './../home/home.component';
import { AuthGuardService } from './../services/auth-guard.service';
import { ProfileComponent } from './../profile/profile.component';
import { ReferralsComponent } from './referrals/referrals.component';
import { RetailerHomeComponent } from './retailer-home/retailer-home.component';
import { RetailerDashboardComponent } from './retailer-dashboard/retailer-dashboard.component';
import { CMSMaterialModule } from '../material-module';
import { RetailerCommissionsComponent } from './retailer-commissions/retailer-commissions.component';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { RetailerCommissionDialogComponent } from './retailer-commission-dialog/retailer-commission-dialog.component';
import { RatesIncentivesComponent } from './rates-incentives/rates-incentives.component';
import { FormsModule } from '@angular/forms';
import { RetailerMilestonesComponent } from './retailer-milestones/retailer-milestones.component';
import { SeamlessLoginComponent } from './seamless-login/seamless-login.component';
import { NoTransactionsComponent } from './no-transactions/no-transactions.component';


const routes: Routes = [

  {
    path: 'home', 
    component: RetailerHomeComponent,
    // canActivate: [
    //   RoleGuardService
    // ],
    data: { 
      expectedRole: [environment.roles.User]
    } 
  },
  {
    path: 'referrals', 
    component: ReferralsComponent,
    // canActivate: [
    //   RoleGuardService
    // ],
    data: {
      expectedRole: [environment.roles.User]
    } 
  },
  {
    path: 'commissions', 
    component: RetailerCommissionsComponent,
    // canActivate: [
    //   RoleGuardService
    // ],
    data: {
      expectedRole: [environment.roles.User]
    }
  },
  {
    path: 'rates-incentives', 
    component: RatesIncentivesComponent,
    // canActivate: [
    //   RoleGuardService
    // ],
    data: {
      expectedRole: [environment.roles.User]
    } 
  },
  {
    path: 'milestones', 
    component: RetailerMilestonesComponent,
    // canActivate: [
    //   RoleGuardService
    // ],
    data: {
      expectedRole: [environment.roles.User]
    } 
  }
]


@NgModule({
  declarations: [ReferralsComponent, RetailerHomeComponent, RetailerDashboardComponent, RetailerCommissionsComponent, RetailerCommissionDialogComponent, RatesIncentivesComponent, RetailerMilestonesComponent, SeamlessLoginComponent, NoTransactionsComponent],
  imports: [
    CommonModule,
    CMSMaterialModule,
    CarouselModule ,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class RetailerModule { }
