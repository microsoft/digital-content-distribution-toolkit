// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, OnInit, AfterViewInit } from '@angular/core';
import {KaizalaService } from 'src/app/services/kaizala.service'
import { ActivatedRoute, Router } from '@angular/router';
import jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-seamless-login',
  templateUrl: './seamless-login.component.html',
  styleUrls: ['./seamless-login.component.css']
})
export class SeamlessLoginComponent implements OnInit, AfterViewInit {
  partnerCode: string;
  partnerProvidedId: string;
  authenticationToken: string;
  roles: string;
  isCountryCodeSection: boolean = true;
  isContactOnlySection: boolean = true;
  isOTPSection: boolean = false;
  otpVerifyErrorMessage: string;
  message: string = "Logging in as retailer...please wait";

  constructor(
    private kaizalaService: KaizalaService,
    private route: ActivatedRoute,
    // private userService: UserService,
    private router: Router
  ) {
    this.route.queryParams.subscribe(params => {
        this.partnerCode = params['partnerCode'];
        this.partnerProvidedId = params['partnerProvidedId'];
        this.authenticationToken = params['authenticationToken'];
        sessionStorage.setItem('partnerCode', this.partnerCode);
        sessionStorage.setItem('partnerProvidedId', this.partnerProvidedId);
        sessionStorage.setItem('authenticationToken', this.authenticationToken);
        let currentUser = {
          authenticationToken: this.authenticationToken
        }
        sessionStorage.setItem('currentUser', JSON.stringify(currentUser));
        if(sessionStorage.getItem('retailerLogged')){

        } else {
          sessionStorage.setItem('retailerLogged', 'true');
          location.reload();
        }
    },
    err => window.alert(err)
    );
   }

  ngOnInit(): void {
    this.fetchRetailer();
  }

  ngAfterViewInit() {
  }

  fetchRetailer() {
    this.kaizalaService.validateRetailerRole(this.authenticationToken).subscribe( 
      res => {
        var response: any = res.body;
        sessionStorage.setItem('roles', response.userRole);
        this.roles =  sessionStorage.getItem('roles');

        const decodedToken = jwt_decode(this.authenticationToken);
        const urnCreds = JSON.parse(decodedToken['urn:microsoft:credentials'])
        const phoneNumber = urnCreds['phoneNumber'];
        sessionStorage.setItem('currentUserName', phoneNumber);
        sessionStorage.setItem('accessedViaPartnerApp', "true");
        // this.kaizalaService.setLoggedInUser(phoneNumber);
        this.kaizalaService.currentUserNameSubject.next(phoneNumber);
        this.kaizalaService.currentUserSubject.next({authenticationToken: this.authenticationToken});
        this.router.navigate(['retailer/dashboard']);
      },
      err => {
        this.otpVerifyErrorMessage = err;
        this.message = "Session expired. Please login again!";
      }
    ) 

  }
}
