import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class RetailerService {
  baseUrl = environment.baseUrl + environment.retailerUrl;
  baseRetailerProviderUrl = environment.baseUrl + environment.retailerProviderUrl;
  constructor(private logger: LogService,
    private http: HttpClient) { }

    getRetailers() {
      let url = this.baseUrl ;
      this.logger.log(`Fetching all Retailers`);
      return this.http.get(url, { observe: 'response'});
    }

    createRetailer(Retailer): Observable<HttpResponse<any>>{ 
      let url = this.baseUrl ;
      this.logger.log(`Creating a Retailer`);
      return this.http.post(url, Retailer,{ observe: 'response', responseType: 'text'});
    }

    filterUpdate(RetailerDate) {
      let url = this.baseUrl + '/filterupdate';
      this.logger.log(`Updating Retailer filters`);
      return this.http.post(url, RetailerDate,{ observe: 'response'});
    }

    getRetailerHistory(RetailerId) {
      let url = this.baseUrl + '/' + RetailerId +'/commands';
      this.logger.log(`Getting Retailer history`);
      return this.http.get(url,{ observe: 'response'});
    }

    assignDeviceToRetailer(request) {
        let url = this.baseUrl + '/assignDevice';
        this.logger.log(`Assign a device to a retailer`);
        return this.http.post(url, request,{ observe: 'response'});
    }

    deployDeviceToRetailer(request) {
      let url = this.baseUrl + '/deployDevice';
      this.logger.log(`Deploying a device to a retailer`);
      return this.http.post(url, request,{ observe: 'response'});
    }

    unassignDeviceToRetailer(request) {
        let url = this.baseUrl + '/unassignDevice';
        this.logger.log(`Unassign a device to a retailer`);
        return this.http.post(url, request,{ observe: 'response'});
    }

    getRetailerByPartnerId(partnerId) {
        let url = this.baseUrl + '/byPartnerId/' + partnerId;
        this.logger.log(`Getting Retailer details by partner ID`);
        return this.http.get(url,{ observe: 'response'});
    }

    getRetailerByReferralCode(referralCode) {
        let url = this.baseUrl + '/byReferralCode/' + referralCode;
        this.logger.log(`Getting Retailer details by referral code`);
        return this.http.get(url,{ observe: 'response'});
    }

    getRetailerByDeviceId(deviceId) {
        let url = this.baseUrl + '/byDeviceId/' + deviceId;
        this.logger.log(`Getting Retailer details by device ID`);
        return this.http.get(url,{ observe: 'response'});
    }

    getRetailerPartners(): Observable<any>{
        let url = this.baseRetailerProviderUrl + '/all' ;
        this.logger.log(`Fetching Retailer Providers`);
        return this.http.get(url);
      }
    
}
