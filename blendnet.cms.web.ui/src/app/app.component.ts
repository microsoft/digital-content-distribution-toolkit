import { Component, Inject, ViewChild } from '@angular/core';
import { MsalService, MsalBroadcastService, MSAL_GUARD_CONFIG, MsalGuardConfiguration } from '@azure/msal-angular';
import { EventMessage, EventType, InteractionType, InteractionStatus, PopupRequest, RedirectRequest, AuthenticationResult, AuthError } from '@azure/msal-browser';
import { Subject } from 'rxjs';
import { b2cPolicies } from './b2c-config';
import { filter, takeUntil } from 'rxjs/operators';
import { MatSidenav } from '@angular/material/sidenav';
import { Contentprovider } from './models/contentprovider.model';
import { ContentProviderService } from './services/content-provider.service';

interface IdTokenClaims extends AuthenticationResult {
  idTokenClaims: {
    tfp?: string
  }
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'CMS';
  isIframe = false;
  loginDisplay = false;
  private readonly _destroying$ = new Subject<void>();


  @ViewChild('sidenav') sidenav: MatSidenav;
  isExpanded = true;
  showHomeSubmenu: boolean = true;
  showContentSubmenu: boolean = true;
  showDeviceSubmenu: boolean = true;
  selectedCP: Contentprovider;
  selectedCPName: string  = localStorage.getItem("contentProviderName") ? 
                          localStorage.getItem("contentProviderName") : "Not Selected";
  

  


  constructor(
    @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private authService: MsalService,
    private msalBroadcastService: MsalBroadcastService,
    private cpService: ContentProviderService
  ) {

  }

    ngOnInit(): void {
      this.isIframe = window !== window.parent && !window.opener;
      this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None),
        takeUntil(this._destroying$)
      )
      .subscribe(() => {
        this.setLoginDisplay();
      });
  
      this.msalBroadcastService.msalSubject$
        .pipe(
          filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS || msg.eventType === EventType.ACQUIRE_TOKEN_SUCCESS),
          takeUntil(this._destroying$)
        )
        .subscribe((result: EventMessage) => {
        
          let payload: IdTokenClaims = <AuthenticationResult>result.payload;
  
          // We need to reject id tokens that were not issued with the default sign-in policy.
          // "tfp" claim in the token tells us what policy is used (NOTE: for new policies (v2.0), use "tfp" instead of "tfp")
          // To learn more about b2c tokens, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview
  
          if (payload.idTokenClaims?.tfp === b2cPolicies.names.forgotPassword) {
            window.alert('Password has been reset successfully. \nPlease sign-in with your new password.');
            return this.authService.logout();
          } else if (payload.idTokenClaims['tfp'] === b2cPolicies.names.editProfile) {
            window.alert('Profile has been updated successfully. \nPlease sign-in again.');
            return this.authService.logout();
          }
  
          return result;
        });
  
        this.msalBroadcastService.msalSubject$
        .pipe(
          filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_FAILURE || msg.eventType === EventType.ACQUIRE_TOKEN_FAILURE),
          takeUntil(this._destroying$)
        )
        .subscribe((result: EventMessage) => {
          if (result.error instanceof AuthError) {
            // Check for forgot password error
            // Learn more about AAD error codes at https://docs.microsoft.com/azure/active-directory/develop/reference-aadsts-error-codes
            if (result.error.message.includes('AADB2C90118')) {
              
              // login request with reset authority
              let resetPasswordFlowRequest = {
                scopes: ["openid"],
                authority: b2cPolicies.authorities.forgotPassword.authority,
              };
              this.login(resetPasswordFlowRequest);
            }
          }
        });
        this.cpService.sharedSelectedCP$.subscribe(selectedCP => {
          this.selectedCPName= selectedCP.name;
        });
    }

    ngDoCheck() {
      this.cpService.sharedSelectedCP$.subscribe(selectedCP => {
        this.selectedCPName= selectedCP.name;
      });
    }
    
    setLoginDisplay() {
      console.log(this.authService.instance.getAllAccounts().length);
      this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
      
    }
  

    login(userFlowRequest?: RedirectRequest | PopupRequest) {
      if (this.msalGuardConfig.interactionType === InteractionType.Popup) {
        if (this.msalGuardConfig.authRequest) {
          this.authService.loginPopup({...this.msalGuardConfig.authRequest, ...userFlowRequest} as PopupRequest)
            .subscribe((response: AuthenticationResult) => {
              console.log(response);
              this.authService.instance.setActiveAccount(response.account);
            });
        } else {
          this.authService.loginPopup(userFlowRequest)
            .subscribe((response: AuthenticationResult) => {
              console.log(response);
              this.authService.instance.setActiveAccount(response.account);
            });
        }
      } else {
        if (this.msalGuardConfig.authRequest){
          console.log("this.msalGuardConfig.authRequest");
          console.log(this.msalGuardConfig.authRequest)
          this.authService.loginRedirect({...this.msalGuardConfig.authRequest, ...userFlowRequest} as RedirectRequest)
        } else {
          this.authService.loginRedirect(userFlowRequest);
        }
      }
    }

    logout() {
      localStorage.removeItem("contentProviderId");
      localStorage.removeItem("contentProviderName");
      this.authService.logout();
    }
  
    editProfile() {
      let editProfileFlowRequest = {
        scopes: ["openid"],
        authority: b2cPolicies.authorities.editProfile.authority,
      };
  
      this.login(editProfileFlowRequest);
    }
  
    ngOnDestroy(): void {
      this._destroying$.next(undefined);
      this._destroying$.complete();
    }
}

