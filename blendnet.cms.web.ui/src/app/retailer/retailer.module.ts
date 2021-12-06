import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ReferralsComponent } from './referrals/referrals.component';
import { CMSMaterialModule } from '../material-module';
import { RetailerCommissionsComponent } from './retailer-commissions/retailer-commissions.component';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { RetailerCommissionDialogComponent } from './retailer-commission-dialog/retailer-commission-dialog.component';
import { RatesIncentivesComponent } from './rates-incentives/rates-incentives.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RetailerMilestonesComponent } from './retailer-milestones/retailer-milestones.component';
import { SeamlessLoginComponent } from './seamless-login/seamless-login.component';
import { NoTransactionsComponent } from './no-transactions/no-transactions.component';
import { RetailerOrdersComponent } from './retailer-orders/retailer-orders.component';
import { RetailerDashboardComponent } from './retailer-dashboard/retailer-dashboard.component';
import { RetailerOrderSuccessComponent } from './retailer-order-success/retailer-order-success.component';
import { RoleGuardService } from '../services/role-guard.service';
import { ActivateNumberComponent } from './activate-number/activate-number.component';
import { RetailerHomeComponent } from './retailer-home/retailer-home.component';


const routes: Routes = [
  {
    path: 'dashboard', 
    component: RetailerDashboardComponent,
    data: { 
      expectedRole: [environment.roles.Retailer]
    } 
  },
  {
    path: 'orders', 
    component: RetailerOrdersComponent,
    data: { 
      expectedRole: [environment.roles.Retailer]
    } 
  },
  {
    path: 'referrals', 
    component: ReferralsComponent,
    data: {
      expectedRole: [environment.roles.Retailer]
    } 
  },
  {
    path: 'commissions', 
    component: RetailerCommissionsComponent,
    data: {
      expectedRole: [environment.roles.Retailer]
    }
  },
  {
    path: 'rates-incentives', 
    component: RatesIncentivesComponent,
    data: {
      expectedRole: [environment.roles.User]
    } 
  },
  {
    path: 'milestones', 
    component: RetailerMilestonesComponent,
    data: {
      expectedRole: [environment.roles.User]
    } 
  },
  {
    path: 'activate-number',
    component: ActivateNumberComponent,
    data: { 
      expectedRole: [environment.roles.Retailer]
    } 
  },
  {
    path: 'retailer-home',
    component: RetailerHomeComponent,
    data: { 
      expectedRole: [environment.roles.Retailer]
    } 
  },
]


@NgModule({
  declarations: [
    ReferralsComponent, 
    RetailerCommissionsComponent, 
    RetailerCommissionDialogComponent, 
    RatesIncentivesComponent, 
    RetailerMilestonesComponent,
    SeamlessLoginComponent, 
    NoTransactionsComponent,
    RetailerDashboardComponent,
    RetailerOrdersComponent,
    RetailerOrderSuccessComponent,
    ActivateNumberComponent,
    RetailerHomeComponent
    ],
  imports: [
    CommonModule,
    CMSMaterialModule,
    CarouselModule ,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    RetailerDashboardComponent
  ]
})
export class RetailerModule { }
