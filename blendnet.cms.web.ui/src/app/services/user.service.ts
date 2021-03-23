import { HttpClient, HttpResponse } from '@angular/common/http';
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

  getUserDetails(upn): Observable<HttpResponse<any>>{
    let url = this.baseUrl + '/getuser?upn=%2B91' + upn;
    this.logger.log(`Fetching user details`);
    return this.http.get(url, { observe: 'response'});
  }
}