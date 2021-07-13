import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from '../log.service';


@Injectable({
  providedIn: 'root'
})
export class RetailerDashboardService {

  baseUrl = environment.retailerDashboardUrl;
  retailerUrl = environment.retailerApiUrl;
  monthSelected:any;
  constructor(
    private logger: LogService,
    private http: HttpClient
  ) { }


  getAggregatedRetailerDetails(partnerCode: string, retailerPartnerProvidedId: string, startDate='', endDate=''): Observable<any>{
    let url = this.baseUrl + 'IncentiveEvent/retailer/regular/';
    // let url = this.baseUrl + 'IncentiveEvent/retailer/events';
      const rangeUrl = (startDate && endDate) ? 'range/': '';
      url += rangeUrl;
      url += partnerCode + '/' + retailerPartnerProvidedId;
    this.logger.log(`Fetching retailer home total details`);
    return this.http.get(url, {params: {partnerCode: partnerCode, retailerPartnerProvidedId: retailerPartnerProvidedId, startDate: startDate, endDate: endDate, version :'1'}});
  }

  
  getMileStonesHome(partnerCode: string, retailerPartnerProvidedId: string): Observable<any>{
    let url = this.baseUrl + 'IncentiveEvent/retailer/milestone/' + retailerPartnerProvidedId + '/' + partnerCode;
    return this.http.get(url, {params: {partnerCode: partnerCode, retailerPartnerProvidedId: retailerPartnerProvidedId, version :'1'}});
  }

  getReferralsCommissionsInDetail(partnerCode: string, retailerPartnerProvidedId: string, startDate='', endDate='', eventType: string): Observable<any>{
    let url = this.baseUrl + 'IncentiveEvent/retailer/events';
    return this.http.get(url, {params: {partnerCode: partnerCode, retailerPartnerProvidedId: retailerPartnerProvidedId, version :'1', eventType: eventType, startDate: startDate, endDate: endDate}});
  }

  getReferralsCommissionsCarouselInfo(partnerCode: string, retailerPartnerProvidedId: string): Observable<any>{
    let url = this.baseUrl + 'IncentiveEvent/retailer/regular/'+ partnerCode + '/' + retailerPartnerProvidedId;
    return this.http.get(url, {params: {partnerCode: partnerCode, retailerPartnerProvidedId: retailerPartnerProvidedId, version :'1'}});
  }

  setMonthSelected(month) {
    this.monthSelected = month;
  }

  getMonthSelected() {
    return this.monthSelected;
  }

  getProfile(partnerCode: string, retailerPartnerProvidedId: string) {
    let url = this.retailerUrl + '/Retailer/'+ partnerCode + '/' + retailerPartnerProvidedId + '/me';
    return this.http.get(url);  
  }

}
