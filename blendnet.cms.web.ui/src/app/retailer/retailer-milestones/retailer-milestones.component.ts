// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service'
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { EventType } from 'src/app/models/incentive.model';
import { ContentProviderService } from 'src/app/services/content-provider.service';
import { Location } from '@angular/common';
import { ContentProviderLtdInfo } from 'src/app/models/contentprovider.model';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-retailer-milestones',
  templateUrl: './retailer-milestones.component.html',
  styleUrls: ['./retailer-milestones.component.css']
})
export class RetailerMilestonesComponent implements OnInit, AfterViewInit, OnDestroy {
  totalMilestoneEarnings = 0
  milestone:any;
  milestonesDuration: Array<any>=[];
  milestoneSelect: any;
  milestonesCarouselArr: Array<any> = [];
  partnerCode = sessionStorage.getItem('partnerCode');
  retailerPartnerProvidedId = sessionStorage.getItem('partnerProvidedId');
  baseHref = this.retailerDashboardService.getBaseHref();
  contentProviders: ContentProviderLtdInfo[];
  constructor(
    private retailerDashboardService: RetailerDashboardService,
    public router: Router,
    public location: Location,
    public userService: UserService,
    private contentProviderService: ContentProviderService
  ) { }

  ngOnInit(): void {
    this.getContentProviders();
    this.partnerCode = this.retailerDashboardService.getpartnerCode();
    this.retailerPartnerProvidedId = this.retailerDashboardService.getRetailerPartnerProvidedId();
    this.milestone = this.retailerDashboardService.getMilestoneSelected();
    this.getMilestoneDetails();
    this.getMilestoneTotal(this.milestone);
  }


  getContentProviders() {
    if(sessionStorage.getItem("CONTENT_PROVIDERS_PUBLISHED")) {
      this.contentProviders =  JSON.parse(sessionStorage.getItem("CONTENT_PROVIDERS_PUBLISHED"));
      this.contentProviders.forEach(contentProvider => {
        this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
      }); 
    } else {
      this.contentProviderService.browseContentProviders().subscribe(
        res => {
        this.contentProviders = res;
        sessionStorage.setItem("CONTENT_PROVIDERS_PUBLISHED",  JSON.stringify(this.contentProviders));
        this.contentProviders.forEach(contentProvider => {
          this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
        }); 
      });
    }
  }

  getMilestoneDetails(){
    let allMilestones: Array<any>=[];
    this.retailerDashboardService.getAllPublishedIncentivePlansByPlanType(this.retailerPartnerProvidedId, "milestone", this.partnerCode).subscribe( res => {
      res.forEach(milestone => {
        if(milestone.audience.audienceType=="RETAILER") {
            const firstDateString = new Date(milestone.startDate);
            const lastDateString = new Date(milestone.endDate); 
            allMilestones.push({
              id: milestone.id,
              name: milestone.planName,
              startDate: firstDateString,
              endDate: lastDateString,
              dateString: firstDateString.toDateString() + " - " + lastDateString.toDateString()});
        };
      });
      allMilestones.sort(function(a,b){return a.endDate - b.endDate});
      allMilestones.reverse();
      const currentDate = new Date();
      var currentActivePlan = false;
      //to check if there are any current active plans
      for(var i=0;i<allMilestones.length;i++){
        if((allMilestones[i].startDate.toDateString()<currentDate && allMilestones[i].endDate.toDateString()>currentDate) || allMilestones[i].startDate==currentDate || allMilestones[i].endDate==currentDate){
          currentActivePlan=true;
          for(var c=i;c<i+5 && c<allMilestones.length;c++){
            this.milestonesDuration.push({
              id: allMilestones[c].id,
              name: allMilestones[c].name,
              dateString: allMilestones[c].dateString});
            }
            break;
        }
      }
      if(!currentActivePlan){
        for(var c=0;c<5 && c<allMilestones.length;c++){
          this.milestonesDuration.push({
            id: allMilestones[c].id,
            name: allMilestones[c].name,
            dateString: allMilestones[c].dateString});
          }
      }
      let milestoneSelected = this.retailerDashboardService.getMilestoneSelected()? this.retailerDashboardService.getMilestoneSelected() : this.milestonesDuration[0];
        if(milestoneSelected!=null) {
          this.getMilestoneTotal(milestoneSelected);
          this.milestoneSelect = milestoneSelected;
        }
      //this.carouselInit =true;
    } ,err => {
      console.log('error in milestone duration fetch');
      this.totalMilestoneEarnings = 0;
      //this.carouselInit =true;
    });
  }

  milestoneSelected(event) {
    this.retailerDashboardService.setMilestoneSelected(event.value);
    this.getMilestoneTotal(event.value);
  }

  getMilestoneTotal(milestone) {
    let totalMilestoneEarnings = 0;
    let milestonesCarouselArr = []
    this.retailerDashboardService.getMileStonesPlanDetails(this.partnerCode,this.retailerPartnerProvidedId, milestone.id).subscribe( res => {
      
      if(res.planDetails) {
        res.planDetails.forEach(planDetail => {
          if(planDetail.formula && planDetail.formula.formulaType === 'DIVIDE_AND_MULTIPLY') {
            planDetail.formulaName = planDetail.formula.formulaType;
            if(planDetail.result && planDetail.result.value) {
              const value = planDetail.result.value;
              totalMilestoneEarnings+=value;
            }
            if(!planDetail.result) {
              planDetail.result = {
                residualValue: 0,
                value: 0
              }
            }
            if(planDetail.formula.firstOperand && planDetail.formula.secondOperand && planDetail.result) {
              milestonesCarouselArr.push({
                formulaType: planDetail.formulaName,
                ruleType: planDetail.ruleType,
                firstOperand: planDetail.formula.firstOperand,
                secondOperand: planDetail.formula.secondOperand,
                value : planDetail.result.value ? planDetail.result.value : 0,
                residualValue : planDetail.result.residualValue ? planDetail.result.residualValue : 0,
                progress: ((planDetail.result.residualValue ? planDetail.result.residualValue : 0))*100/planDetail.formula.firstOperand,
                referral: planDetail.eventType === EventType['Retailer Referral'],
                contentProviderId: planDetail.eventSubType
              });
            } 
          } else if(planDetail.formula && planDetail.formula.formulaType === 'RANGE') {
            planDetail.formulaName = planDetail.formula.formulaType;
            if(planDetail.result && planDetail.result.value) {
              const value = planDetail.result.value;
              totalMilestoneEarnings+=value;      
            }
            if(!planDetail.result) {
              planDetail.result = {
                residualValue: 0,
                value: 0
              }
            }
            if(planDetail.result) {
              milestonesCarouselArr.push({
                formulaType: planDetail.formulaName,
                ruleType: planDetail.ruleType,
                value : planDetail.result.value ? planDetail.result.value : 0,
                eventType: planDetail.eventType,
                contentProviderId: planDetail.eventSubType
              });
            }
          }
        });
        this.totalMilestoneEarnings = totalMilestoneEarnings;
        this.milestonesCarouselArr = milestonesCarouselArr;
      }
    } ,err => {
      console.log('error in milestone fetch');
      this.totalMilestoneEarnings = totalMilestoneEarnings;
    });
  }

  getRegularRatesIncentives() {
    this.retailerDashboardService.getRegularRatesIncentives(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
      res => {

      },
      err => {
        
      }
    )
  }

  
  navigateToDashboard() {
    this.location.back();
  }

  ngAfterViewInit() {
    console.log('setting routed to' + true);
    //this.userService.setRetailerRouted(true);
  }

  ngOnDestroy() {
    //this.userService.setRetailerRouted(false);
  }
  
  compareById(milestone1: any, milestone2: any): boolean {
    return milestone1 && milestone2 && milestone1.id === milestone2.id;
  }

}
