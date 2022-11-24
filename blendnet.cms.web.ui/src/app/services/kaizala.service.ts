// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { last, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ContentProviderService } from './content-provider.service';
import { LogService } from './log.service';
import jwt_decode from "jwt-decode";
import { UserService } from './user.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class KaizalaService {
  public currentUserSubject: BehaviorSubject<any>;
  public currentUser: Observable<any>;
  public currentUserNameSubject: BehaviorSubject<any>;
  public currentUserName: Observable<any>;
  public loggedInUser: BehaviorSubject<any>;


  
  baseUrl0 = environment.kaizalaApi0;
  baseUrl1 = environment.kaizalaApi1;
  baseUrl2 = environment.kaizalaApi2;


  constructor(
    private logger: LogService,
    private http: HttpClient,
    private contentProviderService: ContentProviderService,
    private userService: UserService,
    private router: Router
  ) {
    // this.currentUserSubject = new BehaviorSubject<any>(sessionStorage.getItem('currentUser'));
    // this.loggedInUser = new BehaviorSubject<any>(sessionStorage.getItem('loggedInUser'));
    this.currentUserSubject = new BehaviorSubject<any>(JSON.parse(sessionStorage.getItem('currentUser')));
    this.loggedInUser = new BehaviorSubject<any>(JSON.parse(sessionStorage.getItem('loggedInUser')));
    this.currentUser = this.currentUserSubject.asObservable();
    this.currentUserNameSubject = new BehaviorSubject<any>(sessionStorage.getItem('currentUserName'));
    this.currentUserName = this.currentUserNameSubject.asObservable();

   }

  public get currentUserValue() {
    return this.currentUserSubject.value;
  }

  public get loggedInValue() {
    return this.loggedInUser.value;
  }

  public get currentUserNameValue() {
    return this.currentUserNameSubject.value;
  }

  public isLoggedIn() {
    // return this.currentUserValue !== null;
    return this.loggedInValue !== null;
  }


  getURLSuffix (lastDigit) {
    var url = "";
    switch(lastDigit) {
      case '0' : 
      case '4' :
      case '6' :
        url = this.baseUrl0;
        break;
      case '1' :
      case '3' :
      case '5' :
        url = this.baseUrl1;
        break;
      case '2' :
      case '7' :
      case '8' :
      case '9' :
        url = this.baseUrl2;
        break;
    }
    return url;
  }

  getOTP(contact: string) {
    const lastDigit = contact.charAt(contact.length-1);
    var url = this.getURLSuffix(lastDigit).concat(environment.kaizalaSignUpSignIn);
    this.logger.log(`Calling Authentication by OTP`);
    var request = {
      "phoneNumber": contact,
      "UseVoice" : false
    }

    return this.http.post(url, request, {
      headers: {
        'AppName': environment.kaizalaAppName},
      observe: 'response'
    });

  }

  verifyOTP(otp: string, countryCode: string, contact: string) {
    const lastDigit = contact.charAt(contact.length-1);
    var url = this.getURLSuffix(lastDigit).concat(environment.kaizalaVerifyOTP);
    this.logger.log(`Calling Verify OTP`);
    var request = {
        "phoneNumber": countryCode.concat(contact),
        "pin": otp,
        "permissions": ["1.1"]
      }
      
    return this.http.post(url, request, 
      {
        headers: {'AppName': environment.kaizalaAppName}
      }).pipe(map(user => {
        sessionStorage.setItem('currentUserName', contact);
        sessionStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);
        return user;
      }));
  }

  getUserRoles(contact: string) {
    const lastDigit = contact.charAt(contact.length-1);
    var url = this.getURLSuffix(lastDigit).concat(environment.kaizalaGetUserRoles);
    let params = new HttpParams().set(environment.kaizalaAppNameParam, environment.kaizalaAppName);
    //.concat(environment.appName);
    var user = this.currentUserValue;
    return this.http.get(url, 
      {
        params: params,
        headers: {'accessToken': user.authenticationToken},
        observe: 'response'
      });
  }

  validateRetailerRole(authenticationToken) {
    const decodedToken = jwt_decode(authenticationToken);
    const urnCreds = JSON.parse(decodedToken['urn:microsoft:credentials'])
    const phoneNumber = urnCreds['phoneNumber'];
    const lastDigit = phoneNumber.charAt(phoneNumber.length-1);
    var url = this.getURLSuffix(lastDigit).concat(environment.kaizalaGetUserRoles);
    //.concat(environment.appName);
    let params = new HttpParams().set(environment.kaizalaAppNameParam, environment.kaizalaAppName);
    return this.http.get(url, 
      {
        params: params,
        headers: {'accessToken': authenticationToken},
        observe: 'response'
      });
  }

  logout() {
    // remove the selected Content Provider from the local storage
    sessionStorage.clear();
    this.contentProviderService.changeDefaultCP(null);
    // remove user from local storage and set current user to null
    this.userService.resetRegisteredUserValue();
    this.currentUserSubject.next(null);
    this.currentUserNameSubject.next(null);
    this.loggedInUser.next(null);
    this.router.navigate(['/login']);
  }
}
