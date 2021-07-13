import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  baseUrl = environment.retailerApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }

  getRetailerPartners(): Observable<any>{
    let url = this.baseUrl + '/RetailerProvider/all' ;
    this.logger.log(`Fetching Retailer Provider`);
    return this.http.get(url);
  }


}