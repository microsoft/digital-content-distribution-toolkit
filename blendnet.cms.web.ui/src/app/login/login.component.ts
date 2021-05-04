import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import { ActivatedRoute, Router } from '@angular/router';
// import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
// import { EventMessage, EventType } from '@azure/msal-browser';
// import { filter } from 'rxjs/operators';
import { KaizalaService } from '../services/kaizala.service';
// import { UserService } from '../services/user.service';


interface CountryCode {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  // showLoginDisplay = false;
  username ="";
  role:string[]= [];
  roles: string;
  // token;
  selectedCountryCodeValue: string;
  otpSendErrorMessage: string;
  otpVerifyErrorMessage: string;

  countryCodes: CountryCode[] = [
    {value: '+91', viewValue: 'India (+91)'},
    {value: '+92', viewValue: 'Pakistan (+92)'},
    {value: '+93', viewValue: 'Sri Lanka (+93)'},
    {value: '+94', viewValue: 'China (+94)'},
    {value: '+95', viewValue: 'Russia (+95)'},
    {value: '+193', viewValue: 'Bangladesh (+192)'},
    {value: '+96', viewValue: 'Thailand (+96)'},
    {value: '+97', viewValue: 'Vietnam (+97)'},
    {value: '+98', viewValue: 'Combodia (+98)'},
    {value: '+99', viewValue: 'Phillipines (+99)'},
    {value: '+931', viewValue: 'United Arab Emirates (+931)'},
    {value: '+937', viewValue: 'Indonesia (+937)'}
  ];


  @ViewChild('sidenav') sidenav: MatSidenav;
  isExpanded = true;
  showHomeSubmenu: boolean = true;
  showContentSubmenu: boolean = true;
  showDeviceSubmenu: boolean = true;
  isCountryCodeSection: boolean = false;
  isContactOnlySection: boolean = true;
  isOTPSection: boolean = false;
  contact;
  otp;
  returnUrl: string;

  constructor(
    // private authService: MsalService, 
    // private msalBroadcastService: MsalBroadcastService,
    private kaizalaService: KaizalaService,
    // private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    ) { 
      // redirect to home if already logged in
      if (this.kaizalaService.currentUserValue) { 
        this.router.navigate(['/']);
      }
    }

  ngOnInit(): void {
    // this.msalBroadcastService.msalSubject$
    //   .pipe(
    //     filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS),
    //   )
    //   .subscribe({
    //     next: (result: EventMessage) => {
    //       console.log(result);
    //       if (result?.payload?.account) {
    //         this.authService.instance.setActiveAccount(result.payload.account);
    //       }
    //     },
    //     error: (error) => console.log(error)
    //   });

    // this.setLoginDisplay();
    this.contact = new FormControl('', [Validators.required, Validators.maxLength(10), Validators.minLength(10), Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    this.otp = new FormControl('', [Validators.required, Validators.maxLength(6),  Validators.minLength(4),  Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    this.otpSendErrorMessage = "";
    this.otpVerifyErrorMessage = "";
    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

  }

  getContactErrorMessage() {
    if (this.contact.hasError('required')) {
      return 'You must enter a value';
    }
    return this.contact.invalid ? 'Not a valid Phone Number' : '';
  }

  getOTPErrorMessage() {
    if (this.otp.hasError('required')) {
      return 'You must enter a value';
    }

    return this.otp.hasError('contact') ? 'Not a valid OTP' : '';
  }

  // setLoginDisplay() {
    // this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    // if(this.loginDisplay) {
    //   this.token = this.authService.instance.getAllAccounts()[0].idTokenClaims;
    //   this.token.groups.forEach(group => {
    //     this.role.push(group);
    //   });
    //   this.username = this.token.givenName;
    //   console.log(this.username);
    //   console.log(this.role);
    // }
  //   this.showLoginDisplay = localStorage.getItem("currentUser") ? (localStorage.getItem("roles") ? false: true) : true;
  //   if(!this.showLoginDisplay) {
  //     var roles = localStorage.getItem("roles");
  //   }
  // }

  showContactOnlySection() {
    this.isContactOnlySection = true;
    this.isCountryCodeSection = false;
    this.isOTPSection = false;
  }

  showCountryCodeSection() {
    this.isContactOnlySection = false;
    this.isCountryCodeSection = true;
    this.isOTPSection = false;
    this.otpSendErrorMessage = "";
    this.otpVerifyErrorMessage = "";
    this.otp.setValue("");
  }

  showOTPSection() {
    this.otpSendErrorMessage = "";
    this.kaizalaService.getOTP(this.selectedCountryCodeValue.concat(this.contact.value)).subscribe(
      res => {
        if(res.status === 200) {
          this.isContactOnlySection = false;
          this.isCountryCodeSection = false;
          this.isOTPSection = true;
        }    
      },
      err => {
        this.otpSendErrorMessage = err;
      }
    );
  }

  verifyOTP () {
    this.kaizalaService.verifyOTP(this.otp.value, this.selectedCountryCodeValue, this.contact.value).subscribe(
      res => {
        // this.loginDisplay = true;
        // var response: any = res.body;
        // localStorage.setItem("userId", response.userId);
        // localStorage.setItem("clientId", response.clientId);
        // localStorage.setItem("token", response.authenticationToken);
        this.kaizalaService.getUserRoles(this.contact.value).subscribe(
          res => {
            var response: any = res.body;
            localStorage.setItem("roles", response.userRole);
            this.roles =  localStorage.getItem("roles");
            // this.showLoginDisplay = false;
            // this.userService.changeShowLoginDisplay(false);
            // // this.router.navigate(['/'])
            // // .then(() =>
            //   window.location.reload();
            // // );
            this.isContactOnlySection = true;
            this.isCountryCodeSection = false;
            this.isOTPSection = false; 
            this.router.navigate([this.returnUrl]);
          },
          err => {
            this.otpVerifyErrorMessage = err;
          }
        )
      },
      err => {
        this.otpVerifyErrorMessage = err;
      }
    );
  }

  logout() {
    this.kaizalaService.logout();
  }

  // ngDoCheck() {
  //   this.userService.showLoginDisplay$.subscribe(showLoginDisplay => {
  //     if(showLoginDisplay) {
  //       this.showLoginDisplay = showLoginDisplay;
  //     }
  //   });
  // }
 
}
