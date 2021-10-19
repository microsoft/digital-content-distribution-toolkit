import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from '../log.service';

@Injectable({
  providedIn: 'root'
})
export class ActivateNumberService {

  baseUrl:string = environment.baseUrl+environment.whitelistedUserApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  activateNumber(phoneNumber: string): Observable<any> {
    this.logger.log('activating user phonenumber');
    let payload = {
      phoneNumber: phoneNumber,
      partnerCode: sessionStorage.getItem('partnerCode'),
      partnerProvidedRetailerId: sessionStorage.getItem('partnerProvidedId')
    }
    return this.http.post(this.baseUrl, payload);
  }
}
