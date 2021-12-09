import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { KaizalaService } from 'src/app/services/kaizala.service';
import { UserService } from 'src/app/services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-retailer-login',
  templateUrl: './retailer-login.component.html',
  styleUrls: ['./retailer-login.component.css']
})
export class RetailerLoginComponent implements OnInit {

  isPhoneNumberSection: boolean = true;
  isOTPSection: boolean = false;
  isRetailerIdSection: boolean = false;
  invalidContact: boolean = false;
  otpLength: number = 4;
  baseHref = environment.baseHref;

  selectedCountryCodeValue: string;
  otpSendErrorMessage: string;
  otpVerifyErrorMessage: string;
  retailerLinkErrorMessage: string;
  roles: string;
  returnUrl: string;
  

  otp;
  countryCodes;
  contact;
  partnerCode;
  retailerId;
  user;

  constructor(
    private userService: UserService,
    private kaizalaService: KaizalaService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // Redirect to home if retailer already logged in
    if(this.kaizalaService.loggedInValue && this.userService.registeredUserValue) {
      this.router.navigate(['/retailer/orders']);
    }
   }

  ngOnInit(): void {
    console.log('login page')
    this.countryCodes = environment.countryCodes;
    this.selectedCountryCodeValue = this.countryCodes[0].value;
    this.otpSendErrorMessage = "";
    this.otpVerifyErrorMessage = "";
    this.retailerLinkErrorMessage = "";
    this.contact = "";
    this.retailerId = "";
    this.partnerCode = "";
    this.returnUrl = '/retailer/orders';
  }

  goToPreviousPage() {
    if(this.isOTPSection){
      this.otpVerifyErrorMessage = "";
      this.isOTPSection = false;
      this.isPhoneNumberSection = true;
    }
    else{
      this.retailerLinkErrorMessage = "";
      this.isRetailerIdSection = false;
      this.isOTPSection = true;
    }
  }

  showOTPSection() {
    this.kaizalaService.getOTP(this.selectedCountryCodeValue.concat(this.contact)).subscribe(
      res => {
        if(res.status === 200) {
          this.otpSendErrorMessage = "";
          this.isPhoneNumberSection = false;
          this.isRetailerIdSection = false;
          this.isOTPSection = true;
        }
      },
      err => {
        this.otpSendErrorMessage = err;
      }
    )
  }

  verifyOTP() {
    this.otp = this.getOTPValue();
    if(this.otp.length < this.otpLength) {
      this.otpVerifyErrorMessage = "Incorrect OTP. Please check and fill again.";
      return;
    }

    this.kaizalaService.verifyOTP(this.otp, this.selectedCountryCodeValue, this.contact).subscribe(
      res => {
        this.user = res;
        this.isPhoneNumberSection = false;
        this.isOTPSection = false;
        this.isRetailerIdSection = true;
      },
      err => {
        this.otpVerifyErrorMessage = err;
      }
    );
  }

  linkRetailerId() {
  
    let userPayload = {
      userName: this.contact,
      channelId: environment.channelName
    }
    this.userService.createUser(userPayload).subscribe(res => {
      this.kaizalaService.getUserRoles(this.contact).subscribe(
        res => {
          let response: any = res.body;
          this.roles = response.userRole;
          let retailerPayload = {
            partnerCode: this.partnerCode,
            partnerProvidedId: this.retailerId
          }

          this.userService.linkRetailer(retailerPayload).subscribe(
            res => {
              sessionStorage.setItem("roles", this.roles);
              sessionStorage.setItem("accessedViaPartnerApp", "false");
              sessionStorage.setItem('loggedInUser', JSON.stringify(this.user));
              sessionStorage.setItem('partnerCode', this.partnerCode);
              sessionStorage.setItem('partnerProvidedId', this.retailerId);
              // this.kaizalaService.currentUserNameSubject.next(this.contact);
              this.kaizalaService.loggedInUser.next(this.user);
              this.router.navigate([this.returnUrl]);
            },
            err => {
              console.log(err);
              this.retailerLinkErrorMessage = "Incorrect retailer id. Please check and fill again.";
            }
          )
        },
        err => {
          this.otpVerifyErrorMessage = err;
        }
      )
    },
    err => {
      console.log(err);
      this.otpVerifyErrorMessage = "User could not be registered. Please contact admin";
    }
    );

    
    //for testing
    // sessionStorage.setItem("accessedViaPartnerApp", "false");
    // sessionStorage.setItem('roles', "Retailer");
    // sessionStorage.setItem("currentUser", "test");
    // sessionStorage.setItem('currentUserName', "test 100");
    // this.kaizalaService.currentUserNameSubject.next("test 100");
    // this.kaizalaService.currentUserSubject.next("test");
    // this.kaizalaService.loggedInUser.next("test");
    // testing code end
    // console.log(this.returnUrl);
    // if(!this.retailerLinkErrorMessage){
    //   console.log('eror; ', this.retailerLinkErrorMessage);
    //   this.router.navigate([this.returnUrl]);
    // }
    
  }

  OTPInput() {
    const myDiv = document.getElementById("OTPInput");
    const inputs = myDiv.querySelectorAll<HTMLInputElement>('#OTPInput > .retailer-otp-input');
    for(let i=0; i<inputs.length; i++){
      inputs[i].addEventListener('keydown', function(event: KeyboardEvent) {
        if(event.key === "Backspace") {
          if(inputs[i].value == ''){
            if(i!=0){
              inputs[i-1].focus();
            }
          } else {
            inputs[i].value = '';
          }
        } else if (event.key === "ArrowLeft" && i!=0) {
          inputs[i-1].focus();
        } else if(event.key === "ArrowRight" && i!=inputs.length-1){
          inputs[i+1].focus();
        } else if(event.key !== "ArrowLeft" && event.key !== "ArrowRight") {
          inputs[i].setAttribute("type", "text");
          inputs[i].value = '';
        }
      });

      inputs[i].addEventListener('input', function () {
        if (i===inputs.length-1 && inputs[i].value != '') {
          return true;
        } else if(inputs[i].value !== '') {
          inputs[i+1].focus();
        }
      });
    }
  }

  getOTPValue() {
    const inputs = document.querySelectorAll<HTMLInputElement>('#OTPInput > *[id]');
    let compiledOTP = '';
    for(let i=0;i<this.otpLength;i++){
      compiledOTP += inputs[i].value;
    }
    return compiledOTP;
  }

}
