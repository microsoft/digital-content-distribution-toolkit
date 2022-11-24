// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, mergeMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Incentive, PlanType } from '../models/incentive.model';
import { LogService } from './log.service';
import { RetailerService } from './retailer.service';

@Injectable({
  providedIn: 'root'
})
export class IncentiveService {

  baseUrl = environment.baseUrl + environment.incentiveApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient,
    private retailerService: RetailerService
  ) {
    
   }


  createIncentivePlanRetailer(incentivePlan){
    let url = this.baseUrl + "/retailer";
    this.logger.log(`Creating retailerincentive plan`);
    return this.http.post(url, incentivePlan);

  }

    createIncentivePlanConsumer(incentivePlan){
    let url = this.baseUrl + "/consumer";
    this.logger.log(`Creating consumer incentive plan`);
    return this.http.post(url, incentivePlan, { observe: 'response'});

  }
  getRetailerIncentivesByPartnerAndPlanType(partner, type):  Observable<Incentive> { 
    let url= this.baseUrl + "/retailer/" + type +"/" + partner;
    this.logger.log(`Getting the incentive plans for retailers`);
    return this.http.get<Incentive>(url);
        
  }

  getRetailerIncentives() {
    return this.retailerService.getRetailerPartners().pipe(mergeMap((res : any) => {
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

  
  getConsumerIncentivesByPlanType(type) {
    let url = this.baseUrl + "/consumer/" + type;
    this.logger.log(`Getting the incentive plans for consumers`);
    return this.http.get(url, { observe: 'response'});
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
    return this.http.put(url, null);
  }

  publishConsumerIncentivePlan(planId) {
    let url = this.baseUrl + "/consumer/publish/"+ planId;
    this.logger.log(`Publishing consumer incentive plan by ID`);
    return this.http.put(url, null);
  }

  deleteDraftRetailerIncentivePlan(planId, subType) {
    let url = this.baseUrl + "/retailer/"+ planId + "/" + subType;
    this.logger.log(`Deleting retailer incentive plan by ID and sub type`);
    return this.http.delete(url);
  }

  deleteDraftConsumerIncentivePlan(planId) {
    let url = this.baseUrl + "/consumer/"+ planId;
    this.logger.log(`Deleting consumer incentive plan by ID`);
    return this.http.delete(url);
  }


  changeDateRetailerIncentivePlan(planId, subType, endDate){
    let url = this.baseUrl + "/retailer/changeenddate/"+ planId + "/" + subType + "/" + endDate;
    this.logger.log(`Updating retailer incentive plan end date by ID `);
    return this.http.put(url, null);
  }

  changeDateConsumerIncentivePlan(planId, endDate){
    let url = this.baseUrl + "/consumer/changeenddate/"+ planId + "/" + endDate;
    this.logger.log(`Updating consumer incentive plan end date by ID `);
    return this.http.put(url, null);
  
  }

  updateRetailerDraftPlan(planId, plan){
    let url = this.baseUrl + "/retailer/"+ planId;
    this.logger.log(`Updating retailer incentive plan in draft state by ID `);
    return this.http.put(url, plan);
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
