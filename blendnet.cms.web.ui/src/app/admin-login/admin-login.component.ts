// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import { ActivatedRoute, Router } from '@angular/router';
import { KaizalaService } from '../services/kaizala.service';
import { UserService } from '../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { lengthConstants } from '../constants/length-constants';
import { CustomValidator } from '../custom-validator/custom-validator';

@Component({
  selector: 'app-admin-login',
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.css']
})
export class AdminLoginComponent implements OnInit {
  username ="";
  role:string[]= [];
  roles: string;
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
  countryCodes;
  returnUrl: string;
  user;
  loginForm: FormGroup;



  constructor(
    private toastr : ToastrService,
    private kaizalaService: KaizalaService,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    ) { 

      // redirect to home if already logged in
      if (this.kaizalaService.loggedInValue && this.userService.registeredUserValue) { 
        this.router.navigate(['/home']);
      }
    }

  ngOnInit(): void {
    this.countryCodes = environment.countryCodes;
    this.selectedCountryCodeValue = this.countryCodes[0].value;
    this.loginForm = new FormGroup({
        contact: new FormControl('', [Validators.maxLength(lengthConstants.phoneMaxLength), 
          Validators.minLength(lengthConstants.phoneMinLength), 
          CustomValidator.numeric]),
        otp: new FormControl('', [Validators.maxLength(lengthConstants.otpMaxLength), 
          Validators.minLength(lengthConstants.otpMinLength), 
          CustomValidator.numeric])
    });
    this.otpSendErrorMessage = "";
    this.otpVerifyErrorMessage = "";
    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/home';

  }

 
  generateOTP() {
    this.kaizalaService.getOTP(this.selectedCountryCodeValue.concat(this.loginForm.get('contact').value)).subscribe(
      res => {
        if(res.status === 200) {
          this.isContactOnlySection = false;
          this.isCountryCodeSection = false;
          this.isOTPSection = true;
          this.toastr.success(`OTP sent to mobile number ${this.loginForm.get('contact').value}`, "", {
            timeOut: 5000,
          });
        }    
      },
      err => {
        this.otpSendErrorMessage = err.code + " " + err.msg;
      }
    );
  }

  phoneNoInvalid() {
    return this.loginForm.get('contact').invalid;
  }
  verifyOTP () {
    this.otpVerifyErrorMessage = "";
    this.otpSendErrorMessage = "";
    this.kaizalaService.verifyOTP(this.loginForm.get('otp').value, this.selectedCountryCodeValue, this.loginForm.get('contact').value).subscribe(
      res => {
          this.user = res;
          var payload: any;
          payload = {
            userName : this.loginForm.get('contact').value,
            channelId : environment.channelName
          }
          this.userService.createUser(payload).subscribe(res => {
            console.log("User created successfully");
            this.kaizalaService.getUserRoles(this.loginForm.get('contact').value).subscribe(
              res => {
                var response: any = res.body;
                sessionStorage.setItem("roles", response.userRole);
                sessionStorage.setItem("accessedViaPartnerApp", "false");
                this.roles =  sessionStorage.getItem("roles");
                this.isContactOnlySection = true;
                this.isCountryCodeSection = false;
                this.isOTPSection = false; 
                sessionStorage.setItem('loggedInUser', JSON.stringify(this.user));
                this.kaizalaService.loggedInUser.next(this.user);
                this.router.navigate([this.returnUrl]);
              },
              err => {
                this.otpVerifyErrorMessage = err.code + " " + err.msg;
                this.loginForm.get('contact').setValue("");
              }
            )
          },
          err => {
            console.error(err);
            this.loginForm.get('otp').setValue("");
            this.toastr.error("User could not be registered. Please contact admin.")
            this.otpVerifyErrorMessage ="User could not be registered. Please contact admin";
          });
      },
      err => {
        this.loginForm.get('otp').setValue("");
        if(err.status === 401) {
          this.otpVerifyErrorMessage = "Please enter valid OTP code received on the phone";
        } else if(err.status === 409) {
          this.otpVerifyErrorMessage = "You have reached maximum limit of OTP attempts";
        } else {
          this.otpVerifyErrorMessage = err.msg;
        }
      }
    );
  }

  logout() {
    this.kaizalaService.logout();
  }

 
}
