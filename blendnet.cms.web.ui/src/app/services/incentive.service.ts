import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Incentive, PlanType, RetailerPartner } from '../models/incentive.model';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class IncentiveService {

  baseUrl = environment.incentiveApiUrl;

  constructor(
    private logger: LogService,
    private http: HttpClient
  ) {
    
   }

  getIncentiveByIdAndSubType(planId, subType) {
    let url = this.baseUrl + "/"+ planId + "/" + subType;
    this.logger.log(`Fetching incentive plan by ID and subtype`);
    return this.http.get(url, { observe: 'response'});

  }

  createIncentivePlanRetailer(incentivePlan){
    let url = this.baseUrl + "/retailer";
    this.logger.log(`Creating incentive plan`);
    return this.http.post(url, incentivePlan, { observe: 'response'});

  }

    createIncentivePlanConsumer(incentivePlan){
    let url = this.baseUrl + "/consumer";
    this.logger.log(`Creating incentive plan`);
    return this.http.post(url, incentivePlan, { observe: 'response'});

  }

  getRetailerIncentives(): Observable<any[]> {
    let regularIncentiveUrl1 = this.baseUrl + "/retailer/" + PlanType.REGULAR +"/" + RetailerPartner.NOVO;
    let milestoneIncentiveUrl1 = this.baseUrl + "/retailer/" + PlanType.MILESTONE +"/" + RetailerPartner.NOVO;

    let regularIncentiveUrl2 = this.baseUrl + "/retailer/" + PlanType.REGULAR +"/" + RetailerPartner.TSTP;
    let milestoneIncentiveUrl2 = this.baseUrl + "/retailer/" + PlanType.MILESTONE +"/" + RetailerPartner.TSTP;

    let regularIncentives1 = this.http.get(regularIncentiveUrl1).pipe(
      catchError(error => of(error)));
    let milestoneIncentives1 = this.http.get(milestoneIncentiveUrl1).pipe(
      catchError(error => of(error)));

    let regularIncentives2 = this.http.get(regularIncentiveUrl2).pipe(
      catchError(error => of(error)));
    let milestoneIncentives2 = this.http.get(milestoneIncentiveUrl2).pipe(
      catchError(error => of(error)));

    this.logger.log(`Getting incentive plans for retailer`);
    // return forkJoin([regularIncentives1, milestoneIncentives1]);

    return forkJoin([regularIncentives1, milestoneIncentives1, regularIncentives2, milestoneIncentives2]);
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
}
