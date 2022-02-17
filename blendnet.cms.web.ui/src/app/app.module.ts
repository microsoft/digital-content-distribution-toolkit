import { BrowserModule } from '@angular/platform-browser';
import { LOCALE_ID, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ContentDetailsDialog, UnprocessedComponent } from './unprocessed/unprocessed.component';
import { ContentTokenDialog, ProcessConfirmDialog, ProcessedComponent } from './processed/processed.component';
import { DevicesComponent } from './devices/devices.component';
import { ManageContentComponent } from './manage-content/manage-content.component';
import { CMSMaterialModule } from './material-module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ContentProviderComponent } from './content-provider/content-provider.component';
import { SasKeyComponent } from './sas-key/sas-key.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { AddContentProviderComponent } from './add-content-provider/add-content-provider.component';
import { AdminLoginComponent } from './admin-login/admin-login.component';
import { ToastrModule } from 'ngx-toastr';
import { HttpErrorInterceptor } from './interceptor/http-error.interceptor';
import { SpinnerOverlayComponent } from './spinner-overlay/spinner-overlay.component';
import { SpinnerInterceptor } from './interceptor/spinner.interceptor';
import { BroadcastComponent, BroadcastDetailsDialog } from './broadcast/broadcast.component';
import { SubscriptionComponent } from './subscription/subscription.component';
import { HomeComponent } from './home/home.component';
import { JwtInterceptor } from './interceptor/jwt.interceptor';
import { CommonDialogComponent } from './common-dialog/common-dialog.component';
import { AddSubscriptionDialog } from './subscription/add-subscription-dialog';
import { ProfileComponent } from './profile/profile.component';
import { PascalToStringPipe } from './custom-pipes/pascalcase-to-string.pipe';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { IncentiveManagementComponent } from './incentive-management/incentive-management.component';
import { AddIncentiveComponent } from './incentive-management/add-incentive.component';
import { DisableControlDirective } from './custom-directives/disable-control.directive';
import { AddEventDialog } from './incentive-management/add-event-dialog';
import { NotificationsComponent } from './notifications/notifications.component';
import { NotificationsDialog } from './notifications/notifications-dialog';
import { RetailerLoginComponent } from './retailer-login/retailer-login.component';
import { LoginComponent } from './login/login.component';
import { DeviceDialogComponent } from './devices/device-dialog.component';
import { ContentProviderHeaderComponent } from './content-provider-header/content-provider-header.component';
import { DeviceFilterHistoryComponent } from './devices/device-filter-history.component';
import { DeviceFiltersComponent } from './devices/device-filters.component';
import { AdditionalHistoryDialog } from './devices/device-additional-history.component';
import { DeviceAssignComponent } from './devices/device-assign.component';
import { DeviceRetailerHistoryComponent } from './devices/device-retailer-history.component';
import { DeviceContentsComponent } from './devices/device-contents.component';
import { AdminRetailerDashboardComponent } from './admin-retailer-dashboard/admin-retailer-dashboard.component';
import {RetailerModule} from './retailer/retailer.module';
import { EditSubscriptionComponent } from './subscription/edit-subscription.component';
import { ExportUserDataComponent } from './export-user-data/export-user-data.component';
import { EditIncentiveEndDateComponent } from './incentive-management/edit-incentive-enddate.component';
import { DeleteUserDataComponent } from './delete-user-data/delete-user-data.component';


@NgModule({
  declarations: [
    AppComponent,
    UnprocessedComponent,
    ProcessedComponent,
    DevicesComponent,
    ManageContentComponent,
    ContentProviderComponent,
    SasKeyComponent,
    AddContentProviderComponent,
    AdminLoginComponent,
    LoginComponent,
    SpinnerOverlayComponent,
    ContentTokenDialog,
    ProcessConfirmDialog,
    BroadcastComponent,
    SubscriptionComponent,
    HomeComponent,
    AddSubscriptionDialog,
    CommonDialogComponent,
    ProfileComponent,
    PascalToStringPipe,
    ContentDetailsDialog,
    IncentiveManagementComponent,
    AddIncentiveComponent,
    DisableControlDirective,
    AddEventDialog,
    NotificationsComponent,
    NotificationsDialog,
    RetailerLoginComponent,
    BroadcastDetailsDialog,
    DeviceDialogComponent,
    ContentProviderHeaderComponent,
    DeviceFilterHistoryComponent,
    DeviceFiltersComponent,
    AdditionalHistoryDialog,
    DeviceAssignComponent,
    DeviceRetailerHistoryComponent,
    DeviceContentsComponent,
    AdminRetailerDashboardComponent,
    EditSubscriptionComponent,
    ExportUserDataComponent,
    EditIncentiveEndDateComponent,
    DeleteUserDataComponent
  ],
  imports: [
    HttpClientModule,
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    CMSMaterialModule,
    FormsModule,
    MatNativeDateModule,
    ReactiveFormsModule,
    FlexLayoutModule ,
    ToastrModule.forRoot({
      timeOut: 3500,
      positionClass: 'toast-top-center',
      preventDuplicates: true
    }
    ),
    CarouselModule,
    RetailerModule
  ],
  exports: [RouterModule],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: SpinnerInterceptor,
      multi: true,
    },
    { provide: LOCALE_ID, useValue: 'en-IN' },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
