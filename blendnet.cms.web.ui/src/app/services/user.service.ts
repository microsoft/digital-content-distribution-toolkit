import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.userApiUrl;
  isRetailerRouted = false;
  private loggedInUser = new BehaviorSubject<any>(null);
  loggedInUser$ = this.loggedInUser.asObservable();

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  getUserDetails(upn): Observable<any>{
    let url = this.baseUrl + '/user/' + upn;
    this.logger.log(`Fetching user details`);
    return this.http.get(url);
  }

  createUser(user)  {
    let url = this.baseUrl + '/user';
    this.logger.log(`Creating new user `);
    return this.http.post(url, user);
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