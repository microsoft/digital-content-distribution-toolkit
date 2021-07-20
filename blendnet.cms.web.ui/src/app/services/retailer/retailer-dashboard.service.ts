import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LogService } from '../log.service';
import * as moment from 'moment';


@Injectable({
  providedIn: 'root'
})
export class RetailerDashboardService {

  baseUrl = environment.retailerDashboardUrl;
  retailerUrl = environment.retailerApiUrl;
  retailerIncentive = environment.retailerDashboardUrl;
  partnerCode = sessionStorage.getItem('partnerCode');
  retailerPartnerProvidedId = sessionStorage.getItem('partnerProvidedId');
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


  getpartnerCode() {
    return sessionStorage.getItem('partnerCode');
  }

  getRetailerPartnerProvidedId() {
    let partnerProvidedId = sessionStorage.getItem('parnterProvidedId');
    return partnerProvidedId
  }

  getMilestoneRatesIncentives(partnerCode: string, retailerPartnerProvidedId: string) {
    let url = this.retailerIncentive + 'IncentiveBrowse/retailer/active/'+ retailerPartnerProvidedId + '/MILESTONE/' + partnerCode;
    return this.http.get(url);  
  }

  // api/v1/IncentiveBrowse/retailer/all/NVP/REGULAR/NOVO
  getRegularRatesIncentives(partnerCode: string, retailerPartnerProvidedId: string) {
    let url = this.retailerIncentive + 'IncentiveBrowse/retailer/active/'+ retailerPartnerProvidedId + '/REGULAR/' + partnerCode;
    return this.http.get(url);  
  }

  generateReadableDate(occuredDate) {
    const splitString = occuredDate.split('-')
    let totalString = '';
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
      "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    totalString+=splitString[2]+', ';
    totalString+=monthNames[parseInt(splitString[1])]+', ';
    totalString+=splitString[0];
    return totalString;
  }

  generateReadableTime(occuredDate) {
    return moment(new Date(occuredDate)).format('h:mm A');
  }

  getBaseHref() {
    return 'cmsui'
  }

  formatDate(date) {
    return moment(new Date(date)).format('DD, MMM, YYYY h:mm A');
  }

  formatDateOnlyDay(date) {
    return moment(new Date(date)).format('DD MMM, YYYY');
  }


}
