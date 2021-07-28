import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  userBaseUrl = environment.userApiUrl;
  createUserBaseUrl = environment.createUserApiUrl;
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
    let url = this.userBaseUrl + '/user/' + upn;
    this.logger.log(`Fetching user details`);
    return this.http.get(url);
  }

  public get registeredUserValue() {
    return this.registeredUserSubject.value;
  }

  
  public resetRegisteredUserValue() {
    this.registeredUserSubject.next(null);
  }

  createUser(user)  {
    let url = this.createUserBaseUrl + '/user';
    this.logger.log(`Creating new user `);
    return this.http.post(url, user).pipe(map(userId => {
      sessionStorage.setItem('registeredUser', userId.toString())
      this.registeredUserSubject.next(userId);
      return userId;
    }));

    



  }

  setRetailerRouted(flag) {
    console.log('setting routed to' + flag)
    this.isRetailerRouted = flag;
  }

  getRetailerRouted() {
    return this.isRetailerRouted;
  }
  
  setLoggedInUser(user) {
    this.loggedInUser.next(user)
  }
}