import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import { ActivatedRoute, Router } from '@angular/router';
// import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
// import { EventMessage, EventType } from '@azure/msal-browser';
// import { filter } from 'rxjs/operators';
import { KaizalaService } from '../services/kaizala.service';
import { UserService } from '../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';


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


  @ViewChild('sidenav') sidenav: MatSidenav;
  isExpanded = true;
  showHomeSubmenu: boolean = true;
  showContentSubmenu: boolean = true;
  showDeviceSubmenu: boolean = true;
  isCountryCodeSection: boolean = true;
  isContactOnlySection: boolean = true;
  isOTPSection: boolean = false;
  contact;
  otp;
  countryCodes;
  returnUrl: string;

  constructor(
    // private authService: MsalService, 
    // private msalBroadcastService: MsalBroadcastService,
    private toastr : ToastrService,
    private kaizalaService: KaizalaService,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    ) { 
      // redirect to home if already logged in
      if (this.kaizalaService.currentUserValue) { 
        this.router.navigate(['/']);
      }
    }

  ngOnInit(): void {
    this.countryCodes = environment.countryCodes;
    this.selectedCountryCodeValue = this.countryCodes[0].value;
    this.contact = new FormControl('', [Validators.required, Validators.maxLength(10), Validators.minLength(10), Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    this.otp = new FormControl('', [Validators.required, Validators.maxLength(6),  Validators.minLength(4),  Validators.pattern(/^-?(0|[1-9]\d*)?$/)]);
    this.otpSendErrorMessage = "";
    this.otpVerifyErrorMessage = "";
    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

  }

  getContactErrorMessage() {
    this.otpSendErrorMessage= "";
    if (this.contact.hasError('required') || this.contact.invalid) {
      return 'Please enter a valid Phone Number';
    }
  }

  getOTPErrorMessage() {
    this.otpVerifyErrorMessage = "";
    if (this.otp.hasError('required') || this.otp.invalid) {
      return 'Please enter a valid OTP received on the phone';
    }

  }

  showContactOnlySection() {
    this.isContactOnlySection = true;
    this.isCountryCodeSection = false;
    this.isOTPSection = false;
  }

  showCountryCodeSection() {
    this.selectedCountryCodeValue = this.countryCodes[0].value;
    this.isContactOnlySection = false;
    this.isCountryCodeSection = true;
    this.isOTPSection = false;
    this.otpSendErrorMessage = "";
    this.otpVerifyErrorMessage = "";
    this.otp.setValue("");
  }

  resendOTP() {
    this.otpSendErrorMessage = "";
    this.otpVerifyErrorMessage = "";
    this.showOTPSection();
  }
  showOTPSection() {
    //this.kaizalaService.getOTP(this.selectedCountryCodeValue.concat(this.contact.value));
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
        var response : any = res;
        if(response.isNewUser) {
          var user: any;
          user = {
            userName : this.contact.value,
            channelId : environment.channelName
          }
          this.userService.createUser(user).subscribe(res => {
            console.log("User created successfully");
          },
          err => {
            console.log(err);
            this.toastr.warning("User could not be created in CMS. Please contact admin.")
          })
        }
        this.kaizalaService.getUserRoles(this.contact.value).subscribe(
          res => {
            var response: any = res.body;
            localStorage.setItem("roles", response.userRole);
            this.roles =  localStorage.getItem("roles");
            this.isContactOnlySection = true;
            this.isCountryCodeSection = false;
            this.isOTPSection = false; 
            this.userService.setLoggedInUser(this.contact.value);
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
