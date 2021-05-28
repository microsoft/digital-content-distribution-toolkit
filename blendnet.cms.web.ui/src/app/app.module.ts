import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { UnprocessedComponent } from './unprocessed/unprocessed.component';
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
import { LoginComponent } from './login/login.component';
import { ToastrModule } from 'ngx-toastr';
import { HttpErrorInterceptor } from './interceptor/http-error.interceptor';
import { SpinnerOverlayComponent } from './spinner-overlay/spinner-overlay.component';
import { SpinnerInterceptor } from './interceptor/spinner.interceptor';
import { BroadcastComponent } from './broadcast/broadcast.component';
import { SubscriptionComponent } from './subscription/subscription.component';
import { HomeComponent } from './home/home.component';
import { JwtInterceptor } from './interceptor/jwt.interceptor';
import { CommonDialogComponent } from './common-dialog/common-dialog.component';
import { AddSubscriptionDialog } from './subscription/add-subscription-dialog';
import { ProfileComponent } from './profile/profile.component';

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
    LoginComponent,
    SpinnerOverlayComponent,
    ContentTokenDialog,
    ProcessConfirmDialog,
    BroadcastComponent,
    SubscriptionComponent,
    HomeComponent,
    AddSubscriptionDialog,
    CommonDialogComponent,
    ProfileComponent
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
    )
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
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
