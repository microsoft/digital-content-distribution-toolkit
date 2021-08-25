import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { RetailerDashboardService } from 'src/app/services/retailer/retailer-dashboard.service';
import { EventType } from 'src/app/models/incentive.model';
import { Contentproviders } from '../../models/incentive.model';
import { ContentProviderService } from 'src/app/services/content-provider.service';

@Component({
  selector: 'app-rates-incentives',
  templateUrl: './rates-incentives.component.html',
  styleUrls: ['./rates-incentives.component.css']
})
export class RatesIncentivesComponent implements OnInit, AfterViewInit, OnDestroy {
  ratesIncentivesReferrals = [];
  ratesIncentivesMilestones = [];
  ratesIncentivesCommissions = [];
  partnerCode;
  retailerPartnerProvidedId;
  eventType = EventType;
  contentProviders;
  referralsValid;
  milestonesValid;
  incentivesValid;
  baseHref = this.retailerDashboardService.getBaseHref();
  constructor(
    public userService: UserService,
    public router: Router,
    public dialog: MatDialog,
    private retailerDashboardService: RetailerDashboardService,
    private contentProviderService: ContentProviderService
  ) { }

  ngOnInit(): void {
    this.getContentProviders();
    this.partnerCode = this.retailerDashboardService.getpartnerCode();
    this.retailerPartnerProvidedId = this.retailerDashboardService.getRetailerPartnerProvidedId();
    this.retailerDashboardService.getMilestoneRatesIncentives(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
      res => {
        // res = {"planName":"Milestone Plan","planType":"MILESTONE","startDate":"2021-08-31T18:30:00.000Z","endDate":"2021-09-30T18:29:59.000Z","audience":{"audienceType":"RETAILER","subTypeName":"NOVO"},"planDetails":[{"eventType":"RETAILER_INCOME_REFERRAL_COMPLETED","eventTitle":"50 referrals","eventSubType":null,"ruleType":"COUNT","formula":{"formulaType":"DIVIDE_AND_MULTIPLY","firstOperand":"50","rangeOperand":[]}},{"eventType":"RETAILER_INCOME_ORDER_COMPLETED","eventTitle":"Zee5 10 orders","eventSubType":"6790061e-f62c-4b5c-8476-29a17f3fe9ef","ruleType":"COUNT","formula":{"formulaType":"DIVIDE_AND_MULTIPLY","firstOperand":"10","rangeOperand":[]}},{"eventType":"RETAILER_INCOME_ORDER_COMPLETED","eventTitle":"SonyLiv Incremental offer","eventSubType":"c6d9f36f-3789-418e-994c-e4a7d1afedf1","ruleType":"COUNT","formula":{"formulaType":"RANGE_AND_MULTIPLY","firstOperand":"","rangeOperand":[{"startRange":"1","endRange":"5","output":"50"},{"startRange":"6","endRange":"10","output":"100"},{"startRange":"11","endRange":"20","output":"500"}]}},{"eventType":"RETAILER_INCOME_ORDER_COMPLETED","eventTitle":"Voot 10 orders","eventSubType":"54f87671-61d7-4644-956b-b237097ec634","ruleType":"COUNT","formula":{"formulaType":"DIVIDE_AND_MULTIPLY","firstOperand":"10","rangeOperand":[]}}]}
        this.milestonesValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
        if(res["planDetails"] && res["planDetails"].length) {
          this.ratesIncentivesMilestones = res["planDetails"];
        }
      },
      err => {

      }
    )

  }

  ngAfterViewInit() {
    console.log('setting routed to' + true);
    this.userService.setRetailerRouted(true);
  }

  ngOnDestroy() {
    this.userService.setRetailerRouted(false);
  }

  navigateToHome() {
    this.router.navigate(['retailer/home']);
  }

  getContentProviders() {
    this.contentProviderService.browseContentProviders().subscribe(
      res => {
        this.retailerDashboardService.getRegularRatesIncentives(this.partnerCode, this.retailerPartnerProvidedId).subscribe(
          res => {
            if(res["planDetails"] && res["planDetails"].length) {
              this.referralsValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
              this.incentivesValid = this.retailerDashboardService.formatDateOnlyDay(res['endDate']);
              res["planDetails"].forEach(eachEvent => {
                if(eachEvent.eventType ===  EventType["Retailer Referral"]) {
                  this.ratesIncentivesReferrals.push(eachEvent);
                } else if(eachEvent.eventType ===  EventType["Retailer Order Complete"]) {
                  this.ratesIncentivesCommissions.push(eachEvent);
                }
              });
            }
          },
          err => {
    
          }
        )
        this.contentProviders = res;
        this.contentProviders.forEach(contentProvider => {
          this.contentProviders[contentProvider.contentProviderId] = contentProvider.name;
        }); 
      },
      err => console.log(err)
    )
  
  }


}
