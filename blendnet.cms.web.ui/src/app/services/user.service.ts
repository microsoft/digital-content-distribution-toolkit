import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.userApiUrl;

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
}