import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Incentive, PlanType, RetailerPartner } from '../models/incentive.model';
import { ConfigService } from './config.service';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class IncentiveService {

  baseUrl = environment.incentiveApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient,
    private configService: ConfigService
  ) {
    
   }


  createIncentivePlanRetailer(incentivePlan){
    let url = this.baseUrl + "/retailer";
    this.logger.log(`Creating retailerincentive plan`);
    return this.http.post(url, incentivePlan, { observe: 'response'});

  }

    createIncentivePlanConsumer(incentivePlan){
    let url = this.baseUrl + "/consumer";
    this.logger.log(`Creating consumer incentive plan`);
    return this.http.post(url, incentivePlan, { observe: 'response'});

  }

  getRetailerIncentives(): Observable<any> {
    return this.configService.getRetailerPartners().pipe(mergeMap((res : any) => {
      var regularPlanUrls = {};
      var milestonePlanUrls = {};
      var planResponse = [];
      res.forEach(partner => {
        regularPlanUrls[partner.partnerCode] = this.baseUrl + "/retailer/" + PlanType.REGULAR +"/" + partner.partnerCode;
        milestonePlanUrls[partner.partnerCode] = this.baseUrl + "/retailer/" + PlanType.MILESTONE +"/" + partner.partnerCode;
        planResponse.push(this.http.get(regularPlanUrls[partner.partnerCode]).pipe(
             catchError(error => of(error))));
        planResponse.push(this.http.get(milestonePlanUrls[partner.partnerCode]).pipe(
              catchError(error => of(error))));
      });
      return forkJoin(planResponse);
    }));
    

  }


  getConsumerIncentives() {
    let regularIncentiveUrl = this.baseUrl + "/consumer/" + PlanType.REGULAR;
    let milestoneIncentiveUrl = this.baseUrl + "/consumer/" + PlanType.MILESTONE;

    let regularIncentives = this.http.get(regularIncentiveUrl).pipe(
      catchError(error => of(error)));
    let milestoneIncentives = this.http.get(milestoneIncentiveUrl).pipe(
      catchError(error => of(error)));
    
    this.logger.log(`Getting incentive plans for consumer`);
    return forkJoin([regularIncentives, milestoneIncentives]);
  }

  getRetailerIncentivePlanByIdAndPartner(planId, subType) {
    let url = this.baseUrl + "/retailer/"+ planId + "/" + subType;
    this.logger.log(`Fetching retailer incentive plan by ID and subtype`);
    return this.http.get(url, { observe: 'response'});
  }

  getConsumerIncentivePlanById(planId) {
    let url = this.baseUrl + "/consumer/"+ planId ;
    this.logger.log(`Fetching consumer incentive plan by ID`);
    return this.http.get(url, { observe: 'response'});
  }

  publishRetailerIncentivePlan(planId, subType) {
    let url = this.baseUrl + "/retailer/publish/"+ planId + "/" + subType;
    this.logger.log(`Publishing retailer incentive plan by ID and sub type`);
    return this.http.put(url, { observe: 'response'});
  }

  publishConsumerIncentivePlan(planId) {
    let url = this.baseUrl + "/consumer/publish/"+ planId;
    this.logger.log(`Publishing consumer incentive plan by ID`);
    return this.http.put(url, { observe: 'response'});
  }

  deleteDraftRetailerIncentivePlan(planId, subType) {
    let url = this.baseUrl + "/retailer/"+ planId + "/" + subType;
    this.logger.log(`Deleting retailer incentive plan by ID and sub type`);
    return this.http.delete(url, { observe: 'response'});
  }

  deleteDraftConsumerIncentivePlan(planId) {
    let url = this.baseUrl + "/consumer/"+ planId;
    this.logger.log(`Deleting consumer incentive plan by ID`);
    return this.http.delete(url, { observe: 'response'});
  }


  changeDateRetailerIncentivePlan(planId, subType, endDate){
    let url = this.baseUrl + "/retailer/changeenddate/"+ planId + "/" + subType + "/" + endDate;
    this.logger.log(`Updating retailer incentive plan end date by ID `);
    return this.http.put(url, { observe: 'response'});
  }

  changeDateConsumerIncentivePlan(planId, endDate){
    let url = this.baseUrl + "/consumer/changeenddate/"+ planId + "/" + endDate;
    this.logger.log(`Updating consumer incentive plan end date by ID `);
    return this.http.put(url, { observe: 'response'});
  
  }

  updateRetailerDraftPlan(planId, plan){
    let url = this.baseUrl + "/retailer/"+ planId;
    this.logger.log(`Updating retailer incentive plan in draft state by ID `);
    return this.http.put(url, plan, { observe: 'response'});
  }

  updateConsumerDraftPlan(planId, plan){
    let url = this.baseUrl + "/consumer/"+ planId;
    this.logger.log(`Updating consumer incentive plan in draft state by ID `);
    return this.http.put(url, plan, { observe: 'response'});
  }

  getEventList(audience) {
    let url = this.baseUrl + "/eventlist/" + audience;
    this.logger.log(`Get event list for audience `);
    return this.http.get(url, { observe: 'response'});
  }
}
