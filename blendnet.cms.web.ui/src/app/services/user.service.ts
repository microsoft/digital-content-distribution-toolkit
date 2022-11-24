// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ExportDataReq } from '../models/export-user-data-req.model';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  userBaseUrl = environment.baseUrl + environment.userApiUrl;
  userBasicBaseUrl = environment.baseUrl +  environment.userBasicApiUrl;
  userOnboardingBaseUrl = environment.baseUrl +  environment.userOnboardingApiUrl;

  isRetailerRouted = false;
  private loggedInUser = new BehaviorSubject<any>(null);
  loggedInUser$ = this.loggedInUser.asObservable();
  baseHref = 'cmsui'
  private registeredUserSubject: BehaviorSubject<any>;
  public registeredUser: Observable<any>;
  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { 
    
  this.registeredUserSubject = new BehaviorSubject<any>(sessionStorage.getItem('registeredUser'));
  this.registeredUser = this.registeredUserSubject.asObservable();

  }

  getUserDetails(upn): Observable<any>{
    let url = this.userBaseUrl + '/user';
    this.logger.log(`Fetching user details`);
    var contact = {
      phoneNumber : upn
    }
    return this.http.post(url, contact );
  }

  public get registeredUserValue() {
    return this.registeredUserSubject.value;
  }

  
  public resetRegisteredUserValue() {
    this.registeredUserSubject.next(null);
  }

  createUser(user)  {
    let url = this.userOnboardingBaseUrl + '/user';
    this.logger.log(`Creating new user `);
    return this.http.post(url, user).pipe(map(userId => {
      sessionStorage.setItem('registeredUser', userId.toString())
      this.registeredUserSubject.next(userId);
      return userId;
    }));
  }

  getUserProfile() {
    let url = this.userBasicBaseUrl + '/me';
    return this.http.get(url);
  }

  linkRetailer(payload) {
    let url = this.userOnboardingBaseUrl + '/linkRetailer';
    this.logger.log('Linking retailer id with userid');
    return this.http.post(url, payload);
  }

  getExportUserDataRequests(): Observable<ExportDataReq[]>{
    let url = this.userBaseUrl + '/dataExport/list';
    this.logger.log('Fetch all open export data list');
    return this.http.get<ExportDataReq[]>(url);
  }

  
  getDeleteUserDataRequests(): Observable<ExportDataReq[]>{
    let url = this.userBaseUrl + '/dataDelete/list';
    this.logger.log('Fetch all open export data list');
    return this.http.get<ExportDataReq[]>(url);
  }

  completeDataExportRequest(user){
    let url = this.userBaseUrl + '/dataExport/complete';
    this.logger.log('Complete the export data request');
    return this.http.post(url, user);
  }
  
  completeDataDeleteRequest(user){
    let url = this.userBaseUrl + '/dataDelete/complete';
    this.logger.log('Complete the delete data request');
    return this.http.post(url, user);
  }

  getRequestDetails(request){
    let url = this.userBaseUrl + '/command';
    this.logger.log('Getting the export user data request details');
    return this.http.post(url, request);
  }

  // setRetailerRouted(flag) {
  //   console.log('setting routed to' + flag)
  //   this.isRetailerRouted = flag;
  // }

  // getRetailerRouted() {
  //   return this.isRetailerRouted;
  // }
  
  // setLoggedInUser(user) {
  //   this.loggedInUser.next(user)
  // }
}